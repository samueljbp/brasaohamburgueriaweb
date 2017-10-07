using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueriaWeb.Repository;
using System.Threading.Tasks;
using BrasaoHamburgueria.Helpers;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueriaWeb.Helpers;
using BrasaoHamburgueriaWeb.Extentions;

namespace BrasaoHamburgueriaWeb.Controllers
{
    public class ConsultasController : Controller
    {
        private PedidoRepository _rep = new PedidoRepository();

        // GET: Consultas
        public ActionResult PedidosRealizados()
        {
            return View();
        }

        public async Task<JsonResult> GetPedido(int codPedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var peds = await _rep.GetPedidosConsulta(codPedido);
                var ped = peds.FirstOrDefault();
                ped.DescricaoFormaPagamento = Util.GetDescricaoFormaPagamentoPedido(ped.FormaPagamento);

                result.data = ped;

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