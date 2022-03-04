using System.Windows;
using Somewhere2.ApplicationState;

namespace Somewhere2.WPFApplication
{
    public partial class App : Application
    {
        public App(RuntimeData runtimeData)
            => RuntimeData = runtimeData;

        private RuntimeData RuntimeData { get; }
    }
}