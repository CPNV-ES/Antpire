using Antpire.Screens;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;

namespace Antpire.Systems; 

internal class UserInputsSystem : UpdateSystem {
    private readonly float CAMERA_SPEED = 400.0f;
    private readonly SimulationState simulationState;
    private Point mouseLastPosition;
    private float scrollWheelLastValue;

    public UserInputsSystem(SimulationState state) {
        simulationState = state;
        mouseLastPosition = new Point();
    }

    public override void Update(GameTime gameTime) {
        var keyboardState = Keyboard.GetState();
        var dt = gameTime.GetElapsedSeconds();

        if (keyboardState.IsKeyDown(Keys.F1)) {
            simulationState.CurrentWorldSpace = WorldSpace.Anthill;
        }
        if (keyboardState.IsKeyDown(Keys.F2)) {
            simulationState.CurrentWorldSpace = WorldSpace.Garden;
        }
        if (keyboardState.IsKeyDown(Keys.F3)) {
            simulationState.CurrentCameraState.Zoom *= 1.02f * dt * 60.0f;
        }
        if (keyboardState.IsKeyDown(Keys.F4)) {
            simulationState.CurrentCameraState.Zoom *= 0.98f * dt * 60.0f;
        }
        if (keyboardState.IsKeyDown(Keys.F5)) {
            simulationState.CurrentCameraState.Zoom = 1f;
        }

        if (keyboardState.IsKeyDown(Keys.Left)) {
            simulationState.CurrentCameraState.Position -= new Vector2(1, 0) * dt * CAMERA_SPEED * (1 / simulationState.CurrentCameraState.Zoom);
        }
        if (keyboardState.IsKeyDown(Keys.Right)) {
            simulationState.CurrentCameraState.Position += new Vector2(1, 0) * dt * CAMERA_SPEED * (1 / simulationState.CurrentCameraState.Zoom);
        }
        if (keyboardState.IsKeyDown(Keys.Up)) {
            simulationState.CurrentCameraState.Position -= new Vector2(0, 1) * dt * CAMERA_SPEED * (1 / simulationState.CurrentCameraState.Zoom);
        }
        if (keyboardState.IsKeyDown(Keys.Down)) {
            simulationState.CurrentCameraState.Position += new Vector2(0, 1) * dt * CAMERA_SPEED * (1 / simulationState.CurrentCameraState.Zoom);
        }

        var mouseState = Mouse.GetState();
        if(mouseState.RightButton == ButtonState.Pressed) {
            simulationState.CurrentCameraState.Position += (mouseLastPosition.ToVector2() - mouseState.Position.ToVector2()) * (1 / simulationState.CurrentCameraState.Zoom);
        }
    
            
        if(Math.Abs(scrollWheelLastValue - mouseState.ScrollWheelValue) > float.Epsilon) {
            simulationState.CurrentCameraState.Zoom += (mouseState.ScrollWheelValue - scrollWheelLastValue) / 10.0f * dt;
        }

        scrollWheelLastValue = mouseState.ScrollWheelValue;
        mouseLastPosition = mouseState.Position;
    }
}