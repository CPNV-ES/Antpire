using System.Runtime.Serialization;
using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Newtonsoft.Json;

namespace Antpire.Drawing; 

[Serializable]
internal class RectangleRenderable : IRenderable {
    public Color Color = Color.White;
    public Polygon Polygon;
    public float Thickness = 1.0f;
    
    [JsonProperty]
    private Point boundingBox;

    public RectangleRenderable(Vector2 size, float rotation, Color color, float thickness = 1.0f) {
        Color = color;
        Polygon = new Polygon(new[] { 
            new Vector2(0, 0),
            new Vector2(size.X, 0),
            new Vector2(size.X, size.Y),
            new Vector2(0, size.Y),
        });
        Polygon.Rotate(rotation);
        this.Thickness = thickness;
        var br = Polygon.BoundingRectangle.ToRectangle();
        boundingBox = new(br.Width * 2, br.Height * 2); // TODO: Shouldn't be times two, but the BoundingRectangle can't account for vertices with negative positions. Should bring all vertices to have X and Y be positive.
    }

    [IgnoreDataMember]
    public Point BoundingBox => boundingBox;
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)Layer).FillPath(new SolidColorBrush(Color), Polygon.Vertices.Select(x => x + trans.Position + BoundingBox.ToVector2()/2).ToList());
    }
}