using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Graphics2D.Brushes;

namespace Antpire.Drawing; 

[Serializable]
internal class CircleRenderable : IRenderable {
    public int Radius;
    public int Sides = 64;
    
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

    public Point BoundingBox => new Point(Radius*2, Radius*2); 
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)Layer).FillCircle(brush, trans.Position, Radius, 64);
    }
}