using System;
using System.Collections.Generic;
using System.Linq;
using BrasaoSolution.Model;
using BrasaoSolution.Repository.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using BrasaoSolution.Helper;
using System.Diagnostics;
using Newtonsoft.Json;
using BrasaoSolution.ViewModel;

namespace BrasaoSolution.Repository
{
    public class PedidoRepository
    {
        private BrasaoContext _contexto;

        public PedidoRepository()
        {
            _contexto = new BrasaoContext();
        }

        public async Task AplicaDescontoPedido(PedidoViewModel pedido, string loginUsuario)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
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

                        var historico = new HistoricoPedido();

                        historico.CodPedido = pedido.CodPedido;
                        historico.DataHora = DateTime.Now;
                        historico.CodSituacao = pedido.Situacao;
                        historico.Usuario = loginUsuario;
                        historico.Descricao = "Aplicação de " + pedido.PercentualDesconto.ToString() + "%  de desconto ao pedido.";

                        _contexto.HistoricosPedido.Add(historico);

                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public async Task AlteraSituacaoPedido(PedidoViewModel pedido, string loginUsuario)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var ped = _contexto.Pedidos.Where(p => p.CodPedido == pedido.CodPedido).FirstOrDefault();

                    var historico = new HistoricoPedido();

                    historico.CodPedido = pedido.CodPedido;
                    historico.DataHora = DateTime.Now;
                    historico.Usuario = loginUsuario;

                    int codSituacaoAnterior = ped.CodSituacao;

