using System;
using System.Collections.Generic;
using System.Linq;
using BrasaoSolution.Casa.Model;
using BrasaoSolution.Helper;
using Microsoft.EntityFrameworkCore;

namespace BrasaoSolution.Web.Casa
{
    public class BrasaoSolutionRepository
    {
        private BrasaoSolutionContext _contexto;

        public BrasaoSolutionRepository(BrasaoSolutionContext contexto) => this._contexto = contexto;

        public EmpresaViewModel GetEmpresa(int codEmpresa)
        {
            var empresa = (
                from emps in _contexto.Empresas
                join bairros in _contexto.Bairros on emps.CodBairro equals bairros.CodBairro
                join cidades in _contexto.Cidades on bairros.CodCidade equals cidades.CodCidade
                where emps.CodEmpresa == codEmpresa
                select new EmpresaViewModel
                {
                    CodEmpresa = emps.CodEmpresa,
                    RazaoSocial = emps.RazaoSocial,
                    NomeFantasia = emps.NomeFantasia,
                    InscricaoEstadual = emps.InscricaoEstadual,
                    Logomarca = emps.Logomarca,
                    CNPJ = emps.CNPJ,
                    CodBairro = emps.CodBairro,
                    NomeBairro = bairros.Nome,
                    CodCidade = bairros.CodCidade,
                    NomeCidade = cidades.Nome,
                    CodEmpresaMatriz = emps.CodEmpresaMatriz,
                    Complemento = emps.Complemento,
                    Estado = cidades.Estado,
                    Logradouro = emps.Logradouro,
                    Numero = emps.Numero,
                    Telefone = emps.Telefone,
                    Email = emps.Email,
                    Facebook = emps.Facebook,
                    ImagemBackgroundAutenticada = emps.ImagemBackgroundAutenticada,
                    ImagemBackgroundPublica = emps.ImagemBackgroundPublica,
                    CorPrincipal = emps.CorPrincipal,
                    CorSecundaria = emps.CorSecundaria,
                    CorPrincipalContraste = emps.CorPrincipalContraste,
                    CorDestaque = emps.CorDestaque,
                    TextoInstitucional = emps.TextoInstitucional,
                    EhFilial = (emps.CodEmpresaMatriz == null),
                    EmpresaAtiva = emps.EmpresaAtiva,
                    UrlSite = emps.UrlSite
                }
                ).FirstOrDefault();

            if(empresa != null)
            {
                if (!String.IsNullOrEmpty(empresa.Logomarca))
                {
                    empresa.LogomarcaMini = empresa.Logomarca.Replace("img_logo", "mini-img_logo");
                }

                if (!String.IsNullOrEmpty(empresa.ImagemBackgroundPublica))
                {
                    empresa.ImagemBackgroundPublicaMini = empresa.ImagemBackgroundPublica.Replace("img_bg_publica", "mini-img_bg_publica");
                }

                if (!String.IsNullOrEmpty(empresa.ImagemBackgroundAutenticada))
                {
                    empresa.ImagemBackgroundAutenticadaMini = empresa.ImagemBackgroundAutenticada.Replace("img_bg_autenticada", "mini-img_bg_autenticada");
                }
            }

            //FillImagensEmpresa(lista, HttpContext.Current.Server.MapPath("~").ToString());

            return empresa;
        }

