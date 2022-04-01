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
    private SimulationState simState;
    private float updateFrequency = 60.0f / 10.0f;
    private float lastUpdate = 0.0f;

    public SightSystem(SimulationState simState) : base(Aspect.All(typeof(Insect), typeof(SimulationPosition))) {
        this.simState = simState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        simulationPosition = mapperService.GetMapper<SimulationPosition>();
        insectMapper = mapperService.GetMapper<Insect>();
    }

    public override void Update(GameTime gameTime) {
        lastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if(lastUpdate < updateFrequency) {
            return;
        }
        
        foreach (var entityId in ActiveEntities) {
            var entity = simulationPosition.Get(entityId);
            var insect = insectMapper.Get(entityId);

            var hboxAround = CollisionSystem.GetHitboxesInRadius(entity.Position, 512.0f);
            var lastSeen = insect.HitboxesInSight;
            insect.HitboxesInSight = hboxAround.ToList();
            insect.NewHitboxesInSight = insect.HitboxesInSight.Except(lastSeen).ToList();  
            if(insect.NewHitboxesInSight.Count != 0)
                Debug.WriteLine($"{entityId} has just seen {insect.NewHitboxesInSight.Count} new hitboxes {string.Join("," , insect.NewHitboxesInSight.Select(x => x.Name).ToList())}");
        }
    }
}
