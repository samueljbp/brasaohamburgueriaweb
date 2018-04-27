﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using BrasaoHamburgueria.Helper;
using BrasaoHamburgueria.Web.Helpers;

namespace BrasaoHamburgueria.Web.Repository
{
    public class PedidoRepository
    {
        private BrasaoContext _contexto;

        public PedidoRepository()
        {
            _contexto = new BrasaoContext();
        }

        public async Task AplicaDescontoPedido(PedidoViewModel pedido)
        {
            var ped = _contexto.Pedidos.Where(p => p.CodPedido == pedido.CodPedido).FirstOrDefault();

            if (ped != null)
            {
                ped.ValorDesconto = pedido.ValorDesconto;
                ped.PercentualDesconto = pedido.PercentualDesconto;
                ped.MotivoDesconto = pedido.MotivoDesconto;

                if (ped.CodFormaPagamento == "D" && ped.TrocoPara != null && ped.TrocoPara.Value > 0)
                {
                    ped.Troco = ped.TrocoPara - ped.ValorTotal - ped.ValorDesconto.Value;
                }

                await _contexto.SaveChangesAsync();
            }
        }

        public async Task AlteraSituacaoPedido(PedidoViewModel pedido, string loginUsuario)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var ped = _contexto.Pedidos.Where(p => p.CodPedido == pedido.CodPedido).FirstOrDefault();

                    if (ped != null)
                    {
                        ped.CodSituacao = pedido.Situacao;

                        if (ped.CodSituacao == (int)SituacaoPedidoEnum.Concluido)
                        {
                            ped.FeedbackCliente = pedido.FeedbackCliente;
                        }

                        if (ped.CodSituacao == (int)SituacaoPedidoEnum.EmProcessoEntrega)
                        {
                            ped.CodEntregador = pedido.CodEntregador;
                        }

                        decimal saldoAtualizadoPrograma = -1;
                        if (ped.CodSituacao == (int)SituacaoPedidoEnum.Cancelado)
                        {
                            ped.MotivoCancelamento = pedido.MotivoCancelamento;

                            //no caso do cancelamento do pedido, se o usuário estiver em programa de recompensa, estorna o saldo

                            ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository();
                            var programa = _progRep.GetProgramaFidelidadeUsuario(loginUsuario);

                            var linhasEstornar = _contexto.ExtratosUsuariosProgramasFidelidade.Where(e => e.CodPedido == pedido.CodPedido).ToList();

                            if (programa != null && programa.LoginUsuario != null && programa.TermosAceitos != null && programa.TermosAceitos.Value && linhasEstornar != null && linhasEstornar.Count > 0)
                            {
                                foreach (var linhaEstornar in linhasEstornar)
                                {
                                    //credita pontos referentes ao pedido
                                    SaldoUsuarioProgramaFidelidade saldo = _contexto.SaldosUsuariosProgramasFidelidade.Where(s => s.CodProgramaFidelidade == programa.CodProgramaFidelidade && s.LoginUsuario == loginUsuario).FirstOrDefault();
                                    var pontosEstornar = linhaEstornar.ValorLancamento;
                                    saldo.Saldo = saldo.Saldo - linhaEstornar.ValorLancamento;
                                    saldoAtualizadoPrograma = saldo.Saldo;

                                    ExtratoUsuarioProgramaFidelidade extrato = new ExtratoUsuarioProgramaFidelidade();
                                    extrato.CodPedido = ped.CodPedido;
                                    extrato.CodProgramaFidelidade = programa.CodProgramaFidelidade;
                                    extrato.DataHoraLancamento = DateTime.Now;
                                    if (linhaEstornar.ValorLancamento > 0)
                                    {
                                        extrato.DescricaoLancamento = "Estorno de " + linhaEstornar.ValorLancamento.ToString("0.00") + " pontos referentes ao cancelamento do pedido " + ped.CodPedido + " de valor " + ped.ValorTotal.ToString("C");
                                    }
                                    else
                                    {
                                        extrato.DescricaoLancamento = "Devolução de " + Math.Abs(linhaEstornar.ValorLancamento).ToString("0.00") + " pontos resgatados referentes ao cancelamento do pedido " + ped.CodPedido;
                                    }

                                    extrato.LoginUsuario = loginUsuario;
                                    extrato.SaldoPosLancamento = saldo.Saldo;
                                    extrato.ValorLancamento = -1 * linhaEstornar.ValorLancamento;
                                    _contexto.ExtratosUsuariosProgramasFidelidade.Add(extrato);

                                    await _contexto.SaveChangesAsync();
                                }
                            }
                        }

                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();

                        if (saldoAtualizadoPrograma >= 0)
                        {
                            SessionData.ProgramaFidelidadeUsuario.Saldo = saldoAtualizadoPrograma;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao executar a transação: " + ex.Message);
                }
            }
        }

