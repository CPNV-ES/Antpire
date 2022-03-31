namespace Antpire.Components; 

[Serializable]
internal class Signaling {
    public enum Channels {
        Fight,
        Working,
        LifeCycle,
        EggsManagement,
        EnemySpotted
    }

    public Ant ant;

    public String content;

    public TimeSpan timestamp;
}