using System.Security.Cryptography;
using Antpire.Components;
using Antpire.Drawing;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Shapes;
using System;

namespace Antpire.Utils;

public class GardenGenerator {
    public struct AnthillOptions {
        public AnthillOptions() { }
        public int Scouts = 8; 
		public int Soldiers = 4; 
		public int Workers = 16;  
		public int Farmers = 16;
    }

    public struct GardenGenerationOptions {
        
        public GardenGenerationOptions() { }
        
        public string Seed = new Guid().ToString();
        public int ChunkSize = 768;
        public int Width = 3;
        public int Height = 3;
        
        public Range<int> RocksPerChunk = new(1, 1);
        public Range<int> RockSize = new(30, 100);
        public Range<int> RockVertices = new(5, 13);
        
        public Range<int> TrunksPerChunk = new(1, 1);
        public Range<int> TrunkSize = new(1, 1);
        
        public Range<int> TwigsPerChunk = new(1, 1);
        public Range<int> TwigSize = new(1, 1);
        
        public Range<int> BushesPerChunk = new(1, 1);
        public Range<int> BushSize = new(7, 13);
        public Range<int> FruitsPerLeaf = new(1, 3);
        
        public List<AnthillOptions> Anthills = new List<AnthillOptions> {
            new AnthillOptions(), new AnthillOptions(),
        };
        
        public Range<int> AliveAphids = new(1, 1);
        public Range<int> DeadAphids = new(1, 1);
        //public Range<int> WanderingAnts = new(10, 10);
        public Range<int> WanderingAnts = new(10000, 10000);
    }


    public GardenGenerationOptions GenerationOptions { get; init; }
    public Game Game { get; init; }

    private Random random;
    private readonly Antpire.ContentProvider contentProvider;
    
    public GardenGenerator(Game game, GardenGenerationOptions options) {
        Game = game;
        GenerationOptions = options;
        contentProvider = Game.Services.GetService<Antpire.ContentProvider>();
    }
    
    public void GenerateGarden(World world) {
        random = new Random(IntSeedFromString(GenerationOptions.Seed));

        var w = GenerationOptions.Width * GenerationOptions.ChunkSize;
        var h = GenerationOptions.Height* GenerationOptions.ChunkSize;
        var outsideColor = new Color(0, 87, 0);
        
        AddBgRect(new Rectangle(0, 0, w, h), Color.ForestGreen, world);
        AddBgRect(new Rectangle(-w, 0, w, h), outsideColor, world, (int)DrawBatch.Layer.BelowInsect);
        AddBgRect(new Rectangle(w, 0, w, h), outsideColor, world, (int)DrawBatch.Layer.BelowInsect);
        AddBgRect(new Rectangle(0, h, w, h), outsideColor, world, (int)DrawBatch.Layer.BelowInsect);
        AddBgRect(new Rectangle(0, -h, w, h), outsideColor, world, (int)DrawBatch.Layer.BelowInsect);

        PlaceRiver(world);
        PlaceAnthills(world);
        PlaceAphids(world);
        //PlaceWanderingAnts(world, new(0,0), random.Next(GenerationOptions.WanderingAnts));

        for(var y = 0; y < GenerationOptions.Height; y++) {
            for(var x = 0; x < GenerationOptions.Width; x++) {
                PlaceRocksInChunk(new(x, y), world);
                PlaceTrunksInChunk(new (x, y), world);
                PlaceBushesInChunk(new (x, y), world);
                PlaceTwigsInChunk(new (x, y), world);
            }
        }
    }

    private void AddBgRect(Rectangle r, Color c, World world, int layer = 0) {
        var gardenBg = world.CreateEntity();
        gardenBg.Attach(new SimulationPosition {
            Position = new Vector2(r.Width/2.0f, r.Height/2.0f), WorldSpace = WorldSpace.Garden
        });
        gardenBg.Attach(new Renderable {
            RenderItem = new PolygonRenderable {
                Color = c,
                Layer = layer,
                Polygon = ShapeUtils.GetRectanglePolygon(r),
            }
        });
    }

    private Vector2 GetRandomPointInChunk(Point chunk, int margin = 0) => 
        new Vector2(
            random.Next(chunk.X * GenerationOptions.ChunkSize + margin/2, (chunk.X + 1) * GenerationOptions.ChunkSize - margin/2),
            random.Next(chunk.Y * GenerationOptions.ChunkSize + margin/2, (chunk.Y + 1) * GenerationOptions.ChunkSize - margin/2)
        );

