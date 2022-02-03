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

        public WalkingSystem() : base(Aspect.All(typeof(Ant), typeof(SimulationPosition)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            simulationPosition = mapperService.GetMapper<SimulationPosition>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var newPosition = simulationPosition.Get(entityId);

                /*
                var going = new Transform2(transform.Position.X + bouncingLogo.Velocity.X, transform.Position.Y + bouncingLogo.Velocity.Y);
                if (going.Position.X < 0 || going.Position.X + bouncingLogo.Size.X > graphicsDevice.Viewport.Width) {
                    bouncingLogo.Velocity.X *= -1;
                }

                if (going.Position.Y < 0 || going.Position.Y + bouncingLogo.Size.Y > graphicsDevice.Viewport.Height) {
                    bouncingLogo.Velocity.Y *= -1;
                }
                transform.Position = going.Position;

                */
                newPosition.Position = new Point(newPosition.Position.X + 1, newPosition.Position.Y + 1);
            }
        }
    }
}