        public List<ClasseItemCardapioViewModel> GetCardapio(int codEmpresa)
        {
            var retorno = _contexto.Classes
                .Include(c => c.Itens).ThenInclude(i => i.Classe)
                .Include(c => c.Itens).ThenInclude(i => i.Complemento)
                .ToList()
                .Where(c => c.Itens.Where(a => a.Ativo).Count() > 0)
                .Select(c =>
                new ClasseItemCardapioViewModel
                {
                    CodClasse = c.CodClasse,
                    DescricaoClasse = c.DescricaoClasse,
                    Imagem = c.Imagem,
                    OrdemExibicao = c.OrdemExibicao,
                    Itens = c.Itens.Where(i => (i.CodEmpresa != null ? i.CodEmpresa : codEmpresa) == codEmpresa && i.Ativo).Select(i =>
                        new ItemCardapioViewModel
                        {
                            CodItemCardapio = i.CodItemCardapio,
                            CodClasse = i.CodClasse,
                            Nome = i.Nome,
                            Preco = i.Preco,
                            Ativo = i.Ativo,
                            Complemento = (i.Complemento != null ?
                                new ComplementoItemCardapioViewModel
                                {
                                    DescricaoLonga = i.Complemento.DescricaoLonga,
                                    Imagem = i.Complemento.Imagem
                                } : null)
                        }).ToList()
                }).ToList();

            var numDiaHoje = (int)DateTime.Now.DayOfWeek;
            var promocoesAtivas = (from promos in _contexto.PromocoesVenda.Include(p => p.ClassesAssociadas).Include(p => p.ItensAssociados).Include(p => p.DiasAssociados)
                                   where promos.PromocaoAtiva && promos.DataHoraInicio <= DateTime.Now && promos.DataHoraFim >= DateTime.Now
                                   && promos.DiasAssociados.Select(d => d.DiaSemana).Contains(numDiaHoje)
                                   && (promos.CodEmpresa != null ? promos.CodEmpresa : codEmpresa) == codEmpresa
                                   select promos).ToList();

            if (promocoesAtivas != null && promocoesAtivas.Count > 0)
            {
                foreach (var promo in promocoesAtivas.OrderBy(p => p.PercentualDesconto).ToList())
                {
                    if (promo.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorClasse)
                    {
                        retorno.Where(c => promo.ClassesAssociadas.Select(x => x.CodClasse).Contains(c.CodClasse)).ToList().ForEach(c => c.Itens.ForEach(y =>
                        {
                            y.PercentualDesconto = (double)promo.PercentualDesconto;
                            y.CodPromocaoVenda = promo.CodPromocaoVenda;
                            y.PrecoComDesconto = Convert.ToDouble(1 - (promo.PercentualDesconto / 100)) * y.Preco;
                        }));
                    }
                    else if (promo.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorItem)
                    {
                        foreach (var classe in retorno)
                        {
                            classe.Itens.Where(i => promo.ItensAssociados.Select(x => x.CodItemCardapio).Contains(i.CodItemCardapio)).ToList().ForEach(y =>
                            {
                                y.PercentualDesconto = (double)promo.PercentualDesconto;
                                y.CodPromocaoVenda = promo.CodPromocaoVenda;
                                y.PrecoComDesconto = Convert.ToDouble(1 - (promo.PercentualDesconto / 100)) * y.Preco;
                            });
                        }
                    }
                }

                ClasseItemCardapioViewModel ofertas = new ClasseItemCardapioViewModel();
                ofertas.CodClasse = -1;
                ofertas.CodImpressoraPadrao = -1;
                ofertas.DescricaoClasse = "--------------> OFERTAS";
                ofertas.Imagem = "";
                ofertas.ImagemMini = "";
                ofertas.OrdemExibicao = 0;
                ofertas.Itens = new List<ItemCardapioViewModel>();
                var listaTemp = new List<ItemCardapioViewModel>();
                retorno.ForEach(e => { listaTemp.AddRange(e.Itens.Where(i => i.PercentualDesconto > 0).ToList()); });
                PropertyCopy.IgnoreExceptions = true;
                listaTemp.ForEach(l =>
                {
                    var item = new ItemCardapioViewModel();
                    PropertyCopy.Copy(l, item);
                    item.CodClasse = -1;
                    ofertas.Itens.Add(item);
                });

                if (ofertas.Itens.Count > 0)
                {
                    retorno.Add(ofertas);
                }
            }

            var combosDB = this.GetCombosDB(true, codEmpresa);

            if (combosDB != null && combosDB.Where(c => c.Ativo).Count() > 0)
            {
                var combos = new ClasseItemCardapioViewModel();
                combos.CodClasse = -2;
                combos.CodImpressoraPadrao = -1;
                combos.DescricaoClasse = "--------------> COMBOS";
                combos.Imagem = "";
                combos.ImagemMini = "";
                combos.OrdemExibicao = 0;
                combos.Itens = combosDB.Where(c => c.Ativo).Select(c => new ItemCardapioViewModel
                {
                    CodItemCardapio = -1 * c.CodItemCardapio,
                    Nome = c.Nome,
                    Descricao = c.Descricao,
                    Preco = c.Preco,
                    CodCombo = c.CodCombo,
                    PrecoCombo = c.PrecoCombo,
                    CodClasse = -2,
                    Ativo = c.Ativo,
                    CodImpressoraProducao = -1
                }).ToList();

                if (combos.Itens.Count > 0)
                {
                    retorno.Add(combos);
                }
            }

            foreach (var classe in retorno)
            {
                classe.Itens = classe.Itens.Where(i => i.Ativo).ToList();
                if (!String.IsNullOrEmpty(classe.Imagem))
                {
                    classe.ImagemMini = classe.Imagem.Replace("img_classe", "mini-img_classe");
                }

                if (classe.Itens != null)
                {
                    foreach (var item in classe.Itens)
                    {
                        if (item.Complemento != null && !String.IsNullOrEmpty(item.Complemento.Imagem))
                        {
                            item.Complemento.ImagemMini = item.Complemento.Imagem.Replace("img_item", "mini-img_item");
                        }
                    }
                }
            }

            return retorno;
        }

