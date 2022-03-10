using System.Runtime.Serialization;
using Antpire.Components;
using Antpire.Utils;

namespace Antpire;
public class SimulationState {
    public CameraState GardenCameraState { get; set; } = new();
    public CameraState AnthillCameraState { get; set; } = new();
    public WorldSpace CurrentWorldSpace;
    public AnthillInteriorGridMap AnthillInteriorGridMap { get; set; }
    private float timeScale = 1.0f;
    public bool Paused { get; set; } = false;
    public GardenGenerator.GardenGenerationOptions GardenGenerationOptions { get; set; } = new();
    
    [IgnoreDataMember]
    public CameraState CurrentCameraState => CurrentWorldSpace == WorldSpace.Anthill ? AnthillCameraState : GardenCameraState;
    
    [IgnoreDataMember]
    public float TimeScale { 
        get => Paused ? 0 : timeScale;
        set => timeScale = value;
    }
}