namespace Somewhere2.Shared
{
    public class ApplicationConfiguration
    {
        public bool AlwaysLaunchInCLI { get; set; } = true;

        public bool ServerDebugPrint { get; set; } = false;
        public int? ServerPort { get; set; } = 5001;
        public string ServerAddress { get; set; } = "localhost";
        
        public void InitializeDefault()
        {
        }
    }
}