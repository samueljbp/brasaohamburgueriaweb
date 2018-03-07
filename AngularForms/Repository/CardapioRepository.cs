using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using BrasaoHamburgueria.Helper;

namespace BrasaoHamburgueria.Web.Repository
{
    public class CardapioRepository : IDisposable
    {
        private BrasaoContext _contexto;

        public CardapioRepository()
        {
            _contexto = new BrasaoContext();
        }

        public DadosItemCardapioViewModel GetDadosItemCardapio(int codItemCardapio)
        {
            return _contexto.ItensCardapio
                    .Where(i => i.CodItemCardapio == codItemCardapio)
                    .Include(i => i.ObservacoesPermitidas)
                    .Include(i => i.ObservacoesPermitidas.Select(o => o.ObservacaoProducao))
                    .Include(i => i.ExtrasPermitidos)
                    .Include(i => i.ExtrasPermitidos.Select(e => e.OpcaoExtra))
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

        public List<ClasseItemCardapioViewModel> GetCardapio()
        {

            var retorno = _contexto.Classes
                .Include(c => c.Itens)
                .Include(c => c.Itens.Select(i => i.Classe))
                .Include(c => c.Itens.Select(i => i.Complemento))
                //.Include(c => c.Itens.Select(i => i.ObservacoesPermitidas))
                //.Include(c => c.Itens.Select(i => i.ObservacoesPermitidas.Select(o => o.ObservacaoProducao)))
                //.Include(c => c.Itens.Select(i => i.ExtrasPermitidos))
                //.Include(c => c.Itens.Select(i => i.ExtrasPermitidos.Select(e => e.OpcaoExtra)))
                .ToList()
                .Where(c => c.Itens.Where(a => a.Ativo).Count() > 0)
                .Select(c =>
                new ClasseItemCardapioViewModel
                {
                    CodClasse = c.CodClasse,
                    DescricaoClasse = c.DescricaoClasse,
                    Imagem = c.Imagem,
                    OrdemExibicao = c.OrdemExibicao,
                    Itens = c.Itens.Select(i =>
                        new ItemCardapioViewModel
                        {
                            CodItemCardapio = i.CodItemCardapio,
                            CodClasse = i.CodClasse,
                            Nome = i.Nome,
                            Preco = i.Preco,
                            Ativo = i.Ativo,
                            /*ObservacoesPermitidas = (i.ObservacoesPermitidas != null ?
                                i.ObservacoesPermitidas.Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.ObservacaoProducao.CodObservacao, DescricaoObservacao = o.ObservacaoProducao.DescricaoObservacao }).ToList() : null),
                            ExtrasPermitidos = (i.ExtrasPermitidos != null ?
                                i.ExtrasPermitidos.Select(e => new OpcaoExtraViewModel { CodOpcaoExtra = e.OpcaoExtra.CodOpcaoExtra, DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra, Preco = e.OpcaoExtra.Preco }).ToList() : null),*/
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
                    ofertas.Itens.Add(item);
                });

                if (ofertas.Itens.Count > 0)
                {
                    retorno.Add(ofertas);
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}