using System.Collections.Generic;
using Somewhere2.ApplicationState;

namespace Somewhere2.WebHost.RazorTemplates
{
    public class GetNotesTemplateModel
    {
        public List<TagItem> Items { get; set; }
    }
}