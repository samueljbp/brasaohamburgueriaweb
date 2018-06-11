using System;
using System.Data.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoSolution.Model;
using BrasaoSolution.Repository.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using BrasaoSolution.Helper;

namespace BrasaoSolution.Repository
{
    public class ConsultasRepository
    {
        private BrasaoContext _contexto;

        public ConsultasRepository()
        {
            _contexto = new BrasaoContext();
        }

        public async Task<List<TaxasEntregaViewModel>> GetTaxasEntrega(DateTime? inicio, DateTime? fim, int? codEntregador, int? codEmpresa)
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

            string query = Queries.QUERY_TAXAS_ENTREGA_EMPRESA;
            if (codEmpresa.HasValue)
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_empresa", codEmpresa.Value));
            }
            else
            {
                query = Queries.QUERY_TAXAS_ENTREGA_TODAS;
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_empresa", DBNull.Value));
            }

            var retorno = await _contexto.Database.SqlQuery<TaxasEntregaViewModel>(query, parametros.ToArray()).ToListAsync();

            retorno = retorno.Where(i => SessionData.EmpresasInt.Contains(i.CodEmpresa != null ? i.CodEmpresa.Value : 0)).ToList();

            return retorno;
        }

        public async Task<List<ProdutosVendidosViewModel>> GetProdutosVendidos(DateTime? inicio, DateTime? fim, int? codClasse, int? codEmpresa)
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

            string query = Queries.QUERY_PRODUTOS_VENDIDOS_EMPRESA;
            if (codEmpresa.HasValue)
            {
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_empresa", codEmpresa.Value));
            }
            else
            {
                query = Queries.QUERY_PRODUTOS_VENDIDOS_TODAS;
                parametros.Add(new System.Data.SqlClient.SqlParameter("cod_empresa", DBNull.Value));
            }

            var retorno = await _contexto.Database.SqlQuery<ProdutosVendidosViewModel>(query, parametros.ToArray()).ToListAsync();

            retorno = retorno.Where(i => SessionData.EmpresasInt.Contains(i.CodEmpresa != null ? i.CodEmpresa.Value : 0)).ToList();

            return retorno;
        }

        public async Task<List<PedidoViewModel>> GetPedidosConsulta(DateTime? inicio, DateTime? fim, int? codPedido, int? codEmpresa)
        {
            var pedidos = await _contexto.Pedidos
                .Where(p => p.DataHora >= (inicio != null ? inicio.Value : p.DataHora) &&
                            p.DataHora <= (fim != null ? fim.Value : p.DataHora) &&
                            p.CodEmpresa == (codEmpresa != null ? codEmpresa.Value : p.CodEmpresa) &&
                            (p.CodPedido == (codPedido != null ? codPedido.Value : p.CodPedido)) &&
                            ((p.CodSituacao >= 2 && p.CodSituacao < 9 && codPedido == null) || codPedido != null))
                .Include(s => s.Empresa)
                .Include(s => s.Bairro)
                .Include(s => s.Bairro.Cidade)
                .Include(s => s.Situacao)
                .Include(s => s.Itens)
                .Include(s => s.FormaPagamentoRef)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
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
                    ValorDesconto = p.ValorDesconto,
                    PercentualDesconto = p.PercentualDesconto,
                    MotivoDesconto = p.MotivoDesconto,
                    MotivoCancelamento = p.MotivoCancelamento,
                    PedidoExterno = p.PedidoExterno,
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
                    FeedbackCliente = p.FeedbackCliente,
                })
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            pedidos = pedidos.Where(i => SessionData.EmpresasInt.Contains(i.CodEmpresa)).ToList();

            return pedidos;
        }
    }
}