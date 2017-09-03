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
using AngularForms.Filters;

namespace AngularForms.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        public ActionResult PedidoRegistrado()
        {
            return View("PedidoRegistrado");
        }

        // GET: Pedido
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> GravarPedido(PedidoViewModel pedidoViewModel)
        {
            var result = new { Succeeded = true, errors = new List<String>(), data = "" };

            var context = new BrasaoContext();

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    Pedido ped = new Pedido();
                    ped.BairroEntrega = pedidoViewModel.DadosCliente.Bairro;
                    ped.BandeiraCartao = pedidoViewModel.BandeiraCartao;
                    ped.CidadeEntrega = pedidoViewModel.DadosCliente.Cidade;
                    ped.CodSituacao = pedidoViewModel.Situacao;
                    ped.ComplementoEntrega = pedidoViewModel.DadosCliente.Complemento;
                    ped.DataHora = DateTime.Now;
                    ped.FormaPagamento = pedidoViewModel.FormaPagamento;
                    ped.LogradouroEntrega = pedidoViewModel.DadosCliente.Logradouro;
                    ped.NomeCliente = pedidoViewModel.DadosCliente.Nome;
                    ped.NumeroEntrega = pedidoViewModel.DadosCliente.Numero;
                    ped.ReferenciaEntrega = pedidoViewModel.DadosCliente.Referencia;
                    ped.TaxaEntrega = pedidoViewModel.TaxaEntrega;
                    ped.TelefoneCliente = pedidoViewModel.DadosCliente.Telefone;
                    ped.Troco = pedidoViewModel.Troco;
                    ped.TrocoPara = pedidoViewModel.TrocoPara;
                    ped.UFEntrega = pedidoViewModel.DadosCliente.Estado;
                    ped.Usuario = User.Identity.GetUserName();
                    ped.ValorTotal = pedidoViewModel.ValorTotal;
                    context.Pedidos.Add(ped);
                    context.SaveChanges();

                    result = new { Succeeded = true, errors = new List<String>(), data = ped.CodPedido.ToString() };

                    foreach(var itemViewModel in pedidoViewModel.Itens)
                    {
                        var item = new ItemPedido();
                        item.CodItemCardapio = itemViewModel.CodItem;
                        item.CodPedido = ped.CodPedido;
                        item.ObservacaoLivre = itemViewModel.ObservacaoLivre;
                        item.PrecoUnitario = itemViewModel.PrecoUnitario;
                        item.Quantidade = itemViewModel.Quantidade;
                        item.SeqItem = itemViewModel.SeqItem;
                        item.ValorExtras = itemViewModel.ValorExtras;
                        item.ValorTotal = itemViewModel.ValorTotalItem;
                        context.ItensPedidos.Add(item);

                        if (itemViewModel.Obs != null)
                        {
                            context.ObservacoesItensPedidos.AddRange(itemViewModel.Obs.Where(o => o != null && o.CodObservacao != null).Select(o => new ObservacaoItemPedido { CodPedido = item.CodPedido, SeqItem = item.SeqItem, CodObservacao = o.CodObservacao }).ToList());
                        }

                        if (itemViewModel.extras != null)
                        {
                            context.ExtrasItensPedidos.AddRange(itemViewModel.extras.Where(e => e != null && e.CodOpcaoExtra != null).Select(o => new ExtraItemPedido { CodPedido = item.CodPedido, SeqItem = item.SeqItem, CodOpcaoExtra = o.CodOpcaoExtra, Preco = o.Preco }).ToList());
                        }
                    }
                    context.SaveChanges();

                    dbContextTransaction.Commit(); 
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    result = new { Succeeded = false, errors = new List<String>{ex.Message}, data = "" };
                }
            }


            return new JsonNetResult { Data = result };
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