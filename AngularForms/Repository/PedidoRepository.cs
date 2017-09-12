using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AngularForms.Model;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AngularForms.Repository
{
    public class PedidoRepository
    {
        private BrasaoContext _contexto;

        public PedidoRepository()
        {
            _contexto = new BrasaoContext();
        }

        public async Task FinalizaPedido(int codPedido)
        {
            var ped = _contexto.Pedidos.Where(p => p.CodPedido == codPedido).FirstOrDefault();

            if (ped != null)
            {
                ped.CodSituacao = (int)SituacaoPedidoEnum.Concluido;
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<int> GravaPedido(PedidoViewModel pedidoViewModel, string loginUsuario)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    Pedido ped = new Pedido();
                    ped.BairroEntrega = pedidoViewModel.DadosCliente.Bairro;
                    ped.BandeiraCartao = pedidoViewModel.BandeiraCartao;
                    ped.CidadeEntrega = pedidoViewModel.DadosCliente.Cidade;
                    ped.CodSituacao = pedidoViewModel.Situacao;
                    ped.ComplementoEntrega = pedidoViewModel.DadosCliente.Complemento;
                    ped.DataHora = DateTime.Now;
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
                    ped.Usuario = loginUsuario;
                    ped.ValorTotal = pedidoViewModel.ValorTotal;
                    _contexto.Pedidos.Add(ped);
                    await _contexto.SaveChangesAsync();

                    foreach (var itemViewModel in pedidoViewModel.Itens)
                    {
                        var item = new ItemPedido();
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

                    await _contexto.SaveChangesAsync();

                    dbContextTransaction.Commit();

                    return ped.CodPedido;
                }
                catch (Exception ex)
                {
                    throw ex; ;
                }
            }
        }

        public async Task<PedidoViewModel> GetPedidoAberto(string loginUsuario)
        {
            return await _contexto.Pedidos.Where(p => new List<int> { (int)SituacaoPedidoEnum.AguardandoConfirmacao, (int)SituacaoPedidoEnum.Confirmado, (int)SituacaoPedidoEnum.EmPreparacao, (int)SituacaoPedidoEnum.EmProcessoEntrega }.Contains(p.CodSituacao) && p.Usuario == loginUsuario)
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
            var pedidos = await _contexto.Pedidos.Where(p => p.Usuario == loginUsuario)
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
                        }).ToList().Union(new List<ObservacaoItemPedidoViewModel> { new ObservacaoItemPedidoViewModel { CodObservacao = -1, DescricaoObservacao = i.ObservacaoLivre } }).ToList(),
                        extras = i.Extras.Select(e => new ExtraItemPedidoViewModel
                        {
                            CodOpcaoExtra = e.CodOpcaoExtra,
                            DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra,
                            Preco = e.Preco
                        }).ToList()
                    }).ToList()
                })
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return pedidos;
        }

        public async Task<List<PedidoViewModel>> GetPedidosAbertos()
        {
            var pedidos = await _contexto.Pedidos.Where(p => !(new List<int> { 5, 9 }).Contains(p.CodSituacao))
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
                    BandeiraCartao = p.BandeiraCartao,
                    Troco = p.Troco,
                    TrocoPara = p.TrocoPara,
                    Usuario = p.Usuario,
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
                        }).ToList().Union(new List<ObservacaoItemPedidoViewModel> { new ObservacaoItemPedidoViewModel { CodObservacao = -1, DescricaoObservacao = i.ObservacaoLivre } }).ToList(),
                        extras = i.Extras.Select(e => new ExtraItemPedidoViewModel
                        {
                            CodOpcaoExtra = e.CodOpcaoExtra,
                            DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra,
                            Preco = e.Preco
                        }).ToList()
                    }).ToList()
                })
                .OrderBy(p => p.DataPedido)
                .ToListAsync();

            return pedidos;
        }
    }
}