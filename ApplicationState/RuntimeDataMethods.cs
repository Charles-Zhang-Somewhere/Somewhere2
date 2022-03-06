using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Somewhere2.Constants;
using Somewhere2.GUIApplication;

namespace Somewhere2.ApplicationState
{
    public partial class RuntimeData
    {
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
        public void UpdateSystemEntry(string path, string[] tags, string note = null)
        {
            if (SystemEntries.ContainsKey(path))
            {
                if(tags != null) SystemEntries[path].Tags = tags;
                if(note != null) SystemEntries[path].Notes = note;
            }
            else SystemEntries[path] = new TagItem(path, tags ?? new string[]{}, note ?? string.Empty);
        }
        /// <param name="tags">Empty to set to empty, null to ignore</param>
        public void UpdateItem(string path, string note, string[] tags)
        {
            if (path.StartsWith(StringConstants.NoteURLProtocol))
            {
                Notes.Add(new TagItem(path, tags ?? new string []{}, note ?? string.Empty));    // Always add new notes since there is no name key
            }
            else UpdateSystemEntry(path, tags, note);
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