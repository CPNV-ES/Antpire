using Microsoft.Xna.Framework.Graphics;

namespace Antpire.Drawing {
    internal interface IRenderable {
        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion);
    }
}
