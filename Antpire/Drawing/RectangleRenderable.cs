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

        public RectangleRenderable(Vector2 size, float rotation, Color color) {
            Color = color;
            polygon = new Polygon(new Vector2[4] { 
                new Vector2(0, 0),
                new Vector2(size.X, 0),
                new Vector2(size.X, size.Y),
                new Vector2(0, size.Y),
            });
            polygon.Rotate(rotation);
        }

        public void Render(SpriteBatch spriteBatch, Point position) {
            spriteBatch.DrawPolygon(new Vector2(position.X, position.Y), polygon, Color);
        }
    }
}