        private bool ValidaValorPedido(PedidoViewModel pedidoViewModel)
        {
            if (pedidoViewModel.CodPedido > 0)
            {
                return true;
            }

            var valorTotalPedido = 0.0;
            var valorExtras = 0.0;

            var numDiaHoje = (int)DateTime.Now.DayOfWeek;
            var promocoesAtivas = (from promos in _contexto.PromocoesVenda.Include(p => p.ClassesAssociadas).Include(p => p.ItensAssociados).Include(p => p.DiasAssociados)
                                   where promos.PromocaoAtiva && promos.DataHoraInicio <= DateTime.Now && promos.DataHoraFim >= DateTime.Now
                                   && promos.DiasAssociados.Select(d => d.DiaSemana).Contains(numDiaHoje)
                                   select promos).ToList();

            foreach (var item in pedidoViewModel.Itens)
            {
                valorExtras = 0;
                if (item.extras != null && item.extras.Count > 0)
                {
                    foreach (var extra in item.extras)
                    {
                        if (extra != null)
                        {
                            var extraBase = _contexto.Extras.Find(extra.CodOpcaoExtra);
                            if (extraBase != null)
                            {
                                valorExtras += item.Quantidade * extraBase.Preco;
                            }
                        }
                    }
                }

                var itemBase = _contexto.ItensCardapio.Find(item.CodItem);

                var percentualDesconto = 0.0;
                if (itemBase != null)
                {
                    if (promocoesAtivas != null && promocoesAtivas.Count > 0)
                    {
                        foreach (var promo in promocoesAtivas.OrderBy(p => p.PercentualDesconto).ToList())
                        {
                            if ((promo.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorClasse && promo.ClassesAssociadas.Select(c => c.CodClasse).Contains(itemBase.CodClasse)) ||
                                (promo.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorItem && promo.ItensAssociados.Select(i => i.CodItemCardapio).Contains(itemBase.CodItemCardapio)))
                            {
                                percentualDesconto = (double)promo.PercentualDesconto;
                            }
                        }
                    }

                    valorTotalPedido += item.Quantidade * ((1 - (percentualDesconto / 100)) * itemBase.Preco);
                }
                else if (item.CodCombo != null)
                {
                    var comboBase = _contexto.Combos.Find(item.CodCombo.Value);
                    if (comboBase != null)
                    {
                        valorTotalPedido += item.Quantidade * comboBase.PrecoCombo;
                    }
                }

                valorTotalPedido += valorExtras;
            }

            valorTotalPedido += pedidoViewModel.TaxaEntrega;

            if (pedidoViewModel.ValorDesconto != null && pedidoViewModel.ValorDesconto.Value > 0)
            {
                valorTotalPedido -= pedidoViewModel.ValorDesconto.Value;
            }

            if (pedidoViewModel.DinheiroAUtilizarProgramaRecompensa > 0)
            {
                valorTotalPedido -= (double)pedidoViewModel.DinheiroAUtilizarProgramaRecompensa;
            }

            if (pedidoViewModel.ValorTotal != valorTotalPedido)
            {
                return false;
            }

            return true;
        }

