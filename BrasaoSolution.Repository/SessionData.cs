using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.SessionState;
using BrasaoSolution.Model;
using System.Security.Claims;
using System.Net.Http;
using BrasaoSolution.Repository.Context;
using System.Diagnostics;
using BrasaoSolution.Helper;
using Newtonsoft.Json;

namespace BrasaoSolution.Repository
{
    public static class SessionData
    {
        private static string TryGetCurrentUsername()
        {
            try
            {
                return HttpContext.Current.User.Identity.Name;
            }
            catch (Exception ex)
            {
                BrasaoUtil.GravaLog("Erro no método TryGetUsername: " + ex.Message, EventLogEntryType.Error);
            }

            return "";
        }

        private static T TryAccessSessionObject<T>(string name)
        {
            try
            {
                object value = HttpContext.Current.Session[name];
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                BrasaoUtil.GravaLog("Erro no método TryAccessSessionObject com tipo " + typeof(T).ToString() + " e nome " + name + ": " + ex.Message, EventLogEntryType.Error);

                return default(T);
            }
        }

        private static void TrySetSessionObject<T>(T value, string name)
        {
            try
            {
                HttpContext.Current.Session.Add(name, (T)Convert.ChangeType(value, typeof(T)));
            }
            catch (Exception ex)
            {
                BrasaoUtil.GravaLog("Erro no método TrySetSessionObject com tipo " + typeof(T).ToString() + " e nome " + name + ": " + ex.Message, EventLogEntryType.Error);
            }
        }

        public static int CodLojaSelecionada
        {
            get
            {
                int codLojaSelecionada = TryAccessSessionObject<int>("CodLojaSelecionada");
                if (codLojaSelecionada > 0)
                {
                    return codLojaSelecionada;
                }

                string usuario = TryGetCurrentUsername();
                ApplicationDbContext contexto = new ApplicationDbContext();
                Usuario usuarioDb = contexto.DadosUsuarios.Where(d => d.Email == usuario).FirstOrDefault();
                int lojaDb = 0;

                string domainName = "";
                try
                {
                    domainName = HttpContext.Current.Request.Url.Host;
                }
                catch (Exception ex)
                {
                    BrasaoUtil.GravaLog("Erro ao tentar acessar HttpContext.Current.Request.Url.Host: " + ex.Message, EventLogEntryType.Error);
                }

                //se a loja tiver URL propria e o acesso estiver sendo feito por ela, cai nela
                if (domainName != "" && HttpContext.Current.Session != null)
                {
                    var emps = SessionData.TodasEmpresas.Where(e => e.UrlSite == domainName).ToList();
                    if (emps != null && emps.Count > 0)
                    {
                        if (emps.Count == 1)
                        {
                            TrySetSessionObject(emps[0].CodEmpresa, "CodLojaSelecionada");
                            return emps[0].CodEmpresa;
                        }
                        else
                        {
                            if (usuarioDb != null && usuarioDb.CodEmpresaPreferencial != null)
                            {
                                if (emps.Where(e => e.CodEmpresa == usuarioDb.CodEmpresaPreferencial.Value).Count() > 0)
                                {
                                    TrySetSessionObject(usuarioDb.CodEmpresaPreferencial.Value, "CodLojaSelecionada");
                                    return usuarioDb.CodEmpresaPreferencial.Value;
                                }
                                else
                                {
                                    var matriz = emps.Where(e => e.CodEmpresaMatriz == null).FirstOrDefault();
                                    if (matriz != null)
                                    {
                                        TrySetSessionObject(matriz.CodEmpresa, "CodLojaSelecionada");
                                        return matriz.CodEmpresa;
                                    }
                                }
                            }
                            else
                            {
                                var matriz = emps.Where(e => e.CodEmpresaMatriz == null).FirstOrDefault();
                                if (matriz != null)
                                {
                                    TrySetSessionObject(matriz.CodEmpresa, "CodLojaSelecionada");
                                    return matriz.CodEmpresa;
                                }
                            }
                        }
                    }
                }


                if (usuarioDb != null && usuarioDb.CodEmpresaPreferencial != null)
                {
                    lojaDb = usuarioDb.CodEmpresaPreferencial.Value;
                }
                else
                {
                    lojaDb = SessionData.Empresas.Where(e => e.CodEmpresaMatriz == null).FirstOrDefault().CodEmpresa;

                    if (lojaDb == 0)
                    {
                        lojaDb = SessionData.Empresas.FirstOrDefault().CodEmpresa;
                    }
                }

                TrySetSessionObject(lojaDb, "CodLojaSelecionada");

                return lojaDb;
            }
            set
            {
                TrySetSessionObject<int>(value, "CodLojaSelecionada");
            }
        }

