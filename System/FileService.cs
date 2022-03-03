using System;
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
        
        public static ApplicationConfiguration CheckConfigFile()
        {
            if (File.Exists(ConfigFilePath))
            {
                return new YamlDotNet.Serialization.Deserializer().Deserialize<ApplicationConfiguration>(
                    File.ReadAllText(ConfigFilePath));
            }
            else
            {
                ApplicationConfiguration configuration = new ApplicationConfiguration();
                configuration.InitializeDefault();
                File.WriteAllText(ConfigFilePath, new YamlDotNet.Serialization.Serializer().Serialize(configuration));
                return configuration;
            }
        }

        public static string[] CheckRecentFile()
        {
            if (File.Exists(RecentFilePath))
            {
                return new YamlDotNet.Serialization.Deserializer().Deserialize<string[]>(
                    File.ReadAllText(RecentFilePath));
            }
            else
            {
                string[] recents = new string[] { };
                File.WriteAllText(RecentFilePath, new YamlDotNet.Serialization.Serializer().Serialize(recents));
                return recents;
            }
        }
    }
}