using BrasaoSolution.Repository;
using BrasaoSolution.ViewModel;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BrasaoHamburgueria.Web.API
{
    [RoutePrefix("api/cardapio")]
    public class TesteController : ApiController
    {
        [Route("GetDadosItemCardapio")]
        [Authorize]
        [HttpGet]
        public DadosItemCardapioViewModel GetDadosItemCardapio(int codItemCardapio)
        {
            CardapioRepository rep = new CardapioRepository();

            return rep.GetDadosItemCardapio(codItemCardapio);
        }

        [Route("GetCardapio")]
        [Authorize]
        [HttpGet]
        public List<ClasseItemCardapioViewModel> GetCardapio(int codEmpresa)
        {
            CardapioRepository rep = new CardapioRepository();

            return rep.GetCardapio(codEmpresa);
        }
    }
}
