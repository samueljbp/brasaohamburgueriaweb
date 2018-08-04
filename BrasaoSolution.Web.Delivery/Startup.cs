using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using BrasaoSolution.Web.SignalR;
using System.Web.Http;
using Microsoft.Owin.Cors;

[assembly: OwinStartupAttribute(typeof(BrasaoSolution.Web.Startup))]
namespace BrasaoSolution.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var config = new HttpConfiguration();
            var aaa = new CorsOptions();
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);


            var idProvider = new CustomUserIdProvider();

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);          

            app.MapSignalR();
        }
    }
}