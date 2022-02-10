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

namespace Antpire.Systems
{
    internal class WalkingSystem : EntityUpdateSystem
    {
        private ComponentMapper<SimulationPosition> simulationPosition;
        private ComponentMapper<Insect> insectMapper;

        public WalkingSystem() : base(Aspect.All(typeof(Insect), typeof(SimulationPosition)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            simulationPosition = mapperService.GetMapper<SimulationPosition>();
            insectMapper = mapperService.GetMapper<Insect>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var entity = simulationPosition.Get(entityId);
                var insect = insectMapper.Get(entityId);
                Vector2 location = new Vector2(entity.Position.X, entity.Position.Y);

                if (insect.shouldChangeDestination)
                    changeDirection(insect, location);

                walks(entity, insect, location);
            }
        }

        private void walks(SimulationPosition entity, Insect insect, Vector2 location)
        {
            Vector2 newPosition = Vector2.Add(location, Vector2.Multiply(insect.velocity, 2f));
            entity.Position = new Point((int)newPosition.X, (int)newPosition.Y);
        }

        private void changeDirection(Insect insect, Vector2 location)
        {
            insect.velocity = Vector2.Subtract(insect.destination, location);
            insect.velocity.Normalize();

            insect.shouldChangeDestination = false;
        }
    }
}
