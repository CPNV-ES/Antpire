using Antpire.Components;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal class AnthillInteriorGridmapRenderable : IRenderable {
        public AnthillInteriorGridMap GridMap { get; set; } 

        public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
            for (int y = 0; y < GridMap.Grid.GetLength(0); y++) {
                for (int x = 0; x < GridMap.Grid.GetLength(0); x++) {
                    GridMap.TilesRenderables[GridMap.Grid[y,x]].Render(spriteBatch, new Transform2((new Vector2(x * GridMap.TileWidth + trans.Position.X, y * GridMap.TileWidth + trans.Position.Y))), viewRegion);
                }
            }
        }
    }
}