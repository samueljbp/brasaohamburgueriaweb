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

namespace AngularForms.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        // GET: Pedido
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetCardapio()
        {
            var context = new BrasaoContext();

            var classes = context.Classes
                .Include(c => c.Itens)
                .Include(c => c.Itens.Select(i => i.Classe))
                .Include(c => c.Itens.Select(i => i.Complemento))
                .Include(c => c.Itens.Select(i => i.ObservacoesPermitidas))
                .Include(c => c.Itens.Select(i => i.ObservacoesPermitidas.Select(o => o.ObservacaoProducao)))
                .Include(c => c.Itens.Select(i => i.ExtrasPermitidos))
                .Include(c => c.Itens.Select(i => i.ExtrasPermitidos.Select(e => e.OpcaoExtra)))
                .ToList()
                .Select(c =>
                new
                {
                    CodClasse = c.CodClasse,
                    DescricaoClasse = c.DescricaoClasse,
                    Itens = c.Itens.Select(i =>
                        new
                        {
                            CodItemCardapio = i.CodItemCardapio,
                            Nome = i.Nome,
                            Preco = i.Preco,
                            ObservacoesPermitidas = (i.ObservacoesPermitidas != null ?
                                i.ObservacoesPermitidas.Select(o => new { CodObservacao = o.ObservacaoProducao.CodObservacao, DescricaoObservacao = o.ObservacaoProducao.DescricaoObservacao }) : null),
                            ExtrasPermitidos = (i.ExtrasPermitidos != null ?
                                i.ExtrasPermitidos.Select(e => new { CodOpcaoExtra = e.OpcaoExtra.CodOpcaoExtra, DescricaoOpcaoExtra = e.OpcaoExtra.DescricaoOpcaoExtra, Preco = e.OpcaoExtra.Preco }) : null),
                            Complemento = (i.Complemento != null ?
                                new
                                {
                                    DescricaoLonga = i.Complemento.DescricaoLonga,
                                    Imagem = i.Complemento.Imagem
                                } : null)
                        })
                }).ToList();

            //var aaa = classes.Select(c => 
            //    new { CodClasse = c.CodClasse, 
            //          DescricaoClasse = c.DescricaoClasse, 
            //          Itens = c.Itens.Select(i => 
            //              new { CodItem = i.CodItemCardapio, 
            //                    Nome = i.Nome,
            //                    ObservacoesPermitidas = (i.ObservacoesPermitidas != null ? 
            //                        i.ObservacoesPermitidas.Select(o => new { CodObservacao = o.ObservacaoProducao.CodObservacao, Descricao = o.ObservacaoProducao.DescricaoObservacao }) : null),
            //                    Complemento = (i.Complemento != null ? 
            //                        new { DescricaoLonga = i.Complemento.DescricaoLonga, 
            //                              Imagem = i.Complemento.Imagem } : null)
            //                  })
            //        }).ToList();

            //return Json(cardapio, "application/json", JsonRequestBehavior.AllowGet);

            return new JsonNetResult { Data = new { classes = classes } };
        }
    }
}