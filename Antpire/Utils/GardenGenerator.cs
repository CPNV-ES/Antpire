using Antpire.Components;
using Antpire.Drawing;
using Antpire.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Shapes;

namespace Antpire.Utils; 

public static class RandomExtensions
{
    public static double NextDouble(
        this Random random,
        double minValue,
        double maxValue)
    {
        return random.NextDouble() * (maxValue - minValue) + minValue;
    }
}


public class GardenGenerator {
    public struct GardenGenerationOptions {
        public int ChunkSize = 1024;
        public int Width = 3;
        public int Height = 3;
        public Range<int> RocksPerChunk = new(1, 1);
        public Range<int> RockSize = new(30, 100);
        public Range<int> RockVertices = new(5, 13);
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

    private Random random = new Random();
   
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

        PlaceRiver(world);

        for(var y = 0; y < GenerationOptions.Height; y++) {
            for(var x = 0; x < GenerationOptions.Width; x++) {
                PlaceRocksInChunk(new(x, y), world);
            }
        }
    }

    private Point GetRandomPointInChunk(Point chunk) => 
        new Point(
            random.Next(chunk.X * GenerationOptions.ChunkSize, (chunk.X + 1) * GenerationOptions.ChunkSize),
            random.Next(chunk.Y * GenerationOptions.ChunkSize, (chunk.Y + 1) * GenerationOptions.ChunkSize)
        );

    private Point GetCenterChunk() => 
        new Point(
            GenerationOptions.Width / 2,
            GenerationOptions.Height / 2
        ); 
    
    private bool IsPointOutsideBoundaries(Point point) =>
        point.X < 0 || point.Y < 0 || 
        point.X > GenerationOptions.Width * GenerationOptions.ChunkSize || point.Y > GenerationOptions.Height * GenerationOptions.ChunkSize;
    
    private void PlaceRiver(World world) {
        var segments = new List<Vector2>();
        var chunk = GetCenterChunk();
        var currentPoint = GetRandomPointInChunk(chunk).ToVector2();
        var riverHeading = random.NextDouble() * Math.PI*2;
        segments.Add(currentPoint);
        do {
            riverHeading += random.NextDouble(-Math.PI/4, Math.PI/4);
            currentPoint += new Vector2(256).Rotate((float)riverHeading);
            segments.Add(currentPoint);
            var v = new Vector2();
        } while(!IsPointOutsideBoundaries(currentPoint.ToPoint()));
        
        var river = world.CreateEntity();
        river.Attach(new SimulationPosition { Position = new Point(0, 0), WorldSpace = WorldSpace.Garden });
        river.Attach(new Renderable { RenderItem = new PathRenderable { Color = Color.Blue, Segments = segments.ToArray(), Thickness = 50 } });
    }

    private void PlaceRocksInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.RocksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
            var rock = world.CreateEntity();
            rock.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            rock.Attach(new Renderable {
                RenderItem = new PolygonRenderable {
                    Color = Color.DarkGray,
                    Polygon = new Polygon(ShapeUtils.GenerateConvexPolygon(random.Next(GenerationOptions.RockVertices), random.Next(GenerationOptions.RockSize))),
                    Thickness = 5.0f
                }
            });
        }
    }
}
