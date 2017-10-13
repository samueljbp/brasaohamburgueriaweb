using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueria.Web.Extentions;
using System.Threading.Tasks;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Repository;

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

        public async Task<JsonResult> SolicitarSincronismoTronSolution()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                //result.data = await _rep.GetPedidosAbertos(null);
                result.data = await _rep.ExecutaIntegracaoTronSolution();

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