    private Vector2 GetRandomPointInGarden() =>
        new Vector2(
            random.Next(0, GenerationOptions.Width * GenerationOptions.ChunkSize),
            random.Next(0, GenerationOptions.Height * GenerationOptions.ChunkSize)
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
        var currentPoint = GetRandomPointInChunk(chunk);
        var riverHeading = random.NextDouble() * Math.PI*2;
        segments.Add(currentPoint);
        
        // TODO: This could lead to infinite loop?
        do {
            riverHeading += random.NextDouble(-Math.PI/2, Math.PI/2);
            currentPoint += new Vector2(1024).Rotate((float)riverHeading);
            segments.Add(currentPoint);
        } while(!IsPointOutsideBoundaries(currentPoint.ToPoint()));
        
        var pathRenderable = new SmoothPathRenderable { Color = Color.Blue, Segments = segments.ToArray(), Thickness = 50, Layer = 2};
        var col = ShapeUtils.GeneratePolygonFromLine(pathRenderable.BezierSegments, 50.0f).Reverse().ToArray();
        
        var river = world.CreateEntity();
        river.Attach(new SimulationPosition { Position = Vector2.Zero, WorldSpace = WorldSpace.Garden });
        river.Attach(new Renderable { RenderItem = pathRenderable });
        river.Attach(new Hitbox { Name = "River", Polygon = new Polygon(col) });
    }

    private void PlaceAnthills(World world) {
        var maxAnthills = (GenerationOptions.Width * 2 + GenerationOptions.Height * 2 - 4); 
        var anthillsToGenerate = Math.Min(maxAnthills, GenerationOptions.Anthills.Count);

        Point getFurtestUninhabitedChunk(List<Point> inhabitedChunks) {
            return new Point();
        };

        var distanceMap = new List<int>();
        var inhabitedChunks = new List<Point>();
       
        // TODO: This could lead to infinite loop?
        do {
           var chunk = GetRandomCornerChunk();
           if(!inhabitedChunks.Contains(chunk) && inhabitedChunks.Count(x => ChunkDistance(x, chunk) <= 1) == 0) {
               inhabitedChunks.Add(chunk);
           }
        } while(inhabitedChunks.Count < anthillsToGenerate);

        for(var index = 0; index < inhabitedChunks.Count; index++) {
            var chunk = inhabitedChunks[index];
            var pos = GetRandomPointInChunk(chunk, 200);
            var anthill = world.CreateEntity();
            anthill.Attach(new SimulationPosition { Position = new(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            anthill.Attach(new Renderable {
                RenderItem = new SpriteRenderable(2, contentProvider.Get<Texture2D>("anthill/Anthill")),
            });
            
            PlaceWanderingAnts(world, pos, GenerationOptions.Anthills[index].Scouts);
        }
    }

    private void PlaceRocksInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.RocksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk, 200); 
            var rock = world.CreateEntity();
            var rr = new PolygonRenderable {
                Color = Color.DarkGray,
                Polygon = new Polygon(ShapeUtils.GenerateConvexPolygon(random.Next(GenerationOptions.RockVertices), random.Next(GenerationOptions.RockSize))),
                Thickness = 5.0f
            };

            var shiftedVortices = rr.Polygon.Vertices.Select(x => x - new Vector2(rr.Polygon.BoundingRectangle.Width / 2.0f, rr.Polygon.BoundingRectangle.Height / 2.0f)).ToArray();
            
            rock.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
            rock.Attach(new Renderable {
                RenderItem = rr 
            });
            rock.Attach(new Hitbox {
                Name = "Rock",
                Polygon = new Polygon(shiftedVortices)
            });
        }
    }
    
