using System.Windows;
using Somewhere2.ApplicationState;
using RuntimeData = Somewhere2.Shared.DataTypes.RuntimeData;

namespace Somewhere2.WPFApplication
{
    public partial class App : Application
    {
        public App(RuntimeContext runtimeContext)
            => RuntimeContext = runtimeContext;

        private RuntimeContext RuntimeContext { get; }
    }
}