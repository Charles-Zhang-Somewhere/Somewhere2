using System.Collections.Generic;
using SFML.Graphics;
using Somewhere2.ApplicationState;
using Somewhere2.GUIApplication;

namespace Somewhere2.BaseClasses
{
    public abstract class Control
    {
        public Control()
        {
            Children = new List<Control>();
        }
        public abstract void Initialize(RenderingContext context);
        public abstract void Draw(RenderWindow owner);
        
        protected List<Control> Children { get; }
    }
}