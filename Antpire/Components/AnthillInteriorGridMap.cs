using Antpire.Drawing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antpire.Components {
    internal class AnthillInteriorGridMap {
        public enum TileState {
            Dug,
            Wall
        }

        public TileState[,] Grid { get; set; }
        public Dictionary<TileState, IRenderable> TilesRenderables { get; set; }
        public int TileWidth { get; set; } = 32;
    }
}
