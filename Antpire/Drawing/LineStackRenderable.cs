﻿using System;
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

        public void Render(SpriteBatch spriteBatch, Point position) {
            Segments.Chunk(2).ForEach(p => spriteBatch.DrawLine(p[0] + position.ToVector2(), p[1] + position.ToVector2(), Color, Thickness));
        }
    }
}