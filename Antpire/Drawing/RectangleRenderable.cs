using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing; 

internal class RectangleRenderable : IRenderable {
    public Color Color = Color.White;
    private Polygon polygon;
    private float thickness = 1.0f;
    private Point boundingBox;

    public RectangleRenderable(Vector2 size, float rotation, Color color, float thickness = 1.0f) {
        Color = color;
        polygon = new Polygon(new[] { 
            new Vector2(0, 0),
            new Vector2(size.X, 0),
            new Vector2(size.X, size.Y),
            new Vector2(0, size.Y),
        });
        polygon.Rotate(rotation);
        this.thickness = thickness;
        var br = polygon.BoundingRectangle.ToRectangle();
        boundingBox = new(br.Width, br.Height);
    }

    public Point BoundingBox => boundingBox;
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).DrawPolygon(trans.Position, polygon, Color, thickness);
    }
}