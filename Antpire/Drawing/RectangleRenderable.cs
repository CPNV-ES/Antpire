using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal class RectangleRenderable : IRenderable {
        public Color Color = Color.White;
        private Vector2[] points;

        public RectangleRenderable(Vector2 size, float rotation, Color color) {
            Color = color;
            points = new Vector2[4];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(size.X, 0);
            points[2] = new Vector2(size.X, size.Y);
            points[3] = new Vector2(0, size.Y);

            for(int i = 0; i< 4; i++) {
                points[i].X = (float)(points[i].X * Math.Cos(rotation) - points[i].Y * Math.Sin(rotation));
                points[i].Y = (float)(points[i].X * Math.Sin(rotation) + points[i].Y * Math.Cos(rotation));
            }
        }

        public void Render(SpriteBatch spriteBatch, Point position) {
            spriteBatch.DrawPolygon(new Vector2(position.X, position.Y), points, Color);
        }
    }
}
