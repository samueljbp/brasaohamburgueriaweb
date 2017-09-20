using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueriaWeb.Context;
using BrasaoHamburgueriaWeb.Extentions;
using System.Data.Entity;
using BrasaoHamburgueriaWeb.Repository;
using System.Threading.Tasks;
using BrasaoHamburgueriaWeb.Helpers;

namespace BrasaoHamburgueriaWeb.Controllers
{
    [AllowCrossSiteJsonAttribute]
    public class CardapioController : Controller
    {
        private CardapioRepository _rep = new CardapioRepository();

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

        public JsonResult GetCardapio()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = _rep.GetCardapio();

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