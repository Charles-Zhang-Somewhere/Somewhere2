using System;
using System.Windows.Threading;
using Somewhere2.GUIApplication;
using Somewhere2.Shared;
using Somewhere2.Shared.DataTypes;

namespace Somewhere2.ApplicationState
{
    public class RuntimeContext
    {
        #region Constructor
        public RuntimeContext()
        {
            if (Singleton == null)
                Singleton = this;
            else
            {
                throw new InvalidOperationException("RuntimeData is already initialized! Singleton is not null.");
            }

            RuntimeData = new RuntimeData();
        }
        #endregion
        
        #region Global Contexts
        public Shared.DataTypes.RuntimeData RuntimeData { get; set; } 
        public RenderingContext RenderingContext { get; set; }
        public MainApplication MainGUIApplication { get; set; }
        public Dispatcher STADispatcher { get; set; }
        public static RuntimeContext Singleton { get; set; }
        #endregion

        #region Interface
        public void InitializeRenderingContext()
        {
            RenderingContext = new RenderingContext()
            {
                MainWindow = null,
                BasicRendering = BasicRenderingInfrastructure.Setup()
            };
        }
        #endregion
    }
}