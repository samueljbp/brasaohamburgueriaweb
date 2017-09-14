using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AngularForms.Extentions;

namespace AngularForms.Controllers
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