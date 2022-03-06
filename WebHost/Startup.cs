using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Somewhere2.ApplicationState;

namespace Somewhere2.WebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        #region Runtime Components
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<RuntimeData>(RuntimeData.Singleton);
            
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/WebHost/Pages"); 
        }

        public void Configure(IApplicationBuilder app)
        {
            // Custom first-level routing
            app.Use(async (context, next) =>
            {
                // Example
                // await context.Response.WriteAsync("Hello, world!");

                // Continue next routings
                await next();
            });
            
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();

            // Automatic endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/Notes", WebHostEndpoints.EndpointGetNotes);
                
                // Blazor
                endpoints.MapBlazorHub();
                // endpoints.MapFallbackToPage("/_Host");
            });
            
            // Final catch-all
            app.Use(async (context, next) => {
                await context.Response.WriteAsync("Cannot find target endpoint.");
            });
        }
        #endregion
    }
}