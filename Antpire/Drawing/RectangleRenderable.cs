using LilyPath;
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
        boundingBox = new(br.Width * 2, br.Height * 2); // TODO: Shouldn't be times two, but the BoundingRectangle can't account for vertices with negative positions. Should bring all vertices to have X and Y be positive.
    }

    public Point BoundingBox => boundingBox;
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)Layer).FillPath(new SolidColorBrush(Color), polygon.Vertices.Select(x => x + trans.Position + BoundingBox.ToVector2()/2).ToList());
    }
}