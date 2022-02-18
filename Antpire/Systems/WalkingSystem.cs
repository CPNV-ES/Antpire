using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Antpire.Screens;

namespace Antpire.Systems
{
    internal class WalkingSystem : EntityUpdateSystem
    {
        private ComponentMapper<SimulationPosition> simulationPosition;
        private ComponentMapper<Insect> insectMapper;
        private SimulationState simState;

        private int GARDEN_WIDTH = 800; 
        private int GARDEN_HEIGHT = 800;

        public WalkingSystem(SimulationState simState) : base(Aspect.All(typeof(Insect), typeof(SimulationPosition))) {
            this.simState = simState;
            GARDEN_WIDTH = simState.GardenGenerationOptions.Width * simState.GardenGenerationOptions.ChunkSize;
            GARDEN_HEIGHT = simState.GardenGenerationOptions.Height * simState.GardenGenerationOptions.ChunkSize;
        }

        public override void Initialize(IComponentMapperService mapperService) {
            simulationPosition = mapperService.GetMapper<SimulationPosition>();
            insectMapper = mapperService.GetMapper<Insect>();
        }

        public override void Update(GameTime gameTime) {
            foreach (var entityId in ActiveEntities) {
                var entity = simulationPosition.Get(entityId);
                var insect = insectMapper.Get(entityId);
                Vector2 location = new Vector2(entity.Position.X, entity.Position.Y);

                if (insect.shouldChangeDestination)
                    changeDirection(insect, location);

                walks(entity, insect, location);
            }
        }

        private void walks(SimulationPosition entity, Insect insect, Vector2 location) {
            Vector2 newPosition = location + Vector2.Multiply(insect.velocity, 2f);
            
            // TODO: Clean this code up
            if(newPosition.X < 0 || newPosition.X > GARDEN_WIDTH || newPosition.Y < 0 || newPosition.Y > GARDEN_HEIGHT) {
                var rot = entity.Rotation + MathF.PI;
                var vec = Vector2.One.Rotate(rot);
                vec = new Vector2((float)MathF.Cos(rot), (float)MathF.Sin(rot));
                var newTarget = location + vec*100;
                insect.changeDestinationTo(newTarget);
                entity.Rotation = rot;
                newPosition = location + Vector2.Multiply(insect.velocity, 2f);
            }
            
            entity.Position = newPosition;
        }

        private void changeDirection(Insect insect, Vector2 location) {
            insect.velocity = Vector2.Subtract(insect.destination, location);
            insect.velocity.Normalize();

            insect.shouldChangeDestination = false;
        }
    }
}
