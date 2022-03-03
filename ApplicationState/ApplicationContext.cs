using SFML.Graphics;

namespace Somewhere2.ApplicationState
{
    public class ApplicationContext
    {
        public ApplicationWindow MainApplication { get; set; }
        public RenderWindow Window { get; set; }
        public BasicRenderingInfrastructure BasicRendering { get; set; }
    }
}