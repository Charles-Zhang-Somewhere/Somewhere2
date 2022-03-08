using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Somewhere.WebHost;
using Somewhere2.ApplicationState;
using Somewhere2.Shared;
using Somewhere2.Shared.SystemService;
using Somewhere2.SystemService;
using Somewhere2.WebHost;
using CommandHandler = Somewhere2.CLIApplication.CommandHandler;
using RuntimeData = Somewhere2.Shared.DataTypes.RuntimeData;

namespace Somewhere2
{
    internal static class Program
    {
        [STAThreadAttribute]
        private static void Main(string[] args)
        {
            // Initialize application data
            RuntimeContext runtimeContext = new RuntimeContext()
            {
                STADispatcher = Dispatcher.CurrentDispatcher
            };
            PrepareFileServices(runtimeContext.RuntimeData);
            
            CreateHybridHost(runtimeContext);
        }
        
        private static void CreateHybridHost(RuntimeContext runtimeContext)
        {
            runtimeContext.RuntimeData.WebHostInfo = Entrance.SetupAndRunWebHost(runtimeContext.RuntimeData.Configuration);
            SetupAndRunCommandHandler(runtimeContext);
            SetupAndRunWPFApplication(runtimeContext); // Application will stall here as the main thread
        }

        #region Routines
        private static void PrepareFileServices(RuntimeData runtimeData)
        {
            runtimeData.Configuration = FileService.CheckConfigFile();
            runtimeData.Recents = FileService.CheckRecentFile();
        }
        private static void SetupAndRunCommandHandler(RuntimeContext runtimeContext)
        {
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true;

                new CommandHandler(runtimeContext).Start();

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }).Start();
        }
        private static void SetupAndRunWPFApplication(RuntimeContext runtimeContext)
        {
            var app = new WPFApplication.App(runtimeContext);
            app.InitializeComponent();
            app.Run();
        }
        #endregion
    }
}