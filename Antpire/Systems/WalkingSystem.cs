using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using Antpire.Screens;

namespace Antpire.Systems; 

internal class WalkingSystem : EntityUpdateSystem
{
    private ComponentMapper<SimulationPosition> simulationPosition;
    private ComponentMapper<Insect> insectMapper;
    private SimulationState simState;

    private int gardenWidth => simState.GardenGenerationOptions.Width * simState.GardenGenerationOptions.ChunkSize;
    private int gardenHeight => simState.GardenGenerationOptions.Height * simState.GardenGenerationOptions.ChunkSize; 

    public WalkingSystem(SimulationState simState) : base(Aspect.All(typeof(Insect), typeof(SimulationPosition))) {
        this.simState = simState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        simulationPosition = mapperService.GetMapper<SimulationPosition>();
        insectMapper = mapperService.GetMapper<Insect>();
    }

    public override void Update(GameTime gameTime) {
        foreach (var entityId in ActiveEntities) {
            var entity = simulationPosition.Get(entityId);
            var insect = insectMapper.Get(entityId);

            insect.Velocity = Vector2.Subtract(insect.Destination, entity.Position);
            insect.Velocity.Normalize();

            walk(entity, insect, (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }

    private void walk(SimulationPosition entity, Insect insect, float deltaTime) {
        var newPosition = entity.Position + Vector2.Multiply(insect.Velocity, 140.0f * deltaTime * simState.TimeScale);
            
        // If trying to move out of the garden's bounds, turn around
        if(newPosition.X < 0 || newPosition.X > gardenWidth || newPosition.Y < 0 || newPosition.Y > gardenHeight) {
            newPosition = MoveAround(entity, insect);
        }

        // If the entity collides an hitbox, it turns around
        float circle = entity.Scale * MathF.PI;

        //Get hitboxes from the collision system
        
        List<Hitbox> hitboxes = CollisionSystem.hitboxes;
        foreach(Hitbox hitbox in hitboxes) {

            float dx = newPosition.X - hitbox.position.X;
            float dy = newPosition.Y - hitbox.position.Y;
            
            var distance = Math.Sqrt(dx * dx + dy * dy);

            if(distance < circle + hitbox.hitbox) {
                newPosition = MoveAround(entity, insect);
            }
        }

        /*
        var circle1 = { radius: 20, x: 5, y: 5};
        var circle2 = { radius: 12, x: 10, y: 5 };

        var dx = circle1.x - circle2.x;
        var dy = circle1.y - circle2.y;
        var distance = Math.sqrt(dx * dx + dy * dy);

        if (distance<circle1.radius + circle2.radius) {
            // collision détectée !
        }*/


        entity.Position = newPosition;
    }

    // [DHI] TODO: Clean this code up, maybe shouldn't be in this system
    private Vector2 MoveAround(SimulationPosition position, Insect insect) {
        var rot = position.Rotation + MathF.PI;   // 180 degrees turn
        var vec = new Vector2(MathF.Cos(rot), MathF.Sin(rot));
        var newTarget = position.Position + vec * 100;
        insect.Destination = newTarget;
        position.Rotation = rot;
        return position.Position + Vector2.Multiply(insect.Velocity, 2f);
    }
}