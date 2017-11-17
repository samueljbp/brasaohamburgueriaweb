using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using BrasaoHamburgueria.Web.Extentions;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using BrasaoHamburgueria.Web.Filters;
using BrasaoHamburgueria.Web.Helpers;
using BrasaoHamburgueria.Web.Repository;
using BrasaoHamburgueria.Helper;

namespace BrasaoHamburgueria.Web.Controllers
{
    public class ProgramaFidelidadeController : Controller
    {
        private ProgramaFidelidadeRepository _rep = new ProgramaFidelidadeRepository();

        // GET: ProgramaFidelidade
        public ActionResult Extrato()
        {
            return View();
        }

        public async Task<JsonResult> GetExtratoProgramaFidelidade(string loginUsuario)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var extrato = await _rep.GetExtratoUsuarioProgramaRecompensa(loginUsuario);

                result.data = extrato;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> RegistraAceiteProgramaFidelidade(ProgramaFidelidadeUsuarioViewModel prog)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            prog.LoginUsuario = User.Identity.GetUserName();

            ProgramaFidelidadeRepository _progRep = new ProgramaFidelidadeRepository();

            try
            {
                SessionData.ProgramaFidelidadeUsuario.TermosAceitos = prog.TermosAceitos;
                SessionData.ProgramaFidelidadeUsuario.LoginUsuario = prog.LoginUsuario;
                SessionData.ProgramaFidelidadeUsuario.DataHoraAceite = DateTime.Now;
                SessionData.ProgramaFidelidadeUsuario.Saldo = 0;
                await _progRep.RegistraAceite(prog);
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
    }
}