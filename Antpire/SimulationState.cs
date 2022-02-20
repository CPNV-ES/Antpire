using Antpire.Components;
using Antpire.Utils;

namespace Antpire;

internal class SimulationState {
    public CameraState GardenCameraState { get; set; } = new();
    public CameraState AnthillCameraState { get; set; } = new();
    public CameraState CurrentCameraState => CurrentWorldSpace == WorldSpace.Anthill ? AnthillCameraState : GardenCameraState;
    public WorldSpace CurrentWorldSpace;
    public AnthillInteriorGridMap AnthillInteriorGridMap { get; set; }
    private float timeScale = 1.0f;
    public float TimeScale { 
        get => Paused ? 0 : timeScale;
        set => timeScale = value;
    }
    public bool Paused { get; set; } = false;
    public GardenGenerator.GardenGenerationOptions GardenGenerationOptions { get; set; } = new();
}