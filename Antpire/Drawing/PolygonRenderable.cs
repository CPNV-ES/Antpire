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
    internal class PolygonRenderable : IRenderable {
        public Polygon Polygon;
        public Color Color = Color.White;

        public void Render(SpriteBatch spriteBatch, Point position) {
            spriteBatch.DrawPolygon(new Vector2(position.X, position.Y), Polygon, Color);
        }
    }
}
