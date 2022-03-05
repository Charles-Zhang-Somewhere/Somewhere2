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
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            
            // // Output a "hello world" to the user who accesses the server
            // app.Use(async (context, next) =>
            // {
            //     await context.Response.WriteAsync("Hello, world!");
            // });
        }
        #endregion
    }
}