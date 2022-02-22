using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing; 

internal class PolygonRenderable : IRenderable {
    private Polygon polygon;

    public Polygon Polygon { 
        get => polygon; 
        set {
            polygon = value;
            var br = Polygon.BoundingRectangle.ToRectangle();
            BoundingBox = new(br.Width, br.Height);
        }
    }
    public Color Color = Color.White;
    public float Thickness = 1.0f;

    public Point BoundingBox { get; private set; }
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).DrawPolygon(trans.Position, Polygon, Color, Thickness);
    }
}