using System.IO;
using SFML.Graphics;

namespace Somewhere2.GUIApplication
{
    public class BasicRenderingInfrastructure
    {
        private Stream DefaultFontAsset { get; set; }
        
        public Font DefaultFont { get; set; }
        
        public void Setup(RenderWindow window)
        {
            DefaultFontAsset = Helpers.ReadBinaryResource("Somewhere2.Assets.Fonts.Roboto.Roboto-Regular.ttf");
            DefaultFont = new Font(DefaultFontAsset);
        }
    }
}