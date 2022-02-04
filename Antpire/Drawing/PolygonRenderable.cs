using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing {
    internal class PolygonRenderable : IRenderable {
        private Polygon polygon;
        private Rectangle boundingBox;

        public Polygon Polygon { 
            get => polygon; 
            set {
                polygon = value;
                boundingBox = Polygon.BoundingRectangle.ToRectangle();
            }
        }
        public Color Color = Color.White;
        public float Thickness = 1.0f;

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { Location = boundingBox.Location + position, Size = boundingBox.Size }))
                spriteBatch.DrawPolygon(new Vector2(position.X, position.Y), Polygon, Color, Thickness);
        }
    }
}
