using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using BrasaoHamburgueriaWeb.SignalR;

[assembly: OwinStartupAttribute(typeof(BrasaoHamburgueriaWeb.Startup))]
namespace BrasaoHamburgueriaWeb
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