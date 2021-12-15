using Antpire.Components;
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

namespace Antpire.Systems {
    internal class SimulationRenderSystem : EntityDrawSystem {
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;
        private readonly SimulationState simulationState;

        private ComponentMapper<SimulationPosition> simPositionMapper; 
        private ComponentMapper<Renderable> renderableMapper;

        public SimulationRenderSystem(GraphicsDevice graphicsDevice, SimulationState state) : base(Aspect.All(typeof(SimulationPosition), typeof(Renderable))) {
            this.graphicsDevice = graphicsDevice;
            simulationState = state;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService) {
            simPositionMapper = mapperService.GetMapper<SimulationPosition>();
            renderableMapper = mapperService.GetMapper<Renderable>();
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            
            foreach (var entityId in ActiveEntities) {
                var pos = simPositionMapper.Get(entityId);
                var render = renderableMapper.Get(entityId);
 
                if (pos.WorldSpace == WorldSpace.Anthill && simulationState.CurrentWorldSpace == WorldSpace.Anthill) {
                    render.RenderItem.Render(spriteBatch, pos.Position);
                }
                else if (pos.WorldSpace == WorldSpace.Garden && simulationState.CurrentWorldSpace == WorldSpace.Garden) {
                    render.RenderItem.Render(spriteBatch, pos.Position);
                }
            }

            spriteBatch.End();
        }

    }
}
