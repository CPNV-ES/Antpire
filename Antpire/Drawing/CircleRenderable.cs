using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class CircleRenderable : IRenderable {
    public int Radius;
    public int Sides = 64;
    public Color Color = Color.White;
    public float Thickness = 1.0f;

    public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
        if (viewRegion.Intersects(new Rectangle { X = (int)trans.Position.X, Y = (int)trans.Position.Y, Height = Radius, Width = Radius }))
            spriteBatch.DrawCircle(new CircleF { Position = trans.Position, Radius = Radius}, Sides, Color, Thickness);
    }
}