                    if (ped != null)
                    {
                        ped.CodSituacao = pedido.Situacao;

                        historico.CodSituacao = pedido.Situacao;
                        historico.Descricao = "STATUS: Situacação do pedido alterada de " + codSituacaoAnterior + " para " + pedido.Situacao + ".";

                        if (ped.CodSituacao == (int)SituacaoPedidoEnum.Concluido)
                        {
                            ped.FeedbackCliente = pedido.FeedbackCliente;

                            historico.Descricao = "STATUS: Situacação do pedido alterada de " + codSituacaoAnterior + " para " + pedido.Situacao + " (conclusão do pedido).";
                        }

                        if (ped.CodSituacao == (int)SituacaoPedidoEnum.EmProcessoEntrega)
                        {
                            ped.CodEntregador = pedido.CodEntregador;

                            historico.Descricao = "STATUS: Situacação do pedido alterada de " + codSituacaoAnterior + " para " + pedido.Situacao + " (saiu para entrega).";
                        }

                        decimal saldoAtualizadoPrograma = -1;
                        if (ped.CodSituacao == (int)SituacaoPedidoEnum.Cancelado)
                        {
                            ped.MotivoCancelamento = pedido.MotivoCancelamento;

                            historico.Descricao = "STATUS: Situacação do pedido alterada de " + codSituacaoAnterior + " para " + pedido.Situacao + " (cancelamento do pedido).";

                            //no caso do cancelamento do pedido, se o usuário estiver em programa de recompensa, estorna o saldo

                            ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository();
                            var programa = _progRep.GetProgramaFidelidadeUsuario(loginUsuario, pedido.CodEmpresa);

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

                        _contexto.HistoricosPedido.Add(historico);

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
                                   && (promos.CodEmpresa != null ? promos.CodEmpresa : pedidoViewModel.CodEmpresa) == pedidoViewModel.CodEmpresa
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
            try
            {
                if (pedidoViewModel.DadosCliente.Telefone.Length < 14)
                {
                    throw new Exception("O telefone não está preenchido corretamente");
                }
            }
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro ao validar telefone: " + ex.Message, EventLogEntryType.Error);

                throw ex;
            }
            

            try
            {
                if (pedidoViewModel.PedidoExterno && pedidoViewModel.CodPedido <= 0)
                {
                    BrasaoUtil.GravaLog("Verifica se existe pedido em aberto para Telefone " + pedidoViewModel.DadosCliente.Telefone + " e empresa " + pedidoViewModel.CodEmpresa, EventLogEntryType.Information);

                    //var ped = _rep.GetPedidoAberto("", pedidoViewModel.DadosCliente.Telefone).Result;
                    var ped = BrasaoSolution.Helper.AsyncHelpers.RunSync<PedidoViewModel>(() => this.GetPedidoAberto("", pedidoViewModel.DadosCliente.Telefone, pedidoViewModel.CodEmpresa));

                    if (ped != null)
                    {
                        BrasaoUtil.GravaLog("Cliente já tem pedido. Encontrou.", EventLogEntryType.Information);
                        throw new Exception("O cliente " + pedidoViewModel.DadosCliente.Telefone + " possui o pedido " + ped.CodPedido + " em aberto. Finalize-o antes de fazer outro pedido para este cliente.");
                    }
                    else
                    {
                        BrasaoUtil.GravaLog("Cliente não tem pedido. Não encontrou.", EventLogEntryType.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro ao validar se cliente já possui pedido: " + ex.Message, EventLogEntryType.Error);

                throw ex;
            }
            
            try
            {
                //primeiro verifica se a casa está aberta para delivery
                if (!pedidoViewModel.PedidoExterno && !ParametroRepository.CasaAberta() && pedidoViewModel.CodPedido <= 0)
                {
                    var horarioFuncionamento = ParametroRepository.GetHorarioAbertura();
                    throw new Exception("No momento estamos fechados. Abriremos " + horarioFuncionamento.DiaSemana + " das " + horarioFuncionamento.Abertura.ToString("HH:mm") + " às " + horarioFuncionamento.Fechamento.ToString("HH:mm") + ".");
                }
            }
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro ao verificar se a casa está aberta: " + ex.Message, EventLogEntryType.Error);

                throw ex;
            }
            

            try
            {
                //valida o valor do pedido contra a base de dados para evitar fraude por manipulação do JS
                if (!ValidaValorPedido(pedidoViewModel))
                {
                    throw new Exception("O valor do pedido ou dos itens foi manipulado durante a requisição. Favor tentar novamente.");
                }
            }
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro ao validar o valor do pedido: " + ex.Message, EventLogEntryType.Error);

                throw ex;
            }
            

            try
            {
                //valida uso do saldo do programa de recompensa
                if (pedidoViewModel.UsaSaldoProgramaFidelidade)
                {
                    using (ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository())
                    {
                        var programa = _progRep.GetProgramaFidelidadeUsuario(loginUsuario, pedidoViewModel.CodEmpresa);

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
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro ao validar o programa de fidelidade: " + ex.Message, EventLogEntryType.Error);

                throw ex;
            }
            
        }

        public async Task<int> GravaPedido(PedidoViewModel pedidoViewModel, string loginUsuario)
        {
            BrasaoUtil.GravaLog("Valida pedido.", EventLogEntryType.Information);
            try
            {
                ValidaPedido(pedidoViewModel, loginUsuario);
            }
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro na validação do pedido. \nUsuario: " + loginUsuario + " \nPedido com erro: " + JsonConvert.SerializeObject(pedidoViewModel), EventLogEntryType.Error);
                
                throw ex;
            }
            BrasaoUtil.GravaLog("Validou pedido com sucesso.", EventLogEntryType.Information);


            var historico = new HistoricoPedido();


            historico.DataHora = DateTime.Now;
            historico.Usuario = loginUsuario;


            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    BrasaoUtil.GravaLog("Inicia gravação do registro do pedido.", EventLogEntryType.Information);
                    Pedido ped;

                    if (pedidoViewModel.CodPedido <= 0)
                    {
                        ped = new Pedido();
                        ped.CodSituacao = pedidoViewModel.Situacao;
                        ped.DataHora = DateTime.Now;
                        ped.Usuario = loginUsuario;

                        historico.Descricao = "Criação do pedido .";
                        historico.CodSituacao = ped.CodSituacao;
                    }
                    else
                    {
                        ped = _contexto.Pedidos.Find(pedidoViewModel.CodPedido);

                        historico.Descricao = "Alteração do pedido " + pedidoViewModel.CodPedido;
                        historico.CodSituacao = ped.CodSituacao;
                    }

                    ped.CodEmpresa = pedidoViewModel.CodEmpresa;
                    if (pedidoViewModel.CodBandeiraCartao > 0)
                    {
                        ped.CodBandeiraCartao = pedidoViewModel.CodBandeiraCartao;
                    }
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

                    ped.CodBairro = null;
                    ped.ComplementoEntrega = null;
                    ped.LogradouroEntrega = null;
                    ped.NumeroEntrega = null;
                    ped.ReferenciaEntrega = null;

                    if (!ped.RetirarNaCasa)
                    {
                        ped.CodBairro = pedidoViewModel.DadosCliente.CodBairro;
                        ped.ComplementoEntrega = pedidoViewModel.DadosCliente.Complemento;
                        ped.LogradouroEntrega = pedidoViewModel.DadosCliente.Logradouro;
                        ped.NumeroEntrega = pedidoViewModel.DadosCliente.Numero;
                        ped.ReferenciaEntrega = pedidoViewModel.DadosCliente.Referencia;
                    }

                    if (pedidoViewModel.CodPedido <= 0)
                    {
                        _contexto.Pedidos.Add(ped);
                    }

                    await _contexto.SaveChangesAsync();
                    BrasaoUtil.GravaLog("Gravou registro do pedido com sucesso.", EventLogEntryType.Information);

                    ItemPedido item;

                    BrasaoUtil.GravaLog("Executa trecho de combo.", EventLogEntryType.Information);
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
                    BrasaoUtil.GravaLog("Executou trecho de combo com sucesso.", EventLogEntryType.Information);

                    BrasaoUtil.GravaLog("Inicia gravação dos itens.", EventLogEntryType.Information);
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

                    BrasaoUtil.GravaLog("Gravou itens com sucesso.", EventLogEntryType.Information);

                    decimal saldoAtualizadoPrograma = -1;

                    if (!pedidoViewModel.PedidoExterno)
                    {
                        BrasaoUtil.GravaLog("Se não for pedido externo, verifica programa de fidelidade.", EventLogEntryType.Information);
                        ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository();
                        var programa = _progRep.GetProgramaFidelidadeUsuario(loginUsuario, pedidoViewModel.CodEmpresa);

                        BrasaoUtil.GravaLog("Buscou programa na base.", EventLogEntryType.Information);
                        if (programa != null && programa.LoginUsuario != null && programa.TermosAceitos != null && programa.TermosAceitos.Value && pedidoViewModel.ValorTotal > 0)
                        {
                            BrasaoUtil.GravaLog("Encontrou programa: " + JsonConvert.SerializeObject(programa), EventLogEntryType.Information);
                            //credita pontos referentes ao pedido
                            BrasaoUtil.GravaLog("Busca objeto do saldo.", EventLogEntryType.Information);
                            SaldoUsuarioProgramaFidelidade saldo = _contexto.SaldosUsuariosProgramasFidelidade.Where(s => s.CodProgramaFidelidade == programa.CodProgramaFidelidade && s.LoginUsuario == loginUsuario).FirstOrDefault();
                            var pontosCreditar = (decimal)pedidoViewModel.ValorTotal * programa.PontosGanhosPorUnidadeMonetariaGasta;
                            saldo.Saldo = saldo.Saldo + pontosCreditar;
                            saldoAtualizadoPrograma = saldo.Saldo;
                            BrasaoUtil.GravaLog("Atualizo saldo.", EventLogEntryType.Information);

                            BrasaoUtil.GravaLog("Cria entrada de extrato.", EventLogEntryType.Information);
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
                            BrasaoUtil.GravaLog("Gravou extrato.", EventLogEntryType.Information);
                        }

                        if (pedidoViewModel.UsaSaldoProgramaFidelidade)
                        {
                            BrasaoUtil.GravaLog("Usa pontos do programa.", EventLogEntryType.Information);
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
                            BrasaoUtil.GravaLog("Usou pontos.", EventLogEntryType.Information);
                        }
                    }

                    BrasaoUtil.GravaLog("Adiciona histórico.", EventLogEntryType.Information);
                    historico.CodPedido = ped.CodPedido;
                    _contexto.HistoricosPedido.Add(historico);

                    _contexto.SaveChanges();
                    BrasaoUtil.GravaLog("Historico gravado.", EventLogEntryType.Information);

                    if (saldoAtualizadoPrograma >= 0)
                    {
                        BrasaoUtil.GravaLog("Atualiza saldo na sessão.", EventLogEntryType.Information);
                        SessionData.ProgramaFidelidadeUsuario.Saldo = saldoAtualizadoPrograma;
                        BrasaoUtil.GravaLog("Atualizou saldo na sessão.", EventLogEntryType.Information);
                    }

                    dbContextTransaction.Commit();

                    try
                    {
                        TrataDadosUsuario(pedidoViewModel);
                    }
                    catch(Exception ex)
                    {
                        BrasaoUtil.GravaLog("Erro tratando dados do usuário: " + ex.Message, EventLogEntryType.Error);
                    }

                    return ped.CodPedido;
                }
                catch (Exception ex)
                {
                    BrasaoUtil.GravaLog("Erro na gravação do pedido. \nUsuario: " + loginUsuario + " \nPedido com erro: " + JsonConvert.SerializeObject(pedidoViewModel), EventLogEntryType.Error);
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
                    PropertyCopy.Copy(usuVm, usu);
                    //UsuarioCopy.ViewModelToDB(usuVm, usu);
                    usu.UsuarioExterno = true;
                    contexto.DadosUsuarios.Add(usu);
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {

                    //nao faz nada porque o pedido foi gravado e sao transacoes diferentes
                }
            }
            else if (pedidoViewModel.DadosCliente.Salvar) //atualiza dados do usuário logado apenas se não for pedido administrativo
            {
                try
                {
                    ApplicationDbContext contexto = new ApplicationDbContext();
                    string userName = System.Web.HttpContext.Current.User.Identity.Name;
                    var usu = contexto.DadosUsuarios.Where(d => d.Email == userName).FirstOrDefault();
                    if (usu != null)
                    {
                        var id = usu.Id;
                        PropertyCopy.Copy(pedidoViewModel.DadosCliente, usu);
                        usu.Id = id;
                        //UsuarioCopy.ViewModelToDB(usuVm, usu);
                        contexto.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //nao faz nada porque o pedido foi gravado e sao transacoes diferentes
                }
            }
        }

        public async Task<PedidoViewModel> GetPedidoAberto(string loginUsuario, string telefone, int? codEmpresa)
        {
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();
            var tempoMedioEspera = ParametroRepository.GetTempoMedioEspera();

            return await _contexto.Pedidos
                .Where(p => new List<int> { (int)SituacaoPedidoEnum.AguardandoConfirmacao, (int)SituacaoPedidoEnum.Confirmado, (int)SituacaoPedidoEnum.EmPreparacao, (int)SituacaoPedidoEnum.EmProcessoEntrega }.Contains(p.CodSituacao) &&
                p.CodEmpresa == (codEmpresa != null ? codEmpresa : p.CodPedido) &&
                p.Usuario == (loginUsuario != "" ? loginUsuario : p.Usuario) &&
                p.TelefoneCliente == (telefone != "" ? telefone : p.TelefoneCliente) &&
                (!p.PedidoExterno || telefone != ""))
                .Include(c => c.Itens)
                .Include(c => c.Empresa)
                .Include(c => c.Bairro)
                .Include(c => c.Bairro.Cidade)
                .Include(s => s.FormaPagamentoRef)
                .Include(s => s.BandeiraCartaoRef)
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    CodEmpresa = p.CodEmpresa,
                    NomeEmpresa = p.Empresa.NomeFantasia,
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
                        CodBairro = p.CodBairro,
                        NomeBairro = p.Bairro.Nome,
                        CodCidade = p.Bairro.CodCidade,
                        NomeCidade = p.Bairro.Cidade.Nome,
                        Complemento = p.ComplementoEntrega,
                        Estado = p.Bairro.Cidade.Estado,
                        Logradouro = p.LogradouroEntrega,
                        Nome = p.NomeCliente,
                        Numero = p.NumeroEntrega,
                        Referencia = p.ReferenciaEntrega,
                        Telefone = p.TelefoneCliente
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<PedidoViewModel>> GetUltimosPedidos(string loginUsuario, int codEmpresa)
        {
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();

            var pedidos = await _contexto.Pedidos.Where(p => p.Usuario == loginUsuario && !p.PedidoExterno && p.CodEmpresa == codEmpresa)
                .Include(s => s.Situacao)
                .Include(s => s.Empresa)
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
                    CodEmpresa = p.CodEmpresa,
                    NomeEmpresa = p.Empresa.NomeFantasia,
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

        public async Task<List<PedidoViewModel>> GetPedidosAbertos(int? codPedido, bool paraConsulta, bool somenteItensProducao, int? codEmpresa)
        {
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();
            var portaImpressaoCozinha = ParametroRepository.GetPortaImpressoraCozinha();

            var pedidos = await _contexto.Pedidos.Where(p => ((!(new List<int> { 5, 9 }).Contains(p.CodSituacao) && p.CodPedido == (codPedido != null ? codPedido.Value : p.CodPedido) && !paraConsulta) ||
                                                              (paraConsulta && codPedido != null && p.CodPedido == codPedido)) &&
                                                             p.CodEmpresa == (codEmpresa != null ? codEmpresa.Value : p.CodEmpresa) &&
                                                              SessionData.EmpresasInt.Contains(p.CodEmpresa))
                .Include(s => s.Situacao)
                .Include(s => s.Empresa)
                .Include(s => s.Entregador)
                .Include(s => s.Itens)
                .Include(s => s.Bairro)
                .Include(s => s.Bairro.Cidade)
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
                    CodEmpresa = p.CodEmpresa,
                    NomeEmpresa = p.Empresa.NomeFantasia,
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
                        CodBairro = p.Bairro.CodBairro,
                        NomeBairro = p.Bairro.Nome,
                        NomeCidade = p.Bairro.Cidade.Nome,
                        CodCidade = p.Bairro.CodCidade,
                        Complemento = p.ComplementoEntrega,
                        Estado = p.Bairro.Cidade.Estado,
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


        public async Task<List<HistoricoPedidoViewModel>> GetHistoricoPedido(int codPedido)
        {
            return await _contexto.HistoricosPedido.Include(h => h.Situacao).Where(h => h.CodPedido == codPedido).Select(h => new HistoricoPedidoViewModel
            {
                CodPedido = h.CodPedido,
                DataHora = h.DataHora,
                CodSituacao = h.CodSituacao,
                DescricaoSituacao = h.Situacao.Descricao,
                Descricao = h.Descricao,
                Usuario = h.Usuario
            }).OrderBy(h => h.DataHora).ToListAsync();

        }
    }
}