        public DadosItemCardapioViewModel GetDadosItemCardapio(int codItemCardapio)
        {
            return _contexto.ItensCardapio
                    .Where(i => i.CodItemCardapio == codItemCardapio)
                    .Include(i => i.ObservacoesPermitidas).ThenInclude(o => o.ObservacaoProducao)
                    .Include(i => i.ExtrasPermitidos).ThenInclude(e => e.OpcaoExtra)
                    .ToList()
                    .Select(i =>
                    new DadosItemCardapioViewModel
                    {
                        CodItemCardapio = i.CodItemCardapio,
                        Observacoes = (i.ObservacoesPermitidas != null ?
                               i.ObservacoesPermitidas.Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.ObservacaoProducao.CodObservacao, DescricaoObservacao = o.ObservacaoProducao.DescricaoObservacao }).ToList() : null),
                        Extras = (i.ExtrasPermitidos != null ?
                                i.ExtrasPermitidos.Select(e => new OpcaoExtraViewModel { CodOpcaoExtra = e.OpcaoExtra.CodOpcaoExtra, DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra, Preco = e.OpcaoExtra.Preco }).ToList() : null)
                    }).FirstOrDefault();
        }

        public List<ComboViewModel> GetCombosDB(bool combosDoDia, int codEmpresa)
        {
            var combos = (from cmb in _contexto.Combos
                               .Include(c => c.Itens).ThenInclude(i => i.Item)
                               .Include(c => c.Empresa)
                               .Where(c => (c.CodEmpresa != null ? c.CodEmpresa : codEmpresa) == codEmpresa)
                          select new ComboViewModel
                          {
                              CodCombo = cmb.CodCombo,
                              CodEmpresa = cmb.CodEmpresa,
                              NomeEmpresa = cmb.Empresa.NomeFantasia,
                              Nome = cmb.NomeCombo,
                              Descricao = cmb.DescricaoCombo,
                              Preco = cmb.PrecoCombo,
                              PrecoCombo = cmb.PrecoCombo,
                              Ativo = cmb.Ativo,
                              DataHoraInicio = cmb.DataHoraInicio,
                              DataHoraFim = cmb.DataHoraFim,
                              Itens = cmb.Itens.Select(i => new ComboItemCardapioViewModel
                              {
                                  CodCombo = cmb.CodCombo,
                                  CodItemCardapio = i.CodItemCardapio,
                                  Quantidade = i.Quantidade,
                                  Nome = i.Item.Nome
                              }).ToList(),
                              DiasAssociados = cmb.DiasAssociados.Select(d => new DiaSemanaViewModel
                              {
                                  NumDiaSemana = d.DiaSemana
                              }).ToList(),
                          }
                                ).OrderBy(c => c.CodCombo).ToList();

            foreach (var combo in combos)
            {
                combo.DataInicio = combo.DataHoraInicio.ToString("dd/MM/yyyy");
                combo.HoraInicio = combo.DataHoraInicio.ToString("HH:mm");
                combo.DataFim = combo.DataHoraFim.ToString("dd/MM/yyyy");
                combo.HoraFim = combo.DataHoraFim.ToString("HH:mm");
            }

            if (combosDoDia)
            {
                var numDiaHoje = (int)DateTime.Now.DayOfWeek;

                return (from cmbs in combos
                        where cmbs.Ativo &&
                              cmbs.DataHoraInicio <= DateTime.Now &&
                              cmbs.DataHoraFim >= DateTime.Now &&
                              cmbs.DiasAssociados.Select(d => d.NumDiaSemana).Contains(numDiaHoje)
                        select cmbs).ToList();
            }

            return combos;
        }
    }
}
