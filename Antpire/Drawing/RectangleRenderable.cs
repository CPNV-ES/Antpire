using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing {
    internal class RectangleRenderable : IRenderable {
        public Color Color = Color.White;
        private Polygon polygon;
        private float Thickness = 1.0f;
        private Rectangle boundingBox;

        public RectangleRenderable(Vector2 size, float rotation, Color color, float thickness = 1.0f) {
            Color = color;
            polygon = new Polygon(new Vector2[4] { 
                new Vector2(0, 0),
                new Vector2(size.X, 0),
                new Vector2(size.X, size.Y),
                new Vector2(0, size.Y),
            });
            polygon.Rotate(rotation);
            Thickness = thickness;
            boundingBox = polygon.BoundingRectangle.ToRectangle();
        }

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { Location = boundingBox.Location + position, Size = boundingBox.Size }))
                spriteBatch.DrawPolygon(new Vector2(position.X, position.Y), polygon, Color, Thickness);
        }
    }
}
