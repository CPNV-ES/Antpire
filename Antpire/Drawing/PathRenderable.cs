using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;

namespace Antpire.Drawing {
    internal class PathRenderable : IRenderable {
        public Vector2[] Segments;
        public Color Color = Color.White;
        public float Thickness = 2.0f;

        public void Render(SpriteBatch spriteBatch, Point position) {
            Segments.Window(2).ForEach(p => spriteBatch.DrawLine(p[0], p[1], Color, Thickness));
        }
    }
}
