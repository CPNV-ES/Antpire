using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Antpire.Drawing;

namespace Antpire.Drawing; 

internal interface IRenderable {
    public void Render(DrawBatch drawBatch, Transform2 transform, Rectangle viewRegion);
}