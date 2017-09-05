using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using AngularForms.SignalR;

[assembly: OwinStartupAttribute(typeof(AngularForms.Startup))]
namespace AngularForms
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