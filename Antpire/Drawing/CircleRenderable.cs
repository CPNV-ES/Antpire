using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class CircleRenderable : IRenderable {
    public int Radius;
    public int Sides = 64;
    public Color Color = Color.White;
    public float Thickness = 1.0f;

    public Point BoundingBox => new Point(Radius * 2, Radius * 2); 
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).DrawCircle(new CircleF { Position = trans.Position, Radius = Radius}, Sides, Color, Thickness);
    }
}