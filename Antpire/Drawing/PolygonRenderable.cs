using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Newtonsoft.Json;

namespace Antpire.Drawing; 

[Serializable]
internal class PolygonRenderable : IRenderable {
    [JsonProperty]
    private Polygon polygon;

    public Polygon Polygon { 
        get => polygon; 
        set {
            polygon = value;
            var br = Polygon.BoundingRectangle.ToRectangle();
            BoundingBox = new(br.Width, br.Height);
        }
    }
    
    private Color color = Color.White;
    public Color Color {
        get => color;
        set {
            brush = new SolidColorBrush(value);
            color = value;
        }
    }
    private SolidColorBrush brush;
    
    public float Thickness = 1.0f;

    public Point BoundingBox { get; private set; }
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)Layer).FillPath(brush, Polygon.Vertices.Reverse().ToList().Select(x => x + trans.Position - BoundingBox.ToVector2() / 2).ToList()); 
    }
}