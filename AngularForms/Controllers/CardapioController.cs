using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AngularForms.Model;
using AngularForms.Extentions;
using System.Data.Entity;

namespace AngularForms.Controllers
{
    public class CardapioController : Controller
    {
        public ActionResult Listar()
        {
            var context = new BrasaoContext();

            var classes = context.Classes.Include(c => c.Itens)
                .Include(c => c.Itens.Select(i => i.Classe))
                .Include(c => c.Itens.Select(i => i.Complemento))
                .ToList();

            ViewBag.Classes = classes;

            return View();
        }
    }
}