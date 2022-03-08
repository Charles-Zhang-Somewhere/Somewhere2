using System.IO;
using System.Linq;
using System.Reflection;

namespace Somewhere2.Shared
{
    public static class Helpers
    {
        public static Stream ReadBinaryResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(Somewhere2)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            return stream;
        }
        
        public static string ReadTextResource(Assembly sourceAssembly, string name)
        {
            // Determine path
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(Somewhere2)))
            {
                resourcePath = sourceAssembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using (Stream stream = sourceAssembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}