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
                Vector2 location = new Vector2(entity.Position.X, entity.Position.Y);


                scouting(entity, insect, location, gameTime);

                if (insect.shouldChangeDestination)
                    changeDirection(insect, location);

                walks(entity, insect, location);
            }
        }

        private void scouting(SimulationPosition entity, Insect insect, Vector2 location, GameTime gameTime)
        {
            /* Should I set this code inside the WalkingSystem or inside the Insect (or ant) component ?


            */

            /*
            int counter = 1;
            int limit = 50;
            float countDuration = 2f; //every  2s.
            float currentTime = 0f;

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

            if (currentTime >= countDuration)
            {
                counter++;
                currentTime -= countDuration; // "use up" the time
                                              //any actions to perform
            }
            if (counter >= limit)
            {
                insect.changeDestinationTo(new Vector2(50, 50));
                counter = 0;//Reset the counter;
                            //any actions to perform
            }*/
        }

        private void walks(SimulationPosition entity, Insect insect, Vector2 location)
        {
            Vector2 newPosition = Vector2.Add(location, insect.velocity);
            entity.Position = new Point((int)newPosition.X, (int)newPosition.Y);
        }

        private void changeDirection(Insect insect, Vector2 location)
        {
            insect.velocity = Vector2.Subtract(location / insect.destination, location);
            insect.velocity.Normalize();
            insect.velocity = Vector2.Multiply(insect.velocity, 1);

            insect.shouldChangeDestination = false;
        }
    }
}
