using System;
using System.IO;
using Somewhere2.Constants;
using NotImplementedException = System.NotImplementedException;

namespace Somewhere2.ApplicationState
{
    public class RuntimeData
    {
        #region Settings and Temporary Data
        public ApplicationConfiguration Configuration { get; set; }
        public string[] Recents { get; set; }
        #endregion

        #region Opened Database
        public string DatabaseName { get; set; }
        public bool Loaded => Items != null;
        public TagItems Items { get; set; }
        #endregion

        public void LoadDatabaseFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            Items = new YamlDotNet.Serialization.Deserializer().Deserialize<TagItems>(content);
            DatabaseName = GetDatabaseName(filePath);
        }

        public void CreateAndLoadDatabaseFile(string filePath)
        {
            Items = new TagItems();
            string content = new YamlDotNet.Serialization.Serializer().Serialize(Items);
            File.WriteAllText(filePath, content);
            DatabaseName = GetDatabaseName(filePath);
        }

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