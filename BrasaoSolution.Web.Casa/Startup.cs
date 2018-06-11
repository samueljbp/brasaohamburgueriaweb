using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrasaoSolution.Web.Casa
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BrasaoSolutionContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddNodeServices(options => {
                options.LaunchWithDebugging = true;
                options.DebuggingPort = 9229;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            var ptBrCulture = new CultureInfo("pt-BR");
            var localizationOptions = new RequestLocalizationOptions()
            {
                SupportedCultures = new List<CultureInfo>()
                {
                    ptBrCulture
                },
                SupportedUICultures = new List<CultureInfo>()
                {
                    ptBrCulture
                },
                DefaultRequestCulture = new RequestCulture(ptBrCulture),
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false,
                RequestCultureProviders = null
            };

            app.UseRequestLocalization(localizationOptions);

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

        }
    }
}
