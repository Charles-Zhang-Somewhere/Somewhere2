using System.IO;
using SFML.Graphics;
using Somewhere2.Shared;

namespace Somewhere2.GUIApplication
{
    public class BasicRenderingInfrastructure
    {
        private Stream DefaultFontAsset { get; set; }
        
        public Font DefaultFont { get; set; }
        
        public static BasicRenderingInfrastructure Setup()
        {
            var fontAsset = Helpers.ReadBinaryResource("Somewhere2.Assets.Fonts.Roboto.Roboto-Regular.ttf");
            var font = new Font(fontAsset);
            
            BasicRenderingInfrastructure infrastructure = new BasicRenderingInfrastructure()
            {
                DefaultFontAsset = fontAsset,
                DefaultFont = font
            };
            return infrastructure;
        }
    }
}