using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrasaoSolution.Helper
{
    public static class Queries
    {
        public const string QUERY_PRODUTOS_VENDIDOS_EMPRESA = @"select IC.COD_ITEM_CARDAPIO CodItemCardapio, IC.NOME Nome, IC.COD_CLASSE CodClasse, CI.DESCRICAO_CLASSE DescricaoClasse,
                                                               P.COD_EMPRESA CodEmpresa, E.NOME_FANTASIA NomeEmpresa, 
                                                               sum(IP.QUANTIDADE) Quantidade, sum(IP.VALOR_TOTAL) ValorTotal
                                                          from ITEM_PEDIDO IP inner join
	                                                           PEDIDO P on IP.COD_PEDIDO = P.COD_PEDIDO inner join
	                                                           ITEM_CARDAPIO IC on IP.COD_ITEM_CARDAPIO = IC.COD_ITEM_CARDAPIO INNER JOIN
	                                                           CLASSE_ITEM_CARDAPIO CI ON CI.COD_CLASSE = IC.COD_CLASSE INNER JOIN
                                                               EMPRESA E ON P.COD_EMPRESA = E.COD_EMPRESA
                                                         where P.COD_SITUACAO not in (1, 9)
                                                           and P.DATA_HORA >= ISNULL(@data_inicio, P.DATA_HORA - 1)
                                                           and P.DATA_HORA < ISNULL(@data_fim, P.DATA_HORA + 1)
                                                           and IC.COD_CLASSE = ISNULL(@cod_classe, IC.COD_CLASSE)
                                                           and P.COD_EMPRESA = ISNULL(@cod_empresa, P.COD_EMPRESA)
                                                        group by IC.COD_ITEM_CARDAPIO, IC.NOME, IC.COD_CLASSE, CI.DESCRICAO_CLASSE, P.COD_EMPRESA, E.NOME_FANTASIA
                                                        order by sum(IP.QUANTIDADE) desc";

        public const string QUERY_PRODUTOS_VENDIDOS_TODAS = @"select IC.COD_ITEM_CARDAPIO CodItemCardapio, IC.NOME Nome, IC.COD_CLASSE CodClasse, CI.DESCRICAO_CLASSE DescricaoClasse,
                                                               NULL CodEmpresa, 'Todas' NomeEmpresa, 
                                                               sum(IP.QUANTIDADE) Quantidade, sum(IP.VALOR_TOTAL) ValorTotal
                                                          from ITEM_PEDIDO IP inner join
	                                                           PEDIDO P on IP.COD_PEDIDO = P.COD_PEDIDO inner join
	                                                           ITEM_CARDAPIO IC on IP.COD_ITEM_CARDAPIO = IC.COD_ITEM_CARDAPIO INNER JOIN
	                                                           CLASSE_ITEM_CARDAPIO CI ON CI.COD_CLASSE = IC.COD_CLASSE INNER JOIN
                                                               EMPRESA E ON P.COD_EMPRESA = E.COD_EMPRESA
                                                         where P.COD_SITUACAO not in (1, 9)
                                                           and P.DATA_HORA >= ISNULL(@data_inicio, P.DATA_HORA - 1)
                                                           and P.DATA_HORA < ISNULL(@data_fim, P.DATA_HORA + 1)
                                                           and IC.COD_CLASSE = ISNULL(@cod_classe, IC.COD_CLASSE)
                                                           and P.COD_EMPRESA = ISNULL(@cod_empresa, P.COD_EMPRESA)
                                                        group by IC.COD_ITEM_CARDAPIO, IC.NOME, IC.COD_CLASSE, CI.DESCRICAO_CLASSE
                                                        order by sum(IP.QUANTIDADE) desc";

        public const string QUERY_TAXAS_ENTREGA_EMPRESA = @"select ISNULL(P.COD_ENTREGADOR, 0) CodEntregador,
                                                           P.COD_EMPRESA CodEmpresa, S.NOME_FANTASIA NomeEmpresa,
	                                                       CASE WHEN MAX(E.NOME) IS NULL THEN 'SEM ASSOCIACAO' ELSE MAX(E.NOME) END Nome,
	                                                       COUNT(P.COD_PEDIDO) QtdPedidos,
	                                                       SUM(ISNULL(E.VALOR_POR_ENTREGA, P.TAXA_ENTREGA)) TotalTaxasEntrega
                                                      from PEDIDO P INNER JOIN
                                                           EMPRESA S ON P.COD_EMPRESA = S.COD_EMPRESA LEFT JOIN
	                                                       ENTREGADOR E ON P.COD_ENTREGADOR = E.COD_ENTREGADOR
                                                     where (P.COD_ENTREGADOR = ISNULL(@cod_entregador, P.COD_ENTREGADOR) OR (P.COD_ENTREGADOR IS NULL AND @cod_entregador IS NULL))
                                                       and P.RETIRAR_NA_CASA = 0
                                                       and P.COD_SITUACAO not in (1, 9)
                                                       and P.DATA_HORA >= ISNULL(@data_inicio, P.DATA_HORA - 1)
                                                       and P.DATA_HORA < ISNULL(@data_fim, P.DATA_HORA + 1)
                                                       and P.COD_EMPRESA = ISNULL(@cod_empresa, P.COD_EMPRESA)
                                                    group by ISNULL(P.COD_ENTREGADOR, 0), P.COD_EMPRESA, S.NOME_FANTASIA
                                                    order by ISNULL(P.COD_ENTREGADOR, 0)";

        public const string QUERY_TAXAS_ENTREGA_TODAS = @"select ISNULL(P.COD_ENTREGADOR, 0) CodEntregador,
                                                           NULL CodEmpresa, 'Todas' NomeEmpresa,  
	                                                       CASE WHEN MAX(E.NOME) IS NULL THEN 'SEM ASSOCIACAO' ELSE MAX(E.NOME) END Nome,
	                                                       COUNT(P.COD_PEDIDO) QtdPedidos,
	                                                       SUM(ISNULL(E.VALOR_POR_ENTREGA, P.TAXA_ENTREGA)) TotalTaxasEntrega
                                                      from PEDIDO P INNER JOIN
                                                           EMPRESA S ON P.COD_EMPRESA = S.COD_EMPRESA LEFT JOIN
	                                                       ENTREGADOR E ON P.COD_ENTREGADOR = E.COD_ENTREGADOR
                                                     where (P.COD_ENTREGADOR = ISNULL(@cod_entregador, P.COD_ENTREGADOR) OR (P.COD_ENTREGADOR IS NULL AND @cod_entregador IS NULL))
                                                       and P.RETIRAR_NA_CASA = 0
                                                       and P.COD_SITUACAO not in (1, 9)
                                                       and P.DATA_HORA >= ISNULL(@data_inicio, P.DATA_HORA - 1)
                                                       and P.DATA_HORA < ISNULL(@data_fim, P.DATA_HORA + 1)
                                                       and P.COD_EMPRESA = ISNULL(@cod_empresa, P.COD_EMPRESA)
                                                    group by ISNULL(P.COD_ENTREGADOR, 0)
                                                    order by ISNULL(P.COD_ENTREGADOR, 0)";
    }
}
