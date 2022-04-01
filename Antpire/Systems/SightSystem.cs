using System.Diagnostics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using Antpire.Screens;
using MonoGame.Extended;

namespace Antpire.Systems; 

internal class SightSystem : EntityUpdateSystem {
    private ComponentMapper<SimulationPosition> simulationPosition;
    private ComponentMapper<Insect> insectMapper;
    private ComponentMapper<Ant> antMapper;
    private SimulationState simState;
    private float updateFrequency = 60.0f / 10.0f;
    private float lastUpdate = float.MaxValue;

    public SightSystem(SimulationState simState) : base(Aspect.All(typeof(Insect), typeof(SimulationPosition))) {
        this.simState = simState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        simulationPosition = mapperService.GetMapper<SimulationPosition>();
        insectMapper = mapperService.GetMapper<Insect>();
        antMapper = mapperService.GetMapper<Ant>();
    }

    public override void Update(GameTime gameTime) {
        lastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if(lastUpdate < updateFrequency) {
            return;
        }
        
        foreach (var entityId in ActiveEntities) {
            var entity = simulationPosition.Get(entityId);
            var insect = insectMapper.Get(entityId);

            var hboxAround = CollisionSystem.GetHitboxesInRadius(entity.Position, 250.0f);
            var lastSeen = insect.HitboxesInSight;
            insect.HitboxesInSight = hboxAround.ToList();
            insect.NewHitboxesInSight = insect.HitboxesInSight.Except(lastSeen).ToList();
            
            // Only log for ants
            if(antMapper.Has(entityId)) {
                var ant = antMapper.Get(entityId);
                if(insect.NewHitboxesInSight.Count != 0) {
                    foreach(var h in insect.NewHitboxesInSight) {
                        LoggerSystem.Signal(ant, entityId, $"Just sighted a {h.Name}", Signaling.Channel.Sight);
                    }
                }
            }
        }
    }
}
