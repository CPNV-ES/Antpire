using Antpire.Components;
using Antpire.Drawing;
using Antpire.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Antpire.Utils; 

public class GardenGenerator {
    public struct GardenGenerationOptions {
        public float ChunkSize = 128.0f;
        public int Width = 3;
        public int Height = 3;
        public Range<int> RocksPerChunk = new(1, 1);
        public Range<int> TrunksPerChunk = new(1, 1);
        public Range<int> TwigsPerChunk = new(1, 1);
        public Range<int> BushesPerChunk = new(1, 1);
        public Range<int> FruitsPerBush = new(1, 3);
        public int Anthills = 2;
        public int AliveAphids = 1;
        public int DeadAphids = 1;
    }    
    
    public GardenGenerationOptions GenerationOptions { get; init; }
    public Game Game { get; init; }
    
    public void GenerateGarden(World world) {
        var content = Game.Services.GetService<Antpire.ContentProvider>();
        
        // Init the anthill
        var anthills = new List<Point>() { new(500, 400) };

        foreach (var pos in anthills) {
            var anthill = world.CreateEntity();
            anthill.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

            anthill.Attach(new Renderable {
                RenderItem = new SpriteRenderable(500, content.Get<Texture2D>("anthill/Anthill")),
            });
        }
    }
}
