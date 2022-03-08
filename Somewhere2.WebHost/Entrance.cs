using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Somewhere2.Shared;
using Somewhere2.SystemService;
using Somewhere2.WebHost;

namespace Somewhere.WebHost
{
    public static class Entrance
    {
        public static WebHostInfo SetupAndRunWebHost(ApplicationConfiguration configuration)
        {
            // Configure web host explicitly
            int port = configuration.ServerPort ?? NetworkHelper.FindFreeTcpPort();
            string hostAddress = $"http://{configuration.ServerAddress ?? "localhost"}:{port}";
            WebHostInfo webHostInfo = new WebHostInfo()
            {
                Port = port,
                Address = hostAddress,
                ShouldLog = configuration.ServerDebugPrint
            };

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
            
            return webHostInfo;
        }
    }
}