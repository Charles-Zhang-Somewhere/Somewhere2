using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Somewhere2.BaseClasses;
using Somewhere2.Controls;

namespace Somewhere2.ApplicationState
{
    public class ApplicationWindow
    {
        #region Interface
        public ApplicationWindow()
        {
            InitializeWindow();
            InitializeWindowHandlers();
            InitializeDrawContexts();
        }
        public void Run()
        {
            while (AppWindow.IsOpen)
            {
                AppWindow.Clear();
                Draw();
                AppWindow.Display();
                AppWindow.WaitAndDispatchEvents();
            }
        }
        #endregion

        #region Configurations
        const string WindowTitle = "Somewhere 2 - Simpler & Better";
        #endregion

        #region Members
        private ApplicationContext ApplicationContext { get; set; } 
        private RenderWindow AppWindow { get; set; }
        private List<Control> Controls { get; set; }
        #endregion

        #region Private
        private void InitializeWindow()
        {
            AppWindow = new RenderWindow(VideoMode.DesktopMode, WindowTitle);
            BasicRenderingInfrastructure rendering = new BasicRenderingInfrastructure();
            rendering.Setup(AppWindow);

            ApplicationContext = new ApplicationContext()
            {
                MainApplication = this,
                Window = AppWindow,
                BasicRendering = rendering
            };
        }
        private void InitializeWindowHandlers()
        {
            AppWindow.Closed += (sender, eventArgs) => AppWindow.Close();
            AppWindow.MouseButtonPressed += AppWindowOnMouseButtonPressed;
        }
        private void InitializeDrawContexts()
        {
            Controls = new List<Control>();
            Controls.Add(new Button("Test TestJ"));

            foreach (Control control in Controls)
            {
                control.Initialize(ApplicationContext);
            }
        }
        private void Draw()
        {
            // Draw current screen
            foreach (Control control in Controls)
            {
                control.Draw(ApplicationContext);
            }
        }
        #endregion

        #region Event Handlers
        private void AppWindowOnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                
            }
        }
        #endregion
    }
}