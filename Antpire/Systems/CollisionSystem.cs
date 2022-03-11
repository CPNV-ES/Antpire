using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using MonoGame.Extended;

namespace Antpire.Systems; 

internal class CollisionSystem : EntityUpdateSystem {
    private ComponentMapper<Hitbox> hitboxMapper;

    private Random rand = new Random();
    private SimulationState simState;

    public static List<Hitbox> hitboxes = new List<Hitbox>();
    public CollisionSystem(SimulationState simulationState) : base(Aspect.All(typeof(Hitbox))) {
        simState = simulationState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        hitboxMapper = mapperService.GetMapper<Hitbox>();
    }

    public override void Update(GameTime gameTime) {
        
        foreach(var entityId in ActiveEntities) {
            var hitbox= hitboxMapper.Get(entityId);

            if (!hitboxes.Contains(hitbox))
                hitboxes.Add(hitbox);
        }
    }
}