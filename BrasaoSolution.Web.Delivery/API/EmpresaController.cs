using BrasaoSolution.Repository;
using BrasaoSolution.ViewModel;
using System.Web.Http;

namespace BrasaoHamburgueria.Web.API
{
    [RoutePrefix("api/empresa")]
    public class EmpresaController : ApiController
    {
        [Route("GetEmpresa")]
        [Authorize]
        [HttpGet]
        public EmpresaViewModel GetEmpresa(int codEmpresa)
        {
            CadastrosRepository rep = new CadastrosRepository();

            return rep.GetEmpresa(codEmpresa);
        }
    }
}
