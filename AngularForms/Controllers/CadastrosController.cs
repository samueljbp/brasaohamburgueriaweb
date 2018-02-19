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

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize]
    public class CadastrosController : Controller
    {
        private CadastrosRepository _rep = new CadastrosRepository();

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
                var itens = await _rep.GetItensCardapioByNome(chave);

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
                var itens = await _rep.GetItensCardapio();

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
                var parametros = await _rep.GetParametrosSistema();

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
                var observacao = await _rep.GravarParametroSistema(par, modoCadastro);
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
                var impressoras = await _rep.GetImpressorasProducao();

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