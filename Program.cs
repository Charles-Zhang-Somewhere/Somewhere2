using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Somewhere2.CLIApplication;
using Somewhere2.TUIApplication.Applet;
using Somewhere2;
using Somewhere2.ApplicationState;
using Somewhere2.SystemService;
using Somewhere2.WebHost;
using Somewhere2.WPFApplication;
using Terminal.Gui;
using CommandHandler = Somewhere2.CLIApplication.CommandHandler;

namespace Somewhere2
{
    internal static class Program
    {
        [STAThreadAttribute]
        private static void Main(string[] args)
        {
            CreateHybridHost();
        }
        
        private static void CreateHybridHost()
        {
            void OpenBrowser()
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(RuntimeData.Singleton.WebHostInfo.Address)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
            
            // Configure web host explicitly
            int port = NetworkHelper.FindFreeTcpPort();
            string hostAddress = $"http://localhost:{port}";
            bool shouldLog = true;
            WebHostInfo webHostInfo = new WebHostInfo()
            {
                Port = port,
                Address = hostAddress,
                ShouldLog = shouldLog,
                URL = hostAddress
            };
            // Initialize application data
            RuntimeData runtimeData = new RuntimeData()
            {
                STADispatcher = Dispatcher.CurrentDispatcher,
                WebHostInfo = webHostInfo
            };

            SetupAndRunWebHost(webHostInfo);
            SetupAndRunCommandHandler(runtimeData);
            SetupAndRunWPFApplication(runtimeData); // Application will stall here as the main thread
        }

        #region Routines
        private static void SetupAndRunWebHost(WebHostInfo webHostInfo)
        {
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