        private void ValidaPedido(PedidoViewModel pedidoViewModel, string loginUsuario)
        {
            if (pedidoViewModel.DadosCliente.Telefone.Length < 14)
            {
                throw new Exception("O telefone não está preenchido corretamente");
            }

            if (pedidoViewModel.PedidoExterno && pedidoViewModel.CodPedido <= 0)
            {
                //var ped = _rep.GetPedidoAberto("", pedidoViewModel.DadosCliente.Telefone).Result;
                var ped = BrasaoHamburgueria.Helper.AsyncHelpers.RunSync<PedidoViewModel>(() => this.GetPedidoAberto("", pedidoViewModel.DadosCliente.Telefone));

                if (ped != null)
                {
                    throw new Exception("O cliente " + pedidoViewModel.DadosCliente.Telefone + " possui o pedido " + ped.CodPedido + " em aberto. Finalize-o antes de fazer outro pedido para este cliente.");
                }
            }

            //primeiro verifica se a casa está aberta para delivery
            if (!pedidoViewModel.PedidoExterno && !ParametroRepository.CasaAberta() && pedidoViewModel.CodPedido <= 0)
            {
                var horarioFuncionamento = ParametroRepository.GetHorarioAbertura();
                throw new Exception("No momento estamos fechados. Abriremos " + horarioFuncionamento.DiaSemana + " das " + horarioFuncionamento.Abertura.ToString("HH:mm") + " às " + horarioFuncionamento.Fechamento.ToString("HH:mm") + ".");
            }

            //valida o valor do pedido contra a base de dados para evitar fraude por manipulação do JS
            if (!ValidaValorPedido(pedidoViewModel))
            {
                throw new Exception("O valor do pedido ou dos itens foi manipulado durante a requisição. Favor tentar novamente.");
            }

            //valida uso do saldo do programa de recompensa
            if (pedidoViewModel.UsaSaldoProgramaFidelidade)
            {
                using (ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository())
                {
                    var programa = _progRep.GetProgramaFidelidadeUsuario(loginUsuario);

                    if (programa == null || programa.LoginUsuario == null || !programa.TermosAceitos.Value)
                    {
                        throw new Exception("O usuário não está inscrito em nenhum programa de fidelidade no momento.");
                    }

                    if (pedidoViewModel.PontosAUtilizarProgramaRecompensa > programa.Saldo.Value)
                    {
                        throw new Exception("O usuário não possui saldo suficiente para a operação. O saldo disponível no programa para este usuário é de " + programa.Saldo.Value + " pontos.");
                    }

                    pedidoViewModel.DinheiroAUtilizarProgramaRecompensa = programa.ValorDinheiroPorPontoParaResgate * pedidoViewModel.PontosAUtilizarProgramaRecompensa;
                }
            }
        }

