using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class CircleRenderable : IRenderable {
    public int Radius;
    public int Sides = 64;
    public Color Color = Color.White;
    public float Thickness = 1.0f;


    public void Render(DrawBatch drawBatch, Transform2 trans, Rectangle viewRegion) {
        if (viewRegion.Intersects(new Rectangle { X = (int)trans.Position.X - Radius, Y = (int)trans.Position.Y - Radius, Height = Radius * 2, Width = Radius * 2 }))
            drawBatch.GetSpriteDrawBatch(0).DrawCircle(new CircleF { Position = trans.Position, Radius = Radius}, Sides, Color, Thickness);
    }
    }
}