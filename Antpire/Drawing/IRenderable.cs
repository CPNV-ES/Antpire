using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Antpire.Drawing;

namespace Antpire.Drawing; 

internal interface IRenderable {
    public Point BoundingBox { get; }
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans);
}