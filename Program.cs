using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using Somewhere2.ApplicationState;
using Somewhere2.Constants;
using Somewhere2.GUIApplication;
using Somewhere2.System;

namespace Somewhere2
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new CommandHandler();
            handler.Start();
        }
    }
    
    class CommandHandler
    {
        #region Construction
        public CommandHandler()
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
            RuntimeData = new RuntimeData();
            
            PrepareFileServices();
        }
        #endregion
        
        #region Interface
        public void Start()
        {
            string Shorten(string path)
            {
                return
                    $"{Path.GetPathRoot(path)}{Path.DirectorySeparatorChar}...{Path.DirectorySeparatorChar}{Path.GetFileName(path)}";
            }
            
            PrintWelcomeText();
            while (!ShouldExit)
            {
                Console.Write($"> {Shorten(CurrentWorkingDirectory)}: ");
                string input = Console.ReadLine();
                PreProcessInput(input);
            }
        }

        public void PreProcessInput(string input)
        {
            string command = input.Split(' ').First().Trim();
            Dictionary<string, string> mapping = new Dictionary<string, string>();
            
            string[] rawArguments = new string[]{};
            if (input.Trim().Length > command.Length)
            {
                switch (command)
                {
                    case "add":
                    case "cd":
                    case "tag":
                    case "note":
                        rawArguments = input.Contains('"') 
                            ? input.SplitCommandLine().ToArray() 
                            : new string[]{command, input.Substring(command.Length + 1).Trim()};
                        break;
                    default:
                        rawArguments = input.Split(' ');
                        break;
                }   
            }

            string[] databaseCommands = new string[] { 
                "add", "tag", "tagfile", "tagfolder", 
                "rm", 
                "note", "tags", "items", "notes" 
            };
            if (!RuntimeData.Loaded && databaseCommands.Contains(command))
                ColorfulPrintLine("<Error>Load a database first before executing tagging operations.</>");
            else
                HandleCommands(command, rawArguments.Skip(1).ToArray(), mapping);
        }
        #endregion

        #region States
        public bool ShouldExit { get; set; }
        /// <summary>
        /// Notice that CurrentWorkingDirectory is strictly a command-line concept and should not be part of the
        /// application's RuntimeData
        /// </summary>
        public string CurrentWorkingDirectory { get; set; }
        public RuntimeData RuntimeData { get; set; }
        #endregion

        #region Utilities

        #endregion

        #region Commands

        #endregion

        #region Routines
        private void AddRecent(string value, RecentType type)
        {
            RuntimeData.Recents.Insert(0, new Recent()
            {
                Value = value,
                Annotation = type
            });
            RuntimeData.Recents = RuntimeData.Recents.Distinct().ToList();
            FileService.UpdateRecentFile(RuntimeData.Recents);
        }
        /// <summary>
        /// Check whether the filename part of the file, e.g. database file, contains proper suffix
        /// </summary>
        string CheckAppendSuffix(string originalPath, string extension)
            => !originalPath.EndsWith(extension)
                ? $"{originalPath}{extension}"
                : originalPath; 
        private void ClearRecent()
        {
            RuntimeData.Recents = new List<Recent>();
            FileService.UpdateRecentFile(RuntimeData.Recents);
        }
        private void ColorfulPrint(string text)
        {
            // Save previous color
            var previous = Console.ForegroundColor;

            StringBuilder buffer = new StringBuilder();
            string currentStyle = "Default";
            void SwitchStyle()
            {
                switch (currentStyle)
                {
                    // Special names
                    case "Code":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    default:
                    case "Default":
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case "Bold":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Body":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Emphasis":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "Warning":
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "Error":
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    // Color names
                    case "Blue":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "Cyan":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case "Gray":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Green":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case "Orange":
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "White":
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }
            void EnterSpan(string style)
            {
                // Output existing buffer
                if (buffer.Length != 0)
                {
                    SwitchStyle();
                    Console.Write(buffer);
                }
                // Start new buffer
                currentStyle = style;
                buffer.Clear();
            }

            // Parse and output each character as buffer
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                char cNext = i < text.Length - 1 ? text[i + 1] : char.MinValue;
                char cNextNext = i < text.Length - 2 ? text[i + 2] : char.MinValue;
                
                // Bolding
                if (c == '*' && cNext == '*')
                {
                    if (currentStyle != "Bold")
                        EnterSpan("Bold");    
                    else 
                        EnterSpan("Default");
                    i++;    // Skip next
                }
                // Tag starting
                else if (c == '<' && cNext != '/')
                {
                    string remaining = text.Substring(i + 1);
                    int endingBracket = remaining.IndexOf('>');
                    if (endingBracket > 1)
                    {
                        string tag = remaining.Substring(0, endingBracket);
                        EnterSpan(tag);
                        i += endingBracket + 1;
                    }
                }
                // Tag ending
                else if (c == '<' && cNext == '/' && cNextNext == '>')
                {
                    EnterSpan("Default");
                    i += 2; // Skip next 2
                }
                else
                    buffer.Append(c);
            }
            
            // Print remaining
            if (buffer.Length != 0)
                EnterSpan("Finish");
            // Reset
            Console.ForegroundColor = previous;
        }
        private void ColorfulPrintLine(string text)
        {
            ColorfulPrint(text);
            Console.WriteLine();
        }
        private void ColorfulPrintLine(string text, string tagWrapper)
        {
            ColorfulPrint($"<{tagWrapper}>{text}</>");
            Console.WriteLine();
        }
        private void HandleCommands(string command, string[] arguments, Dictionary<string, string> kepMaps)
        {
            // Alphabetical order
            switch (command)
            {
                case "add":
                case "tag":
                    Tag(arguments);
                    break;
                case "cd":
                    if (Directory.Exists(arguments[0]))
                    {
                        CurrentWorkingDirectory = arguments[0];
                        AddRecent(CurrentWorkingDirectory, RecentType.Folder);
                    }
                    else
                        ColorfulPrintLine("Not a valid directory.", "Error");
                    break;
                case "exit":
                    ShouldExit = true;
                    break;
                case "ls":
                    ColorfulPrintLine($"<White>{"Name".PadRight(60)}Type</>");
                    foreach (string path in Directory.EnumerateFileSystemEntries(CurrentWorkingDirectory))
                    {
                        string extension = Path.GetExtension(path);
                        string filename = Path.GetFileName(path).PadRight(60);
                        if(extension == StringConstants.SomewhereExtension) ColorfulPrintLine($"<Cyan>{filename}</>{extension}");
                        else ColorfulPrintLine($"<Orange>{filename}</>{extension}");
                    }
                    break;
                case "note":
                    string name = arguments[0];
                    
                    break;
                case "open":
                {
                    string shorthandPath = arguments[0];
                    string normalizedPath = NormalizeFilePath(shorthandPath);
                    string fullPath = CheckAppendSuffix(normalizedPath, StringConstants.DatabaseSuffix);
                    OpenDatabaseFile(fullPath);
                    break;
                }
                case "gui":
                    RunGUI();
                    break;
                case "help":
                    string text = Helpers.ReadTextResource($"Somewhere2.Documentation.Commands.{arguments[0]}.txt");
                    ColorfulPrintLine(text);
                    break;
                case "pwd":
                    ColorfulPrintLine(CurrentWorkingDirectory);
                    break;
                case "recent":
                    PrintRecent(arguments);
                    break;
                case "setting":
                    ColorfulPrintLine("This command will open an external editor.");
                    Thread.Sleep(2000);
                    FileService.EditConfigFile(RuntimeData);
                    break;
            }
        }
        private void HideConsole()
            => WindowHelper.HideConsole();
        /// <summary>
        /// Decide whether a path is absolute or relative
        /// </summary>
        string NormalizeFilePath(string shorthand)
            // If it contains a valid parent directory, then it's absolute
            => Directory.Exists(Path.GetDirectoryName(shorthand))
                ? shorthand
                : Path.Combine(CurrentWorkingDirectory, shorthand);
        private void OpenDatabaseFile(string filePath)
        {
            // Load existing one
            if (File.Exists(filePath))
            {
                RuntimeData.LoadDatabaseFile(filePath);
            }
            // Create one if possible
            else if (Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                RuntimeData.CreateAndLoadDatabaseFile(filePath);
            }

            if (RuntimeData.Loaded)
                PrintIntroText(filePath);
        }
        private void PrintWelcomeText()
        {
            string text = Helpers.ReadTextResource("Somewhere2.Documentation.Welcome.txt");
            ColorfulPrintLine(text);
        }
        private void PrintIntroText(string databasePath)
        {
            ColorfulPrintLine($"<Blue>{RuntimeData.DatabaseName}</> ({databasePath})");
            string text = Helpers.ReadTextResource("Somewhere2.Documentation.NewDatabase.txt");
            ColorfulPrintLine(text);
        }
        private void PrepareFileServices()
        {
            RuntimeData.Configuration = FileService.CheckConfigFile();
            RuntimeData.Recents = FileService.CheckRecentFile();
        }
        private void RunGUI()
        {
            HideConsole();
            
            Application window = new Application(RuntimeData);
            window.Run();
        }
        string[] SplitTags(string csv)
            => csv.Split(',').Select(t => t.Trim().ToLower()).Distinct().OrderBy(t => t).ToArray();
        #endregion

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
            
            RuntimeData.Update(normalizedPath, SplitTags(tags));
        }

        private void InteractiveTag()
        {
            ColorfulPrintLine($"<Bold>Start interactive tagging at current working directory.</> ({CurrentWorkingDirectory})");
            string text = Helpers.ReadTextResource("Somewhere2.Documentation.InteractiveTag.txt");
            ColorfulPrintLine(text);
            
            
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
        #endregion
    }
}