using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueria.Web.Extentions;
using System.Threading.Tasks;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Repository;
using BrasaoHamburgueria.Web.Filters;

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize]
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