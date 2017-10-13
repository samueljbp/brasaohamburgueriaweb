using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BrasaoHamburgueria.Web.Extentions;
using System.Configuration;
using BrasaoHamburgueria.Web.Repository;

namespace BrasaoHamburgueria.Web.Controllers
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
	}
}