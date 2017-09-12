using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AngularForms.Model;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Threading;
using System.Globalization;

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

        public static bool CasaAberta()
        {
            var diaSemana = (int)DateTime.Now.DayOfWeek;

            var horarios = _contexto.FuncionamentosEstabelecimento.Where(p => p.DiaSemana == diaSemana && p.TemDelivery).OrderBy(p => p.Abertura).ToList();

            if (horarios != null)
            {
                foreach(var horario in horarios)
                {
                    var abertura = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + horario.Abertura);
                    var fechamento = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + horario.Fechamento);

                    if (horario.TemDelivery && DateTime.Now >= abertura && DateTime.Now <= fechamento)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static HorarioFuncionamento GetHorarioAbertura()
        {
            var diaSemana = (int)DateTime.Now.DayOfWeek;

            var abertura = _contexto.FuncionamentosEstabelecimento.Where(p => p.DiaSemana >= diaSemana && p.TemDelivery).OrderBy(p => p.Abertura).FirstOrDefault();

            HorarioFuncionamento horario = new HorarioFuncionamento();

            horario.Abertura = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + abertura.Abertura);
            horario.Fechamento = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + abertura.Fechamento);

            horario.DiaSemana = new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)abertura.DiaSemana);

            return horario;
        }
    }
}