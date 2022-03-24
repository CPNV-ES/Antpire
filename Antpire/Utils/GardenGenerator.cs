﻿using Antpire.Components;
using Antpire.Drawing;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Shapes;
using System;

namespace Antpire.Utils;

public class GardenGenerator {
    public struct GardenGenerationOptions {
        public GardenGenerationOptions() { }

        public int ChunkSize = 768;
        public int Width = 3;
        public int Height = 3;
        public Range<int> RocksPerChunk = new(1, 1);
        public Range<int> RockSize = new(30, 100);
        public Range<int> RockVertices = new(5, 13);
        public Range<int> TrunksPerChunk = new(1, 5);
        public Range<int> TwigsPerChunk = new(1, 1);
        public Range<int> BushesPerChunk = new(1, 1);
        public Range<int> FruitsPerBush = new(1, 3);
        public int Anthills = 2;
        public Range<int> AliveAphids = new(1, 1);
        public Range<int> DeadAphids = new(1, 1);
        //public Range<int> WanderingAnts = new(10, 10);
        public Range<int> WanderingAnts = new(10000, 10000);
    }


    public GardenGenerationOptions GenerationOptions { get; init; }
    public Game Game { get; init; }

    private readonly Random random = new Random();
    private readonly Antpire.ContentProvider contentProvider;
    
    public GardenGenerator(Game game, GardenGenerationOptions options) {
        Game = game;
        GenerationOptions = options;
        contentProvider = Game.Services.GetService<Antpire.ContentProvider>();
    }
    
    public void GenerateGarden(World world) {
        PlaceRiver(world);
        PlaceAnthills(world);
        PlaceAphids(world);
        PlaceWanderingAnts(world);

        for(var y = 0; y < GenerationOptions.Height; y++) {
            for(var x = 0; x < GenerationOptions.Width; x++) {
                PlaceRocksInChunk(new(x, y), world);
                PlaceTrunksInChunk(new (x, y), world);
                PlaceBushesInChunk(new (x, y), world);
                PlaceTwigsInChunk(new (x, y), world);
            }
        }
    }

    private Vector2 GetRandomPointInChunk(Point chunk) => 
        new Vector2(
            random.Next(chunk.X * GenerationOptions.ChunkSize, (chunk.X + 1) * GenerationOptions.ChunkSize),
            random.Next(chunk.Y * GenerationOptions.ChunkSize, (chunk.Y + 1) * GenerationOptions.ChunkSize)
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
            riverHeading += random.NextDouble(-Math.PI/4, Math.PI/4);
            currentPoint += new Vector2(256).Rotate((float)riverHeading);
            segments.Add(currentPoint);
        } while(!IsPointOutsideBoundaries(currentPoint.ToPoint()));
        
        var river = world.CreateEntity();
        river.Attach(new SimulationPosition { Position = new (0, 0), WorldSpace = WorldSpace.Garden });
        river.Attach(new Renderable { RenderItem = new SmoothPathRenderable { Color = Color.Blue, Segments = segments.ToArray(), Thickness = 50 } });
    }

    private void PlaceAnthills(World world) {
        var maxAnthills = (GenerationOptions.Width * 2 + GenerationOptions.Height * 2 - 4);
        var anthillsToGenerate = GenerationOptions.Anthills > maxAnthills ? maxAnthills : GenerationOptions.Anthills;
        
       var inhabitedChunks = new List<Point>();
       
       // TODO: This could lead to infinite loop?
       do {
           var chunk = GetRandomCornerChunk();
           if(!inhabitedChunks.Contains(chunk) && inhabitedChunks.Count(x => ChunkDistance(x, chunk) <= 2) == 0) {
               inhabitedChunks.Add(chunk);
           }
       } while(inhabitedChunks.Count < anthillsToGenerate);
       
       foreach(var chunk in inhabitedChunks) {
           var pos = GetRandomPointInChunk(chunk);
           var anthill = world.CreateEntity();
           anthill.Attach(new SimulationPosition { Position = new (pos.X - 250, pos.Y - 250), WorldSpace = WorldSpace.Garden });
           anthill.Attach(new Renderable {
               RenderItem = new SpriteRenderable(2, contentProvider.Get<Texture2D>("anthill/Anthill")),
           });
       }
    }

    private void PlaceRocksInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.RocksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
            var rock = world.CreateEntity();
            var rr = new PolygonRenderable {
                Color = Color.DarkGray,
                Polygon = new Polygon(ShapeUtils.GenerateConvexPolygon(random.Next(GenerationOptions.RockVertices), random.Next(GenerationOptions.RockSize))),
                Thickness = 5.0f
            };
            
            rock.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
            rock.Attach(new Renderable {
                RenderItem = rr 
            });
            rock.Attach(new Hitbox { position = pos, polygon = rr.Polygon });
        }
    }
    
    private void PlaceTrunksInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.TrunksPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
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
            
            trunk.Attach(new SimulationPosition { Scale = 1, Position = new (pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            trunk.Attach(new Renderable {
                RenderItem = rr 
            });
            trunk.Attach(new Hitbox { position = pos, polygon = rr.Polygon });
        }
    }
    
    private void PlaceTwigsInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.TwigsPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
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
            aphid.Attach(new SimulationPosition { Position = new (pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            aphid.Attach(new Renderable {
                RenderItem = new SpriteRenderable(0.25f, contentProvider.Get<Texture2D>(tex), 0.0f, (int)DrawBatch.Layer.Insect)
            });
        }
        
        for(var i = 0; i < random.Next(GenerationOptions.AliveAphids); i++) {
            createAphid(dead: false);
        }                      
        for(var i = 0; i < random.Next(GenerationOptions.DeadAphids); i++) {
            createAphid(dead: true);
        }
    }

    private void PlaceWanderingAnts(World world) {
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

        for (var i = 0; i < random.Next(GenerationOptions.WanderingAnts); i++) {
            createWanderingAnt();
        }
    }

    private void PlaceBushesInChunk(Point chunk, World world) {
        for(var i = 0; i < random.Next(GenerationOptions.BushesPerChunk); i++) {
            var pos = GetRandomPointInChunk(chunk); 
            // Generate the bush's leaves
            var leavesPositions = new List<Vector2>() { new(0, 0) };
            var fruitsPositions = new List<Vector2>();

            var colors = new Color[] { Color.Red, Color.GreenYellow, Color.OrangeRed };
            
            for (int b = 0; b < random.Next(7, 13); b++) {
                leavesPositions.Add(ShapeUtils.GetRandomPointInCircle(50));
            }

            foreach(var leaf in leavesPositions) {
               var leafEntity = world.CreateEntity(); 
               leafEntity.Attach(new SimulationPosition { Position = pos + new Vector2(leaf.X, leaf.Y), WorldSpace = WorldSpace.Garden });
               leafEntity.Attach(new Renderable {
                   RenderItem = new CircleRenderable { Sides = 32, Color = Color.DarkGreen, Thickness = 50.0f, Radius = 50 - (int)Math.Sqrt(leaf.X*leaf.X + leaf.Y*leaf.Y)/3 } as IRenderable,
               });
            
                
               // Generate fruits
               for (int f = 0; f < random.Next(1, 3); f++) {
                    fruitsPositions.Add(ShapeUtils.GetRandomPointInCircle(20) + new Vector2(leaf.X, leaf.Y) + new Vector2(30, 30));                
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
}
