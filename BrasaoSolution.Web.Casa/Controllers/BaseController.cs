using BrasaoSolution.Helper;
using BrasaoSolution.Casa.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BrasaoSolution.Web.Casa.Controllers
{
    public class BaseController : Controller
    {
        protected BrasaoSolutionContext _brasaoContext;
        protected IHttpContextAccessor _contextAnteccessor;
        protected HttpContext _httpContext;

        public BaseController(BrasaoSolutionContext brasaoContext, IHttpContextAccessor httpContextAccessor)
        {
            _brasaoContext = brasaoContext;
            _contextAnteccessor = httpContextAccessor;
            _httpContext = _contextAnteccessor.HttpContext;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            EmpresaViewModel emp = new EmpresaViewModel();
            var str = _httpContext.Session.GetString("EmpresaAtual");

            if (!String.IsNullOrEmpty(str))
            {
                emp = JsonConvert.DeserializeObject<EmpresaViewModel>(str);
            }
            else
            {
                var empDb = _brasaoContext.Empresas.FirstOrDefault();
                PropertyCopy.Copy(empDb, emp);

                str = JsonConvert.SerializeObject(emp);
                _httpContext.Session.SetString("EmpresaAtual", str);
            }

            ViewBag.EmpresaAtual = emp;

            base.OnActionExecuted(context);
        }
    }
}
