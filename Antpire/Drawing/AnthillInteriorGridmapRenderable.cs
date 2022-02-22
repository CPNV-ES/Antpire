using Antpire.Components;
using LilyPath.Shapes;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing; 

internal class AnthillInteriorGridmapRenderable : IRenderable {
    public AnthillInteriorGridMap GridMap { get; set; }

    public Point BoundingBox => new(GridMap.TileWidth * GridMap.Grid.GetLength(1), GridMap.TileWidth* GridMap.Grid.GetLength(0));
    public int Layer { get; init; }
    
    public void Render(DrawBatch drawBatch, Transform2 trans) {
        for (int y = 0; y < GridMap.Grid.GetLength(1); y++) {
            for (int x = 0; x < GridMap.Grid.GetLength(0); x++) {
                GridMap.TilesRenderables[GridMap.Grid[y,x]].Render(drawBatch, new Transform2((new Vector2(x * GridMap.TileWidth + trans.Position.X, y * GridMap.TileWidth + trans.Position.Y))));
            }
        }
    }
}