using NotImplementedException = System.NotImplementedException;

namespace Somewhere2.ApplicationState
{
    public class ApplicationConfiguration
    {
        public bool AlwaysLaunchInCLI { get; set; } = true;

        public bool ServerPrintLog { get; set; } = false;
        public int ServerPort { get; set; } = 5001;
        public string ServerAddress { get; set; } = "localhost";
        
        public void InitializeDefault()
        {
        }
    }
}