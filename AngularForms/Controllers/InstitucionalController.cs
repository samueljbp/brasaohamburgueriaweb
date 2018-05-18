using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueria.Web.Extentions;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;
using BrasaoHamburgueria.Web.Repository;
using BrasaoHamburgueria.Web.Filters;
using BrasaoHamburgueria.Web.Helpers;

namespace BrasaoHamburgueria.Web.Controllers
{
    public class InstitucionalController : Controller
    {
        private InstitucionalRepository _rep = new InstitucionalRepository();

        #region Horário de funcionamento
        public ActionResult HorarioFuncionamento()
        {
            return View();
        }

        public async Task<JsonResult> GetHorariosFuncionamento()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var ehAdmin = HttpContext.User.IsInRole(Constantes.ROLE_ADMIN);
                var opcoes = await _rep.GetHorariosFuncionamento(SessionData.CodLojaSelecionada, ehAdmin);

                result.data = opcoes;

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
        public async Task<JsonResult> ExcluiFuncionamentoEstabelecimento(FuncionamentoEstabelecimentoViewModel funcionamento)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiFuncionamentoEstabelecimento(funcionamento);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarFuncionamentoEstabelecimento(FuncionamentoEstabelecimentoViewModel funcionamento, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarFuncionamentoEstabelecimento(funcionamento, modoCadastro, SessionData.CodLojaSelecionada);
                result.Succeeded = true;
                result.data = observacao;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        public ActionResult Sobre()
        {
            return View();
        }

        
    }
}