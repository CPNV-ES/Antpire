using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;

namespace Antpire.Drawing {
    internal class PathRenderable : IRenderable {
        private Vector2[] segments;
        private Rectangle boundingBox = new();

        public Vector2[] Segments {
            get { return segments; }
            set {
                segments = value;
                boundingBox.X = (int)segments.OrderBy(x => x.X).First().X;
                boundingBox.Y = (int)segments.OrderBy(x =>x.Y).First().Y;
                boundingBox.Width = (int)segments.OrderBy(x => x.X).Last().X;
                boundingBox.Height = (int)segments.OrderBy(x => x.Y).Last().Y;
            }
        }
        public Color Color = Color.White;
        public float Thickness = 2.0f;

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            if(viewRegion.Intersects(boundingBox))
                Segments.Window(2).ForEach(p => spriteBatch.DrawLine(p[0] + position.ToVector2(), p[1] + position.ToVector2(), Color, Thickness));
        }
    }
}
