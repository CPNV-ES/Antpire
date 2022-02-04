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

                //insect.changeDestinationTo(new Vector2(100, 100));


                Vector2 location = new Vector2(entity.Position.X, entity.Position.Y);

                if (insect.shouldChangeDestination)
                {
                    insect.velocity = Vector2.Subtract(location / insect.destination, location);
                    insect.velocity.Normalize();
                    insect.velocity = Vector2.Multiply(insect.velocity, 1);

                    insect.shouldChangeDestination = false;
                }

                Vector2 newPosition = Vector2.Add(location, insect.velocity);

                entity.Position = new Point((int)newPosition.X, (int)newPosition.Y);
            }
        }
    }
}
