using System;
using System.Collections.Generic;
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
            
            new MainApplication(RuntimeData).Run();
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
        string[] SplitTags(string csv, char splitter = ',')
            => csv.Split(splitter).Select(t => t.Trim().ToLower()).Distinct().OrderBy(t => t).ToArray();
        #endregion
    }
}