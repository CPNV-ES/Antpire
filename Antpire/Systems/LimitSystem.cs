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

namespace Antpire.Systems {
    internal class LimitSystem : EntityUpdateSystem {
        private ComponentMapper<SimulationPosition> simulationPosition;
        private ComponentMapper<Insect> insectMapper;
        private ComponentMapper<BorderLimit> borderLimitMapper;

        public LimitSystem() : base(Aspect.All(typeof(Insect), typeof(SimulationPosition), typeof(BorderLimit))) {

        }

        public override void Initialize(IComponentMapperService mapperService) {
            simulationPosition = mapperService.GetMapper<SimulationPosition>();
            insectMapper = mapperService.GetMapper<Insect>();
            borderLimitMapper = mapperService.GetMapper<BorderLimit>();
        }

        public override void Update(GameTime gameTime) {
            foreach (var entityId in ActiveEntities) {
                var entity = simulationPosition.Get(entityId);
                var insect = insectMapper.Get(entityId);
            }
        }
    }
}