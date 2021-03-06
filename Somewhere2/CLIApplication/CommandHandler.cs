using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Somewhere2.ApplicationState;
using Somewhere2.GUIApplication;
using RuntimeData = Somewhere2.Shared.DataTypes.RuntimeData;

namespace Somewhere2.CLIApplication
{
    internal partial class CommandHandler
    {
        #region Construction
        public CommandHandler(RuntimeContext runtimeContext)
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
            RuntimeContext = runtimeContext;
            RuntimeData = runtimeContext.RuntimeData;
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
                if(!string.IsNullOrWhiteSpace(input))
                    PreprocessInput(input);
            }
        }
        #endregion

        #region States
        public bool ShouldExit { get; set; }
        /// <summary>
        /// Notice that CurrentWorkingDirectory is strictly a command-line concept and should not be part of the
        /// application's RuntimeData
        /// </summary>
        public string CurrentWorkingDirectory { get; set; }
        public RuntimeContext RuntimeContext { get; set; }
        public RuntimeData RuntimeData { get; set; }
        #endregion
    }
}