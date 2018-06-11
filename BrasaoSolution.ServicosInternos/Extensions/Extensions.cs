using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static void ValidaRetornoImpressora(this int idRetorno, string porta)
        {
            if (idRetorno == 0)
            {
                throw new Exception("Falha de comunicação durante o envio dos comandos de texto para a porta " + porta + ".");
            }
        }
    }
}