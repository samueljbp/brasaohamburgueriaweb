using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;
using System.Drawing;
using System.Data.Entity;

namespace BrasaoHamburgueria.Web.Repository
{
    public class CadastrosRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();

        #region Cadastros de combo de cardápio

        public List<ComboViewModel> GetCombosDB(bool combosDoDia)
        {
            var combos = (from cmb in _contexto.Combos
                               .Include(c => c.Itens)
                               .Include(c => c.Itens.Select(i => i.Item))
                          select new ComboViewModel
                          {
                              CodCombo = cmb.CodCombo,
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

        public async Task<List<ComboViewModel>> GetCombos()
        {
            return GetCombosDB(false);
        }

        public async Task<ComboViewModel> GravarCombo(ComboViewModel combo, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    combo.DataHoraInicio = Convert.ToDateTime(combo.DataInicio.Substring(0, 2) + "/" + combo.DataInicio.Substring(3, 2) + "/" + combo.DataInicio.Substring(6) + " " + combo.HoraInicio.Substring(0, 2) + ":" + combo.HoraInicio.Substring(3));
                    combo.DataHoraFim = Convert.ToDateTime(combo.DataFim.Substring(0, 2) + "/" + combo.DataFim.Substring(3, 2) + "/" + combo.DataFim.Substring(6) + " " + combo.HoraFim.Substring(0, 2) + ":" + combo.HoraFim.Substring(3));

                    if (combo.DataHoraFim < combo.DataHoraInicio)
                    {
                        throw new Exception("O término da vigência não pode ser anterior ao início.");
                    }

                    if (modoCadastro == "A") //alteração
                    {
                        var comboAlterar = _contexto.Combos.Find(combo.CodCombo);

                        if (comboAlterar != null)
                        {
                            if (combo.Itens != null)
                            {
                                _contexto.ItensCombo.RemoveRange(_contexto.ItensCombo.Where(c => c.CodCombo == combo.CodCombo.Value).ToList());
                                _contexto.ItensCombo.AddRange(combo.Itens.GroupBy(i => i.CodItemCardapio).Select(o => new ComboItemCardapio { CodCombo = combo.CodCombo.Value, CodItemCardapio = o.First().CodItemCardapio, Quantidade = o.Sum(x => x.Quantidade) }));
                            }

                            if (combo.DiasAssociados != null)
                            {
                                _contexto.DiasSemanaCombo.RemoveRange(_contexto.DiasSemanaCombo.Where(c => c.CodCombo == combo.CodCombo.Value).ToList());
                                _contexto.DiasSemanaCombo.AddRange(combo.DiasAssociados.Select(o => new DiaSemanaCombo { DiaSemana = o.NumDiaSemana, CodCombo = combo.CodCombo.Value }));
                            }

                            comboAlterar.NomeCombo = combo.Nome;
                            comboAlterar.DescricaoCombo = combo.Descricao;
                            comboAlterar.Ativo = combo.Ativo;
                            comboAlterar.PrecoCombo = combo.Preco;
                            comboAlterar.DataHoraInicio = combo.DataHoraInicio;
                            comboAlterar.DataHoraFim = combo.DataHoraFim;

                            await _contexto.SaveChangesAsync();
                            dbContextTransaction.Commit();
                        }

                        return combo;
                    }
                    else if (modoCadastro == "I") //inclusão
                    {
                        var comboIncluir = new Combo();
                        if (combo.CodCombo == null)
                        {
                            comboIncluir.CodCombo = 1;
                            var cod = _contexto.Combos.Select(o => o.CodCombo).DefaultIfEmpty(-1).Max();
                            if (cod > 0)
                            {
                                comboIncluir.CodCombo = cod + 1;
                            }
                            combo.CodCombo = comboIncluir.CodCombo;
                        }
                        else
                        {
                            var valida = _contexto.Combos.Find(combo.CodCombo);

                            if (valida != null)
                            {
                                throw new Exception("Já existe um combo cadastrado com o código " + combo.CodCombo);
                            }

                            comboIncluir.CodCombo = combo.CodCombo.Value;
                        }

                        comboIncluir.NomeCombo = combo.Nome;
                        comboIncluir.DescricaoCombo = combo.Descricao;
                        comboIncluir.Ativo = combo.Ativo;
                        comboIncluir.PrecoCombo = combo.Preco;
                        comboIncluir.DataHoraInicio = combo.DataHoraInicio;
                        comboIncluir.DataHoraFim = combo.DataHoraFim;

                        _contexto.Combos.Add(comboIncluir);

                        if (combo.Itens != null)
                        {
                            _contexto.ItensCombo.AddRange(combo.Itens.GroupBy(i => i.CodItemCardapio).Select(o => new ComboItemCardapio { CodCombo = combo.CodCombo.Value, CodItemCardapio = o.First().CodItemCardapio, Quantidade = o.Sum(x => x.Quantidade) }));
                        }

                        if (combo.DiasAssociados != null)
                        {
                            _contexto.DiasSemanaCombo.AddRange(combo.DiasAssociados.Select(o => new DiaSemanaCombo { DiaSemana = o.NumDiaSemana, CodCombo = combo.CodCombo.Value }));
                        }

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();

                        return combo;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return null;
        }

        public async Task<string> ExcluiCombo(ComboViewModel combo)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var comboExcluir = await _contexto.Combos.FindAsync(combo.CodCombo);

                    if (comboExcluir != null)
                    {
                        _contexto.ItensCombo.RemoveRange(_contexto.ItensCombo.Where(i => i.CodCombo == combo.CodCombo).ToList());
                        _contexto.DiasSemanaCombo.RemoveRange(_contexto.DiasSemanaCombo.Where(i => i.CodCombo == combo.CodCombo).ToList());

                        _contexto.Combos.Remove(comboExcluir);
                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return "Registro não encontrado na base de dados.";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }


            return "";
        }

        #endregion

        #region Cadastros de promoções de venda

        public async Task<List<TipoAplicacaoDescontoPromocaoViewModel>> GetTiposAplicacaoDesconto()
        {
            return await _contexto.TiposDescontoPromocao
                .OrderBy(d => d.CodTipoAplicacaoDesconto)
                .Select(d => new TipoAplicacaoDescontoPromocaoViewModel
                {
                    CodTipoAplicacaoDesconto = d.CodTipoAplicacaoDesconto,
                    Descricao = d.Descricao
                }).ToListAsync();
        }

        public async Task<List<PromocaoVendaViewModel>> GetPromocoesVenda()
        {
            var programas = await (from promos in _contexto.PromocoesVenda
                                    .Include(p => p.DiasAssociados)
                                    .Include(p => p.ClassesAssociadas)
                                    .Include(p => p.ClassesAssociadas.Select(c => c.Classe))
                                    .Include(p => p.ItensAssociados)
                                    .Include(p => p.ItensAssociados.Select(i => i.Item))
                                   join tipo in _contexto.TiposDescontoPromocao on promos.CodTipoAplicacaoDesconto equals tipo.CodTipoAplicacaoDesconto
                                   where promos.PromocaoAtiva
                                   orderby promos.CodPromocaoVenda
                                   select new PromocaoVendaViewModel
                                   {
                                       CodPromocaoVenda = promos.CodPromocaoVenda,
                                       DescricaoPromocao = promos.DescricaoPromocao,
                                       DataHoraInicio = promos.DataHoraInicio,
                                       DataHoraFim = promos.DataHoraFim,
                                       CodTipoAplicacaoDesconto = promos.CodTipoAplicacaoDesconto,
                                       DescricaoTipoAplicacaoDesconto = tipo.Descricao,
                                       PercentualDesconto = promos.PercentualDesconto,
                                       PromocaoAtiva = promos.PromocaoAtiva,
                                       DiasAssociados = promos.DiasAssociados.Select(d => new DiaSemanaViewModel
                                       {
                                           NumDiaSemana = d.DiaSemana
                                       }).ToList(),
                                       ClassesAssociadas = promos.ClassesAssociadas.Select(c => new ClasseItemCardapioPromocaoVendaViewModel
                                       {
                                           CodClasse = c.CodClasse,
                                           CodPromocaoVenda = promos.CodPromocaoVenda,
                                           DescricaoClasse = c.Classe.DescricaoClasse
                                       }).ToList(),
                                       ItensAssociados = promos.ItensAssociados.Select(i => new ItemCardapioPromocaoVendaViewModel
                                       {
                                           CodPromocaoVenda = promos.CodPromocaoVenda,
                                           CodItemCardapio = i.CodItemCardapio,
                                           Nome = i.Item.Nome
                                       }).ToList()
                                   }).ToListAsync();

            foreach (var promo in programas)
            {
                promo.DataInicio = promo.DataHoraInicio.ToString("dd/MM/yyyy");
                promo.HoraInicio = promo.DataHoraInicio.ToString("HH:mm");
                promo.DataFim = promo.DataHoraFim.ToString("dd/MM/yyyy");
                promo.HoraFim = promo.DataHoraFim.ToString("HH:mm");
            }

            return programas;
        }

        public async Task<string> ExcluiPromocaoVenda(PromocaoVendaViewModel promo)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var promoExcluir = await _contexto.PromocoesVenda.FindAsync(promo.CodPromocaoVenda);

                    if (promoExcluir != null)
                    {
                        var diasSemanaExcluir = await _contexto.DiasSemanaPromocaoVenda.Where(d => d.CodPromocaoVenda == promo.CodPromocaoVenda).ToListAsync();
                        if (diasSemanaExcluir != null)
                        {
                            _contexto.DiasSemanaPromocaoVenda.RemoveRange(diasSemanaExcluir);
                        }

                        var classesExcluir = await _contexto.ClassesPromocaoVenda.Where(c => c.CodPromocaoVenda == promo.CodPromocaoVenda).ToListAsync();
                        if (classesExcluir != null)
                        {
                            _contexto.ClassesPromocaoVenda.RemoveRange(classesExcluir);
                        }

                        var itensExcluir = await _contexto.ItensPromocaoVenda.Where(i => i.CodPromocaoVenda == promo.CodPromocaoVenda).ToListAsync();
                        if (itensExcluir != null)
                        {
                            _contexto.ItensPromocaoVenda.RemoveRange(itensExcluir);
                        }

                        _contexto.PromocoesVenda.Remove(promoExcluir);
                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return "Registro não encontrado na base de dados.";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }


            return "";
        }


        public async Task<PromocaoVendaViewModel> GravarPromocaoVenda(PromocaoVendaViewModel promocao, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    promocao.DataHoraInicio = Convert.ToDateTime(promocao.DataInicio.Substring(0, 2) + "/" + promocao.DataInicio.Substring(3, 2) + "/" + promocao.DataInicio.Substring(6) + " " + promocao.HoraInicio.Substring(0, 2) + ":" + promocao.HoraInicio.Substring(3));
                    promocao.DataHoraFim = Convert.ToDateTime(promocao.DataFim.Substring(0, 2) + "/" + promocao.DataFim.Substring(3, 2) + "/" + promocao.DataFim.Substring(6) + " " + promocao.HoraFim.Substring(0, 2) + ":" + promocao.HoraFim.Substring(3));

                    if (promocao.DataHoraFim < promocao.DataHoraInicio)
                    {
                        throw new Exception("O término da vigência não pode ser anterior ao início.");
                    }

                    if (modoCadastro == "A") //alteração
                    {
                        var promocaoAlterar = _contexto.PromocoesVenda.Find(promocao.CodPromocaoVenda);

                        if (promocaoAlterar != null)
                        {
                            if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorClasse && promocao.ClassesAssociadas != null)
                            {
                                _contexto.ClassesPromocaoVenda.RemoveRange(_contexto.ClassesPromocaoVenda.Where(c => c.CodPromocaoVenda == promocao.CodPromocaoVenda).ToList());
                                _contexto.ClassesPromocaoVenda.AddRange(promocao.ClassesAssociadas.Select(o => new ClasseItemCardapioPromocaoVenda { CodClasse = o.CodClasse, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                            }
                            else if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorItem && promocao.ItensAssociados != null)
                            {
                                _contexto.ItensPromocaoVenda.RemoveRange(_contexto.ItensPromocaoVenda.Where(i => i.CodPromocaoVenda == promocao.CodPromocaoVenda).ToList());
                                _contexto.ItensPromocaoVenda.AddRange(promocao.ItensAssociados.Select(o => new ItemCardapioPromocaoVenda { CodItemCardapio = o.CodItemCardapio, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                            }

                            if (promocao.DiasAssociados != null)
                            {
                                _contexto.DiasSemanaPromocaoVenda.RemoveRange(_contexto.DiasSemanaPromocaoVenda.Where(d => d.CodPromocaoVenda == promocao.CodPromocaoVenda).ToList());
                                _contexto.DiasSemanaPromocaoVenda.AddRange(promocao.DiasAssociados.Select(o => new DiaSemanaPromocaoVenda { DiaSemana = o.NumDiaSemana, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                            }

                            promocaoAlterar.DescricaoPromocao = promocao.DescricaoPromocao;
                            promocaoAlterar.CodTipoAplicacaoDesconto = promocao.CodTipoAplicacaoDesconto;
                            promocaoAlterar.PromocaoAtiva = promocao.PromocaoAtiva;
                            promocaoAlterar.PercentualDesconto = promocao.PercentualDesconto;
                            promocaoAlterar.DataHoraInicio = promocao.DataHoraInicio;
                            promocaoAlterar.DataHoraFim = promocao.DataHoraFim;

                            await _contexto.SaveChangesAsync();
                            dbContextTransaction.Commit();
                        }

                        return promocao;
                    }
                    else if (modoCadastro == "I") //inclusão
                    {
                        var promocaoIncluir = new PromocaoVenda();
                        if (promocao.CodPromocaoVenda <= 0)
                        {
                            promocaoIncluir.CodPromocaoVenda = 1;
                            var cod = _contexto.PromocoesVenda.Select(o => o.CodPromocaoVenda).DefaultIfEmpty(-1).Max();
                            if (cod > 0)
                            {
                                promocaoIncluir.CodPromocaoVenda = cod + 1;
                            }
                            promocao.CodPromocaoVenda = promocaoIncluir.CodPromocaoVenda;
                        }
                        else
                        {
                            var valida = _contexto.PromocoesVenda.Find(promocao.CodPromocaoVenda);

                            if (valida != null)
                            {
                                throw new Exception("Já existe uma promoção cadastrada com o código " + promocao.CodPromocaoVenda);
                            }

                            promocaoIncluir.CodPromocaoVenda = promocao.CodPromocaoVenda;
                        }

                        promocaoIncluir.DescricaoPromocao = promocao.DescricaoPromocao;
                        promocaoIncluir.CodTipoAplicacaoDesconto = promocao.CodTipoAplicacaoDesconto;
                        promocaoIncluir.PromocaoAtiva = promocao.PromocaoAtiva;
                        promocaoIncluir.PercentualDesconto = promocao.PercentualDesconto;
                        promocaoIncluir.DataHoraInicio = promocao.DataHoraInicio;
                        promocaoIncluir.DataHoraFim = promocao.DataHoraFim;

                        _contexto.PromocoesVenda.Add(promocaoIncluir);

                        if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorClasse && promocao.ClassesAssociadas != null)
                        {
                            _contexto.ClassesPromocaoVenda.AddRange(promocao.ClassesAssociadas.Select(o => new ClasseItemCardapioPromocaoVenda { CodClasse = o.CodClasse, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                        }
                        else if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorItem && promocao.ItensAssociados != null)
                        {
                            _contexto.ItensPromocaoVenda.AddRange(promocao.ItensAssociados.Select(o => new ItemCardapioPromocaoVenda { CodItemCardapio = o.CodItemCardapio, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                        }

                        if (promocao.DiasAssociados != null)
                        {
                            _contexto.DiasSemanaPromocaoVenda.AddRange(promocao.DiasAssociados.Select(o => new DiaSemanaPromocaoVenda { DiaSemana = o.NumDiaSemana, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                        }

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();

                        return promocao;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return null;
        }


        #endregion

        #region Associação de observações a itens de cardápio

        public async Task GravarObservacoesItens(List<ObservacaoProducaoViewModel> obs, int codItemCardapio, int codClasse)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    if (codClasse > 0)
                    {
                        _contexto.ObservacoesPermitidas.RemoveRange(_contexto.ObservacoesPermitidas.Include(o => o.Item).Where(o => o.Item.CodClasse == codClasse));
                        var itens = _contexto.ItensCardapio.Where(i => i.CodClasse == codClasse).ToList();
                        foreach (var item in itens)
                        {
                            _contexto.ObservacoesPermitidas.AddRange(obs.Select(o => new ObservacaoProducaoPermitidaItemCardapio { CodItemCardapio = item.CodItemCardapio, CodObservacao = o.CodObservacao }));
                        }
                    }

                    if (codItemCardapio > 0)
                    {
                        _contexto.ObservacoesPermitidas.RemoveRange(_contexto.ObservacoesPermitidas.Where(o => o.CodItemCardapio == codItemCardapio));
                        _contexto.ObservacoesPermitidas.AddRange(obs.Select(o => new ObservacaoProducaoPermitidaItemCardapio { CodItemCardapio = codItemCardapio, CodObservacao = o.CodObservacao }));
                    }

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu uma falha durante a execução da operação com a seguinte mensagem: " + ex.Message);
                }
            }
        }

        public async Task GravarOpcoesExtraItens(List<OpcaoExtraViewModel> opcoes, int codItemCardapio, int codClasse)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    if (codClasse > 0)
                    {
                        _contexto.ExtrasPermitidos.RemoveRange(_contexto.ExtrasPermitidos.Include(o => o.Item).Where(o => o.Item.CodClasse == codClasse));
                        var itens = _contexto.ItensCardapio.Where(i => i.CodClasse == codClasse).ToList();
                        foreach (var item in itens)
                        {
                            _contexto.ExtrasPermitidos.AddRange(opcoes.Select(o => new OpcaoExtraItemCardapio { CodItemCardapio = item.CodItemCardapio, CodOpcaoExtra = o.CodOpcaoExtra }));
                        }
                    }

                    if (codItemCardapio > 0)
                    {
                        _contexto.ExtrasPermitidos.RemoveRange(_contexto.ExtrasPermitidos.Where(o => o.CodItemCardapio == codItemCardapio));
                        _contexto.ExtrasPermitidos.AddRange(opcoes.Select(o => new OpcaoExtraItemCardapio { CodItemCardapio = codItemCardapio, CodOpcaoExtra = o.CodOpcaoExtra }));
                    }

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu uma falha durante a execução da operação com a seguinte mensagem: " + ex.Message);
                }
            }
        }

        #endregion

        #region Cadastros de opções extra
        public async Task<List<OpcaoExtraViewModel>> GetOpcoesExtra()
        {
            return _contexto.Extras.OrderBy(o => o.CodOpcaoExtra).Select(o => new OpcaoExtraViewModel { CodOpcaoExtra = o.CodOpcaoExtra, DescricaoOpcaoExtra = o.DescricaoOpcaoExtra, Preco = o.Preco }).ToList();
        }

        public async Task<OpcaoExtraViewModel> GravarOpcaoExtra(OpcaoExtraViewModel opcao, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                if (modoCadastro == "A") //alteração
                {
                    var opcaoAlterar = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                    if (opcaoAlterar != null)
                    {
                        opcaoAlterar.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                        opcaoAlterar.Preco = opcao.Preco;

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }

                    return opcao;
                }
                else if (modoCadastro == "I") //inclusão
                {
                    var opcaoIncluir = new OpcaoExtra();
                    if (opcao.CodOpcaoExtra <= 0)
                    {
                        opcaoIncluir.CodOpcaoExtra = 1;
                        int? cod = _contexto.Extras.Max(o => o.CodOpcaoExtra);
                        if (cod != null)
                        {
                            opcaoIncluir.CodOpcaoExtra = cod.Value + 1;
                        }
                        opcao.CodOpcaoExtra = opcaoIncluir.CodOpcaoExtra;
                    }
                    else
                    {
                        var valida = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                        if (valida != null)
                        {
                            throw new Exception("Já existe uma opção extra cadastrada com o código " + opcao.CodOpcaoExtra);
                        }

                        opcaoIncluir.CodOpcaoExtra = opcao.CodOpcaoExtra;
                    }

                    opcaoIncluir.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                    opcaoIncluir.Preco = opcao.Preco;

                    _contexto.Extras.Add(opcaoIncluir);

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    return opcao;
                }
            }

            return null;
        }

        public async Task<string> ExcluiOpcaoExtra(OpcaoExtraViewModel opcao)
        {
            if (_contexto.ExtrasItensPedidos.Where(i => i.CodOpcaoExtra == opcao.CodOpcaoExtra).Count() > 0)
            {
                return "Exclusão não permitida. Esta opção extra já foi utilizada em pedidos registrados na base.";
            }

            if (_contexto.ExtrasPermitidos.Where(i => i.CodOpcaoExtra == opcao.CodOpcaoExtra).Count() > 0)
            {
                return "Exclusão não permitida. Esta opção extra está associada a itens de cardápio.";
            }

            var opcaoExcluir = await _contexto.Extras.FindAsync(opcao.CodOpcaoExtra);

            if (opcaoExcluir != null)
            {
                _contexto.Extras.Remove(opcaoExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion

        #region Cadastros de observações
        public async Task<List<ObservacaoProducaoViewModel>> GetObservacoes()
        {
            return _contexto.ObservacoesProducao.OrderBy(o => o.CodObservacao).Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.CodObservacao, DescricaoObservacao = o.DescricaoObservacao }).ToList();
        }

        public async Task<ObservacaoProducaoViewModel> GravarObservacao(ObservacaoProducaoViewModel obs, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var obsAlterar = _contexto.ObservacoesProducao.Find(obs.CodObservacao);

                if (obsAlterar != null)
                {
                    obsAlterar.DescricaoObservacao = obs.DescricaoObservacao;

                    await _contexto.SaveChangesAsync();
                }

                return obs;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var obsIncluir = new ObservacaoProducao();
                if (obs.CodObservacao <= 0)
                {
                    obsIncluir.CodObservacao = 1;
                    int? cod = _contexto.ObservacoesProducao.Max(o => o.CodObservacao);
                    if (cod != null)
                    {
                        obsIncluir.CodObservacao = cod.Value + 1;
                    }
                    obs.CodObservacao = obsIncluir.CodObservacao;
                }
                else
                {
                    var valida = _contexto.ObservacoesProducao.Find(obs.CodObservacao);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma observação cadastrada com o código " + obs.CodObservacao);
                    }

                    obsIncluir.CodObservacao = obs.CodObservacao;
                }
                obsIncluir.DescricaoObservacao = obs.DescricaoObservacao;

                _contexto.ObservacoesProducao.Add(obsIncluir);

                await _contexto.SaveChangesAsync();

                return obs;
            }

            return null;
        }

        public async Task<string> ExcluiObservacao(ObservacaoProducaoViewModel obs)
        {
            if (_contexto.ObservacoesItensPedidos.Where(i => i.CodObservacao == obs.CodObservacao).Count() > 0)
            {
                return "Exclusão não permitida. Esta observação já foi utilizada em pedidos registrados na base.";
            }

            if (_contexto.ObservacoesPermitidas.Where(i => i.CodObservacao == obs.CodObservacao).Count() > 0)
            {
                return "Exclusão não permitida. Esta observação está associada a itens de cardápio.";
            }

            var obsExcluir = await _contexto.ObservacoesProducao.FindAsync(obs.CodObservacao);

            if (obsExcluir != null)
            {
                _contexto.ObservacoesProducao.Remove(obsExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion

        #region Parametros de sistema

        public async Task<List<ParametroSistemaViewModel>> GetParametrosSistema()
        {
            return _contexto.ParametrosSistema
                .OrderBy(o => o.CodParametro)
                .Select(o => new ParametroSistemaViewModel
                {
                    CodParametro = o.CodParametro,
                    DescricaoParametro = o.DescricaoParametro,
                    ValorParametro = o.ValorParametro
                }).ToList();
        }

        public async Task<ParametroSistemaViewModel> GravarParametroSistema(ParametroSistemaViewModel par, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var parAlterar = _contexto.ParametrosSistema.Find(par.CodParametro);

                if (parAlterar != null)
                {
                    parAlterar.DescricaoParametro = par.DescricaoParametro;
                    parAlterar.ValorParametro = par.ValorParametro;

                    await _contexto.SaveChangesAsync();
                    BrasaoHamburgueria.Web.Helpers.SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.ParametrosSistema);
                }

                return par;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var parIncluir = new ParametroSistema();
                if (par.CodParametro <= 0)
                {
                    parIncluir.CodParametro = 1;
                    int? cod = _contexto.ParametrosSistema.Max(o => o.CodParametro);
                    if (cod != null)
                    {
                        parIncluir.CodParametro = cod.Value + 1;
                    }
                    par.CodParametro = parIncluir.CodParametro;
                }
                else
                {
                    var valida = _contexto.ParametrosSistema.Find(par.CodParametro);

                    if (valida != null)
                    {
                        throw new Exception("Já existe um parâmetro cadastrado com o código " + par.CodParametro);
                    }

                    parIncluir.CodParametro = par.CodParametro;
                }
                parIncluir.DescricaoParametro = par.DescricaoParametro;
                parIncluir.ValorParametro = par.ValorParametro;

                _contexto.ParametrosSistema.Add(parIncluir);

                await _contexto.SaveChangesAsync();
                BrasaoHamburgueria.Web.Helpers.SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.ParametrosSistema);

                return par;
            }

            return null;
        }

        #endregion

        #region Impressoras de produção

        public async Task<List<ImpressoraProducaoViewModel>> GetImpressorasProducao()
        {
            return _contexto.ImpressorasProducao
                .OrderBy(o => o.CodImpressora)
                .Select(o => new ImpressoraProducaoViewModel
                {
                    CodImpressora = o.CodImpressora,
                    Descricao = o.Descricao,
                    Porta = o.Porta
                }).ToList();
        }

        public async Task<ImpressoraProducaoViewModel> GravarImpressoraProducao(ImpressoraProducaoViewModel imp, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var impAlterar = _contexto.ImpressorasProducao.Find(imp.CodImpressora);

                if (impAlterar != null)
                {
                    impAlterar.Descricao = imp.Descricao;
                    impAlterar.Porta = imp.Porta;

                    await _contexto.SaveChangesAsync();
                }

                return imp;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var impIncluir = new ImpressoraProducao();
                if (imp.CodImpressora <= 0)
                {
                    impIncluir.CodImpressora = 1;
                    int? cod = _contexto.ImpressorasProducao.Max(o => o.CodImpressora);
                    if (cod != null)
                    {
                        impIncluir.CodImpressora = cod.Value + 1;
                    }
                    imp.CodImpressora = impIncluir.CodImpressora;
                }
                else
                {
                    var valida = _contexto.ImpressorasProducao.Find(imp.CodImpressora);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma impressora cadastrada com o código " + imp.CodImpressora);
                    }

                    impIncluir.CodImpressora = imp.CodImpressora;
                }
                impIncluir.Descricao = imp.Descricao;
                impIncluir.Porta = imp.Porta;

                _contexto.ImpressorasProducao.Add(impIncluir);

                await _contexto.SaveChangesAsync();

                return imp;
            }

            return null;
        }

        public async Task<string> ExcluiImpressoraProducao(ImpressoraProducaoViewModel imp)
        {
            if (_contexto.ImpressorasItens.Where(i => i.CodImpressora == imp.CodImpressora).Count() > 0)
            {
                return "Exclusão não permitida. Esta impressora está associada a itens de cardápio.";
            }

            if (_contexto.Classes.Where(i => i.CodImpressoraPadrao == imp.CodImpressora).Count() > 0)
            {
                return "Exclusão não permitida. Esta impressora está associada a classes de itens de cardápio.";
            }

            var impExcluir = await _contexto.ImpressorasProducao.FindAsync(imp.CodImpressora);

            if (impExcluir != null)
            {
                _contexto.ImpressorasProducao.Remove(impExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }

        #endregion

        #region Cadastros de item de cardápio
        public async Task<List<DadosItemCardapioViewModel>> GetItensCardapioByNome(string chave)
        {
            return _contexto.ItensCardapio
                .Where(i => i.Nome.Contains(chave))
                .Include(i => i.ObservacoesPermitidas)
                .Include(i => i.ObservacoesPermitidas.Select(o => o.ObservacaoProducao))
                .Include(i => i.ExtrasPermitidos)
                .Include(i => i.ExtrasPermitidos.Select(o => o.OpcaoExtra))
                .OrderBy(i => i.Nome).Take(10)
                .Select(i => new DadosItemCardapioViewModel
                {
                    CodItemCardapio = i.CodItemCardapio,
                    Nome = i.Nome,
                    Observacoes = i.ObservacoesPermitidas.Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.ObservacaoProducao.CodObservacao, DescricaoObservacao = o.ObservacaoProducao.DescricaoObservacao }).ToList(),
                    Extras = i.ExtrasPermitidos.Select(o => new OpcaoExtraViewModel { CodOpcaoExtra = o.OpcaoExtra.CodOpcaoExtra, DescricaoOpcaoExtra = o.OpcaoExtra.DescricaoOpcaoExtra, Preco = o.OpcaoExtra.Preco }).ToList()
                }).ToList();
        }

        public async Task<List<ItemCardapioViewModel>> GetItensCardapio()
        {
            var lista = (
                from itens in _contexto.ItensCardapio
                from classes in _contexto.Classes.Where(c => c.CodClasse == itens.CodClasse)
                from complementos in _contexto.ComplementosItens
                    .Where(c => c.CodItemCardapio == itens.CodItemCardapio).DefaultIfEmpty()
                from imps in _contexto.ImpressorasItens
                    .Where(i => i.CodItemCardapio == itens.CodItemCardapio).Take(1).DefaultIfEmpty()
                select new ItemCardapioViewModel
                {
                    CodItemCardapio = itens.CodItemCardapio,
                    CodClasse = itens.CodClasse,
                    DescricaoClasse = classes.DescricaoClasse,
                    CodImpressoraProducao = imps.CodImpressora,
                    Complemento = new ComplementoItemCardapioViewModel
                    {
                        DescricaoLonga = complementos.DescricaoLonga,
                        Imagem = complementos.Imagem,
                        OrdemExibicao = complementos.OrdemExibicao
                    },
                    Nome = itens.Nome,
                    Preco = itens.Preco,
                    Sincronizar = itens.Sincronizar,
                    Ativo = itens.Ativo
                }
                ).ToList();

            foreach (var item in lista)
            {
                if (item.Complemento != null && !String.IsNullOrEmpty(item.Complemento.Imagem))
                {
                    item.Complemento.ImagemMini = item.Complemento.Imagem.Replace("img_item", "mini-img_item");
                }
            }

            return lista;
        }

        public async Task<string> ExcluiItemCardapio(ItemCardapioViewModel item)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                if (_contexto.ItensPedidos.Where(i => i.CodItemCardapio == item.CodItemCardapio).Count() > 0)
                {
                    return "Exclusão não permitida. Este item está associado a pedidos realizados.";
                }

                var itemExcluir = await _contexto.ItensCardapio.FindAsync(item.CodItemCardapio);

                if (itemExcluir != null)
                {
                    var complementoExcluir = await _contexto.ComplementosItens.FindAsync(item.CodItemCardapio);

                    if (complementoExcluir != null)
                    {
                        _contexto.ComplementosItens.Remove(complementoExcluir);
                    }

                    var impressorasExcluir = _contexto.ImpressorasItens.Where(i => i.CodItemCardapio == item.CodItemCardapio).ToList();
                    if (impressorasExcluir != null)
                    {
                        foreach (var imp in impressorasExcluir)
                        {
                            _contexto.ImpressorasItens.Remove(imp);
                        }
                    }

                    _contexto.ItensCardapio.Remove(itemExcluir);
                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                else
                {
                    return "Registro não encontrado na base de dados.";
                }
            }

            return "";
        }

        public async Task<ItemCardapioViewModel> GravarItemCardapio(ItemCardapioViewModel item, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                if (modoCadastro == "A") //alteração
                {
                    var itemAlterar = _contexto.ItensCardapio.Find(item.CodItemCardapio);

                    if (itemAlterar != null)
                    {
                        if (item.Preco <= 0)
                        {
                            throw new Exception("O preço informado não é valido. Informe um número maior que zero.");
                        }

                        itemAlterar.Nome = item.Nome;
                        itemAlterar.Preco = item.Preco;
                        itemAlterar.Sincronizar = item.Sincronizar;
                        itemAlterar.Ativo = item.Ativo;
                        itemAlterar.CodClasse = item.CodClasse;

                        var complementoAlterar = _contexto.ComplementosItens.Find(item.CodItemCardapio);
                        var semComplemento = false;
                        if (complementoAlterar == null)
                        {
                            complementoAlterar = new ComplementoItemCardapio();
                            complementoAlterar.CodItemCardapio = itemAlterar.CodItemCardapio;
                            semComplemento = true;
                        }

                        if (complementoAlterar != null)
                        {
                            complementoAlterar.DescricaoLonga = item.Complemento.DescricaoLonga;
                            complementoAlterar.Imagem = item.Complemento.Imagem;
                            if (item.Complemento.OrdemExibicao != null)
                            {
                                complementoAlterar.OrdemExibicao = item.Complemento.OrdemExibicao.Value;
                            }

                            if (semComplemento)
                            {
                                _contexto.ComplementosItens.Add(complementoAlterar);
                            }
                        }

                        var impressorasProducaoItem = _contexto.ImpressorasItens.Where(i => i.CodItemCardapio == item.CodItemCardapio).ToList();
                        if (impressorasProducaoItem != null)
                        {
                            foreach (var imp in impressorasProducaoItem)
                            {
                                _contexto.ImpressorasItens.Remove(imp);
                            }
                        }

                        if (item.CodImpressoraProducao != null)
                        {
                            var impressoraProducaoItem = new ItemCardapioImpressora();
                            impressoraProducaoItem.CodItemCardapio = item.CodItemCardapio;
                            impressoraProducaoItem.CodImpressora = item.CodImpressoraProducao.Value;

                            _contexto.ImpressorasItens.Add(impressoraProducaoItem);
                        }

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }

                    return item;
                }
                else if (modoCadastro == "I") //inclusão
                {
                    var itemIncluir = new ItemCardapio();
                    if (item.CodItemCardapio <= 0)
                    {
                        itemIncluir.CodItemCardapio = 1;
                        int? cod = _contexto.ItensCardapio.Max(o => o.CodItemCardapio);
                        if (cod != null)
                        {
                            itemIncluir.CodItemCardapio = cod.Value + 1;
                        }
                        item.CodItemCardapio = itemIncluir.CodItemCardapio;
                    }
                    else
                    {
                        var valida = _contexto.ItensCardapio.Find(item.CodItemCardapio);

                        if (valida != null)
                        {
                            throw new Exception("Já existe um item de cardápio cadastrado com o código " + item.CodItemCardapio);
                        }

                        itemIncluir.CodItemCardapio = item.CodItemCardapio;
                    }

                    if (item.Preco <= 0)
                    {
                        throw new Exception("O preço informado não é valido. Informe um número maior que zero.");
                    }

                    itemIncluir.Nome = item.Nome;
                    itemIncluir.Preco = item.Preco;
                    itemIncluir.Sincronizar = item.Sincronizar;
                    itemIncluir.Ativo = item.Ativo;
                    itemIncluir.CodClasse = item.CodClasse;

                    ComplementoItemCardapio complementoIncluir = new ComplementoItemCardapio();

                    complementoIncluir.CodItemCardapio = item.CodItemCardapio;
                    complementoIncluir.DescricaoLonga = item.Complemento.DescricaoLonga;
                    complementoIncluir.Imagem = item.Complemento.Imagem;
                    if (item.Complemento.OrdemExibicao != null)
                    {
                        complementoIncluir.OrdemExibicao = item.Complemento.OrdemExibicao.Value;
                    }

                    _contexto.ItensCardapio.Add(itemIncluir);
                    _contexto.ComplementosItens.Add(complementoIncluir);

                    if (item.CodImpressoraProducao != null)
                    {
                        var impressoraProducaoItem = new ItemCardapioImpressora();
                        impressoraProducaoItem.CodItemCardapio = item.CodItemCardapio;
                        impressoraProducaoItem.CodImpressora = item.CodImpressoraProducao.Value;

                        _contexto.ImpressorasItens.Add(impressoraProducaoItem);
                    }

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    return item;
                }
            }

            return null;
        }

        public string GravarImagemItemCardapio(HttpPostedFileBase file, int codItemCardapio, string serverPath)
        {
            var extensao = file.FileName.Split('.')[1].ToString();
            var imgPath = serverPath + @"Content\img\itens_cardapio\" + "img_item" + codItemCardapio.ToString() + "." + extensao;
            file.SaveAs(imgPath);

            if (file.ContentLength > 500000)
            {
                throw new Exception("A imagem deve ter no máximo 500Kb.");
            }

            var thumbPath = serverPath + @"Content\img\itens_cardapio\" + "mini-img_item" + codItemCardapio.ToString() + "." + extensao;

            //cria miniatura

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            Image image = Image.FromFile(imgPath);

            int height = 150;
            int width = Convert.ToInt32(height * (Convert.ToDecimal(image.Width) / Convert.ToDecimal(image.Height)));

            Image thumb = image.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            thumb.Save(thumbPath);

            image.Dispose();

            //grava caminho da imagem no registro da classe
            var complemento = _contexto.ComplementosItens.Find(codItemCardapio);

            if (complemento != null)
            {
                complemento.Imagem = @"Content/img/itens_cardapio/" + "img_item" + codItemCardapio.ToString() + "." + extensao;
                _contexto.SaveChanges();

                return complemento.Imagem;
            }

            return "";
        }

        public void RemoverImagemItemCardapio(ItemCardapioViewModel item, string serverPath)
        {
            var complementoDb = _contexto.ComplementosItens.Find(item.CodItemCardapio);

            if (complementoDb == null || String.IsNullOrEmpty(complementoDb.Imagem))
            {
                return;
            }

            var array = complementoDb.Imagem.Split('/');
            var imagem = serverPath + @"Content\img\itens_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            array = item.Complemento.ImagemMini.Split('/');
            imagem = serverPath + @"Content\img\classes_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            complementoDb.Imagem = null;
            _contexto.SaveChanges();

        }

        #endregion

        #region Cadastros de classes de item de cardápio
        public async Task<List<ClasseItemCardapioViewModel>> GetClassesItemCardapio()
        {
            var lista = _contexto.Classes
                .OrderBy(o => o.CodClasse)
                .Select(o => new ClasseItemCardapioViewModel
                {
                    CodClasse = o.CodClasse,
                    DescricaoClasse = o.DescricaoClasse,
                    CodImpressoraPadrao = o.CodImpressoraPadrao,
                    Sincronizar = o.Sincronizar,
                    Imagem = o.Imagem,
                    OrdemExibicao = o.OrdemExibicao,
                    DescricaoImpressoraPadrao = _contexto.ImpressorasProducao.Where(i => i.CodImpressora == o.CodImpressoraPadrao).FirstOrDefault().Descricao
                }).OrderBy(c => c.DescricaoClasse).ToList();

            foreach (var classe in lista)
            {
                if (!String.IsNullOrEmpty(classe.Imagem))
                {
                    classe.ImagemMini = classe.Imagem.Replace("img_classe", "mini-img_classe");
                }
            }

            return lista;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        public void RemoverImagemClasse(ClasseItemCardapioViewModel classe, string serverPath)
        {
            var classeDb = _contexto.Classes.Find(classe.CodClasse);

            var array = classe.Imagem.Split('/');
            var imagem = serverPath + @"Content\img\classes_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            array = classe.ImagemMini.Split('/');
            imagem = serverPath + @"Content\img\classes_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            if (classeDb != null)
            {

                classeDb.Imagem = null;
                _contexto.SaveChanges();

            }
        }

        public string GravarImagemClasse(HttpPostedFileBase file, int codClasse, string serverPath)
        {
            var extensao = file.FileName.Split('.')[1].ToString();
            var imgPath = serverPath + @"Content\img\classes_cardapio\" + "img_classe" + codClasse.ToString() + "." + extensao;
            file.SaveAs(imgPath);

            if (file.ContentLength > 500000)
            {
                throw new Exception("A imagem deve ter no máximo 500Kb.");
            }

            var thumbPath = serverPath + @"Content\img\classes_cardapio\" + "mini-img_classe" + codClasse.ToString() + "." + extensao;

            //cria miniatura

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            Image image = Image.FromFile(imgPath);

            int height = 150;
            int width = Convert.ToInt32(height * (Convert.ToDecimal(image.Width) / Convert.ToDecimal(image.Height)));

            Image thumb = image.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            thumb.Save(thumbPath);

            image.Dispose();

            //grava caminho da imagem no registro da classe
            var classe = _contexto.Classes.Find(codClasse);

            if (classe != null)
            {
                classe.Imagem = @"Content/img/classes_cardapio/" + "img_classe" + codClasse.ToString() + "." + extensao;
                _contexto.SaveChanges();

                return classe.Imagem;
            }

            return "";
        }

        public async Task<ClasseItemCardapioViewModel> GravarClasseItemCardapio(ClasseItemCardapioViewModel classe, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var classeAlterar = _contexto.Classes.Find(classe.CodClasse);

                if (classeAlterar != null)
                {
                    classeAlterar.DescricaoClasse = classe.DescricaoClasse;
                    classeAlterar.CodImpressoraPadrao = classe.CodImpressoraPadrao;
                    classeAlterar.Imagem = classe.Imagem;
                    classeAlterar.OrdemExibicao = classe.OrdemExibicao;
                    classeAlterar.Sincronizar = classe.Sincronizar;

                    await _contexto.SaveChangesAsync();
                }

                return classe;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var classeIncluir = new ClasseItemCardapio();
                if (classe.CodClasse <= 0)
                {
                    classeIncluir.CodClasse = 1;
                    int? cod = _contexto.Classes.Max(o => o.CodClasse);
                    if (cod != null)
                    {
                        classeIncluir.CodClasse = cod.Value + 1;
                    }
                    classe.CodClasse = classeIncluir.CodClasse;
                }
                else
                {
                    var valida = _contexto.Classes.Find(classe.CodClasse);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma classe de item de cardápio cadastrada com o código " + classe.CodClasse);
                    }

                    classeIncluir.CodClasse = classe.CodClasse;
                }
                classeIncluir.DescricaoClasse = classe.DescricaoClasse;
                classeIncluir.CodImpressoraPadrao = classe.CodImpressoraPadrao;
                classeIncluir.Imagem = classe.Imagem;
                classeIncluir.OrdemExibicao = classe.OrdemExibicao;
                classeIncluir.Sincronizar = classe.Sincronizar;

                _contexto.Classes.Add(classeIncluir);

                await _contexto.SaveChangesAsync();

                return classe;
            }

            return null;
        }

        public async Task<string> ExcluiClasseItemCardapio(ClasseItemCardapioViewModel classe)
        {
            if (_contexto.ItensCardapio.Where(i => i.CodClasse == classe.CodClasse).Count() > 0)
            {
                return "Exclusão não permitida. Esta classe está associada a itens de cardápio.";
            }

            var classeExcluir = await _contexto.Classes.FindAsync(classe.CodClasse);

            if (classeExcluir != null)
            {
                _contexto.Classes.Remove(classeExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion

    }
}