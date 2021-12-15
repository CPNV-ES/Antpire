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

namespace Antpire.Systems {
    internal class SimulationRenderSystem : EntityDrawSystem {
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;
        private readonly SimulationState simulationState;

        private ComponentMapper<SimulationPosition> simPositionMapper;

        public SimulationRenderSystem(GraphicsDevice graphicsDevice, SimulationState state) : base(Aspect.All(typeof(SimulationPosition))) {
            this.graphicsDevice = graphicsDevice;
            simulationState = state;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService) {
            simPositionMapper = mapperService.GetMapper<SimulationPosition>();
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            
            foreach (var entityId in ActiveEntities) {
                var pos = simPositionMapper.Get(entityId);
                if (pos.WorldSpace == WorldSpace.Anthill && simulationState.CurrentWorldSpace == WorldSpace.Anthill) {
                    spriteBatch.DrawCircle(new CircleF(new Point2(pos.Position.X, pos.Position.Y), 10f), 64, Color.Black);
                }
                else if (pos.WorldSpace == WorldSpace.Garden && simulationState.CurrentWorldSpace == WorldSpace.Garden) {
                    spriteBatch.DrawCircle(new CircleF(new Point2(pos.Position.X, pos.Position.Y), 10f), 64, Color.Red);
                }
            }

            spriteBatch.End();
        }

    }
}
