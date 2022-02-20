using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;

namespace Antpire.Drawing; 

internal class PathRenderable : IRenderable {
    private Vector2[] segments;
    private Rectangle boundingBox = new();

    public Vector2[] Segments {
        get => segments;
        set {
            segments = value;
            boundingBox.X = (int)segments.OrderBy(x => x.X).First().X;
            boundingBox.Y = (int)segments.OrderBy(x =>x.Y).First().Y;
            boundingBox.Width = (int)segments.OrderBy(x => x.X).Last().X;
            boundingBox.Height = (int)segments.OrderBy(x => x.Y).Last().Y;
        }
    }
    public Color Color = Color.White;
    public float Thickness = 2.0f;

    public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
        if(viewRegion.Intersects(boundingBox))
            Segments.Window(2).ForEach(p => spriteBatch.DrawLine(p[0] + trans.Position, p[1] + trans.Position, Color, Thickness));
    }
}