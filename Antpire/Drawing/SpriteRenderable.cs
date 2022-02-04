using Microsoft.Xna.Framework.Graphics;

namespace Antpire.Drawing {
    internal class SpriteRenderable : IRenderable{
        private Texture2D texture;
        private int size;

        public SpriteRenderable(int size, Texture2D texture) {
            this.texture = texture;
            this.size = size;
        }

        public void Render(SpriteBatch spriteBatch, Point pos, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { Location = pos, Width = size, Height = size }))
                spriteBatch.Draw(texture, new Rectangle(pos, new Point(size, size)), Color.White);
        }
    }
}
