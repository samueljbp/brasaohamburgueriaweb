using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.SessionState;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Repository;
using System.Security.Claims;

namespace BrasaoHamburgueria.Web.Helpers
{
    public static class SessionData
    {
        public static int CodLojaSelecionada
        {
            get
            {
                var lojaSession = HttpContext.Current.Session["CodLojaSelecionada"];
                if (lojaSession != null)
                {
                    return (int)lojaSession;
                }

                var usuario = HttpContext.Current.User.Identity.Name;
                var contexto = new BrasaoHamburgueria.Web.Context.ApplicationDbContext();
                var usuarioDb = contexto.DadosUsuarios.Where(d => d.Email == usuario).FirstOrDefault();

                var lojaDb = 0;

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

                HttpContext.Current.Session.Add("CodLojaSelecionada", lojaDb);
                return lojaDb;
            }
            set
            {
                if (HttpContext.Current.Session["CodLojaSelecionada"] != null)
                {
                    HttpContext.Current.Session["CodLojaSelecionada"] = value;
                }
                else
                {
                    HttpContext.Current.Session.Add("CodLojaSelecionada", value);
                }
            }
        }

        public static ProgramaFidelidadeUsuarioViewModel ProgramaFidelidadeUsuario
        {
            get
            {
                var programaSession = HttpContext.Current.Session["ProgramaFidelidade"];
                if (programaSession != null)
                {
                    var prog = (ProgramaFidelidadeUsuarioViewModel)programaSession;

                    if (prog.CodEmpresa != SessionData.CodLojaSelecionada)
                    {
                        return null;
                    }

                    return prog;
                }

                var rep = new ProgramaFidelidadeRepository();
                var progDb = rep.GetProgramaFidelidadeUsuario(HttpContext.Current.User.Identity.Name, SessionData.CodLojaSelecionada);

                if (progDb == null)
                {
                    progDb = new ProgramaFidelidadeUsuarioViewModel();
                }

                HttpContext.Current.Session.Add("ProgramaFidelidade", progDb);
                return progDb;
            }
        }

        public static List<ParametroSistemaViewModel> ParametrosSistema
        {
            get
            {
                var parametrosSession = HttpContext.Current.Session["ParametrosSistema"];
                if (parametrosSession != null)
                {
                    return (List<ParametroSistemaViewModel>)parametrosSession;
                }

                var paramsDb = ParametroRepository.GetParametrosSistema();

                if (paramsDb == null)
                {
                    paramsDb = new List<ParametroSistemaViewModel>();
                }

                HttpContext.Current.Session.Add("ParametrosSistema", paramsDb);

                return paramsDb;
            }
        }

        public static List<FuncionamentoEstabelecimento> FuncionamentosEstabelecimento
        {
            get
            {
                var funcionamentoSession = HttpContext.Current.Session["FuncionamentoEstabelecimento"];
                if (funcionamentoSession != null)
                {
                    return (List<FuncionamentoEstabelecimento>)funcionamentoSession;
                }

                BrasaoHamburgueria.Web.Context.BrasaoContext _context = new Context.BrasaoContext();
                var funcsDb = _context.FuncionamentosEstabelecimento.ToList();

                if (funcsDb == null)
                {
                    funcsDb = new List<FuncionamentoEstabelecimento>();
                }

                HttpContext.Current.Session.Add("FuncionamentoEstabelecimento", funcsDb);

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
                var empresasSession = HttpContext.Current.Session["Empresas"];
                if (empresasSession != null)
                {
                    return (List<Empresa>)empresasSession;
                }

                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                IEnumerable<Claim> claims = identity.Claims;
                var empresasUsuario = claims.Where(c => c.Type == BrasaoHamburgueria.Web.Context.ApplicationDbContext.CLAIM_EMPRESA).Select(c => c.Value).ToList();

                BrasaoHamburgueria.Web.Context.BrasaoContext _context = new Context.BrasaoContext();
                var empsDb = _context.Empresas.Include(x => x.Bairro).Include(x => x.Bairro.Cidade).Where(x => (empresasUsuario.Count == 0 || empresasUsuario.Contains(x.CodEmpresa.ToString())) && x.EmpresaAtiva).OrderBy(e => e.CodEmpresa).ToList();

                if (empsDb == null)
                {
                    empsDb = new List<Empresa>();
                }

                HttpContext.Current.Session.Add("Empresas", empsDb);

                foreach(var e in empsDb)
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
                var empresasSession = HttpContext.Current.Session["TodasEmpresas"];
                if (empresasSession != null)
                {
                    return (List<Empresa>)empresasSession;
                }

                BrasaoHamburgueria.Web.Context.BrasaoContext _context = new Context.BrasaoContext();
                var empsDb = _context.Empresas.Include(x => x.Bairro).Include(x => x.Bairro.Cidade).Where(x => x.EmpresaAtiva).OrderBy(e => e.CodEmpresa).ToList();

                if (empsDb == null)
                {
                    empsDb = new List<Empresa>();
                }

                HttpContext.Current.Session.Add("TodasEmpresas", empsDb);

                return empsDb;
            }
        }

        public static void RefreshParam(object param)
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
    }
}