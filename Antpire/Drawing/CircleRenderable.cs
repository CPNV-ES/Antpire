using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal class CircleRenderable : IRenderable {
        public int Radius;
        public int Sides = 64;
        public Color Color = Color.White;
        public float Thickness = 1.0f;

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            if (viewRegion.Intersects(new Rectangle { X = position.X, Y = position.Y, Height = Radius, Width = Radius }))
                spriteBatch.DrawCircle(new CircleF { Position = position, Radius = Radius}, Sides, Color, Thickness);
        }
    }
}
