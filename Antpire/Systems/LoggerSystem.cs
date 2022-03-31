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

    private List<Signaling> Signalings = new List<Signaling>();

    public LoggerSystem(SimulationState simState) : base(Aspect.All(typeof(Ant))) {
        this.simState = simState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        antMapper = mapperService.GetMapper<Ant>();
    }

    public override void Update(GameTime gameTime) {
        foreach (var entityId in ActiveEntities) {
            var ant = antMapper.Get(entityId);

            if (stateHasChanged(ant)) {
                // new signaling !

                Signalings.Add(new Signaling {
                    ant = ant,
                    content = String.Format("[{0}] [{1}] [{2}] [{3}] : The ant's state has changed !", gameTime.TotalGameTime, ant.CurrentState, ant.type, entityId),
                    timestamp = gameTime.TotalGameTime
                });
                //Exemple = [25.04.21 é 16h23] [Combat] [Worker][ID45] : La fourmi 45 est attaquée par [...] !
            }

            // Errase old results from the sginalists

        }
    }

    private bool stateHasChanged(Ant ant) {
        if (ant.CurrentState != ant.OldState) {
            ant.OldState = ant.CurrentState;
            return true;
        }else
            return false;
    }
}