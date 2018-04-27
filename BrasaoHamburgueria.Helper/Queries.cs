using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrasaoHamburgueria.Helper
{
    public static class Queries
    {
        public const string QUERY_PRODUTOS_VENDIDOS = @"select IC.COD_ITEM_CARDAPIO CodItemCardapio, IC.NOME Nome, IC.COD_CLASSE CodClasse, CI.DESCRICAO_CLASSE DescricaoClasse, sum(IP.QUANTIDADE) Quantidade, sum(IP.VALOR_TOTAL) ValorTotal
                                                          from ITEM_PEDIDO IP inner join
	                                                           PEDIDO P on IP.COD_PEDIDO = P.COD_PEDIDO inner join
	                                                           ITEM_CARDAPIO IC on IP.COD_ITEM_CARDAPIO = IC.COD_ITEM_CARDAPIO INNER JOIN
	                                                           CLASSE_ITEM_CARDAPIO CI ON CI.COD_CLASSE = IC.COD_CLASSE
                                                         where P.COD_SITUACAO not in (1, 9)
                                                           and P.DATA_HORA >= ISNULL(@data_inicio, P.DATA_HORA - 1)
                                                           and P.DATA_HORA < ISNULL(@data_fim, P.DATA_HORA + 1)
                                                           and IC.COD_CLASSE = ISNULL(@cod_classe, IC.COD_CLASSE)
                                                        group by IC.COD_ITEM_CARDAPIO, IC.NOME, IC.COD_CLASSE, CI.DESCRICAO_CLASSE
                                                        order by sum(IP.QUANTIDADE) desc";
    }
}
