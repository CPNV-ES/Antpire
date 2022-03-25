using System.Runtime.Serialization;
using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

[Serializable]
internal class SpriteRenderable : IRenderable {
    [IgnoreDataMember]
    public Texture2D Texture;
    
    public string TexturePath { get; private set; }
    public float Scale;
    public int LongestSide;

    public float RotationOffset = 0.0f;
    
    public SpriteRenderable(float scale, Texture2D texture, float rotationOffset = 0.0f, int layer = 0, string texturePath = "") {
        this.Texture = texture;
        this.Scale = scale;
        this.Layer = layer;
        this.TexturePath = texture?.Name ?? texturePath;
        RotationOffset = rotationOffset;
        this.LongestSide = texture == null ? 0 : (int)MathF.Max(texture.Height * scale, texture.Width * scale);
    }
    
    public Point BoundingBox => new(LongestSide, LongestSide);
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        if(Texture == null) return;
        
        Vector2 location = trans.Position;
        Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
        Vector2 origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
        float angle = trans.Rotation;

        drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).Draw(Texture, location, sourceRectangle, Color.White, angle + RotationOffset, origin, Scale, SpriteEffects.None, 1);
    }
}