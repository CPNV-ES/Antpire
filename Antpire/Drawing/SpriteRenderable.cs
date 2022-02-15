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
        private Point size;

        public SpriteRenderable(int size, Texture2D texture) {
            this.texture = texture;
            this.size = new Point(size, size);
        }

        public SpriteRenderable(Point size, Texture2D texture)
        {
            this.texture = texture;
            this.size = size;
        }

        public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { Location = trans.Position.ToPoint(), Width = size.X, Height = size.Y }))
            {
                if (trans.Rotation != 0) {
                    Vector2 location = trans.Position;
                    Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                    Vector2 origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
                    float angle = trans.Rotation;

                    spriteBatch.Draw(texture, location, sourceRectangle, Color.White, angle+MathF.PI/2, origin, 0.5f, SpriteEffects.None, 1);
                }
                else {
                    spriteBatch.Draw(texture, new Rectangle(trans.Position.ToPoint(), size), Color.White);
                }
            }
        }
    }
}
