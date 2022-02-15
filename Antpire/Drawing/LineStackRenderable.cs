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
    internal class LineStackRenderable : IRenderable {
        public Vector2[] Segments;
        public Color Color = Color.White;
        public float Thickness = 2.0f;

        public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { X = (int)trans.Position.X, Y = (int)trans.Position.Y, Height = 2, Width = 2}))
                Segments.Chunk(2).ForEach(p => spriteBatch.DrawLine(p[0] + trans.Position, p[1] + trans.Position, Color, Thickness));
        }
    }
}
