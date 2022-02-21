﻿using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class SpriteRenderable : IRenderable{
    private Texture2D texture;
    private Point size;
    private int longestSide;

    public float RotationOffset = 0.0f;
    
    public SpriteRenderable(Point size, Texture2D texture, float rotationOffset = 0.0f) {
        this.texture = texture;
        this.size = size;
        RotationOffset = rotationOffset;
        this.longestSide = Math.Max(size.X, size.Y);
    }
    
    public SpriteRenderable(int size, Texture2D texture) : this(new Point(size, size), texture) { }


    public void Render(DrawBatch drawBatch, Transform2 trans, Rectangle viewRegion) {
        if(viewRegion.Intersects(new Rectangle { Location = new((int)trans.Position.X - longestSide/2, (int)trans.Position.Y - longestSide/2), Width = longestSide, Height = longestSide })) {
            Vector2 location = trans.Position;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            float angle = trans.Rotation;

            drawBatch.GetSpriteDrawBatch(0).Draw(texture, location, sourceRectangle, Color.White, angle + RotationOffset, origin, 0.5f, SpriteEffects.None, 1);
        }
    }
}