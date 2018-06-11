﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using BrasaoSolution.Helper.Extentions;
using BrasaoSolution.Model;
using BrasaoSolution.Repository.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using BrasaoSolution.Web.Filters;
using BrasaoSolution.Web.Helpers;
using BrasaoSolution.Repository;
using BrasaoSolution.Helper;

namespace BrasaoSolution.Web.Controllers
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

        [Authorize(Roles = Constantes.ROLE_ADMIN + "," + Constantes.ROLE_COZINHA + ", " + Constantes.ROLE_MASTER)]
        public ActionResult ProducaoCozinha()
        {
            return View("ProducaoCozinha");
        }

        public ActionResult PedidoRegistrado()
        {
            return View("PedidoRegistrado");
        }

        public ActionResult AcompanharPedido()
        {
            return View("AcompanharPedidos");
        }

        [Authorize(Roles = Constantes.ROLE_ADMIN + ", " + Constantes.ROLE_MASTER)]
        public ActionResult GerenciarPedidos()
        {
            return View("GerenciarPedidos");
        }

        public ActionResult ConsultarPedidos()
        {
            return View("ConsultarPedidos");
        }

        public async Task<JsonResult> GetPedido(int codPedido, bool paraConsulta, int? codEmpresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var peds = await _rep.GetPedidosAbertos(codPedido, paraConsulta, false, codEmpresa);
                var ped = peds.FirstOrDefault();

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

        public async Task<JsonResult> GetPedidosAbertos(bool somenteProducao, int? codEmpresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetPedidosAbertos(null, false, somenteProducao, codEmpresa);

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
                result.data = await _rep.GetUltimosPedidos(loginUsuario, SessionData.CodLojaSelecionada);

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
                result.data = await _rep.GetPedidoAberto(loginUsuario, "", SessionData.CodLojaSelecionada);
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
        [Authorize(Roles = Constantes.ROLE_ADMIN + ", " + Constantes.ROLE_MASTER)]
        public async Task<JsonResult> AplicaDesconto(PedidoViewModel pedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.AplicaDescontoPedido(pedido, User.Identity.GetUserName());
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        [Authorize(Roles = Constantes.ROLE_ADMIN + ", " + Constantes.ROLE_MASTER)]
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

        [Authorize]
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> AvancarPedido(PedidoViewModel pedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.AlteraSituacaoPedido(pedido, User.Identity.GetUserName());
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
                if (pedidoViewModel.CodEmpresa <= 0)
                {
                    pedidoViewModel.CodEmpresa = SessionData.CodLojaSelecionada;
                }

                result.data = await _rep.GravaPedido(pedidoViewModel, User.Identity.GetUserName());
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        [Authorize(Roles = Constantes.ROLE_ADMIN + "," + Constantes.ROLE_COZINHA + ", " + Constantes.ROLE_MASTER)]
        public async Task<JsonResult> GetHistoricoPedido(int codPedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetHistoricoPedido(codPedido);

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