        public async Task<int> GravaPedido(PedidoViewModel pedidoViewModel, string loginUsuario)
        {
            ValidaPedido(pedidoViewModel, loginUsuario);

            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    Pedido ped;

                    if (pedidoViewModel.CodPedido <= 0)
                    {
                        ped = new Pedido();
                        ped.CodSituacao = pedidoViewModel.Situacao;
                        ped.DataHora = DateTime.Now;
                        ped.Usuario = loginUsuario;
                    }
                    else
                    {
                        ped = _contexto.Pedidos.Find(pedidoViewModel.CodPedido);
                    }


                    ped.CodBandeiraCartao = pedidoViewModel.CodBandeiraCartao;
                    ped.CodFormaPagamento = pedidoViewModel.CodFormaPagamento;
                    ped.NomeCliente = pedidoViewModel.DadosCliente.Nome;
                    ped.TaxaEntrega = pedidoViewModel.TaxaEntrega;
                    ped.CodEntregador = pedidoViewModel.CodEntregador;
                    ped.TelefoneCliente = pedidoViewModel.DadosCliente.Telefone;
                    ped.Troco = pedidoViewModel.Troco;
                    ped.TrocoPara = pedidoViewModel.TrocoPara;
                    ped.ValorTotal = pedidoViewModel.ValorTotal;
                    ped.MotivoCancelamento = pedidoViewModel.MotivoCancelamento;
                    ped.FeedbackCliente = pedidoViewModel.FeedbackCliente;
                    ped.PedidoExterno = pedidoViewModel.PedidoExterno;
                    ped.RetirarNaCasa = pedidoViewModel.RetirarNaCasa;

                    if (!ped.RetirarNaCasa)
                    {
                        ped.BairroEntrega = pedidoViewModel.DadosCliente.Bairro;
                        ped.CidadeEntrega = pedidoViewModel.DadosCliente.Cidade;
                        ped.ComplementoEntrega = pedidoViewModel.DadosCliente.Complemento;
                        ped.LogradouroEntrega = pedidoViewModel.DadosCliente.Logradouro;
                        ped.NumeroEntrega = pedidoViewModel.DadosCliente.Numero;
                        ped.ReferenciaEntrega = pedidoViewModel.DadosCliente.Referencia;
                        ped.UFEntrega = pedidoViewModel.DadosCliente.Estado;
                    }

                    if (pedidoViewModel.CodPedido <= 0)
                    {
                        _contexto.Pedidos.Add(ped);
                    }

                    await _contexto.SaveChangesAsync();

                    ItemPedido item;

                    var itens = pedidoViewModel.Itens.Where(i => i.CodCombo == null).ToList();

                    //combos são explodidos em itens e tem seus itens gravados individualmente. A forma de identificar é a coluna cod_combo. Na hora de recuperar os itens precisa agrupar novamente
                    var combos = pedidoViewModel.Itens.Where(i => i.CodCombo != null);
                    var maxSeqItem = pedidoViewModel.Itens.Max(i => i.SeqItem);
                    foreach (var combo in combos)
                    {
                        var itensCombo = _contexto.ItensCombo.Include(i => i.Item).Where(c => c.CodCombo == combo.CodCombo.Value);
                        foreach (var ic in itensCombo)
                        {
                            maxSeqItem += 1;
                            ItemPedidoViewModel icPedido = new ItemPedidoViewModel();
                            icPedido.CodItem = ic.CodItemCardapio;
                            icPedido.ObservacaoLivre = combo.ObservacaoLivre;
                            icPedido.PrecoUnitario = ic.Item.Preco;
                            icPedido.Quantidade = combo.Quantidade;
                            icPedido.SeqItem = maxSeqItem;
                            icPedido.ValorExtras = combo.ValorExtras;
                            icPedido.ValorTotalItem = icPedido.Quantidade * icPedido.PrecoUnitario;
                            icPedido.CodCombo = combo.CodCombo;
                            icPedido.PrecoCombo = combo.PrecoCombo;
                            itens.Add(icPedido);
                        }
                    }

                    foreach (var itemViewModel in itens)
                    {
                        if (itemViewModel.AcaoRegistro == (int)Comum.AcaoRegistro.Incluir)
                        {
                            item = new ItemPedido();

                            item.CodItemCardapio = itemViewModel.CodItem;
                            item.CodPedido = ped.CodPedido;
                            item.ObservacaoLivre = itemViewModel.ObservacaoLivre;
                            item.PrecoUnitario = itemViewModel.PrecoUnitario;
                            item.Quantidade = itemViewModel.Quantidade;
                            item.SeqItem = itemViewModel.SeqItem;
                            item.ValorExtras = itemViewModel.ValorExtras;
                            item.ValorTotal = itemViewModel.ValorTotalItem;
                            item.CodCombo = itemViewModel.CodCombo;
                            item.PrecoCombo = itemViewModel.PrecoCombo;

                            if (itemViewModel.CodPromocaoVenda != null)
                            {
                                item.CodPromocaoVenda = itemViewModel.CodPromocaoVenda;
                                item.PercentualDesconto = itemViewModel.PercentualDesconto;
                                item.ValorDesconto = itemViewModel.Quantidade * (itemViewModel.PrecoUnitario - itemViewModel.PrecoUnitarioComDesconto);
                            }

                            _contexto.ItensPedidos.Add(item);

                            if (itemViewModel.Obs != null)
                            {
                                _contexto.ObservacoesItensPedidos.AddRange(itemViewModel.Obs.Where(o => o != null && o.CodObservacao > 0).Select(o => new ObservacaoItemPedido { CodPedido = item.CodPedido, SeqItem = item.SeqItem, CodObservacao = o.CodObservacao }).ToList());
                            }

                            if (itemViewModel.extras != null)
                            {
                                _contexto.ExtrasItensPedidos.AddRange(itemViewModel.extras.Where(e => e != null).Select(o => new ExtraItemPedido { CodPedido = item.CodPedido, SeqItem = item.SeqItem, CodOpcaoExtra = o.CodOpcaoExtra, Preco = o.Preco }).ToList());
                            }
                        }
                        else if (itemViewModel.AcaoRegistro == (int)Comum.AcaoRegistro.Cancelar)
                        {
                            item = _contexto.ItensPedidos.Find(pedidoViewModel.CodPedido, itemViewModel.SeqItem);

                            item.Cancelado = true;
                        }
                    }

                    await _contexto.SaveChangesAsync();

                    decimal saldoAtualizadoPrograma = -1;

                    if (!pedidoViewModel.PedidoExterno)
                    {
                        ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository();
                        var programa = _progRep.GetProgramaFidelidadeUsuario(loginUsuario);

                        if (programa != null && programa.LoginUsuario != null && programa.TermosAceitos != null && programa.TermosAceitos.Value && pedidoViewModel.ValorTotal > 0)
                        {
                            //credita pontos referentes ao pedido
                            SaldoUsuarioProgramaFidelidade saldo = _contexto.SaldosUsuariosProgramasFidelidade.Where(s => s.CodProgramaFidelidade == programa.CodProgramaFidelidade && s.LoginUsuario == loginUsuario).FirstOrDefault();
                            var pontosCreditar = (decimal)pedidoViewModel.ValorTotal * programa.PontosGanhosPorUnidadeMonetariaGasta;
                            saldo.Saldo = saldo.Saldo + pontosCreditar;
                            saldoAtualizadoPrograma = saldo.Saldo;

                            ExtratoUsuarioProgramaFidelidade extrato = new ExtratoUsuarioProgramaFidelidade();
                            extrato.CodPedido = ped.CodPedido;
                            extrato.CodProgramaFidelidade = programa.CodProgramaFidelidade;
                            extrato.DataHoraLancamento = DateTime.Now;
                            extrato.DescricaoLancamento = "Crédito de " + pontosCreditar.ToString("0.00") + " pontos referentes ao pedido " + ped.CodPedido + " de valor " + pedidoViewModel.ValorTotal.ToString("C");
                            extrato.LoginUsuario = loginUsuario;
                            extrato.SaldoPosLancamento = saldo.Saldo;
                            extrato.ValorLancamento = pontosCreditar;
                            _contexto.ExtratosUsuariosProgramasFidelidade.Add(extrato);

                            _contexto.SaveChanges();
                        }

                        if (pedidoViewModel.UsaSaldoProgramaFidelidade)
                        {
                            if (programa == null)
                            {
                                throw new Exception("O usuário não está inscrito em nenhum programa de fidelidade no momento.");
                            }

                            SaldoUsuarioProgramaFidelidade saldo = _contexto.SaldosUsuariosProgramasFidelidade.Where(s => s.CodProgramaFidelidade == programa.CodProgramaFidelidade && s.LoginUsuario == loginUsuario).FirstOrDefault();
                            saldo.Saldo = saldo.Saldo - pedidoViewModel.PontosAUtilizarProgramaRecompensa;
                            saldoAtualizadoPrograma = saldo.Saldo;

                            ExtratoUsuarioProgramaFidelidade extrato = new ExtratoUsuarioProgramaFidelidade();
                            extrato.CodPedido = ped.CodPedido;
                            extrato.CodProgramaFidelidade = programa.CodProgramaFidelidade;
                            extrato.DataHoraLancamento = DateTime.Now;
                            extrato.DescricaoLancamento = "Resgate de " + pedidoViewModel.PontosAUtilizarProgramaRecompensa.ToString("0.00") + " pontos durante o registro do pedido " + ped.CodPedido;
                            extrato.LoginUsuario = loginUsuario;
                            extrato.SaldoPosLancamento = saldo.Saldo;
                            extrato.ValorLancamento = -1 * pedidoViewModel.PontosAUtilizarProgramaRecompensa;
                            _contexto.ExtratosUsuariosProgramasFidelidade.Add(extrato);

                            _contexto.SaveChanges();
                        }
                    }

                    dbContextTransaction.Commit();

                    if (saldoAtualizadoPrograma >= 0)
                    {
                        SessionData.ProgramaFidelidadeUsuario.Saldo = saldoAtualizadoPrograma;
                    }

                    TrataDadosUsuario(pedidoViewModel);

                    return ped.CodPedido;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void TrataDadosUsuario(PedidoViewModel pedidoViewModel)
        {
            if (pedidoViewModel.DadosCliente.ClienteNovo)
            {
                try
                {
                    ApplicationDbContext contexto = new ApplicationDbContext();
                    Usuario usu = new Usuario();
                    UsuarioViewModel usuVm = new UsuarioViewModel();
                    PropertyCopy.Copy(pedidoViewModel.DadosCliente, usuVm);
                    UsuarioCopy.ViewModelToDB(usuVm, usu);
                    usu.UsuarioExterno = true;
                    contexto.DadosUsuarios.Add(usu);
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {

                    //nao faz nada porque o pedido foi gravado e sao transacoes diferentes
                }
            }
            else if (!pedidoViewModel.PedidoExterno && pedidoViewModel.DadosCliente.Salvar) //atualiza dados do usuário logado apenas se não for pedido administrativo
            {
                try
                {
                    ApplicationDbContext contexto = new ApplicationDbContext();
                    string userName = System.Web.HttpContext.Current.User.Identity.Name;
                    var usu = contexto.DadosUsuarios.Where(d => d.Email == userName).FirstOrDefault();
                    if (usu != null)
                    {
                        UsuarioViewModel usuVm = new UsuarioViewModel();
                        PropertyCopy.Copy(pedidoViewModel.DadosCliente, usuVm);
                        UsuarioCopy.ViewModelToDB(usuVm, usu);
                        contexto.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //nao faz nada porque o pedido foi gravado e sao transacoes diferentes
                }
            }
        }

        public async Task<PedidoViewModel> GetPedidoAberto(string loginUsuario, string telefone)
        {
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();
            var tempoMedioEspera = ParametroRepository.GetTempoMedioEspera();

            return await _contexto.Pedidos.Where(p => new List<int> { (int)SituacaoPedidoEnum.AguardandoConfirmacao, (int)SituacaoPedidoEnum.Confirmado, (int)SituacaoPedidoEnum.EmPreparacao, (int)SituacaoPedidoEnum.EmProcessoEntrega }.Contains(p.CodSituacao) && p.Usuario == (loginUsuario != "" ? loginUsuario : p.Usuario) && p.TelefoneCliente == (telefone != "" ? telefone : p.TelefoneCliente) && (!p.PedidoExterno || telefone != ""))
                .Include(c => c.Itens)
                .Include(s => s.FormaPagamentoRef)
                .Include(s => s.BandeiraCartaoRef)
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    DataPedido = p.DataHora,
                    CodFormaPagamento = p.CodFormaPagamento,
                    DescricaoFormaPagamento = p.FormaPagamentoRef.DescricaoFormaPagamento,
                    CodBandeiraCartao = p.CodBandeiraCartao,
                    DescricaoBandeiraCartao = p.BandeiraCartaoRef.DescricaoBandeiraCartao,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    TaxaEntrega = p.TaxaEntrega,
                    Troco = p.Troco,
                    TrocoPara = p.TrocoPara,
                    ValorTotal = p.ValorTotal,
                    FeedbackCliente = p.FeedbackCliente,
                    MotivoCancelamento = p.MotivoCancelamento,
                    ValorDesconto = p.ValorDesconto,
                    PercentualDesconto = p.PercentualDesconto,
                    MotivoDesconto = p.MotivoDesconto,
                    PortaImpressaoComandaEntrega = impressoraComanda,
                    TempoMedioEspera = tempoMedioEspera,
                    DadosCliente = new DadosClientePedidoViewModel
                    {
                        Bairro = p.BairroEntrega,
                        Cidade = p.CidadeEntrega,
                        Complemento = p.ComplementoEntrega,
                        Estado = p.UFEntrega,
                        Logradouro = p.LogradouroEntrega,
                        Nome = p.NomeCliente,
                        Numero = p.NumeroEntrega,
                        Referencia = p.ReferenciaEntrega,
                        Telefone = p.TelefoneCliente
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<PedidoViewModel>> GetUltimosPedidos(string loginUsuario)
        {
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();

            var pedidos = await _contexto.Pedidos.Where(p => p.Usuario == loginUsuario && !p.PedidoExterno)
                .Include(s => s.Situacao)
                .Include(s => s.Itens)
                .Include(s => s.FormaPagamentoRef)
                .Include(s => s.BandeiraCartaoRef)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
                .Include(s => s.Itens.Select(i => i.ItemCardapio.ImpressorasAssociadas))
                .Include(s => s.Itens.Select(i => i.ItemCardapio.ImpressorasAssociadas.Select(a => a.ImpressoraProducao)))
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    CodFormaPagamento = p.CodFormaPagamento,
                    DescricaoFormaPagamento = p.FormaPagamentoRef.DescricaoFormaPagamento,
                    DataPedido = p.DataHora,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    DescricaoSituacao = p.Situacao.Descricao,
                    ValorTotal = p.ValorTotal,
                    TaxaEntrega = p.TaxaEntrega,
                    MotivoCancelamento = p.MotivoCancelamento,
                    FeedbackCliente = p.FeedbackCliente,
                    ValorDesconto = p.ValorDesconto,
                    PercentualDesconto = p.PercentualDesconto,
                    MotivoDesconto = p.MotivoDesconto,
                    PortaImpressaoComandaEntrega = impressoraComanda,
                    Itens = p.Itens.Select(i => new ItemPedidoViewModel
                    {
                        CodItem = i.CodItemCardapio,
                        SeqItem = i.SeqItem,
                        DescricaoItem = i.ItemCardapio.Nome,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        ValorExtras = i.ValorExtras,
                        ValorTotalItem = i.ValorTotal,
                        ObservacaoLivre = i.ObservacaoLivre,
                        CodCombo = i.CodCombo,
                        PrecoCombo = i.PrecoCombo,
                        Obs = i.Observacoes.Select(o => new ObservacaoItemPedidoViewModel
                        {
                            CodObservacao = o.CodObservacao,
                            DescricaoObservacao = o.Observacao.DescricaoObservacao
                        }).ToList().Union(new List<ObservacaoItemPedidoViewModel> { new ObservacaoItemPedidoViewModel { CodObservacao = (i.ObservacaoLivre != "" && i.ObservacaoLivre != null ? -1 : -2), DescricaoObservacao = i.ObservacaoLivre } }).ToList().Where(o => o.CodObservacao >= -1).ToList(),
                        extras = i.Extras.Select(e => new ExtraItemPedidoViewModel
                        {
                            CodOpcaoExtra = e.CodOpcaoExtra,
                            DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra,
                            Preco = e.Preco
                        }).ToList(),
                        PortasImpressaoProducao = i.ItemCardapio.ImpressorasAssociadas.Select(a => a.ImpressoraProducao.Porta).ToList()
                    }).ToList().OrderBy(i => i.SeqItem).ToList()
                })
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            AgrupaItensComboPedido(pedidos);

            return pedidos;
        }

        public async Task<List<PedidoViewModel>> GetPedidosAbertos(int? codPedido, bool paraConsulta, bool somenteItensProducao)
        {
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();
            var portaImpressaoCozinha = ParametroRepository.GetPortaImpressoraCozinha();

            var pedidos = await _contexto.Pedidos.Where(p => (!(new List<int> { 5, 9 }).Contains(p.CodSituacao) && p.CodPedido == (codPedido != null ? codPedido.Value : p.CodPedido) && !paraConsulta) || (paraConsulta && codPedido != null && p.CodPedido == codPedido))
                .Include(s => s.Situacao)
                .Include(s => s.Entregador)
                .Include(s => s.Itens)
                .Include(s => s.FormaPagamentoRef)
                .Include(s => s.BandeiraCartaoRef)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
                .Include(s => s.Itens.Select(i => i.ItemCardapio.ImpressorasAssociadas))
                .Include(s => s.Itens.Select(i => i.ItemCardapio.ImpressorasAssociadas.Select(a => a.ImpressoraProducao)))
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    CodFormaPagamento = p.CodFormaPagamento,
                    DescricaoFormaPagamento = p.FormaPagamentoRef.DescricaoFormaPagamento,
                    DataPedido = p.DataHora,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    DescricaoSituacao = p.Situacao.Descricao,
                    CodEntregador = p.CodEntregador,
                    NomeEntregador = p.Entregador.Nome,
                    ValorTotal = p.ValorTotal,
                    TaxaEntrega = p.TaxaEntrega,
                    CodBandeiraCartao = p.CodBandeiraCartao,
                    DescricaoBandeiraCartao = p.BandeiraCartaoRef.DescricaoBandeiraCartao,
                    Troco = p.Troco,
                    TrocoPara = p.TrocoPara,
                    Usuario = p.Usuario,
                    PedidoExterno = p.PedidoExterno,
                    RetirarNaCasa = p.RetirarNaCasa,
                    MotivoCancelamento = p.MotivoCancelamento,
                    FeedbackCliente = p.FeedbackCliente,
                    ValorDesconto = p.ValorDesconto,
                    PercentualDesconto = p.PercentualDesconto,
                    MotivoDesconto = p.MotivoDesconto,
                    PortaImpressaoComandaEntrega = impressoraComanda,
                    DadosCliente = new DadosClientePedidoViewModel
                    {
                        Bairro = p.BairroEntrega,
                        Cidade = p.CidadeEntrega,
                        Complemento = p.ComplementoEntrega,
                        Estado = p.UFEntrega,
                        Logradouro = p.LogradouroEntrega,
                        Nome = p.NomeCliente,
                        Numero = p.NumeroEntrega,
                        Referencia = p.ReferenciaEntrega,
                        Telefone = p.TelefoneCliente
                    },
                    Itens = p.Itens.Where(i => !i.Cancelado && (!somenteItensProducao || (i.ItemCardapio.ImpressorasAssociadas.Where(a => a.ImpressoraProducao.Porta == portaImpressaoCozinha).Count() > 0))).Select(i => new ItemPedidoViewModel
                    {
                        CodItem = i.CodItemCardapio,
                        SeqItem = i.SeqItem,
                        DescricaoItem = i.ItemCardapio.Nome,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        ValorExtras = i.ValorExtras,
                        ValorTotalItem = i.ValorTotal,
                        CodPromocaoVenda = i.CodPromocaoVenda,
                        PercentualDesconto = i.PercentualDesconto,
                        PrecoUnitarioComDesconto = (1 - i.PercentualDesconto / 100) * i.PrecoUnitario,
                        ValorDesconto = i.ValorDesconto,
                        ObservacaoLivre = i.ObservacaoLivre,
                        CodCombo = i.CodCombo,
                        PrecoCombo = i.PrecoCombo,
                        AcaoRegistro = (int)Comum.AcaoRegistro.Nenhuma,
                        PortasImpressaoProducao = i.ItemCardapio.ImpressorasAssociadas.Select(a => a.ImpressoraProducao.Porta).ToList(),
                        Obs = i.Observacoes.Select(o => new ObservacaoItemPedidoViewModel
                        {
                            CodObservacao = o.CodObservacao,
                            DescricaoObservacao = o.Observacao.DescricaoObservacao
                        }).ToList().Union(new List<ObservacaoItemPedidoViewModel> { new ObservacaoItemPedidoViewModel { CodObservacao = (i.ObservacaoLivre != "" && i.ObservacaoLivre != null ? -1 : -2), DescricaoObservacao = i.ObservacaoLivre } }).ToList().Where(o => o.CodObservacao >= -1).ToList(),
                        extras = i.Extras.Select(e => new ExtraItemPedidoViewModel
                        {
                            CodOpcaoExtra = e.CodOpcaoExtra,
                            DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra,
                            Preco = e.Preco
                        }).ToList()
                    }).ToList().OrderBy(i => i.SeqItem).ToList()
                })
                .OrderBy(p => p.DataPedido)
                .ToListAsync();

            AgrupaItensComboPedido(pedidos);

            return pedidos.Where(p => p.Itens.Count > 0).ToList();
        }

        public void AgrupaItensComboPedido(List<PedidoViewModel> pedidos)
        {
            var itensCombo = new List<ItemPedidoViewModel>();
            foreach (var ped in pedidos)
            {
                itensCombo = new List<ItemPedidoViewModel>();
                foreach (var item in ped.Itens.Where(i => i.CodCombo != null).ToList().OrderByDescending(i => i.PortasImpressaoProducao.Count))
                {
                    var comboDb = _contexto.Combos.Find(item.CodCombo.Value);
                    if (comboDb != null && item.PortasImpressaoProducao.Count > 0 && itensCombo.Where(ic => ic.CodCombo == item.CodCombo).Count() == 0)
                    {
                        ItemPedidoViewModel itemCombo = new ItemPedidoViewModel();
                        itemCombo.CodItem = -1;
                        itemCombo.SeqItem = ped.Itens.Where(i => i.CodCombo == item.CodCombo.Value).OrderByDescending(i => i.SeqItem).FirstOrDefault().SeqItem;
                        itemCombo.DescricaoItem = comboDb.NomeCombo;
                        itemCombo.Quantidade = item.Quantidade;
                        itemCombo.PrecoUnitario = item.PrecoCombo;
                        itemCombo.ValorExtras = 0;
                        itemCombo.ValorTotalItem = itemCombo.Quantidade * itemCombo.PrecoUnitario;
                        itemCombo.CodPromocaoVenda = null;
                        itemCombo.PercentualDesconto = 0;
                        itemCombo.PrecoUnitario = comboDb.PrecoCombo;
                        itemCombo.ValorDesconto = 0;
                        itemCombo.ObservacaoLivre = item.ObservacaoLivre;
                        itemCombo.AcaoRegistro = (int)Comum.AcaoRegistro.Nenhuma;
                        itemCombo.PortasImpressaoProducao = item.PortasImpressaoProducao;
                        itemCombo.Obs = item.Obs;
                        itemCombo.extras = item.extras;
                        itemCombo.CodCombo = item.CodCombo;
                        itemCombo.PrecoCombo = item.PrecoCombo;
                        itemCombo.DescricaoCombo = comboDb.DescricaoCombo;
                        itensCombo.Add(itemCombo);
                    }

                    ped.Itens.Remove(item);
                }

                ped.Itens.AddRange(itensCombo);
            }
        }
    }
}