        public static ProgramaFidelidadeUsuarioViewModel ProgramaFidelidadeUsuario
        {
            get
            {
                BrasaoUtil.GravaLog("Busca programa de fidelidade na sessão.", EventLogEntryType.Information);
                ProgramaFidelidadeUsuarioViewModel prog = TryAccessSessionObject<ProgramaFidelidadeUsuarioViewModel>("ProgramaFidelidade");

                if (prog != null)
                {
                    BrasaoUtil.GravaLog("Encontrou: " + JsonConvert.SerializeObject(prog), EventLogEntryType.Information);
                    if (prog.CodEmpresa != null && prog.CodEmpresa != SessionData.CodLojaSelecionada)
                    {
                        BrasaoUtil.GravaLog("É de outra empresa. Retorna null.", EventLogEntryType.Information);
                        return null;
                    }

                    BrasaoUtil.GravaLog("Retorna objeto.", EventLogEntryType.Information);
                    return prog;
                }

                BrasaoUtil.GravaLog("Não encontrou.", EventLogEntryType.Information);

                var rep = new ProgramaFidelidadeRepository();

                BrasaoUtil.GravaLog("Otem usuário logado.", EventLogEntryType.Information);
                string userName = TryGetCurrentUsername();
                BrasaoUtil.GravaLog("Usuário logado: " + userName, EventLogEntryType.Information);
                BrasaoUtil.GravaLog("Busca programa de fidelidade do usuário logado.", EventLogEntryType.Information);
                var progDb = rep.GetProgramaFidelidadeUsuario(userName, SessionData.CodLojaSelecionada);

                if (progDb == null)
                {
                    BrasaoUtil.GravaLog("Não eoncontrou.", EventLogEntryType.Information);
                    progDb = new ProgramaFidelidadeUsuarioViewModel();
                }

                BrasaoUtil.GravaLog("Encontrou: " + JsonConvert.SerializeObject(progDb), EventLogEntryType.Information);

                TrySetSessionObject(progDb, "ProgramaFidelidade");

                BrasaoUtil.GravaLog("Guardou na sessão.", EventLogEntryType.Information);

                return progDb;
            }
        }

        public static List<ParametroSistemaViewModel> ParametrosSistema
        {
            get
            {
                var pars = TryAccessSessionObject<List<ParametroSistemaViewModel>>("ParametrosSistema");
                if (pars != null)
                {
                    return pars;
                }

                var paramsDb = ParametroRepository.GetParametrosSistema();

                if (paramsDb == null)
                {
                    paramsDb = new List<ParametroSistemaViewModel>();
                }

                TrySetSessionObject(paramsDb, "ParametrosSistema");

                return paramsDb;
            }
        }

        public static List<FuncionamentoEstabelecimento> FuncionamentosEstabelecimento
        {
            get
            {
                var funcs = TryAccessSessionObject<List<FuncionamentoEstabelecimento>>("FuncionamentoEstabelecimento");
                if (funcs != null)
                {
                    return funcs;
                }

                BrasaoContext _context = new Context.BrasaoContext();
                var funcsDb = _context.FuncionamentosEstabelecimento.ToList();

                if (funcsDb == null)
                {
                    funcsDb = new List<FuncionamentoEstabelecimento>();
                }

                TrySetSessionObject(funcsDb, "FuncionamentoEstabelecimento");

                return funcsDb;
            }
        }

