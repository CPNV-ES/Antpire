namespace Antpire.Components; 

[Serializable]
internal class Signaling {
    public enum Channel {
        Info,
        Fight,
        Job,
        Ressources,
        LifeCycle,
        EggsManagement,
        Sight
    }

    public Channel channel;

    public Ant ant;

    public String content;

    public TimeSpan timestamp;
}