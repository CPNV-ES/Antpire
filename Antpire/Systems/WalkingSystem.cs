using System.Diagnostics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using Antpire.Screens;
using MonoGame.Extended;

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
        var newPosition = entity.Position + Vector2.Multiply(insect.Velocity, 140.0f * deltaTime * simState.TimeScale); // TODO: Replace the hard coded value with a speed variable
        var ahead = entity.Position + Vector2.Multiply(insect.Velocity, 25.0f); // TODO: Replace the hard coded value
            
        // If trying to move out of the garden's bounds, turn around
        if(newPosition.X < 0 || newPosition.X > gardenWidth || newPosition.Y < 0 || newPosition.Y > gardenHeight) {
            entity.Position = TurnAround(entity, insect);
            return;
        }

        // If the entity collides an solid collision body, it turns around
        if(CollisionSystem.CheckSolidCollision(ahead) || CollisionSystem.CheckSolidCollision(newPosition)) {
            entity.Position = TurnAround(entity, insect);
            return;
        }

        entity.Position = newPosition;
    }

    // [DHI] TODO: Clean this code up, maybe shouldn't be in this system
    private Vector2 TurnAround(SimulationPosition position, Insect insect) {
        var rot = position.Rotation + MathF.PI;   // 180 degrees turn
        var vec = new Vector2(MathF.Cos(rot), MathF.Sin(rot));
        var newTarget = position.Position + vec * 100;
        insect.Destination = newTarget;
        position.Rotation = rot;
        return position.Position + Vector2.Multiply(insect.Velocity, 2f);
    }
}