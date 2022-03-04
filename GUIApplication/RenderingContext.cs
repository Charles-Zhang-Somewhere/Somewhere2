using SFML.Graphics;

namespace Somewhere2.GUIApplication
{
    public class RenderingContext
    {
        #region Rendering Windows
        public RenderWindow MainWindow { get; set; }
        #endregion

        #region Rendering Resources
        public BasicRenderingInfrastructure BasicRendering { get; set; }
        #endregion
    }
}