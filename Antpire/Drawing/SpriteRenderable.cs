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
    internal class SpriteRenderable : IRenderable{
        private Texture2D texture;
        private int size;

        public SpriteRenderable(int size, Texture2D texture) {
            this.texture = texture;
            this.size = size;
        }

        public void Render(SpriteBatch spriteBatch, Point pos) {
            spriteBatch.Draw(texture, new Rectangle(pos, new Point(size, size)), Color.White);
        }
    }
}
