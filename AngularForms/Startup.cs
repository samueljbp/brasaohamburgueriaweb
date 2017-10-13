using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using BrasaoHamburgueria.Web.SignalR;

[assembly: OwinStartupAttribute(typeof(BrasaoHamburgueria.Web.Startup))]
namespace BrasaoHamburgueria.Web
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