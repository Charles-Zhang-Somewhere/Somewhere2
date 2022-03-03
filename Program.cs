using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            string command = input.Split(' ').First();
            Dictionary<string, string> mapping = new Dictionary<string, string>();
            
            string[] rawArguments = null;
            switch (command)
            {
                case "cd":
                    rawArguments = new string[]{command, input.Substring(command.Length + 1).Trim().Trim('"')};
                    break;
                default:
                    rawArguments = input.Split(' ');
                    break;
            }
            HandleCommands(command, rawArguments, mapping);
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
                    // Color names
                    case "Blue":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "Gray":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Green":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
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
        /// <param name="rawArguments">Includes the command itself</param>
        private void HandleCommands(string command, string[] rawArguments, Dictionary<string, string> kepMaps)
        {
            // Decide whether a path is absolute or relative
            string NormalizeFilePath(string shorthand)
                // If it contains a valid parent directory, then it's absolute
                => Directory.Exists(Path.GetDirectoryName(shorthand))
                    ? shorthand
                    : Path.Combine(CurrentWorkingDirectory, shorthand);
            // Check whether the filename part of the file, e.g. database file, contains proper suffix
            string CheckAppendSuffix(string originalPath, string extension)
                => !originalPath.EndsWith(extension)
                    ? $"{originalPath}{extension}"
                    : originalPath; 
            
            // Alphabetical order
            switch (command)
            {
                case "cd":
                    CurrentWorkingDirectory = rawArguments[1];
                    break;
                case "exit":
                    ShouldExit = true;
                    break;
                case "open":
                    string shorthandPath = rawArguments[1];
                    string normalizedPath = NormalizeFilePath(shorthandPath);
                    string fullPath = CheckAppendSuffix(normalizedPath, StringConstants.DatabaseSuffix);
                    OpenDatabaseFile(fullPath);
                    break;
                case "gui":
                    RunGUI();
                    break;
                case "help":
                    string text = Helpers.ReadTextResource($"Somewhere2.Documentation.Commands.{rawArguments[1]}.txt");
                    ColorfulPrintLine(text);
                    break;
                case "pwd":
                    ColorfulPrintLine(CurrentWorkingDirectory);
                    break;
            }
        }
        private void HideConsole()
            => WindowHelper.HideConsole();
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
        #endregion
    }
}