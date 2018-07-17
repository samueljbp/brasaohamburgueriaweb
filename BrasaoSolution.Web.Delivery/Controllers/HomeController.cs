﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BrasaoSolution.Helper.Extentions;
using BrasaoSolution.Repository;
using System.Threading.Tasks;
using BrasaoSolution.Model;
using BrasaoSolution.ViewModel;

namespace BrasaoSolution.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (ParametroRepository.GetEmManutencao() == "S")
            {
                return View("EmManutencao");
            }

            if (User.Identity.IsAuthenticated)
            {
                return View("MenuPrincipal");
            }

            ViewBag.ReturnUrl = "";
            return View();
        }

        public ActionResult MenuPrincipal()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constantes.ROLE_ADMIN + ", " + Constantes.ROLE_MASTER)]
        public async Task<JsonResult> AbreFechaLoja(bool aberta)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await ParametroRepository.AbreFechaCasa(aberta);
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
        public async Task<JsonResult> TrocaLoja(int codLoja)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                SessionData.CodLojaSelecionada = codLoja;
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