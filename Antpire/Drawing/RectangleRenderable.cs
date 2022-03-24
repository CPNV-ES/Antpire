using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing; 

internal class RectangleRenderable : PolygonRenderable {
    public RectangleRenderable(Vector2 size, float rotation, Color color, float thickness = 1.0f) : base() {
        Color = color;
        var polygon = new Polygon(new[] { 
            new Vector2(0, 0),
            new Vector2(size.X, 0),
            new Vector2(size.X, size.Y),
            new Vector2(0, size.Y),
        });
        
        polygon.Rotate(rotation);

        base.Polygon = new Polygon(polygon.Vertices.Reverse());
    }
}