using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;

namespace BrasaoSolution.Helper
{
    public static class BrasaoUtil
    {
        //public static string getDescricaoFormaPagamentoPedido(string codFormaPagamento)
        //{
        //    switch (codFormaPagamento)
        //    {
        //        case "D":
        //            return "Dinheiro";
        //        case "C":
        //            return "Cartão de crédito";
        //        case "B":
        //            return "CArtão de débito";
        //        case "A":
        //            return "Ticket refeição Alelo";
        //        default:
        //            return "Forma de pagamento não definida";
        //    }
        //}

        /// <summary>
        /// Formatar uma string CNPJ
        /// </summary>
        /// <param name="CNPJ">string CNPJ sem formatacao</param>
        /// <returns>string CNPJ formatada</returns>
        /// <example>Recebe '99999999999999' Devolve '99.999.999/9999-99'</example>

        public static void GravaLog(string mensagem, EventLogEntryType tipo)
        {
            bool logAtivado = (ConfigurationManager.AppSettings["LogAtivado"].ToString() == "S");

            if (!logAtivado)
            {
                return;
            }

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "BrasaoSolution.Web.Delivery";
                eventLog.WriteEntry(mensagem, tipo);
            }
        }

        public static string FormatCNPJ(string CNPJ)
        {
            return Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
        }

        /// <summary>
        /// Formatar uma string CPF
        /// </summary>
        /// <param name="CPF">string CPF sem formatacao</param>
        /// <returns>string CPF formatada</returns>
        /// <example>Recebe '99999999999' Devolve '999.999.999-99'</example>

        public static string FormatCPF(string CPF)
        {
            return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
        }
        /// <summary>
        /// Retira a Formatacao de uma string CNPJ/CPF
        /// </summary>
        /// <param name="Codigo">string Codigo Formatada</param>
        /// <returns>string sem formatacao</returns>
        /// <example>Recebe '99.999.999/9999-99' Devolve '99999999999999'</example>

        public static string SemFormatacao(string Codigo)
        {
            return Codigo.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
        }

        public static Exception[] GetInnerExceptions(Exception ex)
        {
            List<Exception> exceptions = new List<Exception>();
            exceptions.Add(ex);

            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                exceptions.Add(currentEx);
            }

            // Reverse the order to the innermost is first
            exceptions.Reverse();

            return exceptions.ToArray();
        }
    }
}
