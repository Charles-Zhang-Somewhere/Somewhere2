using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Somewhere2.ApplicationState;
using Somewhere2.BaseClasses;
using Somewhere2.GUIApplication.Controls;

namespace Somewhere2.GUIApplication.ToolWindows
{
    public class ScratchPad
    {
        #region Interface
        public ScratchPad(RuntimeContext appState)
        {
            RuntimeContext = appState;
            
            InitializeWindow();
            InitializeEventHandlers();
            InitializeControls();
        }
        public void Run()
        {
            while (AppWindow.IsOpen)
            {
                AppWindow.Clear();
                DrawContents();
                AppWindow.Display();
                AppWindow.WaitAndDispatchEvents();
            }
        }
        #endregion

        #region Configurations
        const string WindowTitle = "Scrachpad";
        #endregion

        #region Members
        private RuntimeContext RuntimeContext { get; }
        private RenderWindow AppWindow { get; set; }
        private List<Control> Controls { get; set; }
        #endregion

        #region States
        #endregion

        #region Private
        private void InitializeWindow()
        {
            AppWindow = new RenderWindow(new VideoMode(512, 512), WindowTitle, Styles.Default);
            if (RuntimeContext.RenderingContext == null)
                RuntimeContext.InitializeRenderingContext();
        }
        private void InitializeEventHandlers()
        {
            AppWindow.Closed += (sender, eventArgs) => AppWindow.Close();
            AppWindow.MouseButtonPressed += AppWindowOnMouseButtonPressed;
            AppWindow.MouseButtonReleased += AppWindowOnMouseButtonReleased;
            AppWindow.MouseMoved += AppWindowOnMouseMoved;
            AppWindow.KeyReleased += AppWindowOnKeyReleased;
        }

        private void AppWindowOnKeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Code == Keyboard.Key.V)
            {
                Console.WriteLine(Clipboard.Contents);
            }
        }

        private void InitializeControls()
        {
            Controls = new List<Control>();
            Controls.Add(new Button("Test TestJ"));

            foreach (Control control in Controls)
            {
                control.Initialize(RuntimeContext.RenderingContext);
            }
        }
        private void DrawContents()
        {
            // Draw current screen
            foreach (Control control in Controls)
            {
                control.Draw(AppWindow);
            }
        }
        #endregion

        #region Event Handlers
        private void AppWindowOnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
            }
        }
        private void AppWindowOnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void AppWindowOnMouseMoved(object sender, MouseMoveEventArgs e)
        {
        }
        #endregion
    }
}