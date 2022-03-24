using System.ComponentModel;
using LilyPath;
using Microsoft.Xna.Framework.Graphics;

using LilyDrawBatch = LilyPath.DrawBatch;

namespace Antpire.Drawing; 

public class DrawBatch {
    public enum Layer {
        Background,
        BelowInsect,
        Insect,
        AboveInsect,
    }

    private int layersCount = Enum.GetValues(typeof(Layer)).Length;
    private LilyDrawBatch[] lilyBatches;
    private SpriteBatch[] spriteBatches;
    private GraphicsDevice graphicsDevice;
    
    public DrawBatch(GraphicsDevice device) {
        graphicsDevice = device;
        
        lilyBatches = new LilyDrawBatch[layersCount];
        spriteBatches = new SpriteBatch[layersCount];
        
        for(var i = 0; i < layersCount; i++) {
            lilyBatches[i] = new LilyDrawBatch(device);
            spriteBatches[i] = new SpriteBatch(device);
        }
    }

    public void Begin(Matrix transformMatrix) {
        foreach(LilyDrawBatch b in lilyBatches) {
            b.Begin(DrawSortMode.Deferred, (BlendState)null, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, transformMatrix);
        }
        foreach(SpriteBatch b in spriteBatches) {
            b.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        }
    }

    public void End() {
        for(var i = 0; i < layersCount; i++) {
            lilyBatches[i].End();
            spriteBatches[i].End();
        }
    }
   
    public LilyDrawBatch GetShapeDrawBatch(Layer layer) => lilyBatches[(int)layer];
    
    public SpriteBatch GetSpriteDrawBatch(Layer layer) => spriteBatches[(int)layer];
}