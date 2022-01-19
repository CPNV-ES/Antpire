using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antpire.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Antpire.Drawing {
    internal class AnthillInteriorGridmapRenderable : IRenderable {
        public AnthillInteriorGridMap GridMap { get; set; } 

        public void Render(SpriteBatch spriteBatch, Point position, Rectangle viewRegion) {
            for (int y = 0; y < GridMap.Grid.GetLength(0); y++) {
                for (int x = 0; x < GridMap.Grid.GetLength(0); x++) {
                    GridMap.TilesRenderables[GridMap.Grid[y,x]].Render(spriteBatch, new Point(x*GridMap.TileWidth + position.X, y*GridMap.TileWidth + position.Y), viewRegion);
                }
            }
        }
    }
}
