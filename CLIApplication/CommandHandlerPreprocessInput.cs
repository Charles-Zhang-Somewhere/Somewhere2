using System.Collections.Generic;
using System.Linq;

namespace Somewhere2.CLIApplication
{
    internal partial class CommandHandler
    {
        #region Routines
        public void PreprocessInput(string input)
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
                    case "t":
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
                "add", "t", "tag", "tagfile", "tagfolder", 
                "rm", 
                "note", "tags", "items", "notes",
                //"stats", "sp", "scratchpad"
            };
            if (!RuntimeData.Loaded && databaseCommands.Contains(command))
                ColorfulPrintLine("<Error>Load a database first before executing tagging operations.</>");
            else
                HandleCommands(command, rawArguments.Skip(1).ToArray(), mapping);
        }
        #endregion
    }
}