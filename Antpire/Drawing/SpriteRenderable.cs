using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class SpriteRenderable : IRenderable{
    private Texture2D texture;
    private float scale;
    private int longestSide;

    public float RotationOffset = 0.0f;
    
    public SpriteRenderable(float scale, Texture2D texture, float rotationOffset = 0.0f) {
        this.texture = texture;
        this.scale = scale;
        RotationOffset = rotationOffset;
        this.longestSide = (int)MathF.Max(texture.Height * scale, texture.Width * scale);
    }
    
    public Point BoundingBox => new(longestSide, longestSide);
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        Vector2 location = trans.Position;
        Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        Vector2 origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        float angle = trans.Rotation;

        drawBatch.GetSpriteDrawBatch((DrawBatch.Layer)Layer).Draw(texture, location + BoundingBox.ToVector2()/2, sourceRectangle, Color.White, angle + RotationOffset, origin, scale, SpriteEffects.None, 1);
    }
}