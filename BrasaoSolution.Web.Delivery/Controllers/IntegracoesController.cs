using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BrasaoSolution.Helper.Extentions;
using System.Threading.Tasks;
using BrasaoSolution.Model;
using BrasaoSolution.Repository;
using BrasaoSolution.Web.Filters;
using BrasaoSolution.ViewModel;

namespace BrasaoSolution.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize(Roles = Constantes.ROLE_ADMIN + ", " + Constantes.ROLE_MASTER)]
    public class IntegracoesController : Controller
    {
        IntegracoesRepository _rep = new IntegracoesRepository();

        // GET: Integracoes
        public ActionResult TronSolution()
        {
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> SolicitarSincronismoTronSolution(List<ItemCardapioViewModel> itensTron, List<ClasseItemCardapioViewModel> classesTron)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                //result.data = await _rep.GetPedidosAbertos(null);
                result.data = await _rep.ExecutaIntegracaoTronSolution(itensTron, classesTron);

                result.Succeeded = true;
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