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

namespace Antpire.Systems {
    internal class BouncingLogoRenderSystem : EntityDrawSystem {
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;

        private ComponentMapper<BouncingLogo> bouncingLogoMapper;
        private ComponentMapper<Transform2> transformMapper;

        public BouncingLogoRenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(BouncingLogo), typeof(Transform2))) {
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService) {
            bouncingLogoMapper = mapperService.GetMapper<BouncingLogo>();
            transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            foreach (var entityId in ActiveEntities) { 
                var logo = bouncingLogoMapper.Get(entityId);
                var transform = transformMapper.Get(entityId);

                spriteBatch.Draw(logo.Sprite, new Rectangle((int)transform.Position.X, (int)transform.Position.Y, (int)logo.Size.X, (int)logo.Size.Y), Color.White);
            }

            spriteBatch.End();
        }

    }
}
