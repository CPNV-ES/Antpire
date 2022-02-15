using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal interface IRenderable {
        public void Render(SpriteBatch spriteBatch, Transform2 transform, Rectangle viewRegion);
    }
}
