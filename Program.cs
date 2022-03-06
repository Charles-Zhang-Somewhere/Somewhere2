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
using Somewhere2.ApplicationState;
using Somewhere2.SystemService;
using Somewhere2.WebHost;
using CommandHandler = Somewhere2.CLIApplication.CommandHandler;

namespace Somewhere2
{
    internal static class Program
    {
        [STAThreadAttribute]
        private static void Main(string[] args)
        {
            // Initialize application data
            RuntimeData runtimeData = new RuntimeData()
            {
                STADispatcher = Dispatcher.CurrentDispatcher
            };
            PrepareFileServices(runtimeData);
            
            CreateHybridHost(runtimeData);
        }
        
        private static void CreateHybridHost(RuntimeData runtimeData)
        {
            SetupAndRunWebHost(runtimeData);
            SetupAndRunCommandHandler(runtimeData);
            SetupAndRunWPFApplication(runtimeData); // Application will stall here as the main thread
        }

        #region Routines
        private static void PrepareFileServices(RuntimeData runtimeData)
        {
            runtimeData.Configuration = FileService.CheckConfigFile();
            runtimeData.Recents = FileService.CheckRecentFile();
        }
        private static void SetupAndRunWebHost(RuntimeData runtimeData)
        {
            // Configure web host explicitly
            int port = runtimeData.Configuration.ServerPort ?? NetworkHelper.FindFreeTcpPort();
            string hostAddress = $"http://{runtimeData.Configuration.ServerAddress ?? "localhost"}:{port}";
            WebHostInfo webHostInfo = new WebHostInfo()
            {
                Port = port,
                Address = hostAddress,
                ShouldLog = runtimeData.Configuration.ServerDebugPrint
            };
            runtimeData.WebHostInfo = webHostInfo;
            
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true;

                // Build host
                IHost host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls(webHostInfo.Address);
                        webBuilder.UseWebRoot("wwwroot");
                        webBuilder.UseKestrel();
                        webBuilder.UseStartup<Startup>();
                    })
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                        if (webHostInfo.ShouldLog)
                        {
                            logging.AddConsole();
                            logging.AddDebug();
                        }
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .Build();
                // Start host
                host.Run();
            }).Start();
        }
        private static void SetupAndRunCommandHandler(RuntimeData runtimeData)
        {
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true;

                new CommandHandler(runtimeData).Start();

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }).Start();
        }
        private static void SetupAndRunWPFApplication(RuntimeData runtimeData)
        {
            var app = new WPFApplication.App(runtimeData);
            app.InitializeComponent();
            app.Run();
        }
        #endregion
    }
}