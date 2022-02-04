using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;

namespace Antpire.Drawing {
    internal class LineStackRenderable : IRenderable {
        public Vector2[] Segments;
        public Color Color = Color.White;
        public float Thickness = 2.0f;

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { X = position.X, Y = position.Y, Height = 2, Width = 2}))
                Segments.Chunk(2).ForEach(p => spriteBatch.DrawLine(p[0] + position.ToVector2(), p[1] + position.ToVector2(), Color, Thickness));
        }
    }
}
