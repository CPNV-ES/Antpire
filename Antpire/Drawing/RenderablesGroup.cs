using Microsoft.Xna.Framework.Graphics;
using MoreLinq;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class RenderablesGroup : IRenderable {
    public (IRenderable, Point)[] Children;

    public Point BoundingBox {
        // TODO: Cache this, this property is accessed each render frame and this is therefore evaluated each frame.
        get {
            var xs = Children.OrderBy(x => x.Item1.BoundingBox.X);
            var ys = Children.OrderBy(x => x.Item1.BoundingBox.Y);
            return new Point(
                xs.First().Item1.BoundingBox.X + xs.Last().Item1.BoundingBox.X,
                ys.First().Item1.BoundingBox.Y + ys.Last().Item1.BoundingBox.Y
            );
        }
    }
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        Children.ForEach(child => child.Item1.Render(drawBatch, new Transform2((trans.Position.ToPoint() + child.Item2).ToVector2())));
    }
}