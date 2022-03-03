using SFML.Graphics;

namespace Somewhere2.GUIApplication
{
    public class RenderingContext
    {
        public Application MainApplication { get; set; }
        public RenderWindow Window { get; set; }
        public BasicRenderingInfrastructure BasicRendering { get; set; }
    }
}