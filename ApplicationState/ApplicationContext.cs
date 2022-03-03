using SFML.Graphics;

namespace Somewhere2.ApplicationState
{
    public class ApplicationContext
    {
        public Application MainApplication { get; set; }
        public RenderWindow Window { get; set; }
        public BasicRenderingInfrastructure BasicRendering { get; set; }
    }
}