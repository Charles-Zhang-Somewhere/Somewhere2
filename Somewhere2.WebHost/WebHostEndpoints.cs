using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RazorEngine;
using RazorEngine.Templating;
using Somewhere2.Shared;
using Somewhere2.Shared.DataTypes;
using Somewhere2.WebHost.RazorTemplates;

namespace Somewhere2.WebHost
{
    internal static class WebHostEndpoints
    {
        #region Accesor
        private static RuntimeData Runtime => RuntimeData.Singleton;
        #endregion

        #region Endpoints
        public static async Task EndpointGetItems(HttpContext context)
        {
            string tempalte = GetTemplate("Somewhere2.WebHost.RazorTemplates.GetItemsTemplate.cshtml");
            GetItemsTemplateModel model = new GetItemsTemplateModel()
            {
                Items = Runtime.AllItems.ToList()
            };
            string html = Engine.Razor.RunCompile(tempalte, "GetItems", typeof(GetItemsTemplateModel), model);
            
            await context.Response.WriteAsync(html);
        }
        public static async Task EndpointGetNotes(HttpContext context)
        {
            string tempalte = GetTemplate("Somewhere2.WebHost.RazorTemplates.GetNotesTemplate.cshtml");
            GetNotesTemplateModel model = new GetNotesTemplateModel()
            {
                Items = Runtime.AllItems.ToList()
            };
            string html = Engine.Razor.RunCompile(tempalte, "GetNotes", typeof(GetNotesTemplateModel), model);
            
            await context.Response.WriteAsync(html);
        }
        #endregion

        #region Routines

        private static string GetTemplate(string templateURI)
            => Helpers.ReadTextResource(Assembly.GetExecutingAssembly(),  templateURI.EndsWith(".ignore") ? templateURI : templateURI + ".ignore");
        #endregion
    }
}