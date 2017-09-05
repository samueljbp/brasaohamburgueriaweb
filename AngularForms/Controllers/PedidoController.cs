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
using AngularForms.Helpers;

namespace AngularForms.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        public ActionResult PedidoRegistrado()
        {
            return View("PedidoRegistrado");
        }

        public ActionResult AcompanharPedido()
        {
            return View("AcompanharPedidos");
        }

        public ActionResult GerenciarPedidos()
        {
            return View("GerenciarPedidos");
        }

        public ActionResult ConsultarPedidos()
        {
            return View("ConsultarPedidos");
        }

        public async Task<JsonResult> GetUltimosPedidos(string loginUsuario)
        {
            var result = new { Succeeded = true, errors = new List<String>(), data = new List<PedidoViewModel>() };

            BrasaoContext contexto = new BrasaoContext();

            List<PedidoViewModel> pedidos = new List<PedidoViewModel>();

            try
            {
                pedidos = await contexto.Pedidos.Where(p => p.Usuario == loginUsuario)
                .Include(s => s.Situacao)
                .Include(s => s.Itens)
                .Include(s => s.Itens.Select(i => i.ItemCardapio))
                .Select(p => new PedidoViewModel
                {
                    FormaPagamento = p.FormaPagamento,
                    DataPedido = p.DataHora,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    DescricaoSituacao = p.Situacao.Descricao,
                    ValorTotal = p.ValorTotal,
                    Itens = p.Itens.Select(i => new ItemPedidoViewModel
                    {
                        CodItem = i.CodItemCardapio,
                        SeqItem = i.SeqItem,
                        DescricaoItem = i.ItemCardapio.Nome,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        ValorExtras = i.ValorExtras,
                        ValorTotalItem = i.ValorTotal
                    }).ToList()
                })
                .OrderBy(p => p.DataPedido)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                result = new { Succeeded = false, errors = new List<String> { ex.Message }, data = pedidos };
            }
            finally
            {
                foreach(var ped in pedidos)
                {
                    ped.DescricaoFormaPagamento = Util.GetDescricaoFormaPagamentoPedido(ped.FormaPagamento);
                }

                result = new { Succeeded = true, errors = new List<String>(), data = pedidos };
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetPedidoAberto(string loginUsuario)
        {
            BrasaoContext contexto = new BrasaoContext();

            var pedido = await contexto.Pedidos.Where(p => new List<int> { (int)SituacaoPedidoEnum.AguardandoConfirmacao, (int)SituacaoPedidoEnum.Confirmado, (int)SituacaoPedidoEnum.EmPreparacao, (int)SituacaoPedidoEnum.EmProcessoEntrega }.Contains(p.CodSituacao) && p.Usuario == loginUsuario)
                .Select(p => new PedidoViewModel
                {
                    BandeiraCartao = p.BandeiraCartao,
                    FormaPagamento = p.FormaPagamento,
                    CodPedido = p.CodPedido,
                    Situacao = p.CodSituacao,
                    TaxaEntrega = p.TaxaEntrega,
                    Troco = p.Troco,
                    TrocoPara = p.TrocoPara,
                    ValorTotal = p.ValorTotal,
                    DadosCliente = new DadosClientePedidoViewModel
                    {
                        Bairro = p.BairroEntrega,
                        Cidade = p.CidadeEntrega,
                        Complemento = p.ComplementoEntrega,
                        Estado = p.UFEntrega,
                        Logradouro = p.LogradouroEntrega,
                        Nome = p.NomeCliente,
                        Numero = p.NumeroEntrega,
                        Referencia = p.ReferenciaEntrega,
                        Telefone = p.TelefoneCliente
                    }
                })
                .FirstOrDefaultAsync();

            return new JsonNetResult { Data = pedido };
        }

        // GET: Pedido
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> FinalizaPedido(PedidoViewModel pedido)
        {
            var result = new { Succeeded = true, errors = new List<String>(), data = "" };

            var context = new BrasaoContext();

            var ped = context.Pedidos.Where(p => p.CodPedido == pedido.CodPedido).FirstOrDefault();

            if (ped != null)
            {
                try
                {
                    ped.CodSituacao = (int)SituacaoPedidoEnum.Concluido;
                    await context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    result = new { Succeeded = false, errors = new List<String> {ex.Message}, data = "" };
                }
            }

            return new JsonNetResult { Data = result };
        }

        [HttpPost]
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
                    context.SaveChangesAsync();

                    result = new { Succeeded = true, errors = new List<String>(), data = ped.CodPedido.ToString() };

                    foreach (var itemViewModel in pedidoViewModel.Itens)
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
                            context.ObservacoesItensPedidos.AddRange(itemViewModel.Obs.Where(o => o != null && o.CodObservacao > 0).Select(o => new ObservacaoItemPedido { CodPedido = item.CodPedido, SeqItem = item.SeqItem, CodObservacao = o.CodObservacao }).ToList());
                        }

                        if (itemViewModel.extras != null)
                        {
                            context.ExtrasItensPedidos.AddRange(itemViewModel.extras.Where(e => e != null).Select(o => new ExtraItemPedido { CodPedido = item.CodPedido, SeqItem = item.SeqItem, CodOpcaoExtra = o.CodOpcaoExtra, Preco = o.Preco }).ToList());
                        }
                    }
                    context.SaveChanges();

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    result = new { Succeeded = false, errors = new List<String> { ex.Message }, data = "" };
                }
            }


            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetCardapio()
        {
            bool succeeded = true;
            Object data = new List<dynamic>();
            List<string> errors = new List<string>();

            var context = new BrasaoContext();

            try
            {
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

                succeeded = true;
                data = classes;
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
                succeeded = false;
            }

            var result = new { Succeeded = succeeded, errors = errors, data = data };

            return new JsonNetResult { Data = result };
        }
    }
}