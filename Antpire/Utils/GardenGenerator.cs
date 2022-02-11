using Antpire.Components;
using Antpire.Drawing;
using Antpire.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Shapes;

namespace Antpire.Utils;

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

    private readonly Random random = new Random();
   
    public void GenerateGarden(World world) {
        var content = Game.Services.GetService<Antpire.ContentProvider>();
        
        PlaceRiver(world);
        PlaceAnthills(world);

        for(var y = 0; y < GenerationOptions.Height; y++) {
            for(var x = 0; x < GenerationOptions.Width; x++) {
                PlaceRocksInChunk(new(x, y), world);

                var rectangle = world.CreateEntity();
                rectangle.Attach(new SimulationPosition { Position = new Point(x * GenerationOptions.ChunkSize, y*GenerationOptions.ChunkSize), WorldSpace = WorldSpace.Garden });
                rectangle.Attach(new Renderable {
                    RenderItem = new RectangleRenderable(
                        size: new(GenerationOptions.ChunkSize, GenerationOptions.ChunkSize), 
                        rotation: 0.0f, 
                        color: Color.Red,
                        thickness: 5.0f
                    )
                });
            }
        }
    }

    private Point GetRandomPointInChunk(Point chunk) => 
        new Point(
            random.Next(chunk.X * GenerationOptions.ChunkSize, (chunk.X + 1) * GenerationOptions.ChunkSize),
            random.Next(chunk.Y * GenerationOptions.ChunkSize, (chunk.Y + 1) * GenerationOptions.ChunkSize)
        );

    private Point GetRandomCornerChunk() {
        var x = random.Next(0, 2) == 0 ? 0 : GenerationOptions.Width - 1;
        var y = random.Next(0, 2) == 0 ? 0 : GenerationOptions.Height - 1;
        return new Point(x, y);
    }
    
    private Point GetRandomEdgeChunk() {
        var p = new Point();
        p.X = random.Next(0, GenerationOptions.Width);
        if(p.X == 0 || p.X == GenerationOptions.Width - 1) {
            p.Y = random.Next(0, GenerationOptions.Height);
        }
        else {
            p.Y = random.Next(0, 2) == 0 ? 0 : GenerationOptions.Height - 1;
        }
        
        return p;
    }

    private Point GetCenterChunk() => 
        new Point(
            GenerationOptions.Width / 2,
            GenerationOptions.Height / 2
        );
    
    private int ChunkDistance(Point p1, Point p2) => 
        Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    
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

    private void PlaceAnthills(World world) {
        var maxAnthills = (GenerationOptions.Width * 2 + GenerationOptions.Height * 2 - 4);
        var anthillsToGenerate = GenerationOptions.Anthills > maxAnthills ? maxAnthills : GenerationOptions.Anthills;
        
       var inhabitedChunks = new List<Point>();
       do {
           var chunk = GetRandomCornerChunk();
           if(!inhabitedChunks.Contains(chunk) && inhabitedChunks.Count(x => ChunkDistance(x, chunk) <= 2) == 0) {
               inhabitedChunks.Add(chunk);
           }
       } while(inhabitedChunks.Count < anthillsToGenerate);
       
       foreach(var chunk in inhabitedChunks) {
           var pos = GetRandomPointInChunk(chunk);
           var anthill = world.CreateEntity();
           anthill.Attach(new SimulationPosition { Position = new Point(pos.X - 250, pos.Y - 250), WorldSpace = WorldSpace.Garden });
           anthill.Attach(new Renderable {
               RenderItem = new SpriteRenderable(500, Game.Services.GetService<Antpire.ContentProvider>().Get<Texture2D>("anthill/Anthill")),
           });
       }
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
