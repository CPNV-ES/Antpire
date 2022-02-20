using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal interface IRenderable {
        public void Render(SpriteBatch spriteBatch, Transform2 transform, Rectangle viewRegion);
    }
}
