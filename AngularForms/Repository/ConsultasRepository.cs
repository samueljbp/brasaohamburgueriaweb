using System;
using System.Data.Sql;
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
    public class ConsultasRepository
    {
        private BrasaoContext _contexto;

        public ConsultasRepository()
        {
            _contexto = new BrasaoContext();
        }

        public async Task<List<ProdutosVendidosViewModel>> GetProdutosVendidos(DateTime? inicio, DateTime? fim, int? codClasse)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            if (inicio.HasValue)
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("data_inicio", inicio.Value));
            }
            else
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("data_inicio", DBNull.Value));
            }

            if (fim.HasValue)
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("data_fim", fim.Value));
            }
            else
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("data_fim", DBNull.Value));
            }

            if (codClasse.HasValue)
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_classe", codClasse.Value));
            }
            else
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_classe", DBNull.Value));
            }

            return await _contexto.Database.SqlQuery<ProdutosVendidosViewModel>(Queries.QUERY_PRODUTOS_VENDIDOS, parametros.ToArray()).ToListAsync();
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
    }
}