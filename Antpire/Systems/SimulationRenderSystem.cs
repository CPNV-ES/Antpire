﻿using Antpire.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antpire.Screens;
using Antpire.Drawing;
using MonoGame.Extended.ViewportAdapters;

namespace Antpire.Systems {
    internal class SimulationRenderSystem : EntityDrawSystem {
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;
        private readonly SimulationState simulationState;
        private readonly OrthographicCamera camera;

        private ComponentMapper<SimulationPosition> simPositionMapper; 
        private ComponentMapper<Renderable> renderableMapper;

        public SimulationRenderSystem(GraphicsDevice graphicsDevice, SimulationState state) : base(Aspect.All(typeof(SimulationPosition), typeof(Renderable))) {
            this.graphicsDevice = graphicsDevice;
            simulationState = state;
            spriteBatch = new SpriteBatch(graphicsDevice);

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
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            if(simulationState.CurrentWorldSpace == WorldSpace.Anthill) {
                graphicsDevice.Clear(Color.SaddleBrown);
            }
            else {
                graphicsDevice.Clear(Color.ForestGreen);
            }

            var zoomMagnification = 1 / simulationState.CurrentCameraState.Zoom;
            var zoomScale = simulationState.CurrentCameraState.Zoom - 1;
            var viewRegion = new Rectangle { 
                X = (int)(simulationState.CurrentCameraState.Position.X + zoomScale * graphicsDevice.Viewport.Bounds.Size.X * zoomMagnification / 2),
                Y = (int)(simulationState.CurrentCameraState.Position.Y + zoomScale * graphicsDevice.Viewport.Bounds.Size.Y * zoomMagnification / 2), 
                Width = (int)(graphicsDevice.Viewport.Bounds.Size.X * zoomMagnification),
                Height = (int)(graphicsDevice.Viewport.Bounds.Size.Y * zoomMagnification),
            };

            foreach (var entityId in ActiveEntities) {
                var pos = simPositionMapper.Get(entityId);
                var render = renderableMapper.Get(entityId);
 
                if (pos.WorldSpace == WorldSpace.Anthill && simulationState.CurrentWorldSpace == WorldSpace.Anthill) {
                    render.RenderItem.Render(spriteBatch, new Transform2(pos.Position), viewRegion);
                }
                else if (pos.WorldSpace == WorldSpace.Garden && simulationState.CurrentWorldSpace == WorldSpace.Garden) {
                    render.RenderItem.Render(spriteBatch, new Transform2(pos.Position, pos.Rotation), viewRegion);
                }
            }

            spriteBatch.End();
        }

    }
}
