using Antpire.Components;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Screens;
using Antpire.Drawing;

namespace Antpire.Systems; 

internal class SimulationRenderSystem : EntityDrawSystem {
    private readonly GraphicsDevice graphicsDevice;
    private readonly DrawBatch drawBatch;
    private readonly SimulationState simulationState;
    private readonly OrthographicCamera camera;

    private ComponentMapper<SimulationPosition> simPositionMapper; 
    private ComponentMapper<Renderable> renderableMapper;

    public SimulationRenderSystem(GraphicsDevice graphicsDevice, SimulationState state) : base(Aspect.All(typeof(SimulationPosition), typeof(Renderable))) {
        this.graphicsDevice = graphicsDevice;
        simulationState = state;
        drawBatch = new DrawBatch(graphicsDevice);

        camera = new OrthographicCamera(graphicsDevice);
    }

    public override void Initialize(IComponentMapperService mapperService) {
        simPositionMapper = mapperService.GetMapper<SimulationPosition>();
        renderableMapper = mapperService.GetMapper<Renderable>();
    }

    public override void Draw(GameTime gameTime) {
        camera.Position = simulationState.CurrentCameraState.Position;
        camera.Zoom = simulationState.CurrentCameraState.Zoom;

        var transformMatrix = camera.GetViewMatrix();
        drawBatch.Begin(transformMatrix: transformMatrix);
        if(simulationState.CurrentWorldSpace == WorldSpace.Anthill) {
            graphicsDevice.Clear(Color.SaddleBrown);
        }
        else {
            graphicsDevice.Clear(Color.ForestGreen);
        }

        var viewRegion = camera.BoundingRectangle.ToRectangle();
            
        foreach (var entityId in ActiveEntities) {
            var pos = simPositionMapper.Get(entityId);
            var render = renderableMapper.Get(entityId);

            if(pos.WorldSpace == simulationState.CurrentWorldSpace && 
               viewRegion.Intersects(new Rectangle(pos.Position.ToPoint(), render.RenderItem.BoundingBox))) 
            {
                render.RenderItem.Render(drawBatch, new Transform2(pos.Position, pos.Rotation));
            }
        }

        drawBatch.End();
    }
}