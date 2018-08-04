using BrasaoSolution.Helper;
using BrasaoSolution.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BrasaoSolution.Web.Casa.Controllers
{
    public class BaseController : Controller
    {
        protected IHttpContextAccessor _contextAnteccessor;
        protected HttpContext _httpContext;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
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
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:57919/api/empresa/GetEmpresa");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.GetToken("samuel_jbp@yahoo.com.br", "r852456t"));
                HttpResponseMessage response = client.GetAsync("?codEmpresa=1").Result;
                if (response.IsSuccessStatusCode)
                {
                    emp = response.Content.ReadAsAsync<EmpresaViewModel>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                }

                str = JsonConvert.SerializeObject(emp);
                _httpContext.Session.SetString("EmpresaAtual", str);
            }

            ViewBag.EmpresaAtual = emp;

            base.OnActionExecuted(context);
        }

        private string GetToken(string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>( "grant_type", "password" ),
                            new KeyValuePair<string, string>( "userName", userName ),
                            new KeyValuePair<string, string> ( "password", password )
                        };
            var content = new FormUrlEncodedContent(pairs);
            using (var client = new HttpClient())
            {
                var response = client.PostAsync("http://localhost:57919/Token", content).Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                return JsonConvert.DeserializeObject<TokenData>(json.Result).Access_token;
            }
        }
    }

    public class TokenData
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string UserName { get; set; }
    }
}
