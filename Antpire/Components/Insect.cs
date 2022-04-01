namespace Antpire.Components; 

[Serializable]
internal class Insect {
    public Vector2 Velocity = new Vector2();
    public Vector2 Destination { get; set; } = new Vector2(1, 1);
    public List<Hitbox> NewHitboxesInSight = new List<Hitbox>();
    public List<Hitbox> HitboxesInSight = new List<Hitbox>();
}