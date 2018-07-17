using System;
using System.Collections.Generic;
using System.Linq;
using BrasaoSolution.Repository.Context;
using System.Threading.Tasks;
using System.Globalization;
using System.Configuration;
using BrasaoSolution.ViewModel;

namespace BrasaoSolution.Repository
{
    public class ServidorSMTP
    {
        public string Endereco { get; set; }
        public string Porta { get; set; }
        public string RemetentePadrao { get; set; }
    }

    public static class ParametroRepository
    {
        public static async Task<List<DiaSemanaViewModel>> GetDiasSemana()
        {
            List<DiaSemanaViewModel> dias = new List<DiaSemanaViewModel>();

            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Sunday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Sunday)));
            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Monday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Monday)));
            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Tuesday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Tuesday)));
            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Wednesday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Wednesday)));
            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Thursday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Thursday)));
            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Friday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Friday)));
            dias.Add(new DiaSemanaViewModel((int)DayOfWeek.Saturday, new CultureInfo("pt-BR").DateTimeFormat.GetDayName((DayOfWeek)DayOfWeek.Saturday)));

            return dias;
        }

        public static List<ParametroSistemaViewModel> GetParametrosSistema()
        {
            BrasaoContext _contexto = new BrasaoContext();

            return _contexto.ParametrosSistema.Select(p => new ParametroSistemaViewModel { CodParametro = p.CodParametro, DescricaoParametro = p.DescricaoParametro, ValorParametro = p.ValorParametro }).OrderBy(p => p.CodParametro).ToList();
        }

        public static double GetTaxaEntrega()
        {
            var par = SessionData.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TAXA_ENTREGA).FirstOrDefault();
            if (par != null)
            {
                return Convert.ToDouble(par.ValorParametro);
            }

            return 3.5;
        }

        public async static Task AbreFechaCasa(bool aberta)
        {
            BrasaoContext _contexto = new BrasaoContext();

            var empresa = _contexto.Empresas.Where(e => e.CodEmpresa == SessionData.CodLojaSelecionada).FirstOrDefault();

            if (empresa != null)
            {
                empresa.CasaAberta = (aberta ? "1" : "0");

                await _contexto.SaveChangesAsync();

                SessionData.RefreshParam(SessionData.Empresas);
            }
        }

        public static bool CasaAberta()
        {
            var empresa = SessionData.Empresas.Where(e => e.CodEmpresa == SessionData.CodLojaSelecionada).FirstOrDefault();

            if (empresa != null)
            {
                var casaAberta = Convert.ToInt32(empresa.CasaAberta);

                if (casaAberta <= 0)
                {
                    return false;
                }
            }

            var diaSemana = (int)DateTime.Now.DayOfWeek;

            var horarios = SessionData.FuncionamentosEstabelecimento.Where(p => p.DiaSemana == diaSemana && p.TemDelivery).OrderBy(p => p.Abertura).ToList();

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
            var diaSemana = (int)DateTime.Now.DayOfWeek;

            var abertura = SessionData.FuncionamentosEstabelecimento.Where(p => p.DiaSemana >= diaSemana && p.TemDelivery).OrderBy(p => p.DiaSemana).FirstOrDefault();

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
            var par = SessionData.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_CODIGO_IMPRESSORA_COMANDA).FirstOrDefault();

            if (par != null)
            {
                int codImpressora = Convert.ToInt32(par.ValorParametro);

                if (codImpressora > 0)
                {
                    BrasaoContext _contexto = new BrasaoContext();

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
            var par = SessionData.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TEMPO_MEDIO_ESPERA).FirstOrDefault();

            if (par != null)
            {
                return Convert.ToInt32(par.ValorParametro);
            }

            return 0;
        }

        public static string GetPortaImpressoraCozinha()
        {
            var par = SessionData.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_PORTA_IMPRESSORA_COZINHA).FirstOrDefault();

            if (par != null)
            {
                return par.ValorParametro;
            }

            return "";
        }

        public static bool GetImprimeComandaCozinha()
        {
            var par = SessionData.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_IMPRIME_COMANDA_COZINHA).FirstOrDefault();

            if (par != null)
            {
                return (par.ValorParametro == "1");
            }

            return false;
        }

        public static async Task AlteraTempoMedioEspera(int tempo)
        {
            BrasaoContext _contexto = new BrasaoContext();

            var parTempo = _contexto.ParametrosSistema.Where(p => p.CodParametro == CodigosParametros.COD_PARAMETRO_TEMPO_MEDIO_ESPERA).FirstOrDefault();

            if (parTempo != null)
            {
                parTempo.ValorParametro = tempo.ToString();

                await _contexto.SaveChangesAsync();

                SessionData.RefreshParam(SessionData.FuncionamentosEstabelecimento);
            }
        }
    }
}