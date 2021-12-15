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
using Antpire.Drawing;
using MonoGame.Extended.Shapes;

namespace Antpire.Screens {
    internal class SimulationState {
        public WorldSpace CurrentWorldSpace;
    }

    internal class SimulationScreen : GameScreen {
        private SpriteBatch spriteBatch;
        private World world;
        private SimulationState simState;

        public SimulationScreen(Game game) : base(game) {
            simState = new SimulationState { CurrentWorldSpace = WorldSpace.Anthill };
            world = new WorldBuilder()
                .AddSystem(new SimulationRenderSystem(GraphicsDevice, simState))
                .Build();
            Game.Components.Add(world);

            var test = world.CreateEntity();
            test.Attach(new SimulationPosition { Position = new Point(10, 10), WorldSpace = WorldSpace.Garden });
            test.Attach(new Renderable { RenderItem = new CircleRenderable { Color = Color.Black, Radius = 10, Sides = 32 } });

            var test2 = world.CreateEntity();
            test2.Attach(new SimulationPosition { Position = new Point(10, 10), WorldSpace = WorldSpace.Anthill });
            test2.Attach(new Renderable { 
                RenderItem = new PolygonRenderable { 
                    Color = Color.Black, 
                    Polygon = new Polygon(new List<Vector2> { new (10, 10), new (20, 20), new (30, 75), new (23, 0) } ) 
                } 
            });
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
