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
    public Color Color = Color.White;
    public float Thickness = 1.0f;

    public Point BoundingBox { get; private set; }
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)Layer).FillPath(new SolidColorBrush(Color), Polygon.Vertices.Reverse().ToList().Select(x => x + trans.Position).ToList()); 
    }
}