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

                if (ped.FormaPagamento == "D" && ped.TrocoPara != null && ped.TrocoPara.Value > 0)
                {
                    ped.Troco = ped.TrocoPara - ped.ValorTotal - ped.ValorDesconto.Value;
                }

                await _contexto.SaveChangesAsync();
            }
        }

        public async Task AlteraSituacaoPedido(int codPedido, int codSituacao, string motivoCancelamento, string feedbackCliente)
        {
            var ped = _contexto.Pedidos.Where(p => p.CodPedido == codPedido).FirstOrDefault();

            if (ped != null)
            {
                ped.CodSituacao = codSituacao;

                if (ped.CodSituacao == (int)SituacaoPedidoEnum.Concluido)
                {
                    ped.FeedbackCliente = feedbackCliente;
                }

                if (ped.CodSituacao == (int)SituacaoPedidoEnum.Cancelado)
                {
                    ped.MotivoCancelamento = motivoCancelamento;
                }

                await _contexto.SaveChangesAsync();
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

            foreach(var item in pedidoViewModel.Itens)
            {
                valorExtras = 0;
                if (item.extras != null && item.extras.Count > 0)
                {
                    foreach(var extra in item.extras)
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

                if (itemBase != null)
                {
                    valorTotalPedido += item.Quantidade * itemBase.Preco;
                }

                valorTotalPedido += valorExtras;
            }

            valorTotalPedido += pedidoViewModel.TaxaEntrega;

            if (pedidoViewModel.ValorDesconto != null && pedidoViewModel.ValorDesconto.Value > 0)
            {
                valorTotalPedido -= pedidoViewModel.ValorDesconto.Value;
            }

            if (pedidoViewModel.ValorTotal != valorTotalPedido)
            {
                return false;
            }

            return true;
        }

        public async Task<int> GravaPedido(PedidoViewModel pedidoViewModel, string loginUsuario)
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

                    ped.BairroEntrega = pedidoViewModel.DadosCliente.Bairro;
                    ped.BandeiraCartao = pedidoViewModel.BandeiraCartao;
                    ped.CidadeEntrega = pedidoViewModel.DadosCliente.Cidade;
                    ped.ComplementoEntrega = pedidoViewModel.DadosCliente.Complemento;
                    ped.FormaPagamento = pedidoViewModel.FormaPagamento;
                    ped.LogradouroEntrega = pedidoViewModel.DadosCliente.Logradouro;
                    ped.NomeCliente = pedidoViewModel.DadosCliente.Nome;
                    ped.NumeroEntrega = pedidoViewModel.DadosCliente.Numero;
                    ped.ReferenciaEntrega = pedidoViewModel.DadosCliente.Referencia;
                    ped.TaxaEntrega = pedidoViewModel.TaxaEntrega;
                    ped.TelefoneCliente = pedidoViewModel.DadosCliente.Telefone;
                    ped.Troco = pedidoViewModel.Troco;
                    ped.TrocoPara = pedidoViewModel.TrocoPara;
                    ped.UFEntrega = pedidoViewModel.DadosCliente.Estado;
                    ped.ValorTotal = pedidoViewModel.ValorTotal;
                    ped.MotivoCancelamento = pedidoViewModel.MotivoCancelamento;
                    ped.FeedbackCliente = pedidoViewModel.FeedbackCliente;
                    ped.PedidoExterno = pedidoViewModel.PedidoExterno;

                    if (pedidoViewModel.CodPedido <= 0)
                    {
                        _contexto.Pedidos.Add(ped);
                    }

                    await _contexto.SaveChangesAsync();

                    ItemPedido item;
                    foreach (var itemViewModel in pedidoViewModel.Itens)
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

                    dbContextTransaction.Commit();

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
            else if (pedidoViewModel.DadosCliente.Salvar)
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
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    DataPedido = p.DataHora,
                    BandeiraCartao = p.BandeiraCartao,
                    FormaPagamento = p.FormaPagamento,
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
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    FormaPagamento = p.FormaPagamento,
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
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return pedidos;
        }

        public async Task<List<PedidoViewModel>> GetPedidosConsulta(DateTime? inicio, DateTime? fim)
        {
            var pedidos = await _contexto.Pedidos.Where(p => p.DataHora >= (inicio != null ? inicio.Value : p.DataHora) && p.DataHora <= (fim != null ? fim.Value : p.DataHora) && p.CodSituacao >= 2 && p.CodSituacao < 9)
                .Include(s => s.Situacao)
                .Include(s => s.Itens)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
                //.Include(c => c.Itens.Select(i => i.Observacoes))
                //.Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                //.Include(c => c.Itens.Select(i => i.Extras))
                //.Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    FormaPagamento = p.FormaPagamento,
                    DataPedido = p.DataHora,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    DescricaoSituacao = p.Situacao.Descricao,
                    ValorTotal = p.ValorTotal,
                    TaxaEntrega = p.TaxaEntrega,
                    ValorDesconto = p.ValorDesconto,
                    PercentualDesconto = p.PercentualDesconto,
                    MotivoDesconto = p.MotivoDesconto,
                    MotivoCancelamento = p.MotivoCancelamento,
                    PedidoExterno = p.PedidoExterno,
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
                    FeedbackCliente = p.FeedbackCliente,
                    //PortaImpressaoComandaEntrega = _contexto.ParametrosSistema.Where(a => a.CodParametro == CodigosParametros.COD_PARAMETRO_PORTA_IMPRESSORA_COMANDA).FirstOrDefault().ValorParametro,
                    //Itens = p.Itens.Select(i => new ItemPedidoViewModel
                    //{
                    //    CodItem = i.CodItemCardapio,
                    //    SeqItem = i.SeqItem,
                    //    DescricaoItem = i.ItemCardapio.Nome,
                    //    Quantidade = i.Quantidade,
                    //    PrecoUnitario = i.PrecoUnitario,
                    //    ValorExtras = i.ValorExtras,
                    //    ValorTotalItem = i.ValorTotal,
                    //    ObservacaoLivre = i.ObservacaoLivre,
                    //    Obs = i.Observacoes.Select(o => new ObservacaoItemPedidoViewModel
                    //    {
                    //        CodObservacao = o.CodObservacao,
                    //        DescricaoObservacao = o.Observacao.DescricaoObservacao
                    //    }).ToList().Union(new List<ObservacaoItemPedidoViewModel> { new ObservacaoItemPedidoViewModel { CodObservacao = (i.ObservacaoLivre != "" && i.ObservacaoLivre != null ? -1 : -2), DescricaoObservacao = i.ObservacaoLivre } }).ToList().Where(o => o.CodObservacao >= -1).ToList(),
                    //    extras = i.Extras.Select(e => new ExtraItemPedidoViewModel
                    //    {
                    //        CodOpcaoExtra = e.CodOpcaoExtra,
                    //        DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra,
                    //        Preco = e.Preco
                    //    }).ToList()
                    //}).ToList().OrderBy(i => i.SeqItem).ToList()
                })
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return pedidos;
        }

        public async Task<List<PedidoViewModel>> GetPedidosAbertos(int? codPedido)
        {
            var dataHora = DateTime.Now.AddDays(-2);
            var impressoraComanda = ParametroRepository.GetEnderecoImpressoraComanda();

            var pedidos = await _contexto.Pedidos.Where(p => !(new List<int> { 5, 9 }).Contains(p.CodSituacao) && (p.DataHora > dataHora || p.CodSituacao < 4) && p.CodPedido == (codPedido != null ? codPedido.Value : p.CodPedido))
                .Include(s => s.Situacao)
                .Include(s => s.Itens)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
                .Include(s => s.Itens.Select(i => i.ItemCardapio.ImpressorasAssociadas))
                .Include(s => s.Itens.Select(i => i.ItemCardapio.ImpressorasAssociadas.Select(a => a.ImpressoraProducao)))
                .Include(c => c.Itens.Select(i => i.Observacoes))
                .Include(c => c.Itens.Select(i => i.Observacoes.Select(o => o.Observacao)))
                .Include(c => c.Itens.Select(i => i.Extras))
                .Include(c => c.Itens.Select(i => i.Extras.Select(e => e.OpcaoExtra)))
                .Select(p => new PedidoViewModel
                {
                    FormaPagamento = p.FormaPagamento,
                    DataPedido = p.DataHora,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    DescricaoSituacao = p.Situacao.Descricao,
                    ValorTotal = p.ValorTotal,
                    TaxaEntrega = p.TaxaEntrega,
                    BandeiraCartao = p.BandeiraCartao,
                    Troco = p.Troco,
                    TrocoPara = p.TrocoPara,
                    Usuario = p.Usuario,
                    PedidoExterno = p.PedidoExterno,
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
                    Itens = p.Itens.Where(i => !i.Cancelado).Select(i => new ItemPedidoViewModel
                    {
                        CodItem = i.CodItemCardapio,
                        SeqItem = i.SeqItem,
                        DescricaoItem = i.ItemCardapio.Nome,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        ValorExtras = i.ValorExtras,
                        ValorTotalItem = i.ValorTotal,
                        ObservacaoLivre = i.ObservacaoLivre,
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

            return pedidos;
        }
    }
}