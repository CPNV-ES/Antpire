namespace Antpire.Components; 

[Serializable]
internal class Ant {
    public enum State {
        Idle,
        Attacking,
        Scouting,
        Dying,
        Gathering,
        GoTo
    }   
    
    public enum AntType {
        Worker,
        Farmer,
        Nurse,
        Queen,
        Scout,
        Warrior
    }
        
    public State OldState = State.Idle;
    public State CurrentState = State.Scouting;

    public AntType Type = AntType.Scout;

    public float MinUpdateFrequency = 1/60.0f*5.0f;
    public float TimeTilNextUpdate = 0.0f;
}