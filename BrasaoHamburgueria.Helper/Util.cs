using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrasaoHamburgueria.Helper
{
    public static class BrasaoUtil
    {
        public static string getDescricaoFormaPagamentoPedido(string codFormaPagamento)
        {
            switch (codFormaPagamento)
            {
                case "D":
                    return "Dinheiro";
                case "C":
                    return "Cartão de crédito";
                case "B":
                    return "CArtão de débito";
                case "A":
                    return "Ticket refeição Alelo";
                default:
                    return "Forma de pagamento não definida";
            }
        }
    }
}
