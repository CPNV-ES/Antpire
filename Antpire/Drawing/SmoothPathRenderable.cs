using System.Runtime.Serialization;
using Antpire.Utils;
using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MoreLinq;
using Newtonsoft.Json;

namespace Antpire.Drawing; 

internal class SmoothPathRenderable : IRenderable {
    [JsonProperty]
    private Vector2[] segments;
    [JsonProperty]
    private Vector2[] bezierSegments;
    [JsonProperty]
    private Point boundingBox = new();

    [IgnoreDataMember]
    public Vector2[] Segments {
        get => segments;
        set {
            segments = value;
            bezierSegments = ShapeUtils.GetBezierApproximation(segments, segments.Length * 4);
            boundingBox.X = int.MaxValue;   // TODO: Quick hack, shouldn't impact perfs too much but it's not very nice 
            boundingBox.Y = int.MaxValue;
        }
    }
    public Vector2[] BezierSegments => bezierSegments;
    
    private Color color = Color.White;
    public Color Color {
        get => color;
        set {
            pen = new Pen(value, Thickness);
            color = value;
        }
    }
    private Pen pen;
    public float Thickness = 100.0f;

    [IgnoreDataMember]
    public Point BoundingBox => boundingBox;
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)0).DrawBeziers(pen, bezierSegments, BezierType.Quadratic);
    }
}