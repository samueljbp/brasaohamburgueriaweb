﻿using System;
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
    [AllowCrossSiteJsonAttribute]
    [Authorize]
    public class PedidoController : Controller
    {
        private ApplicationUserManager _userManager;
        private PedidoRepository _rep = new PedidoRepository();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult PedidoRegistrado()
        {
            return View("PedidoRegistrado");
        }

        public ActionResult AcompanharPedido()
        {
            return View("AcompanharPedidos");
        }

        [Authorize(Roles = Constantes.ROLE_ADMIN)]
        public ActionResult GerenciarPedidos()
        {
            return View("GerenciarPedidos");
        }

        public ActionResult ConsultarPedidos()
        {
            return View("ConsultarPedidos");
        }

        public async Task<JsonResult> GetPedido(int codPedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var peds = await _rep.GetPedidosAbertos(codPedido);
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

        public async Task<JsonResult> GetPedidosAbertos()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetPedidosAbertos(null);

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

        public async Task<JsonResult> GetUltimosPedidos(string loginUsuario)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

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
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetPedidoAberto(loginUsuario, "");
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
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> AplicaDesconto(PedidoViewModel pedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.AplicaDescontoPedido(pedido);
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
        public async Task<JsonResult> AlteraTempoMedioEspera(int tempo)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await ParametroRepository.AlteraTempoMedioEspera(tempo);
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
        public async Task<JsonResult> AvancarPedido(PedidoViewModel pedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.AlteraSituacaoPedido(pedido.CodPedido, pedido.Situacao, pedido.MotivoCancelamento, pedido.FeedbackCliente, User.Identity.GetUserName());
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
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GravaPedido(pedidoViewModel, User.Identity.GetUserName());
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