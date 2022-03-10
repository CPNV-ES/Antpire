using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;

namespace Antpire.Drawing; 

[Serializable]
internal class LineStackRenderable : IRenderable {
    public Vector2[] Segments;
    public Color Color = Color.White;
    public float Thickness = 2.0f;
    
    public Point BoundingBox => new(2, 2);   // TODO: Make this actual bounding box 
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        Segments.Chunk(2).ForEach(p => drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).DrawLine(p[0] + trans.Position, p[1] + trans.Position, Color, Thickness));
    }
}