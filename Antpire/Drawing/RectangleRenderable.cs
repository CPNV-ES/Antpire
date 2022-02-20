using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing {
    internal class RectangleRenderable : IRenderable {
        public Color Color = Color.White;
        private Polygon polygon;
        private float thickness = 1.0f;
        private Rectangle boundingBox;

        public RectangleRenderable(Vector2 size, float rotation, Color color, float thickness = 1.0f) {
            Color = color;
            polygon = new Polygon(new[] { 
                new Vector2(0, 0),
                new Vector2(size.X, 0),
                new Vector2(size.X, size.Y),
                new Vector2(0, size.Y),
            });
            polygon.Rotate(rotation);
            this.thickness = thickness;
            boundingBox = polygon.BoundingRectangle.ToRectangle();
        }

        public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { Location = boundingBox.Location + trans.Position.ToPoint(), Size = boundingBox.Size }))
                spriteBatch.DrawPolygon(trans.Position, polygon, Color, thickness);
        }
    }
}
