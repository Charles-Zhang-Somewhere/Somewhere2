using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Somewhere2.BaseClasses;
using Somewhere2.Controls;

namespace Somewhere2.ApplicationState
{
    public class Application
    {
        #region Interface
        public Application()
        {
            InitializeWindow();
            InitializeWindowHandlers();
            InitializeDrawContexts();
            PlayIntro();
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
        const string WindowTitle = "Somewhere 2 - Simpler & Better";
        #endregion

        #region Members
        private ApplicationContext ApplicationContext { get; set; } 
        private RenderWindow AppWindow { get; set; }
        private List<Control> Controls { get; set; }
        #endregion

        #region States
        private Vector2i MoveWindowAnchor { get; set; }
        private bool MoveWindow { get; set; }
        #endregion

        #region Private
        private void InitializeWindow()
        {
            AppWindow = new RenderWindow(new VideoMode(1024, 768), WindowTitle, Styles.None);
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
            AppWindow.MouseButtonReleased += AppWindowOnMouseButtonReleased;
            AppWindow.MouseMoved += AppWindowOnMouseMoved;
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
        private void PlayIntro()
        {
            SoundBuffer buffer = new SoundBuffer(Helpers.ReadBinaryResource("Somewhere2.Assets.Sounds.Intro.wav"));
            Sound sound = new Sound(buffer);
            sound.Play();
        }
        private void DrawContents()
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
                MoveWindowAnchor = new Vector2i(e.X, e.Y);
                MoveWindow = true;
            }
        }
        private void AppWindowOnMouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            if (MoveWindow) MoveWindow = false;
        }

        private void AppWindowOnMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            if (MoveWindow)
            {
                Vector2i current = new Vector2i(e.X, e.Y);
                AppWindow.Position = AppWindow.Position + (current - MoveWindowAnchor);
            } 
        }
        #endregion
    }
}