using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using BrasaoSolution.Web.SignalR;

[assembly: OwinStartupAttribute(typeof(BrasaoSolution.Web.Startup))]
namespace BrasaoSolution.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var idProvider = new CustomUserIdProvider();

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);          

            app.MapSignalR();
        }
    }
}