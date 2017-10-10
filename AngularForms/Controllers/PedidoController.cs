using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using BrasaoHamburgueriaWeb.Extentions;
using BrasaoHamburgueria.Model;
using BrasaoHamburgueriaWeb.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using BrasaoHamburgueriaWeb.Filters;
using BrasaoHamburgueriaWeb.Helpers;
using BrasaoHamburgueriaWeb.Repository;
using BrasaoHamburgueria.Helpers;

namespace BrasaoHamburgueriaWeb.Controllers
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

        public ActionResult PedidoRegistrado()
        {
            return View("PedidoRegistrado");
        }

        public ActionResult AcompanharPedido()
        {
            return View("AcompanharPedidos");
        }

        [Authorize(Roles = Constantes.ROLE_ADMIN)]
        public ActionResult GerenciarPedidos()
        {
            return View("GerenciarPedidos");
        }

        public ActionResult ConsultarPedidos()
        {
            return View("ConsultarPedidos");
        }

        public async Task<JsonResult> GetPedido(int codPedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var peds = await _rep.GetPedidosAbertos(codPedido);
                var ped = peds.FirstOrDefault();
                ped.DescricaoFormaPagamento = Util.GetDescricaoFormaPagamentoPedido(ped.FormaPagamento);

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

        public async Task<JsonResult> GetPedidosAbertos()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await _rep.GetPedidosAbertos(null);

                foreach (var ped in result.data)
                {
                    ped.DescricaoFormaPagamento = Util.GetDescricaoFormaPagamentoPedido(ped.FormaPagamento);
                }

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
                result.data = await _rep.GetUltimosPedidos(loginUsuario);

                foreach (var ped in result.data)
                {
                    ped.DescricaoFormaPagamento = Util.GetDescricaoFormaPagamentoPedido(ped.FormaPagamento);
                }

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
                result.data = await _rep.GetPedidoAberto(loginUsuario, "");
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
        public async Task<JsonResult> AplicaDesconto(PedidoViewModel pedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.AplicaDescontoPedido(pedido);
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
        public async Task<JsonResult> AvancarPedido(PedidoViewModel pedido)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.AlteraSituacaoPedido(pedido.CodPedido, pedido.Situacao, pedido.MotivoCancelamento, pedido.FeedbackCliente);
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

            if (pedidoViewModel.DadosCliente.Telefone.Length < 14)
            {
                result.Succeeded = false;
                result.Errors.Add("O telefone não está preenchido corretamente");
                return new JsonNetResult { Data = result };
            }

            if (pedidoViewModel.PedidoExterno && pedidoViewModel.CodPedido <= 0)
            {
                try
                {
                    //var ped = _rep.GetPedidoAberto("", pedidoViewModel.DadosCliente.Telefone).Result;
                    var ped = BrasaoHamburgueria.Helper.AsyncHelpers.RunSync<PedidoViewModel>(() => _rep.GetPedidoAberto("", pedidoViewModel.DadosCliente.Telefone));

                    if (ped != null)
                    {
                        result.Succeeded = false;
                        result.Errors.Add("O cliente " + pedidoViewModel.DadosCliente.Telefone + " possui o pedido " + ped.CodPedido + " em aberto. Finalize-o antes de fazer outro pedido para este cliente.");
                        return new JsonNetResult { Data = result };
                    }
                }
                catch(Exception ex)
                {
                    result.Succeeded = false;
                result.Errors.Add(ex.Message);
                }
            }

            try
            {
                //primeiro verifica se a casa está aberta para delivery
                if (!pedidoViewModel.PedidoExterno && !ParametroRepository.CasaAberta() && pedidoViewModel.CodPedido <= 0)
                {
                    var horarioFuncionamento = ParametroRepository.GetHorarioAbertura();
                    result.Succeeded = false;
                    result.Errors.Add("No momento estamos fechados. Abriremos " + horarioFuncionamento.DiaSemana + " das " + horarioFuncionamento.Abertura.ToString("HH:mm") + " às " + horarioFuncionamento.Fechamento.ToString("HH:mm") + ".");
                }
                else
                {
                    result.data = await _rep.GravaPedido(pedidoViewModel, User.Identity.GetUserName());
                    if (pedidoViewModel.DadosCliente.ClienteNovo)
                    {
                        try
                        {
                            ApplicationDbContext contexto = new ApplicationDbContext();
                            Usuario usu = new Usuario();
                            UsuarioViewModel usuVm = new UsuarioViewModel();
                            PropertyCopy.Copy(pedidoViewModel.DadosCliente, usuVm);
                            UsuarioCopy.ViewModelToDB(usuVm, usu);
                            usu.UsuarioExterno = true;
                            contexto.DadosUsuarios.Add(usu);
                            contexto.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            //nao faz nada porque o pedido foi gravado e sao transacoes diferentes
                        }
                        
                        result.Succeeded = true;
                    }
                    else if (pedidoViewModel.DadosCliente.Salvar)
                    {
                        try
                        {
                            ApplicationDbContext contexto = new ApplicationDbContext();
                            string userName = User.Identity.GetUserName();
                            var usu = contexto.DadosUsuarios.Where(d => d.Email == userName).FirstOrDefault();
                            if (usu != null)
                            {
                                UsuarioViewModel usuVm = new UsuarioViewModel();
                                PropertyCopy.Copy(pedidoViewModel.DadosCliente, usuVm);
                                UsuarioCopy.ViewModelToDB(usuVm, usu);
                                contexto.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            //nao faz nada porque o pedido foi gravado e sao transacoes diferentes
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
    }
}