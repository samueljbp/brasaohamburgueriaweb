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
    }
}