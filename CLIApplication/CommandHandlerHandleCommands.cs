using System.Collections.Generic;
using System.IO;
using System.Threading;
using Somewhere2.ApplicationState;
using Somewhere2.Constants;
using Somewhere2.System;

namespace Somewhere2.CLIApplication
{
    internal partial class CommandHandler
    {
        #region Routines
        private void HandleCommands(string command, string[] arguments, Dictionary<string, string> kepMaps)
        {
            // Alphabetical order
            switch (command)
            {
                case "add":
                case "tag":
                    Tag(arguments);
                    break;
                case "browse":
                    Browse();
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
                    PrintDirectory();
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
                case "tags":
                    ShowTags();
                    break;
            }
        }
        #endregion
    }
}