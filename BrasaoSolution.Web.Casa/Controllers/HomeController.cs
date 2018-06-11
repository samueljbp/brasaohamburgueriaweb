using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrasaoSolution.Web.Casa.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(BrasaoSolutionContext brasaoContext, IHttpContextAccessor httpContextAccessor) : base(brasaoContext, httpContextAccessor)
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
