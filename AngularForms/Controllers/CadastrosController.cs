using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrasaoHamburgueria.Web.Extentions;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;
using BrasaoHamburgueria.Web.Repository;
using BrasaoHamburgueria.Web.Filters;
using System.Collections.Specialized;
using System.Globalization;
using BrasaoHamburgueria.Web.Helpers;
using System.Security.Claims;

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize(Roles = Constantes.ROLE_ADMIN)]
    public class CadastrosController : Controller
    {
        private CadastrosRepository _rep = new CadastrosRepository();

        #region Empresas

        public ActionResult Empresa()
        {
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> GravarEmpresa(EmpresaViewModel empresa, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var emp = await _rep.GravarEmpresa(empresa, modoCadastro);
                result.Succeeded = true;
                result.data = emp;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        [HttpPost]
        public async Task<JsonResult> RemoverImagemLogoEmpresa(EmpresaViewModel empresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                _rep.RemoverLogoEmpresa(Server.MapPath("~").ToString(), empresa);

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
        public async Task<JsonResult> UploadImagemLogoEmpresa(HttpPostedFileBase file)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                NameValueCollection nvc = Request.Form;
                result.data = _rep.GravarLogoEmpresa(file, Server.MapPath("~"), Convert.ToInt32(nvc["codEmpresa"].ToString()));

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
        public async Task<JsonResult> RemoverImagemFundoPublico(EmpresaViewModel empresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                _rep.RemoverFundoPublicoEmpresa(Server.MapPath("~").ToString(), empresa);

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
        public async Task<JsonResult> UploadImagemFundoPublico(HttpPostedFileBase file)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                NameValueCollection nvc = Request.Form;
                result.data = _rep.GravarFundoPublicoEmpresa(file, Server.MapPath("~"), Convert.ToInt32(nvc["codEmpresa"].ToString()));

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
        public async Task<JsonResult> RemoverImagemFundoAutenticado(EmpresaViewModel empresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                _rep.RemoverFundoAutenticadoEmpresa(Server.MapPath("~").ToString(), empresa);

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
        public async Task<JsonResult> UploadImagemFundoAutenticado(HttpPostedFileBase file)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                NameValueCollection nvc = Request.Form;
                result.data = _rep.GravarFundoAutenticadoEmpresa(file, Server.MapPath("~"), Convert.ToInt32(nvc["codEmpresa"].ToString()));

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetEstados()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var estados = await _rep.GetEstados();

                result.data = estados;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetCidades(string siglaEstado)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var cidades = await _rep.GetCidades(siglaEstado);

                result.data = cidades;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetBairros(int? codCidade)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var bairros = await _rep.GetBairros(codCidade);

                result.data = bairros;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetEmpresas()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var identity = (ClaimsIdentity)HttpContext.User.Identity;
                IEnumerable<Claim> claims = identity.Claims;
                var emps = await _rep.GetEmpresas();

                result.data = emps;

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
        public async Task<JsonResult> ExcluiEmpresa(EmpresaViewModel empresa)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiEmpresa(empresa);
                result.Succeeded = true;
                result.data = retorno;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Forma de pagamento
        public async Task<JsonResult> GetFormasPagamento()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var formas = await _rep.GetFormasPagamento();

                result.data = formas;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        #region Bandeira de cartão
        public async Task<JsonResult> GetBandeirasCartao()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var bands = await _rep.GetBandeirasCartao();

                result.data = bands;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        #region Entregador

        public ActionResult Entregador()
        {
            return View();
        }

        public async Task<JsonResult> GetEntregadores()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var entregadores = await _rep.GetEntregadores(SessionData.CodLojaSelecionada);

                result.data = entregadores;

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
        public async Task<JsonResult> ExcluiEntregador(EntregadorViewModel entregador)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiEntregador(entregador);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarEntregador(EntregadorViewModel entregador, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var entreg = await _rep.GravarEntregador(entregador, modoCadastro);
                result.Succeeded = true;
                result.data = entreg;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Combo de cardápio

        public ActionResult ComboCardapio()
        {
            return View();
        }

        public async Task<JsonResult> GetCombos()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var combos = await _rep.GetCombos();

                result.data = combos;

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
        public async Task<JsonResult> GravarCombo(ComboViewModel combo, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var promo = await _rep.GravarCombo(combo, modoCadastro);
                result.Succeeded = true;
                result.data = promo;
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
        public async Task<JsonResult> ExcluiCombo(ComboViewModel combo)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiCombo(combo);
                result.Succeeded = true;
                result.data = retorno;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Promoções de venda

        public ActionResult PromocaoVenda()
        {
            return View();
        }
        public async Task<JsonResult> GetPromocoesVenda()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var promocoes = await _rep.GetPromocoesVenda(SessionData.CodLojaSelecionada);

                result.data = promocoes;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetTiposAplicacaoDesconto()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var tiposDesconto = await _rep.GetTiposAplicacaoDesconto();

                result.data = tiposDesconto;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetDiasSemana()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                result.data = await ParametroRepository.GetDiasSemana();

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
        public async Task<JsonResult> ExcluiPromocaoVenda(PromocaoVendaViewModel promo)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiPromocaoVenda(promo);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarPromocaoVenda(PromocaoVendaViewModel promocao, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var promo = await _rep.GravarPromocaoVenda(promocao, modoCadastro);
                result.Succeeded = true;
                result.data = promo;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Associação de observações a itens de cardápio

        public ActionResult ItemCardapioObservacaoProducao()
        {
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> GravarObservacoesItens(List<ObservacaoProducaoViewModel> obs, int codItemCardapio, int codClasse)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.GravarObservacoesItens(obs, codItemCardapio, codClasse);
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Associação de opções extra a itens de cardápio

        public ActionResult ItemCardapioOpcaoExtra()
        {
            return View();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> GravarOpcoesItens(List<OpcaoExtraViewModel> opcoes, int codItemCardapio, int codClasse)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                await _rep.GravarOpcoesExtraItens(opcoes, codItemCardapio, codClasse);
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Item cardápio
        public ActionResult ItemCardapio()
        {
            return View();
        }

        public async Task<JsonResult> GetItensCardapioByNome(string chave)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var itens = await _rep.GetItensCardapioByNome(chave, SessionData.CodLojaSelecionada);

                result.data = itens;

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        public async Task<JsonResult> GetItensCardapio()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var itens = await _rep.GetItensCardapio(SessionData.CodLojaSelecionada);

                result.data = itens;

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
        public async Task<JsonResult> ExcluiItemCardapio(ItemCardapioViewModel item)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiItemCardapio(item);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarItemCardapio(ItemCardapioViewModel item, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarItemCardapio(item, modoCadastro);
                result.Succeeded = true;
                result.data = item;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        [HttpPost]
        public async Task<JsonResult> UploadImagemItemCardapio(HttpPostedFileBase file)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                NameValueCollection nvc = Request.Form;
                result.data = _rep.GravarImagemItemCardapio(file, Convert.ToInt32(nvc["codItemCardapio"].ToString()), Server.MapPath("~"));

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
        public async Task<JsonResult> RemoverImagemItemCardapio(ItemCardapioViewModel item)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                _rep.RemoverImagemItemCardapio(item, Server.MapPath("~").ToString());

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Parametros de sistema

        public ActionResult ParametroSistema()
        {
            return View();
        }

        public async Task<JsonResult> GetParametrosSistema()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var parametros = await _rep.GetParametrosSistema(SessionData.CodLojaSelecionada);

                result.data = parametros;

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
        public async Task<JsonResult> GravarParametroSistema(ParametroSistemaViewModel par, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarParametroSistema(par, modoCadastro, SessionData.CodLojaSelecionada);
                result.Succeeded = true;
                result.data = observacao;
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
        public async Task<JsonResult> ExcluiParametro(ParametroSistemaViewModel par)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiParametro(par);
                result.Succeeded = true;
                result.data = retorno;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Impressoras de produção

        public ActionResult ImpressoraProducao()
        {
            return View();
        }

        public async Task<JsonResult> GetImpressorasProducao()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var impressoras = await _rep.GetImpressorasProducao(SessionData.CodLojaSelecionada);

                result.data = impressoras;

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
        public async Task<JsonResult> ExcluiImpressoraProducao(ImpressoraProducaoViewModel imp)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiImpressoraProducao(imp);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarImpressoraProducao(ImpressoraProducaoViewModel imp, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarImpressoraProducao(imp, modoCadastro);
                result.Succeeded = true;
                result.data = observacao;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }

        #endregion

        #region Classe item cardápio
        public ActionResult ClasseItemCardapio()
        {
            return View();
        }

        public async Task<JsonResult> GetClassesItemCardapio()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var classes = await _rep.GetClassesItemCardapio();

                result.data = classes;

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
        public async Task<JsonResult> UploadFile(HttpPostedFileBase file)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                NameValueCollection nvc = Request.Form;
                result.data = _rep.GravarImagemClasse(file, Convert.ToInt32(nvc["codClasse"].ToString()), Server.MapPath("~"));

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
        public async Task<JsonResult> RemoverImagem(ClasseItemCardapioViewModel classe)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                _rep.RemoverImagemClasse(classe, Server.MapPath("~").ToString());

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
        public async Task<JsonResult> ExcluiClasseItemCardapio(ClasseItemCardapioViewModel classe)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiClasseItemCardapio(classe);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarClasseItemCardapio(ClasseItemCardapioViewModel classe, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarClasseItemCardapio(classe, modoCadastro);
                result.Succeeded = true;
                result.data = classe;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        #region Opção extra
        public ActionResult OpcaoExtra()
        {
            return View();
        }

        public async Task<JsonResult> GetOpcoesExtra()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var opcoes = await _rep.GetOpcoesExtra();

                result.data = opcoes;

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
        public async Task<JsonResult> ExcluiOpcaoExtra(OpcaoExtraViewModel opcao)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiOpcaoExtra(opcao);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarOpcaoExtra(OpcaoExtraViewModel opcao, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarOpcaoExtra(opcao, modoCadastro);
                result.Succeeded = true;
                result.data = observacao;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }
        #endregion

        #region Observação produção
        public ActionResult ObservacaoProducao()
        {
            return View();
        }

        public async Task<JsonResult> GetObservacoes()
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var obss = await _rep.GetObservacoes();

                result.data = obss;

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
        public async Task<JsonResult> ExcluiObservacao(ObservacaoProducaoViewModel obs)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var retorno = await _rep.ExcluiObservacao(obs);
                result.Succeeded = true;
                result.data = retorno;
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
        public async Task<JsonResult> GravarObservacao(ObservacaoProducaoViewModel obs, String modoCadastro)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            try
            {
                var observacao = await _rep.GravarObservacao(obs, modoCadastro);
                result.Succeeded = true;
                result.data = observacao;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return new JsonNetResult { Data = result };
        }


        #endregion
    }
}