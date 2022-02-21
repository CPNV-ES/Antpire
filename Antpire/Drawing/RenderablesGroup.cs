using Microsoft.Xna.Framework.Graphics;
using MoreLinq;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class RenderablesGroup : IRenderable {
    public (IRenderable, Point)[] Children;

    public void Render(DrawBatch drawBatch, Transform2 trans, Rectangle viewRegion) {
        Children.ForEach(child => child.Item1.Render(drawBatch, new Transform2((trans.Position.ToPoint() + child.Item2).ToVector2()), viewRegion));
    }
}