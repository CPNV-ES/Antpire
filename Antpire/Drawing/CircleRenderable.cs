using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal class CircleRenderable : IRenderable {
        public int Radius;
        public int Sides = 64;
        public Color Color = Color.White;
        public float Thickness = 1.0f;

        public void Render(SpriteBatch spriteBatch, Point position) {
            spriteBatch.DrawCircle(new CircleF { Position = position, Radius = Radius}, Sides, Color, Thickness);
        }
    }
}
