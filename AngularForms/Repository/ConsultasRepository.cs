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

        public async Task<List<TaxasEntregaViewModel>> GetTaxasEntrega(DateTime? inicio, DateTime? fim, int? codEntregador)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            if (codEntregador.HasValue)
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_entregador", codEntregador.Value));
            }
            else
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_entregador", DBNull.Value));
            }

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

            return await _contexto.Database.SqlQuery<TaxasEntregaViewModel>(Queries.QUERY_TAXAS_ENTREGA, parametros.ToArray()).ToListAsync();
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

        public async Task<List<PedidoViewModel>> GetPedidosConsulta(DateTime? inicio, DateTime? fim, int? codPedido)
        {
            var pedidos = await _contexto.Pedidos
                .Where(p => p.DataHora >= (inicio != null ? inicio.Value : p.DataHora) &&
                            p.DataHora <= (fim != null ? fim.Value : p.DataHora) &&
                            (p.CodPedido == (codPedido != null ? codPedido.Value : p.CodPedido)) &&
                            ((p.CodSituacao >= 2 && p.CodSituacao < 9 && codPedido == null) || codPedido != null))
                .Include(s => s.Situacao)
                .Include(s => s.Itens)
                .Include(s => s.FormaPagamentoRef)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
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
                })
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return pedidos;
        }
    }
}