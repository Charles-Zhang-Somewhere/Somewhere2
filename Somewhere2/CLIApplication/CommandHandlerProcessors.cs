using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Somewhere2.ApplicationState;
using Somewhere2.Shared;
using Somewhere2.Shared.Constants;
using Somewhere2.Shared.DataTypes;

namespace Somewhere2.CLIApplication
{
    internal partial class CommandHandler 
    {
        #region Command Processors
        private void Tag(string[] arguments)
        {
            if (arguments.Length == 0)
            {
                InteractiveTag();
                return;
            }
            
            string shorthandPath = arguments[0];
            string normalizedPath = NormalizeFilePath(shorthandPath);

            string tags = string.Empty;
            if (arguments.Length == 1)
            {
                Console.Write("Enter tags (separate with comma, case-insensitive): ");
                tags = Console.ReadLine();
            }
            else tags = arguments[1];
            
            RuntimeData.UpdateItem(normalizedPath, null, StringHelper.SplitTags(tags));
        }

        private void InteractiveTag()
        {
            ColorfulPrintLine($"<Bold>Start interactive tagging at current working directory.</> ({CurrentWorkingDirectory})");
            string text = Helpers.ReadTextResource(Assembly.GetExecutingAssembly(), "Somewhere2.Documentation.InteractiveTag.txt");
            ColorfulPrintLine(text);

            int operation;
            ColorfulPrint("<Bold>Select Operation</> (<Gray>1) Apply tags to chosen files</> <Gray>2) Remove tags from chosen files</>): ");
            if (!int.TryParse(Console.ReadLine(), out operation)) return;
            
            ColorfulPrint("<Bold>Enter tags</>: ");
            string[] tags = StringHelper.SplitTags(Console.ReadLine());

            ColorfulPrintLine($"<White>{"ID".PadRight(5)}{"Name".PadRight(60)}Type</>");
            string[] entries = Directory.EnumerateFileSystemEntries(CurrentWorkingDirectory).ToArray();
            for (int i = 0; i < entries.Length; i++)
            {
                string path = entries[i];
                string id = $"{i+1}".PadRight(5);
                string extension = Path.GetExtension(path);
                string filename = Path.GetFileName(path).PadRight(60);
                ColorfulPrintLine(extension == StringConstants.SomewhereExtension
                    ? $"{id}<Cyan>{filename}</>{extension}"
                    : $"{id}<Orange>{filename}</>{extension}");
            }
            ColorfulPrint("<White>Choose files (separate with space): </>");
            try
            {
                int[] indices = StringHelper.SplitTags(Console.ReadLine(), ' ')
                    .Select(int.Parse).Where(i => i >= 1 && i <= entries.Length)
                    .Select(i => i - 1).ToArray();

                switch (operation)
                {
                    case 1:
                        ColorfulPrint($"<White>The following entries will be applied with: </>{string.Join(", ", tags)}");
                        break;
                    case 2:
                        ColorfulPrint($"<White>The following entries will be removed with: </>{string.Join(", ", tags)}");
                        break;
                }
                Console.ReadLine();
                foreach (string shorthand in indices.Select(i => entries[i]))
                {
                    ColorfulPrintLine(shorthand);
                }
                ColorfulPrint("<Warning>Continue? [Y/N]</> ");
                string cont = Console.ReadLine();
                // Do something
                // ...
                // Summary
                ColorfulPrintLine($"{indices.Length} {(indices.Length == 1 ? "entry is" : "entries are")} updated.", "Blue");
            }
            catch (Exception e)
            {
                ColorfulPrintLine(e.Message, "Error");
                return;
            }
        }
        private void PrintDirectory()
        {
            ColorfulPrintLine($"<White>{"Name".PadRight(60)}Type</>");
            foreach (string path in Directory.EnumerateFileSystemEntries(CurrentWorkingDirectory))
            {
                string extension = Path.GetExtension(path);
                string filename = Path.GetFileName(path).PadRight(60);
                if(extension == StringConstants.SomewhereExtension) ColorfulPrintLine($"<Cyan>{filename}</>{extension}");
                else ColorfulPrintLine($"<Orange>{filename}</>{extension}");
            }
        }
        private void PrintRecent(string[] arguments)
        {
            if (arguments.Length == 0)
            {
                ColorfulPrintLine($"{"Path".PadRight(70)}Type", "White");
                foreach (Recent recent in RuntimeData.Recents)
                {
                    ColorfulPrintLine($"{recent.Value.PadRight(70)}{recent.Annotation}");
                }
            }
            else
            {
                switch (arguments[0])
                {
                    case "-r":
                    case "clear":
                        ClearRecent();
                        break;
                }   
            }
        }
        private void ShowTags()
        {
            string[] tags = RuntimeData.Tags.ToArray();
            for (int i = 0; i < tags.Length; i++)
            {
                string tag = tags[i];
                if(i != tags.Length - 1)
                    ColorfulPrint($"<Orange>{tag}</>, ");
                else
                    ColorfulPrint($"<Orange>{tag}</>");
            }
            Console.WriteLine();
        }
        #endregion
    }
}