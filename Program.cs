using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Somewhere2.CLIApplication;
using Somewhere2.TUIApplication.Applet;
using Somewhere2;
using Somewhere2.ApplicationState;
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
            RuntimeData runtimeData = new RuntimeData()
            {
                STADispatcher = Dispatcher.CurrentDispatcher
            };
            
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true;

                new CommandHandler(runtimeData).Start();
            }).Start();
            
            var app = new WPFApplication.App(runtimeData);
            app.InitializeComponent();
            app.Run();
        }
    }
}