using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrasaoSolution.Web.Helpers
{
    public static class Util
    {
        //public static string GetDescricaoFormaPagamentoPedido(string formaPagamento)
        //{
        //    switch (formaPagamento)
        //    {
        //        case "D":
        //            return "Dinheiro";
        //        case "C":
        //            return "Cartão de crédito";
        //        case "B":
        //            return "Cartão de débito";
        //        case "A":
        //            return "Ticket refeição Alelo";
        //        default:
        //            return "Forma de pagamento não definida";
        //    }
        //}

        public static string GetDescricaoSituacaoPedido(int codSituacao)
        {
            switch (codSituacao)
            {
                case 0:
                    return "Em aberto";
                case 1:
                    return "Aguardando confirmação";
                case 2:
                    return "Confirmado";
                case 3:
                    return "Em preparação";
                case 4:
                    return "Em processo de entrega";
                case 5:
                    return "Concluído";
                case 9:
                    return "Cancelado";
                default:
                    return "Situação não definida";
            }
        }
    }
}