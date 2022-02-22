using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;

namespace Antpire.Drawing; 

internal class PathRenderable : IRenderable {
    private Vector2[] segments;
    private Point boundingBox = new();

    public Vector2[] Segments {
        get => segments;
        set {
            segments = value;
            boundingBox.X = Math.Abs((int)segments.OrderBy(x => x.X).Last().X) + Math.Abs((int)segments.OrderBy(x => x.X).First().X);
            boundingBox.Y = Math.Abs((int)segments.OrderBy(x => x.Y).Last().Y) + Math.Abs((int)segments.OrderBy(x => x.Y).First().Y);
        }
    }
    public Color Color = Color.White;
    public float Thickness = 2.0f;

    public Point BoundingBox => boundingBox;
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        Segments.Window(2).ForEach(p => drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).DrawLine(p[0] + trans.Position, p[1] + trans.Position, Color, Thickness));
    }
}