using System.Collections.Generic;
using SFML.Graphics;
using Somewhere2.ApplicationState;

namespace Somewhere2.BaseClasses
{
    public abstract class Control
    {
        public Control()
        {
            Children = new List<Control>();
        }
        public abstract void Initialize(ApplicationContext context);
        public abstract void Draw(ApplicationContext context);

        protected List<Control> Children { get; }
    }
}