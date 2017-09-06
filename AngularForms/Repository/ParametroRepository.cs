using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AngularForms.Model;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AngularForms.Repository
{
    public static class ParametroRepository
    {
        private static BrasaoContext _contexto = new BrasaoContext();

        public static double GetTaxaEntrega()
        {
            var par = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TAXA_ENTREGA).FirstOrDefault();
            if (par != null)
            {
                return Convert.ToDouble(par.ValorParametro);
            }

            return 3.5;
        }
    }
}