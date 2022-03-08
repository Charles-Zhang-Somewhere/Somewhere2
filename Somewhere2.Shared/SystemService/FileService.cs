using System;
using System.Collections.Generic;
using System.IO;
using Somewhere2.Shared.DataTypes;

namespace Somewhere2.Shared.SystemService
{
    public static class FileService
    {
        private static string ApplicationDirectory
            => AppDomain.CurrentDomain.BaseDirectory;
        private static string ConfigFilePath
            => Path.Combine(ApplicationDirectory, "Somewhere2.config");
        private static string RecentFilePath
            => Path.Combine(ApplicationDirectory, "Somewhere2.recents");

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

        public static void SaveConfig(ApplicationConfiguration configuration)
        {
            File.WriteAllText(ConfigFilePath, new YamlDotNet.Serialization.Serializer().Serialize(configuration));
        }
        public static void UpdateRecentFile(List<Recent> runtimeDataRecents)
        {
            File.WriteAllText(RecentFilePath, new YamlDotNet.Serialization.Serializer().Serialize(runtimeDataRecents));
        }
    }
}