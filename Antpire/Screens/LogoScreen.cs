using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using Antpire.Systems;
using Antpire.Components;
using MonoGame.Extended;

namespace Antpire.Screens {
    internal class LogoScreen : GameScreen {
        private Texture2D logoSprite;
        private World world;
    
        public LogoScreen(Game game) : base(game) {
            world = new WorldBuilder()
                .AddSystem(new BouncingLogoMoveSystem(GraphicsDevice))
                .AddSystem(new BouncingLogoRenderSystem(GraphicsDevice))
                .Build();
            Game.Components.Add(world);
        }

        public override void LoadContent() {
            logoSprite = Content.Load<Texture2D>("cpnv_logo");

            var logo = world.CreateEntity();
            logo.Attach(new Transform2());
            logo.Attach(new BouncingLogo {
                Sprite = logoSprite,
                Velocity = new Vector2(2, 2),
                Size = new Vector2(840/5, 468/5)
            });

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            world.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {
            world.Update(gameTime);
        }
    }
}
