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
    /* 
     * Just as example
     */
    internal class GardenSimulationSystem : EntityUpdateSystem {
        private readonly SimulationState simulationState;

        private ComponentMapper<SimulationPosition> simPositionMapper;

        public GardenSimulationSystem(SimulationState state) : base(Aspect.All(typeof(SimulationPosition))) {
            simulationState = state;
        }

        public override void Initialize(IComponentMapperService mapperService) {
            simPositionMapper = mapperService.GetMapper<SimulationPosition>();
        }

        public override void Update(GameTime gameTime) {
            foreach (var entityId in ActiveEntities) {
                var pos = simPositionMapper.Get(entityId);
                if (pos.WorldSpace != WorldSpace.Garden) {
                    continue;
                }
            }
        }
    }
}
