using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace Antpire.Drawing {
    internal class RenderablesGroup : IRenderable {
        public (IRenderable, Point)[] Children;

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            Children.ForEach(child => child.Item1.Render(spriteBatch, position + child.Item2, viewRegion));
        }
    }
}
