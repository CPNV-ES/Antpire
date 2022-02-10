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
    internal class AntLogicSystem : EntityUpdateSystem
    {
        private ComponentMapper<SimulationPosition> simulationPosition;
        private ComponentMapper<Ant> AntMapper;
        private ComponentMapper<Insect> insectMapper;

        private Random rand = new Random();

        public AntLogicSystem() : base(Aspect.All(typeof(Ant), typeof(Insect), typeof(SimulationPosition)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            simulationPosition = mapperService.GetMapper<SimulationPosition>();
            AntMapper = mapperService.GetMapper<Ant>();
            insectMapper = mapperService.GetMapper<Insect>();
        }

        public override void Update(GameTime gameTime)
        {
            //Time passed since last Update() 

            foreach (var entityId in ActiveEntities)
            {

                var entity = simulationPosition.Get(entityId);
                var ant = AntMapper.Get(entityId);
                var insect = insectMapper.Get(entityId);

                Vector2 location = new Vector2(entity.Position.X, entity.Position.Y);

                ant.currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


                switch (ant.actualState)
                {
                    case Ant.State.Idle:
                        break;

                    case Ant.State.Scouting:
                        scouting(ant, insect, entity);
                        break;

                    case Ant.State.Attacking:
                        break;

                    case Ant.State.Dying:
                        break;
                }
            }
        }

        private void scouting(Ant ant, Insect insect, SimulationPosition entity) {

            if (ant.currentTime >= ant.countDuration) {
                ant.counter++;
                // "use up" the time
                ant.currentTime -= ant.countDuration; 
            }

            // Every x seconds the ants change the direction
            if (ant.counter >= ant.limit) {
                ant.counter = 0;

                // Ant changes the direction
                int newX = rand.Next(-100, 100);
                int newY = rand.Next(-100, 100);

                insect.changeDestinationTo(new Vector2(entity.Position.X + newX, entity.Position.Y + newY));

                // If the ant found somehing
                // -> Add the founded-thing and drop a home-drop 
            }
        }
    }
}
