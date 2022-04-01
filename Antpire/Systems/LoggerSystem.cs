using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using Antpire.Screens;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Antpire.Systems; 

internal class LoggerSystem : EntityUpdateSystem
{
    private ComponentMapper<Ant> antMapper;
    private SimulationState simState;

    private static List<LogMessage> LogMessages = new List<LogMessage>();

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
                // Log the state change
                LogMessages.Add(createSignage(ant, gameTime, entityId));
                Debug.WriteLine(LogMessages.Last().Content);
            }


            /* DEV : On T-Key pressed, change randomely the state of ants to see if this method worls */
            var keyboardState = Keyboard.GetState();
            var r = new Random();

            if (keyboardState.IsKeyDown(Keys.T)) {
                ant.CurrentState = Ant.State.Attacking;
            }

            if (keyboardState.IsKeyDown(Keys.L)) {
                Debug.WriteLine("List of Signalings : ");

                foreach (LogMessage s in LogMessages.Distinct())
                    Debug.WriteLine(s.Content);
            }
        }
    }

    public static void Signal(Ant ant, int entityId, string message, LogMessage.LogChannel logChannel) {
        var time = new TimeSpan(DateTimeOffset.Now.ToUnixTimeSeconds()); 
        LogMessages.Add(new LogMessage {
           Timestamp = time,
           Ant = ant,
           Channel = logChannel,
           Content = $"[ * * {logChannel} * * ]({time}) {entityId} - {message}"
       }); 
        
       Debug.WriteLine(LogMessages.Last().Content);
    } 
    
    private LogMessage createSignage(Ant ant, GameTime gameTime, int entityId = 0) {
        String content = "";
        LogMessage.LogChannel logChannel;

        switch (ant.CurrentState) { 
            case Ant.State.Idle:
                logChannel = LogMessage.LogChannel.Job;
                content = $"The ant {entityId} is actually making nothing instead of beeing an {ant.Type}.";
                break;
            case Ant.State.Scouting:
                logChannel = LogMessage.LogChannel.Job;
                content = $"The {ant.Type} ant number {entityId} is now wandering around the garden.";
                break;
            case Ant.State.Attacking:
                logChannel = LogMessage.LogChannel.Fight;
                content = $"The {ant.Type} ant number {entityId} is fighting !";
                break;
            case Ant.State.Dying:
                logChannel = LogMessage.LogChannel.LifeCycle;
                content = $"The {ant.Type} ant number {entityId} is Dead. RIP Little ant..";
                break;            
            case Ant.State.Gathering:
                logChannel = LogMessage.LogChannel.Job;
                content = $"The {ant.Type} ant number {entityId} is Gathering resources.";
                break;
            case Ant.State.GoTo:
            default:
                logChannel = LogMessage.LogChannel.Info;
                content = $"The state of the {ant.Type} ant number {entityId} has changed to {ant.CurrentState}.";
                break;
        }
        
        return new LogMessage {
            Channel = LogMessage.LogChannel.Job,
            Ant = ant,
            Content = $"[ * * {logChannel} * * ]({gameTime.TotalGameTime}) - {content}",
            Timestamp = gameTime.TotalGameTime
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