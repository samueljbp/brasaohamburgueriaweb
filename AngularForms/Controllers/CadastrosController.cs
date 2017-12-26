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

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize]
    public class CadastrosController : Controller
    {
        private CadastrosRepository _rep = new CadastrosRepository();

        #region Item cardápio
        public ActionResult ItemCardapio()
        {
            return View();
        }
        #endregion

        #region Impressoras de produção
        
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