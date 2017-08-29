using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AngularForms.Startup))]
namespace AngularForms
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}