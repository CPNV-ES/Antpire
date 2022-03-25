using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using MonoGame.Extended;

namespace Antpire.Systems; 

internal class CollisionSystem : EntityUpdateSystem {
    private ComponentMapper<Hitbox> hitboxMapper;
    private ComponentMapper<SimulationPosition> simPosMapper;

    private Random rand = new Random();
    private SimulationState simState;
    
    private List<int> registeredEntities = new List<int>(); // Kinda hacky, can't handle moving collision boxes

    public static List<Hitbox> hitboxes = new List<Hitbox>();
    public CollisionSystem(SimulationState simulationState) : base(Aspect.All(typeof(Hitbox), typeof(SimulationPosition))) {
        simState = simulationState;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        hitboxMapper = mapperService.GetMapper<Hitbox>();
        simPosMapper = mapperService.GetMapper<SimulationPosition>();
        
    }

    public override void Update(GameTime gameTime) {
        foreach(var entityId in ActiveEntities) {
            var hitbox = hitboxMapper.Get(entityId);

            if(!registeredEntities.Contains(entityId)) {
                var pos = simPosMapper.Get(entityId);
                hitbox.polygon = hitbox.polygon.TransformedCopy(pos.Position, 0.0f, Vector2.One);
                hitboxes.Add(hitbox);
                registeredEntities.Add(entityId);
            }
        }
    }
}