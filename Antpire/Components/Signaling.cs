namespace Antpire.Components; 

[Serializable]
internal class Signaling {
    public enum Channel {
        Info,
        Fight,
        Working,
        LifeCycle,
        EggsManagement,
        EnemySpotted
    }

    public Channel channel;

    public Ant ant;

    public String content;

    public TimeSpan timestamp;
}