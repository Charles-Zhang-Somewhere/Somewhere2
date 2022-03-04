using System;
using System.IO;
using Somewhere2.CLIApplication;
using Somewhere2.TUIApplication.Applet;
using Terminal.Gui;
using CommandHandler = Somewhere2.CLIApplication.CommandHandler;

namespace Somewhere2
{
    internal static class Program
    {
        private static void Main(string[] args)
            => new CommandHandler().Start();
    }
}