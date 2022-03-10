namespace Antpire.Components; 

[Serializable]
internal class Insect {
    public Vector2 Velocity = new Vector2();
    public Vector2 Destination { get; set; } = new Vector2(1, 1);
}