using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using Antpire.Screens;
using MonoGame.Extended;

namespace Antpire.Systems; 

internal class LoggerSystem : EntityUpdateSystem
{
    private ComponentMapper<Ant> antMapper;
    private SimulationState simState;


    public LoggerSystem(SimulationState simState) : base(Aspect.All(typeof(Ant))) {
        this.simState = simState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        antMapper = mapperService.GetMapper<Ant>();
    }

    public override void Update(GameTime gameTime) {
        foreach (var entityId in ActiveEntities) {
            var ant = antMapper.Get(entityId);


        }
    }
}