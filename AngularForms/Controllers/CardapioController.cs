using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Web.Extentions;
using System.Data.Entity;
using BrasaoHamburgueria.Web.Repository;
using System.Threading.Tasks;
using BrasaoHamburgueria.Web.Helpers;

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    public class CardapioController : Controller
    {
        private CardapioRepository _rep = new CardapioRepository();

        public ActionResult Listar()
        {
            var context = new BrasaoContext();

            //var classes = context.Classes.Include(c => c.Itens)
            //    .Include(c => c.Itens.Select(i => i.Classe))
            //    .Include(c => c.Itens.Select(i => i.Complemento))
            //    .Where(c => c.Itens.Where(a => a.Ativo).Count() > 0)
            //    .ToList();

            var classes = _rep.GetCardapio(SessionData.CodLojaSelecionada);

            ViewBag.Classes = classes;

            return View();
        }

        public JsonResult GetDadosItemCardapio(int codItemCardapio)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = _rep.GetDadosItemCardapio(codItemCardapio);

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public JsonResult GetCardapio()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = _rep.GetCardapio(SessionData.CodLojaSelecionada);

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