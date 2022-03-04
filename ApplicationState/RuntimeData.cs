﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Somewhere2.Constants;
using NotImplementedException = System.NotImplementedException;

namespace Somewhere2.ApplicationState
{
    public class RuntimeData
    {
        #region Settings and Temporary Data
        public ApplicationConfiguration Configuration { get; set; }
        public List<Recent> Recents { get; set; }
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