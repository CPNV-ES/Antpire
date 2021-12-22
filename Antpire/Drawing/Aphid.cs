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
    internal class Aphid : IRenderable{
        private Texture2D texture;
        private Vector2 position;
        public Aphid(Vector2 position, Microsoft.Xna.Framework.Content.ContentManager content) {
            texture = content.Load<Texture2D>("AphidAliveDemo");
            this.position = position;
        }

        public void Render(SpriteBatch spriteBatch, Point pos) {
            spriteBatch.Draw(texture, new Vector2(pos.X, pos.Y), Color.White);
        }

    }
}
