using System;
using System.Collections.Generic;
using System.Linq;

namespace Somewhere2.Shared.DataTypes
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

        public static RuntimeData Singleton { get; private set; }
        #endregion
        
        #region Settings and Temporary Data
        public ApplicationConfiguration Configuration { get; set; }
        public List<Recent> Recents { get; set; }
        public WebHostInfo WebHostInfo { get; set; }
        #endregion

        #region Accessors
        public IEnumerable<string> Tags => SystemEntries.Values.SelectMany(v => v.Tags).Distinct()
            .Union(Notes.SelectMany(n => n.Tags).Distinct())
            .Distinct().OrderBy(t => t);
        public IEnumerable<TagItem> AllItems => SystemEntries.Values.Union(Notes);
        #endregion

        #region Opened Database
        public string DatabasePath { get; private set; }
        public string DatabaseName { get; private set; }
        public bool Loaded => SystemEntries != null || Notes != null;
        public Dictionary<string, TagItem> SystemEntries { get; private set; }
        public List<TagItem> Notes { get; private set; }
        #endregion
    }
}