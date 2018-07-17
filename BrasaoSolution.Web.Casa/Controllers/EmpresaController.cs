using BrasaoSolution.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BrasaoSolution.Web.Casa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private BrasaoSolutionContext context;

        public EmpresaController(BrasaoSolutionContext brasaoContext)
        {
            this.context = brasaoContext;
        }

        [HttpGet("[action]")]
        public EmpresaViewModel GetEmpresa(int codEmpresa)
        {
            BrasaoSolutionRepository rep = new BrasaoSolutionRepository(this.context);

            return rep.GetEmpresa(codEmpresa);
        }
    }
}