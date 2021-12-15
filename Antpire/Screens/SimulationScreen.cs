using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using Antpire.Systems;
using Antpire.Components;
using MonoGame.Extended;

namespace Antpire.Screens {
    internal class SimulationState {
        public WorldSpace CurrentWorldSpace;
    }

    internal class SimulationScreen : GameScreen {
        private SpriteBatch spriteBatch;
        private World world;
        private SimulationState simState;

        public SimulationScreen(Game game) : base(game) {
            simState = new SimulationState { CurrentWorldSpace = WorldSpace.Garden };
            world = new WorldBuilder()
                .AddSystem(new SimulationRenderSystem(GraphicsDevice, simState))
                .Build();
            Game.Components.Add(world);

            var test = world.CreateEntity();
            test.Attach(new SimulationPosition { Position = new Vector2(10, 10), WorldSpace = WorldSpace.Garden });
        }

        public override void LoadContent() {
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
