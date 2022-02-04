using Antpire.Components;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Screens;

namespace Antpire.Systems {
    /* 
     * Just as example
     */
    internal class AnthillSimulationSystem : EntityUpdateSystem {
        private readonly SimulationState simulationState;

        private ComponentMapper<SimulationPosition> simPositionMapper;

        public AnthillSimulationSystem(SimulationState state) : base(Aspect.All(typeof(SimulationPosition))) {
            simulationState = state;
        }

        public override void Initialize(IComponentMapperService mapperService) {
            simPositionMapper = mapperService.GetMapper<SimulationPosition>();
        }

        public override void Update(GameTime gameTime) {
            foreach (var entityId in ActiveEntities) {
                var pos = simPositionMapper.Get(entityId);
                if (pos.WorldSpace != WorldSpace.Anthill) {
                    continue;
                }
            }
        }
    }
}
