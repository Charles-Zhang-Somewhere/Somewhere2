using SFML.Graphics;
using SFML.System;
using Somewhere2.BaseClasses;

namespace Somewhere2.GUIApplication.Controls
{
    public class Button: Control
    {
        public Button(string label)
        {
            Label = label;
        }

        #region Properties
        public string Label { get; set; }
        #endregion
        
        #region Components
        private Text Text { get; set; }
        private RectangleShape Shape { get; set; }
        #endregion

        #region Interface

        public override void Initialize(RenderingContext context)
        {
            Text = new Text(Label, context.BasicRendering.DefaultFont);
            Text.CharacterSize = 24;
            Text.FillColor = Color.Blue;
            FloatRect bounds = Text.GetGlobalBounds();
                
            Shape = new RectangleShape(new Vector2f(bounds.Width, Text.CharacterSize));
        }

        public override void Draw(RenderWindow owner)
        {
            owner.Draw(Shape);
            owner.Draw(Text);
        }
        #endregion
    }
}