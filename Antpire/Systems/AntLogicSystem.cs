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
    internal class AntLogicSystem : EntityUpdateSystem
    {
        private ComponentMapper<SimulationPosition> simulationPosition;
        private ComponentMapper<Ant> AntMapper;

        public AntLogicSystem() : base(Aspect.All(typeof(Ant), typeof(SimulationPosition)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            simulationPosition = mapperService.GetMapper<SimulationPosition>();
            AntMapper = mapperService.GetMapper<Ant>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var entity = simulationPosition.Get(entityId);
                var ant = AntMapper.Get(entityId);
                Vector2 location = new Vector2(entity.Position.X, entity.Position.Y);

                switch(ant.actualState)
                {
                    case Ant.State.Idle:
                        break;

                    case Ant.State.Scouting:
                        scouting(ant);
                        break;

                    case Ant.State.Attacking:
                        break;

                    case Ant.State.Dying:
                        break;
                }

            }
        }

        private void scouting(Ant ant) {
            // Every x seconds the ants change the direction

            // If the ant found somehing
            // -> Add the founded-thing and drop a home-drop 

        }
    }
}
