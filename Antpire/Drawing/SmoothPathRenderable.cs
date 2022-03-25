﻿using System.Runtime.Serialization;
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
            boundingBox.X = Math.Abs((int)segments.OrderBy(x => x.X).Last().X) + Math.Abs((int)segments.OrderBy(x => x.X).First().X);
            boundingBox.Y = Math.Abs((int)segments.OrderBy(x => x.Y).Last().Y) + Math.Abs((int)segments.OrderBy(x => x.Y).First().Y);
        }
    }
    public Vector2[] BezierSegments => bezierSegments;
    
    public Color Color = Color.White;
    public float Thickness = 2.0f;

    [IgnoreDataMember]
    public Point BoundingBox => boundingBox;
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        drawBatch.GetShapeDrawBatch((DrawBatch.Layer)0).DrawBeziers(new Pen(Color.Blue, 100.0f), bezierSegments, BezierType.Quadratic);
    }
}