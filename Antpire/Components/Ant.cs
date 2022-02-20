namespace Antpire.Components; 

internal class Ant {
    public enum State {
        Idle,
        Attacking,
        Scouting,
        Dying
    }   

    public State CurrentState = State.Scouting;

    public float MinUpdateFrequency = 0.5f;
    public float TimeTilNextUpdate = 0.0f;
}