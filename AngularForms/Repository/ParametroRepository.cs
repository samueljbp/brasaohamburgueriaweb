using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Threading;
using System.Globalization;
using System.Configuration;

namespace BrasaoHamburgueria.Web.Repository
{
    public class ServidorSMTP
    {
        public string Endereco { get; set; }
        public string Porta { get; set; }
        public string RemetentePadrao { get; set; }
    }

    public static class ParametroRepository
    {
        public static double GetTaxaEntrega()
        {
            BrasaoContext _contexto = new BrasaoContext();

            var par = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TAXA_ENTREGA).FirstOrDefault();
            if (par != null)
            {
                return Convert.ToDouble(par.ValorParametro);
            }

            return 3.5;
        }

        public async static Task AbreFechaCasa(bool aberta)
        {
            BrasaoContext _contexto = new BrasaoContext();

            var parCasaAberta = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_CASA_ABERTA).FirstOrDefault();

            if (parCasaAberta != null)
            {
                parCasaAberta.ValorParametro = (aberta ? "1" : "0");

                await _contexto.SaveChangesAsync();
            }
        }

        public static bool CasaAberta()
        {
            BrasaoContext _contexto = new BrasaoContext();

            var parCasaAberta = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_CASA_ABERTA).FirstOrDefault();

            if (parCasaAberta != null)
            {
                var casaAberta = Convert.ToInt32(parCasaAberta.ValorParametro);

                if (casaAberta <= 0)
                {
                    return false;
                }
            }

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

        public static FuncionamentoEstabelecimentoViewModel GetHorarioAbertura()
        {
            BrasaoContext _contexto = new BrasaoContext();

            var diaSemana = (int)DateTime.Now.DayOfWeek;

            var abertura = _contexto.FuncionamentosEstabelecimento.Where(p => p.DiaSemana >= diaSemana && p.TemDelivery).OrderBy(p => p.Abertura).FirstOrDefault();

            FuncionamentoEstabelecimentoViewModel horario = new FuncionamentoEstabelecimentoViewModel();

            horario.Abertura = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + abertura.Abertura);
            horario.Fechamento = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy") + " " + abertura.Fechamento);

            horario.DiaSemana = abertura.DiaSemana;
            horario.DescricaoDiaSemana = new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)abertura.DiaSemana);

            return horario;
        }

        public static string GetEmManutencao()
        {
            if (ConfigurationManager.AppSettings["EmManutencao"] != null && ConfigurationManager.AppSettings["EmManutencao"].ToString() == "S")
            {
                return "S";
            }

            return "N";
        }

        public static ServidorSMTP GetServidorSMTP()
        {
            ServidorSMTP serv = new ServidorSMTP();

            if (ConfigurationManager.AppSettings["ServidorSMTP"] != null)
            {
                serv.Endereco = ConfigurationManager.AppSettings["ServidorSMTP"].ToString();
            }

            if (ConfigurationManager.AppSettings["PortaSMTP"] != null)
            {
                serv.Porta = ConfigurationManager.AppSettings["PortaSMTP"].ToString();
            }

            if (ConfigurationManager.AppSettings["RemetentePadrao"] != null)
            {
                serv.RemetentePadrao = ConfigurationManager.AppSettings["RemetentePadrao"].ToString();
            }

            return serv;
        }

        public static string GetEnderecoImpressoraComanda()
        {
            BrasaoContext _contexto = new BrasaoContext();

            var par = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_CODIGO_IMPRESSORA_COMANDA).FirstOrDefault();

            if (par != null)
            {
                int codImpressora = Convert.ToInt32(par.ValorParametro);

                if (codImpressora > 0)
                {
                    var impressora = _contexto.ImpressorasProducao.Find(codImpressora);

                    if (impressora != null)
                    {
                        return impressora.Porta;
                    }
                }
            }

            return "";
        }

        public static int GetTempoMedioEspera()
        {
            BrasaoContext _contexto = new BrasaoContext();

            var par = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TEMPO_MEDIO_ESPERA).FirstOrDefault();

            if (par != null)
            {
                return Convert.ToInt32(par.ValorParametro);
            }

            return 0;
        }

        public static async Task AlteraTempoMedioEspera(int tempo)
        {
            BrasaoContext _contexto = new BrasaoContext();

            var parTempo = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TEMPO_MEDIO_ESPERA).FirstOrDefault();

            if (parTempo != null)
            {
                parTempo.ValorParametro = tempo.ToString();

                await _contexto.SaveChangesAsync();
            }
        }
    }
}