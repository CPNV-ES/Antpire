using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using MonoGame.Extended;

namespace Antpire.Systems; 

internal class CollisionSystem : EntityUpdateSystem {
    private ComponentMapper<CollisionBody> collisionBodyMapper;
    private ComponentMapper<SimulationPosition> simPosMapper;

    private static SimulationState simState;
    private static List<CollisionBody>[,] partitionedCollisionBodies;
    private static float partitionSize;
    
    private List<int> registeredEntities = new List<int>(); 

    
    public CollisionSystem(SimulationState simulationState, float? partitionSize = null) : base(Aspect.All(typeof(CollisionBody), typeof(SimulationPosition))) {
        simState = simulationState;
        CollisionSystem.partitionSize = partitionSize ?? simulationState.GardenGenerationOptions.ChunkSize;
        partitionedCollisionBodies = new List<CollisionBody>[simulationState.GardenGenerationOptions.Width, simulationState.GardenGenerationOptions.Height];
        for(var x = 0; x < partitionedCollisionBodies.GetLength(0); x++) {
            for(var y = 0; y < partitionedCollisionBodies.GetLength(1); y++) {
                partitionedCollisionBodies[x, y] = new List<CollisionBody>();
            }
        }
    }

    public override void Initialize(IComponentMapperService mapperService) {
        collisionBodyMapper = mapperService.GetMapper<CollisionBody>();
        simPosMapper = mapperService.GetMapper<SimulationPosition>();
        
    }

    public override void Update(GameTime gameTime) {
        foreach(var entityId in ActiveEntities) {
            var collisionBody = collisionBodyMapper.Get(entityId);
            
            // Kinda hacky as the collision body's position will not be updated
            // TODO: Have a "static" boolean property on the collision body to indicate if its pos should be updated or no
            if(!registeredEntities.Contains(entityId)) {
                var pos = simPosMapper.Get(entityId);
                collisionBody.Polygon = collisionBody.Polygon.TransformedCopy(pos.Position, 0.0f, Vector2.One);
                var bb = collisionBody.Polygon.BoundingRectangle;
                var tl = GetPartitionFromPosition(bb.TopLeft);
                var br = GetPartitionFromPosition(bb.BottomRight);

                for(var x = tl.X; x <= br.X; x++) {
                    for(var y = tl.Y; y <= br.Y; y++) {
                        if(partitionedCollisionBodies[x, y] == null) {
                            partitionedCollisionBodies[x, y] = new List<CollisionBody>();
                        }
                        partitionedCollisionBodies[x, y].Add(collisionBody);
                    }
                }
                
                registeredEntities.Add(entityId);
            }
        }
    }
    
    public static IEnumerable<CollisionBody> GetCollisionBodiesAtPosition(Vector2 position) {
        var partition = GetPartitionFromPosition(position);
        if(partitionedCollisionBodies[partition.X, partition.Y] == null) {
            yield return null;
        }
        foreach(var collisionBody in partitionedCollisionBodies[partition.X, partition.Y]) {
            if(collisionBody.Polygon.Contains(position)) {
                yield return collisionBody;
            }
        }
        yield return null;
    }

    public static bool CheckSolidCollision(Vector2 position) => 
        GetCollisionBodiesAtPosition(position)?.Any(x => x is { BlocksMovement: true }) ?? false;
    
    private static Point GetPartitionFromPosition(Vector2 position) => new(
        Math.Clamp((int)Math.Floor(position.X / partitionSize), 0, simState.GardenGenerationOptions.Width - 1),
        Math.Clamp((int)Math.Floor(position.Y / partitionSize), 0, simState.GardenGenerationOptions.Height - 1)
    );

    public static IEnumerable<CollisionBody> GetCollisionBodiesOverlappingCircle(CircleF circle) {
        var partitions = GetPartitionsOverlappingCircle(circle);
        var bodies = new List<CollisionBody>();
        
        foreach(var part in partitions) {
            foreach(var body in partitionedCollisionBodies[part.X, part.Y]) {
                var inRange = body.Polygon.Vertices.Any(v => Math.Sqrt(Math.Pow(v.X - circle.Center.X, 2) + Math.Pow(v.Y - circle.Center.Y, 2)) <= circle.Radius);
                if(inRange) {
                    bodies.Add(body);
                    break;
                }
            }
        }
        
        return bodies.Distinct();
    }

    private static IEnumerable<Point> GetPartitionsOverlappingCircle(CircleF circle) {
        var position = circle.Center;
        var radius = circle.Radius;
        var centerPart = GetPartitionFromPosition(position);
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