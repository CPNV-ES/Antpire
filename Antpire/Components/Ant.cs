namespace Antpire.Components; 

[Serializable]
internal class Ant {
    public enum State {
        Idle,
        Attacking,
        Scouting,
        Dying
    }   

    public State CurrentState = State.Scouting;

    public float MinUpdateFrequency = 1/60.0f*5.0f;
    public float TimeTilNextUpdate = 0.0f;
}