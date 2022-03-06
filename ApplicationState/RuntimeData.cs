using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Threading;
using Somewhere2.Constants;
using Somewhere2.GUIApplication;
using NotImplementedException = System.NotImplementedException;

namespace Somewhere2.ApplicationState
{
    public partial class RuntimeData
    {
        #region Constructor
        public RuntimeData()
        {
            if (Singleton == null)
                Singleton = this;
            else
            {
                throw new InvalidOperationException("RuntimeData is already initialized! Singleton is not null.");
            }
        }
        #endregion

        #region Settings and Temporary Data
        public ApplicationConfiguration Configuration { get; set; }
        public List<Recent> Recents { get; set; }
        #endregion

        #region Accessors
        public IEnumerable<string> Tags => SystemEntries.Values.SelectMany(v => v.Tags).Distinct()
            .Union(Notes.SelectMany(n => n.Tags).Distinct())
            .Distinct().OrderBy(t => t);
        public IEnumerable<TagItem> AllItems => SystemEntries.Values.Union(Notes);
        #endregion

        #region Global Contexts
        public RenderingContext RenderingContext { get; set; }
        public MainApplication MainGUIApplication { get; set; }
        public WebHostInfo WebHostInfo { get; set; }
        public Dispatcher STADispatcher { get; set; }
        public static RuntimeData Singleton { get; set; }
        #endregion

        #region Opened Database
        public string DatabaseName { get; set; }
        public bool Loaded => SystemEntries != null || Notes != null;
        public Dictionary<string, TagItem> SystemEntries { get; set; }
        public List<TagItem> Notes { get; set; }
        #endregion
    }
}