using Somewhere2.CLIApplication;
using CommandHandler = Somewhere2.CLIApplication.CommandHandler;

namespace Somewhere2
{
    internal static class Program
    {
        private static void Main(string[] args)
            => new CommandHandler().Start();
    }
}