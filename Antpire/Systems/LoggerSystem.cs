using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using Antpire.Screens;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;

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

                Signalings.Add(createSignage(entityId,ant, gameTime));
            }


            /* DEV : On T-Key pressed, change randomely the state of ants to see if this method worls */
            var keyboardState = Keyboard.GetState();
            var r = new Random();

            if (keyboardState.IsKeyDown(Keys.T)) {
                ant.CurrentState = Ant.State.Attacking;
            }

        }
    }

    private Signaling createSignage(int entityId, Ant ant, GameTime gameTime) {
        String content = "";
        Signaling.Channel channel;

        switch (ant.CurrentState) { 
            /* Here you could add any signage that you want */
            case Ant.State.Idle:
                channel = Signaling.Channel.Working;
                content = String.Format("The ant {0} is actually making nothing instead of beeing an {1}.", entityId, ant.type);
                break;
            case Ant.State.Scouting:
                channel = Signaling.Channel.Working;
                content = String.Format("The {0} ant number {1} is now wandering around the garden.", ant.type, entityId);
                break;
            case Ant.State.Attacking:
                channel = Signaling.Channel.Fight;
                content = String.Format("The {0} ant number {1} is fighting !", ant.type, entityId);
                break;
            case Ant.State.Dying:
                channel = Signaling.Channel.LifeCycle;
                content = String.Format("The {0} ant number {1} is Dead. RIP Little ant..", ant.type, entityId);
                break;            
            case Ant.State.Gathering:
                channel = Signaling.Channel.Working;
                content = String.Format("The {0} ant number {1} is Gathering ressources.", ant.type, entityId);
                break;
            default:
                channel = Signaling.Channel.Info;
                content = String.Format("The state of the {0} ant number {1} has changed to {2}.", ant.type, entityId, ant.CurrentState);
                break;
        }
        
        
        return new Signaling {
            channel = Signaling.Channel.Working,
            ant = ant,
            content = String.Format("[ * * {0} * * ]({1}) - {2}", channel, gameTime.TotalGameTime, content),
            timestamp = gameTime.TotalGameTime
        };
    }

    private bool stateHasChanged(Ant ant) {
        if (ant.CurrentState != ant.OldState) {
            ant.OldState = ant.CurrentState;
            return true;
        }else
            return false;
    }
}