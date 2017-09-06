using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using AngularForms.Extentions;
using AngularForms.Model;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using AngularForms.Filters;
using AngularForms.Helpers;
using AngularForms.Repository;

namespace AngularForms.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private PedidoRepository _rep = new PedidoRepository();

        public ActionResult PedidoRegistrado()
        {
            return View("PedidoRegistrado");
        }

        public ActionResult AcompanharPedido()
        {
            return View("AcompanharPedidos");
        }

        public ActionResult GerenciarPedidos()
        {
            return View("GerenciarPedidos");
        }

        public ActionResult ConsultarPedidos()
        {
            return View("ConsultarPedidos");
        }

        public async Task<JsonResult> GetUltimosPedidos(string loginUsuario)
        {
            var result = new ServiceResult(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetUltimosPedidos(loginUsuario);

                foreach (var ped in result.data)
                {
                    ped.DescricaoFormaPagamento = Util.GetDescricaoFormaPagamentoPedido(ped.FormaPagamento);
                }

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetPedidoAberto(string loginUsuario)
        {
            var result = new ServiceResult(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetPedidoAberto(loginUsuario);
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        // GET: Pedido
        public ActionResult Index()
        {
            ViewBag.TaxaEntrega = ParametroRepository.GetTaxaEntrega();
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> FinalizaPedido(PedidoViewModel pedido)
        {
            var result = new ServiceResult(true, new List<string>(), null);

            try
            {
                await _rep.FinalizaPedido(pedido.CodPedido);
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
        public async Task<JsonResult> GravarPedido(PedidoViewModel pedidoViewModel)
        {
            var result = new ServiceResult(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GravaPedido(pedidoViewModel, User.Identity.GetUserName());
                result.Succeeded = true;
            }
            catch(Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
    }
}