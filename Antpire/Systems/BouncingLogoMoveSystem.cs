using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Antpire.Components;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

namespace Antpire.Systems; 

internal class BouncingLogoMoveSystem : EntityUpdateSystem {
    private ComponentMapper<BouncingLogo> bouncingLogoMapper;
    private ComponentMapper<Transform2> transformMapper;

    private GraphicsDevice graphicsDevice;

    public BouncingLogoMoveSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(BouncingLogo), typeof(Transform2))) {
        this.graphicsDevice = graphicsDevice;
    }

    public override void Initialize(IComponentMapperService mapperService) {
        bouncingLogoMapper = mapperService.GetMapper<BouncingLogo>();
        transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime) {
        foreach(var entityId in ActiveEntities) {
            var transform = transformMapper.Get(entityId);
            var bouncingLogo = bouncingLogoMapper.Get(entityId);

            var going = new Transform2(transform.Position.X + bouncingLogo.Velocity.X, transform.Position.Y + bouncingLogo.Velocity.Y);

            if (going.Position.X < 0 || going.Position.X + bouncingLogo.Size.X > graphicsDevice.Viewport.Width) {
                bouncingLogo.Velocity.X *= -1;
            }

            if (going.Position.Y < 0 || going.Position.Y + bouncingLogo.Size.Y > graphicsDevice.Viewport.Height) {
                bouncingLogo.Velocity.Y *= -1;
            }

            transform.Position = going.Position;
        }
    }
}