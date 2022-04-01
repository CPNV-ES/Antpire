﻿using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using MonoGame.Extended;

namespace Antpire.Systems; 

internal class CollisionSystem : EntityUpdateSystem {
    private ComponentMapper<Hitbox> hitboxMapper;
    private ComponentMapper<SimulationPosition> simPosMapper;

    private static SimulationState simState;
    private static List<Hitbox>[,] partitionedCollisionBodies;
    private static float partitionSize;
    
    private List<int> registeredEntities = new List<int>(); 

    
    public CollisionSystem(SimulationState simulationState, float? partitionSize = null) : base(Aspect.All(typeof(Hitbox), typeof(SimulationPosition))) {
        simState = simulationState;
        CollisionSystem.partitionSize = partitionSize ?? simulationState.GardenGenerationOptions.ChunkSize;
        partitionedCollisionBodies = new List<Hitbox>[simulationState.GardenGenerationOptions.Width, simulationState.GardenGenerationOptions.Height];
        
    }

    public override void Initialize(IComponentMapperService mapperService) {
        hitboxMapper = mapperService.GetMapper<Hitbox>();
        simPosMapper = mapperService.GetMapper<SimulationPosition>();
        
    }

    public override void Update(GameTime gameTime) {
        foreach(var entityId in ActiveEntities) {
            var hitbox = hitboxMapper.Get(entityId);
            
            // Kinda hacky, should be able to update a hitbox' position 
            if(!registeredEntities.Contains(entityId)) {
                var pos = simPosMapper.Get(entityId);
                hitbox.Polygon = hitbox.Polygon.TransformedCopy(pos.Position, 0.0f, Vector2.One);
                var bb = hitbox.Polygon.BoundingRectangle;
                var tl = getPartitionFromPosition(bb.TopLeft);
                var br = getPartitionFromPosition(bb.BottomRight);

                for(var x = tl.X; x <= br.X; x++) {
                    for(var y = tl.Y; y <= br.Y; y++) {
                        if(partitionedCollisionBodies[x, y] == null) {
                            partitionedCollisionBodies[x, y] = new List<Hitbox>();
                        }
                        partitionedCollisionBodies[x, y].Add(hitbox);
                    }
                }
                
                registeredEntities.Add(entityId);
            }
        }
    }

    public static bool CheckCollision(Vector2 position) {
        var part = getPartitionFromPosition(position);
        var bodies = partitionedCollisionBodies[part.X, part.Y];

        if(bodies == null) return false;
        
        foreach(var body in bodies) {
            if(body.Polygon.Contains(position)) {
                return true;
            }
        }
        
        return false;
    }
    
    public static IEnumerable<Hitbox> GetHitboxesInRadius(Vector2 position, float radius) {
        return GetHitboxesOverlappingCircle(new CircleF(position, radius));
    }

    private static Point getPartitionFromPosition(Vector2 position) => new(
        Math.Clamp((int)Math.Floor(position.X / partitionSize), 0, simState.GardenGenerationOptions.Width - 1),
        Math.Clamp((int)Math.Floor(position.Y / partitionSize), 0, simState.GardenGenerationOptions.Height - 1)
    );

    private static IEnumerable<Hitbox> GetHitboxesOverlappingCircle(CircleF circle) {
        var partitions = getPartitionsOverlappingCircle(circle);
        var bodies = new List<Hitbox>();
        
        foreach(var part in partitions) {
            foreach(var body in partitionedCollisionBodies[part.X, part.Y] ?? new List<Hitbox>()) {
                var inRange = body.Polygon.Vertices.Any(v => Math.Sqrt(Math.Pow(v.X - circle.Center.X, 2) + Math.Pow(v.Y - circle.Center.Y, 2)) <= circle.Radius);
                if(inRange) {
                    bodies.Add(body);
                    break;
                }
            }
        }
        
        return bodies.Distinct();
    }

    private static IEnumerable<Point> getPartitionsOverlappingCircle(CircleF circle) {
        var position = circle.Center;
        var radius = circle.Radius;
        var centerPart = getPartitionFromPosition(position);
        var partitions = new List<Point>();
        var x = centerPart.X;
        var y = centerPart.Y;
        var r = (int)Math.Ceiling(radius / partitionSize);
         
        // Approximates that the circle is at the center of the partition,
        // so it may include partitions that are not actually touching the circle
        for(var i = -r; i <= r; i++) {
            for(var j = -r; j <= r; j++) {
                partitions.Add(new Point(x + i, y + j));
            }
        }
        
        return partitions.Where(p => p.X >= 0 && p.X < partitionedCollisionBodies.GetLength(0) && p.Y >= 0 && p.Y < partitionedCollisionBodies.GetLength(1));
    }
}