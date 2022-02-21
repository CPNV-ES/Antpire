using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;

namespace Antpire.Systems; 

internal class AntLogicSystem : EntityUpdateSystem {
    private ComponentMapper<Ant> antMapper;
    private ComponentMapper<Insect> insectMapper;
    private ComponentMapper<SimulationPosition> simulationPosition;

    private Random rand = new Random();
    private SimulationState simState;

    public AntLogicSystem(SimulationState simulationState) : base(Aspect.All(typeof(Ant), typeof(Insect), typeof(SimulationPosition))) {
        simState = simulationState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        simulationPosition = mapperService.GetMapper<SimulationPosition>();
        antMapper = mapperService.GetMapper<Ant>();
        insectMapper = mapperService.GetMapper<Insect>();
    }

    public override void Update(GameTime gameTime) {
        foreach(var entityId in ActiveEntities) {
            var position = simulationPosition.Get(entityId);
            var ant = antMapper.Get(entityId);
            var insect = insectMapper.Get(entityId);

            ant.TimeTilNextUpdate -= (float)gameTime.ElapsedGameTime.TotalSeconds * simState.TimeScale;

            switch(ant.CurrentState) {
                case Ant.State.Idle:
                    break;
                case Ant.State.Scouting:
                    scouting(ant, insect, position);
                    break;
                case Ant.State.Attacking:
                    break;
                case Ant.State.Dying:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void scouting(Ant ant, Insect insect, SimulationPosition entity) {
        // Every x seconds the ant changes its direction
        if(ant.TimeTilNextUpdate < 0.0f) {
            ant.TimeTilNextUpdate = ant.MinUpdateFrequency;     // Reset timer

            var rot = (float)rand.NextDouble() * MathF.PI / 8;
            rot = entity.Rotation + rot - MathF.PI / 16;
            entity.Rotation = rot;

            var dest = new Vector2(MathF.Cos(rot), MathF.Sin(rot));
            insect.Destination = entity.Position + dest * 100;
        }
    }
}