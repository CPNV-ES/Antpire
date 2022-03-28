using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Graphics2D.Brushes;

namespace Antpire.Drawing; 

[Serializable]
internal class CircleRenderable : IRenderable {
    public int Radius;
    public int Sides = 64;
    public Color Color = Color.White;
    public float Thickness = 1.0f;

    public Point BoundingBox => new Point(Radius*2, Radius*2); 
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)Layer).FillCircle(new SolidColorBrush(Color), trans.Position, Radius, 64);
    }
}