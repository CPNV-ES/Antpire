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
        public int ChunkSize = 768;
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
                PlaceTrunksInChunk(new (x, y), world);
                PlaceBushesInChunk(new (x, y), world);

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
    
    private void PlaceTrunksInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.RocksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
            var trunk = world.CreateEntity();
            var trunkWidth = random.Next(20, 30);
            trunk.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            trunk.Attach(new Renderable {
                RenderItem = new RectangleRenderable(
                    size: new(trunkWidth, (float)(trunkWidth * (random.NextDouble()*3+2))), 
                    rotation: (float)(random.NextDouble()*Math.PI*2), 
                    color: Color.SaddleBrown,
                    thickness: 30.0f
                )
            });
        }
    }

    private void PlaceBushesInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.RocksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
            // Generate the bush's leaves
            var leavesPositions = new List<Vector2>() { new(0, 0) };
            var fruitsPositions = new List<Vector2>();

            for (int b = 0; b < random.Next(6, 18); b++) {
                leavesPositions.Add(ShapeUtils.GetRandomPointInCircle(50));
                
                // Generate fruits
                for (int f = 0; f < random.Next(0, 5); f++) {
                    fruitsPositions.Add(ShapeUtils.GetRandomPointInCircle(40) + leavesPositions.Last());                
                }
            }
            
            var bush = world.CreateEntity();
            var fruits = world.CreateEntity();
            bush.Attach(new SimulationPosition { Position = pos, WorldSpace= WorldSpace.Garden });
            bush.Attach(new Renderable {
                RenderItem = new RenderablesGroup {
                    Children = leavesPositions.Select(x => 
                        (
                            new CircleRenderable { Sides = 32, Color = Color.DarkGreen, Thickness = 50.0f, Radius = 50 - (int)Math.Sqrt(x.X*x.X + x.Y*x.Y)/3 } as IRenderable,
                            x.ToPoint()
                        )
                    ).ToArray()
                }
            });
            var colors = new Color[] { Color.Red, Color.GreenYellow, Color.OrangeRed };
            fruits.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
            fruits.Attach(new Renderable {
                RenderItem = new RenderablesGroup {
                    Children = fruitsPositions.Select(x =>
                        (
                            new CircleRenderable { Sides = 32, Color = colors[random.Next(0, 3)], Thickness = 8.0f, Radius = random.Next(4, 8) } as IRenderable,
                            x.ToPoint()
                        )
                    ).ToArray()
                }
            });
        }
    }
}
