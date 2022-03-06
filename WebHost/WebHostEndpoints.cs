﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RazorEngine;
using RazorEngine.Templating;
using Somewhere2.ApplicationState;
using Somewhere2.WebHost.RazorTemplates;

namespace Somewhere2.WebHost
{
    internal static class WebHostEndpoints
    {
        #region Accesor
        private static RuntimeData Runtime => RuntimeData.Singleton;
        #endregion
        
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

        #region Routines

        private static string GetTemplate(string templateURI)
            => Helpers.ReadTextResource(templateURI);
        #endregion
    }
}