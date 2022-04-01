using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Components;

[Serializable]
internal class Hitbox {
    public string Name = "";
    public bool BlocksMovement = false;
    public bool BlocksSight = false;
    public Polygon Polygon;
}