        public static Empresa LojaSelecionada
        {
            get
            {
                return SessionData.TodasEmpresas.Where(e => e.CodEmpresa == SessionData.CodLojaSelecionada).FirstOrDefault();
            }
        }

        public static List<int> EmpresasInt
        {
            get
            {
                var retorno = SessionData.Empresas.Select(e => e.CodEmpresa).ToList();

                retorno.Add(0);

                return retorno;
            }
        }

        public static List<Empresa> Empresas
        {
            get
            {
                var emps = TryAccessSessionObject<List<Empresa>>("Empresas");
                if (emps != null)
                {
                    return emps;
                }

                List<string> empresasUsuario = new List<string>();
                try
                {
                    var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;
                    empresasUsuario = claims.Where(c => c.Type == ApplicationDbContext.CLAIM_EMPRESA).Select(c => c.Value).ToList();
                }
                catch(Exception ex)
                {
                    BrasaoUtil.GravaLog("Erro no método Empresas acessando HttpContext.Current.User.Identity: " + ex.Message, EventLogEntryType.Error);
                }

                BrasaoContext _context = new Context.BrasaoContext();
                var empsDb = _context.Empresas.Include(x => x.Bairro).Include(x => x.Bairro.Cidade).Where(x => (empresasUsuario.Count == 0 || empresasUsuario.Contains(x.CodEmpresa.ToString())) && x.EmpresaAtiva).OrderBy(e => e.CodEmpresa).ToList();

                if (empsDb == null)
                {
                    empsDb = new List<Empresa>();
                }

                TrySetSessionObject(empsDb, "Empresas");

                foreach (var e in empsDb)
                {
                    e.TextoInstitucional = "";
                }

                return empsDb;
            }
        }

        public static List<Empresa> TodasEmpresas
        {
            get
            {
                var emps = TryAccessSessionObject<List<Empresa>>("TodasEmpresas");
                if (emps != null)
                {
                    return emps;
                }

                BrasaoContext _context = new Context.BrasaoContext();
                var empsDb = _context.Empresas.Include(x => x.Bairro).Include(x => x.Bairro.Cidade).Where(x => x.EmpresaAtiva).OrderBy(e => e.CodEmpresa).ToList();

                if (empsDb == null)
                {
                    empsDb = new List<Empresa>();
                }

                TrySetSessionObject(empsDb, "TodasEmpresas");

                return empsDb;
            }
        }

        public static void RefreshParam(object param)
        {
            try
            {
                if (param is List<FuncionamentoEstabelecimento>)
                {
                    if (HttpContext.Current.Session["FuncionamentoEstabelecimento"] != null)
                    {
                        HttpContext.Current.Session.Remove("FuncionamentoEstabelecimento");
                    }

                    return;
                }

                if (param is List<ParametroSistemaViewModel>)
                {
                    if (HttpContext.Current.Session["ParametrosSistema"] != null)
                    {
                        HttpContext.Current.Session.Remove("ParametrosSistema");
                    }

                    return;
                }


                if (param is ProgramaFidelidadeUsuarioViewModel)
                {
                    if (HttpContext.Current.Session["ProgramaFidelidade"] != null)
                    {
                        HttpContext.Current.Session.Remove("ProgramaFidelidade");
                    }

                    return;
                }

                if (param is List<Empresa>)
                {
                    if (HttpContext.Current.Session["Empresas"] != null)
                    {
                        HttpContext.Current.Session.Remove("Empresas");
                    }

                    if (HttpContext.Current.Session["TodasEmpresas"] != null)
                    {
                        HttpContext.Current.Session.Remove("TodasEmpresas");
                    }

                    return;
                }
            }
            catch(Exception ex)
            {
                BrasaoUtil.GravaLog("Erro no método RefreshParam com tipo " + param.GetType().Name + ": " + ex.Message, EventLogEntryType.Error);
            }
        }
    }
}