using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Somewhere2.ApplicationState;
using Somewhere2.Constants;
using Somewhere2.GUIApplication;
using Somewhere2.GUIApplication.ToolWindows;
using Somewhere2.SystemService;
using Somewhere2.WPFApplication.Applets;
using ScratchPad = Somewhere2.WPFApplication.Applets.ScratchPad;

namespace Somewhere2.CLIApplication
{
    internal partial class CommandHandler
    {
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
        private void Browse()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CurrentWorkingDirectory;
            openFileDialog.Title = "Open - Select a database file";
            openFileDialog.Filter = $"database files (*{StringConstants.SomewhereExtension})|*{StringConstants.SomewhereExtension}";
            openFileDialog.Multiselect = false;
            if(openFileDialog.ShowDialog() == true)
                OpenDatabaseFile(openFileDialog.FileName);
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
        private void HideConsole()
            => WindowHelper.HideConsole();

        private void MakeNotes(string[] arguments)
        {
            // Parse arguments
            string name = null;
            string content = null;
            string tags = null;
            if (arguments.Length >= 1) name = arguments[0];
            else
            {
                ColorfulPrintLine("Enter interactive notes.", "Warning");
                ColorfulPrint("Enter name of target file (if file doesn't exist, a virtual note will be created): ");
                name = Console.ReadLine();
            }

            if (arguments.Length >= 2) content = arguments[1];
            else
            {
                ColorfulPrint("Enter note content: ");
                content = Console.ReadLine();
            }

            if (arguments.Length >= 3) tags = arguments[2];
            else
            {
                ColorfulPrint("Enter note tags (leave empty to skip): ");
                tags = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tags)) tags = null;
            }
            
            // Make sense of (Shorthand) filename
            string shorthandPath = name;
            string finalPath = null;
            if (File.Exists(shorthandPath) || Directory.Exists(shorthandPath))
            {
                finalPath = shorthandPath;
            }
            else
            {
                string normalizedPath = NormalizeFilePath(shorthandPath);
                string databasePath = CheckAppendSuffix(normalizedPath, StringConstants.DatabaseSuffix);
                if (File.Exists(normalizedPath) || Directory.Exists(normalizedPath))
                    finalPath = normalizedPath;
                if (File.Exists(databasePath) || Directory.Exists(databasePath))
                    finalPath = databasePath;
            }

            if (finalPath == null) finalPath = $"{StringConstants.NoteURLProtocol}{name}";
            
            // Update item: FinalPath cannot be null, content cannot be null, tags can be null
            RuntimeData.UpdateItem(finalPath, content, tags == null ? null : StringHelper.SplitTags(tags));
        }
        /// <summary>
        /// Decide whether a path is absolute or relative
        /// </summary>
        string NormalizeFilePath(string shorthand)
            // If it contains a valid parent directory, then it's absolute
            => Directory.Exists(Path.GetDirectoryName(shorthand))
                ? shorthand
                : Path.Combine(CurrentWorkingDirectory, shorthand);
        private void OpenBrowser()
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(RuntimeData.WebHostInfo.NotesURL)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
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
            {
                PrintIntroText(filePath);
                AddRecent(filePath, RecentType.Database);
            }
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
            
            new MainApplication(RuntimeData).Run();
        }

        private void ShowStats()
        {
            int tags = RuntimeData.Tags.Count();
            ColorfulPrintLine($"<Yellow>Tags</>: <Blue>{tags}</>");
            
            ColorfulPrintLine($"<White>{"Files".PadRight(8)}</><White>{"Folders".PadRight(8)}</><White>{"Notes".PadRight(8)}</><White>{"Total".PadRight(8)}</>");
            int files = RuntimeData.SystemEntries.Count(f => f.Value.Type == ItemType.File);
            int folders = RuntimeData.SystemEntries.Count(f => f.Value.Type == ItemType.Folder);
            int notes = RuntimeData.Notes.Count;
            int total = files + folders + notes;
            ColorfulPrintLine($"<Orange>{files.ToString().PadRight(8)}</><Orange>{folders.ToString().PadRight(8)}</><Orange>{notes.ToString().PadRight(8)}</><Orange>{total.ToString().PadRight(8)}</>");
        }
        private void ShowStatsWindow()
            => new StatsWindow(RuntimeData).Run();
        private void ShowScratchPad()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ScratchPad scratchPad = new ScratchPad();
                scratchPad.Show();
            });
        }

        private void TryOpen(string filename)
        {
            string shorthandPath = filename;
            string normalizedPath = NormalizeFilePath(shorthandPath);
            string fullPath = CheckAppendSuffix(normalizedPath, StringConstants.DatabaseSuffix);
            OpenDatabaseFile(fullPath);
        }
        #endregion
    }
}