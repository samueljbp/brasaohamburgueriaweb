using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoSolution.Repository;
using System.Threading.Tasks;
using BrasaoSolution.Helper;
using BrasaoSolution.Model;
using BrasaoSolution.Web.Helpers;
using BrasaoSolution.Helper.Extentions;

namespace BrasaoSolution.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize(Roles = Constantes.ROLE_ADMIN + ", " + Constantes.ROLE_MASTER)]
    public class ConsultasController : Controller
    {
        private ConsultasRepository _rep = new ConsultasRepository();

        // GET: Consultas

        #region Taxa Entrega
        public ActionResult TaxasEntrega()
        {
            return View();
        }

        public async Task<JsonResult> GetTaxasEntrega(DateTime? dataInicio, DateTime? dataFim, int? codEntregador, int? codEmpresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var taxas = await _rep.GetTaxasEntrega(dataInicio, dataFim, codEntregador, codEmpresa);

                result.data = taxas;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        #region Produtos vendidos
        public ActionResult ProdutosVendidos()
        {
            return View();
        }

        public async Task<JsonResult> GetProdutosVendidos(DateTime? dataInicio, DateTime? dataFim, int? codClasse, int? codEmpresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var prods = await _rep.GetProdutosVendidos(dataInicio, dataFim, codClasse, codEmpresa);

                result.data = prods;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        #region Pedidos realizados
        [Authorize(Roles = Constantes.ROLE_ADMIN)]
        public ActionResult PedidosRealizados()
        {
            return View();
        }

        public async Task<JsonResult> GetPedido(DateTime? dataInicio, DateTime? dataFim, int? codPedido, int? codEmpresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var peds = await _rep.GetPedidosConsulta(dataInicio, dataFim, codPedido, codEmpresa);

                result.data = peds;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion
    }
}