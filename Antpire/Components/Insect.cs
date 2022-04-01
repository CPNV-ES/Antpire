namespace Antpire.Components; 

[Serializable]
internal class Insect {
    public Vector2 Velocity = new Vector2();
    public Vector2 Destination { get; set; } = new Vector2(1, 1);
    public List<CollisionBody> NewCollisionBodiesInSight = new List<CollisionBody>();
    public List<CollisionBody> CollisionBodiesInSight = new List<CollisionBody>();
}