using Antpire.Drawing;

namespace Antpire.Components; 

internal class AnthillInteriorGridMap {
    public enum TileState {
        Wall,
        Dug,
    }

    public TileState[,] Grid { get; set; }
    public Dictionary<TileState, IRenderable> TilesRenderables { get; set; }
    public int TileWidth { get; set; } = 32;

    public void FillRectangle(Rectangle area, TileState tileState) {
        for(int y = area.Y; y < area.Y + area.Height; y++) {
            for (int x = area.X; x < area.X + area.Width; x++) {
                Grid[y, x] = tileState;
            }
        }
    }
}