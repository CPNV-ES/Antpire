namespace Antpire;

internal record CameraState {
    private float zoom = 1.0f;
    public Vector2 Position { get; set; } = new();
    public float Zoom {
        get => zoom;
        set {
            if (value is <= 3f and >= 0.1f)
                zoom = value;
        }
    }
}