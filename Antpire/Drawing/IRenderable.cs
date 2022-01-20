using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Antpire.Drawing {
    internal interface IRenderable {
        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion);
    }
}
