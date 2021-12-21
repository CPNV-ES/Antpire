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
    internal class Aphid {
        private Texture2D texture;
        private Vector2 position;
        public Aphid(Vector2 position) {
            texture = Content.Load<Texture2D>("aphid");
            this.position = position;
        }

        public void Render(SpriteBatch spriteBatch, Vector2 position) {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
