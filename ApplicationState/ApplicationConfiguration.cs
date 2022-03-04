using NotImplementedException = System.NotImplementedException;

namespace Somewhere2.ApplicationState
{
    public class ApplicationConfiguration
    {
        public bool AlwaysLaunchInCLI { get; set; } = true;
        
        public void InitializeDefault()
        {
        }
    }
}