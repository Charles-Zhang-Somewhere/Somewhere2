using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Somewhere2.ApplicationState;
using NotImplementedException = System.NotImplementedException;

namespace Somewhere2.System
{
    public class FileService
    {
        private static string ApplicationDirectory
            => AppDomain.CurrentDomain.BaseDirectory;
        private static string ConfigFilePath
            => Path.Combine(ApplicationDirectory, "Somewhere2.config");
        private static string RecentFilePath
            => Path.Combine(ApplicationDirectory, "Somewhere2.recents");

        private static FileSystemWatcher ConfigWatcher = null;
        private static RuntimeData RuntimeReference = null;

        public static void EditConfigFile(RuntimeData runtime)
        {
            RuntimeReference = runtime;
            
            new Process
            {
                StartInfo = new ProcessStartInfo(ConfigFilePath)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
        private static void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;
            if (RuntimeReference != null)
                RuntimeReference.Configuration = CheckConfigFile();
        }

        public static ApplicationConfiguration CheckConfigFile()
        {
            ApplicationConfiguration configuration = null;
            if (File.Exists(ConfigFilePath))
            {
                configuration = new YamlDotNet.Serialization.Deserializer().Deserialize<ApplicationConfiguration>(
                    File.ReadAllText(ConfigFilePath));
            }
            else
            {
                configuration = new ApplicationConfiguration();
                configuration.InitializeDefault();
                File.WriteAllText(ConfigFilePath, new YamlDotNet.Serialization.Serializer().Serialize(configuration));
            }
            
            if(ConfigWatcher != null) ConfigWatcher.Dispose();
            ConfigWatcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigFilePath));
            ConfigWatcher.NotifyFilter = NotifyFilters.Attributes
                                         | NotifyFilters.CreationTime
                                         | NotifyFilters.DirectoryName
                                         | NotifyFilters.FileName
                                         | NotifyFilters.LastAccess
                                         | NotifyFilters.LastWrite
                                         | NotifyFilters.Security
                                         | NotifyFilters.Size;
            ConfigWatcher.Changed += OnConfigChanged;
            ConfigWatcher.Filter = Path.GetFileName(ConfigFilePath);
            ConfigWatcher.IncludeSubdirectories = false;
            ConfigWatcher.EnableRaisingEvents = true;

            return configuration;
        }

        public static List<Recent> CheckRecentFile()
        {
            if (File.Exists(RecentFilePath))
            {
                return new YamlDotNet.Serialization.Deserializer().Deserialize<List<Recent>>(
                    File.ReadAllText(RecentFilePath));
            }
            else
            {
                List<Recent> recents = new List<Recent>();
                File.WriteAllText(RecentFilePath, new YamlDotNet.Serialization.Serializer().Serialize(recents));
                return recents;
            }
        }

        public static void UpdateRecentFile(List<Recent> runtimeDataRecents)
        {
            File.WriteAllText(RecentFilePath, new YamlDotNet.Serialization.Serializer().Serialize(runtimeDataRecents));
        }
    }
}