using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Repository;

namespace BrasaoHamburgueria.Web.Helpers
{
    public static class SessionData
    {
        public static ProgramaFidelidadeUsuarioViewModel ProgramaFidelidadeUsuario
        {
            get
            {
                var programaSession = HttpContext.Current.Session["ProgramaFidelidade"];
                if (programaSession != null)
                {
                    return (ProgramaFidelidadeUsuarioViewModel)programaSession;
                }

                var rep = new ProgramaFidelidadeRepository();
                var progDb = rep.GetProgramaFidelidadeUsuario(HttpContext.Current.User.Identity.Name);

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
        }
    }
}