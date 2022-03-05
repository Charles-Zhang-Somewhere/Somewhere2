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
    public class RuntimeData
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

        #region Interface
        public void CreateAndLoadDatabaseFile(string filePath)
        {
            SystemEntries = new Dictionary<string, TagItem>();
            Notes = new List<TagItem>();
            Database database = new Database()
            {
                SystemEntries = SystemEntries.Values.OrderBy(i => i.Path).ToList(),
                Notes = Notes
            };
            string content = new YamlDotNet.Serialization.Serializer().Serialize(database);
            File.WriteAllText(filePath, content);
            DatabaseName = GetDatabaseName(filePath);
        }
        public void InitializeRenderingContext()
        {
            RenderingContext = new RenderingContext()
            {
                MainWindow = null,
                BasicRendering = BasicRenderingInfrastructure.Setup()
            };
        }
        public void LoadDatabaseFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            Database database = new YamlDotNet.Serialization.Deserializer().Deserialize<Database>(content);
            SystemEntries = database.SystemEntries.ToDictionary(i => i.Path, i => i);
            Notes = database.Notes;
            DatabaseName = GetDatabaseName(filePath);
        }
        public void Remove(string path)
        {
            if (SystemEntries.ContainsKey(path))
                SystemEntries.Remove(path);
        }
        public void Update(string path, string[] tags)
        {
            if (SystemEntries.ContainsKey(path))
                SystemEntries[path].Tags = tags;
            else SystemEntries[path] = new TagItem(path, tags, string.Empty);
        }
        #endregion
        
        #region Routines
        private string GetDatabaseName(string fullPath)
        {
            string filename = Path.GetFileName(fullPath);
            string name = filename.Substring(0, filename.IndexOf(StringConstants.DatabaseSuffix, StringComparison.InvariantCulture));
            return name;
        }
        #endregion
    }
}