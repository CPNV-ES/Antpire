namespace Antpire.Components; 

[Serializable]
internal class LogMessage {
    public enum LogChannel {
        Info,
        Fight,
        Job,
        Resources,
        LifeCycle,
        EggsManagement,
        Sight
    }

    public LogChannel Channel;
    public Ant Ant;
    public string Content;
    public TimeSpan Timestamp;
}