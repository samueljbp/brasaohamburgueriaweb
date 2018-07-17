using System.Collections.Generic;
using BrasaoSolution.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrasaoSolution.Web.Casa.Controllers
{
    [Route("api/[controller]")]
    public class CardapioController : BaseController
    {
        public CardapioController(BrasaoSolutionContext brasaoContext, IHttpContextAccessor httpContextAccessor) : base(brasaoContext, httpContextAccessor)
        {
        }

        [HttpGet("[action]")]
        public List<ClasseItemCardapioViewModel> GetCardapio(int codEmpresa)
        {
            BrasaoSolutionRepository rep = new BrasaoSolutionRepository(this._brasaoContext);

            return rep.GetCardapio(codEmpresa);
        }

        [HttpGet("[action]")]
        public DadosItemCardapioViewModel GetDadosItemCardapio(int codItemCardapio)
        {
            BrasaoSolutionRepository rep = new BrasaoSolutionRepository(this._brasaoContext);

            return rep.GetDadosItemCardapio(codItemCardapio);
        }
    }
}