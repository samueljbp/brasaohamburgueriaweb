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

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize]
    public class CadastrosController : Controller
    {
        private ObservacaoRepository _rep = new ObservacaoRepository();

        // GET: Cadastros
        public ActionResult ObservacaoProducao()
        {
            return View();
        }

        public async Task<JsonResult> GetObservacoes()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var obss = await _rep.GetObservacoes();

                result.data = obss;

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
        public async Task<JsonResult> ExcluiObservacao(ObservacaoProducaoViewModel obs)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiObservacao(obs);
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
        public async Task<JsonResult> GravarObservacao(ObservacaoProducaoViewModel obs, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarObservacao(obs, modoCadastro);
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
    }
}