using Antpire.Components;
using Antpire.Utils;

namespace Antpire;

internal class SimulationState {
    public CameraState GardenCameraState { get; set; } = new();
    public CameraState AnthillCameraState { get; set; } = new();
    public CameraState CurrentCameraState => CurrentWorldSpace == WorldSpace.Anthill ? AnthillCameraState : GardenCameraState;
    public WorldSpace CurrentWorldSpace;
    public AnthillInteriorGridMap AnthillInteriorGridMap { get; set; }
    public float TimeScale { get; set; } = 1.0f;
    public GardenGenerator.GardenGenerationOptions GardenGenerationOptions { get; set; } = new();
}