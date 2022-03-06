using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Somewhere2.ApplicationState;
using Somewhere2.Constants;
using Somewhere2.SystemService;

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
                    if (arguments.Length != 0 && Directory.Exists(arguments[0]))
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
                case "inspect":
                    InspectDatabaseContent();
                    break;
                case "items":
                    ShowItems();
                    break;
                case "ls":
                    PrintDirectory();
                    break;
                case "note":
                    MakeNotes(arguments);
                    break;
                case "notes":
                    ShowNotes();
                    break;
                case "open":
                    TryOpen(arguments[0]);
                    break;
                case "gui":
                    RunGUI();
                    break;
                case "help":
                    string text = Helpers.ReadTextResource($"Somewhere2.Documentation.Commands.{arguments[0]}.txt");
                    ColorfulPrintLine(text);
                    break;
                case "last":
                    int index = RuntimeData.Recents.FindIndex(r => r.Annotation == RecentType.Database);
                    if (index >= 0)
                    {
                        string databasePath = RuntimeData.Recents[index].Value;
                        OpenDatabaseFile(databasePath);
                        CurrentWorkingDirectory = Path.GetDirectoryName(databasePath);
                    }
                    else
                        ColorfulPrintLine("No record is available!", "Warning");
                    break;
                case "pwd":
                    ColorfulPrintLine(CurrentWorkingDirectory);
                    break;
                case "recent":
                    PrintRecent(arguments);
                    break;
                case "setting":
                    ShowEditConfigFile();
                    break;
                case "stats":
                    ShowStats();
                    break;
                case "sp":
                case "scratchpad":
                    ShowScratchPad();
                    break;
                case "tags":
                    ShowTags();
                    break;
            }
        }
        #endregion
    }
}