    private void PlaceTrunksInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.TrunksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk, 200); 
            var trunk = world.CreateEntity();
            var trunkWidth = random.Next(20, 60);
            var rotation = (float)(random.NextDouble() * Math.PI * 2);
            float trunkHeight = (float)(trunkWidth * (random.NextDouble() * 3 + 2));
            var rr = new RectangleRenderable(
                size: new(trunkWidth, trunkHeight),
                rotation: rotation,
                color: Color.SaddleBrown,
                thickness: 30.0f
            );
            
            var shiftedVortices = rr.Polygon.Vertices.Select(x => x - new Vector2(rr.Polygon.BoundingRectangle.Width / 2.0f, rr.Polygon.BoundingRectangle.Height / 2.0f)).ToArray();
            
            trunk.Attach(new SimulationPosition { Scale = 1, Position = new (pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            trunk.Attach(new Renderable {
                RenderItem = rr 
            });
            trunk.Attach(new Hitbox {
                Name = "Trunk",
                Polygon = new Polygon(shiftedVortices)
            });
        }
    }
    
    private void PlaceTwigsInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.TwigsPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk, 200); 
            var t = world.CreateEntity();
            t.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
            t.Attach(new Renderable { RenderItem = new LineStackRenderable { Segments = ShapeUtils.GenerateLineStack(20, 25), Color = Color.SandyBrown, Thickness = 1.0f } });
        }
    }
    
    private void PlaceAphids(World world) {
        void createAphid(bool dead) {
            var tex = dead ? "aphid/dead" : "aphid/alive";
            var aphid = world.CreateEntity();
            var pos = GetRandomPointInGarden();
            // TODO: Make this a circle instead of a square
            var colPolygon = new Polygon(new[] { 
                new Vector2(-50, -50),
                new Vector2(50, -50),
                new Vector2(50, 50),
                new Vector2(-50, 50),
            });
            aphid.Attach(new SimulationPosition { Position = new (pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            aphid.Attach(new Renderable {
                RenderItem = new SpriteRenderable(0.25f, contentProvider.Get<Texture2D>(tex), 0.0f, (int)DrawBatch.Layer.Insect)
            });
            aphid.Attach(new Hitbox{ Name = "Aphid", Polygon = colPolygon });
        }
        
        for(var i = 0; i < random.Next(GenerationOptions.AliveAphids); i++) {
            createAphid(dead: false);
        }                      
        for(var i = 0; i < random.Next(GenerationOptions.DeadAphids); i++) {
            createAphid(dead: true);
        }
    }

    private void PlaceWanderingAnts(World world, Vector2 pos = default, int number = 1) {
        void createWanderingAnt() {
            var tex = "ant/alivev2";
            var pos = new Vector2(0,0);
            var scale = 0.25f;

            var wanderingAnt = world.CreateEntity();
            wanderingAnt.Attach(new Ant());
            wanderingAnt.Attach(new Insect());
            wanderingAnt.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
            wanderingAnt.Attach(new Renderable {
                RenderItem = new SpriteRenderable(scale, contentProvider.Get<Texture2D>(tex), MathF.PI / 2, (int)DrawBatch.Layer.Insect)
            });
        }

        for (var i = 0; i < number; i++) {
            createWanderingAnt();
        }
    }

    private void PlaceBushesInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.BushesPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk, 200); 
            // Generate the bush's leaves
            var leavesPositions = new List<Vector2>() { new(0, 0) };
            var fruitsPositions = new List<Vector2>();
            var colors = new Color[] { Color.Red, Color.GreenYellow, Color.OrangeRed };
            
            for (int b = 0; b < random.Next(GenerationOptions.BushSize); b++) {
                leavesPositions.Add(ShapeUtils.GetRandomPointInCircle(50));
            }

            foreach(var leaf in leavesPositions) {
               var leafEntity = world.CreateEntity(); 
               leafEntity.Attach(new SimulationPosition { Position = pos + new Vector2(leaf.X, leaf.Y), WorldSpace = WorldSpace.Garden });
               leafEntity.Attach(new Renderable {
                   RenderItem = new CircleRenderable { Sides = 32, Color = Color.DarkGreen, Thickness = 50.0f, Radius = 50 - (int)Math.Sqrt(leaf.X*leaf.X + leaf.Y*leaf.Y)/3 } as IRenderable,
               });
            
               // Generate fruits
               for (int f = 0; f < random.Next(GenerationOptions.FruitsPerLeaf); f++) {
                    fruitsPositions.Add(ShapeUtils.GetRandomPointInCircle(20) + new Vector2(leaf.X, leaf.Y));                
               }
            }
            
            foreach(var fruit in fruitsPositions) {
               var fruitEntity = world.CreateEntity(); 
               fruitEntity.Attach(new SimulationPosition { Position = pos + new Vector2(fruit.X, fruit.Y), WorldSpace = WorldSpace.Garden });
               fruitEntity.Attach(new Renderable {
                   RenderItem = new CircleRenderable { Sides = 32, Color = colors[random.Next(0, 3)], Thickness = 16.0f, Radius = random.Next(4, 8) } as IRenderable,
               });
            }
        }
    }
    
    private static int IntSeedFromString(string seed) {
        using var algo = SHA1.Create();
        return BitConverter.ToInt32(algo.ComputeHash(Encoding.UTF8.GetBytes(seed)));
    }
}
