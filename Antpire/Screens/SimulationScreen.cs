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
using Microsoft.Xna.Framework.Input;
using Antpire.Utils;

namespace Antpire.Screens {
    internal class SimulationState {
        public WorldSpace CurrentWorldSpace;
        public Vector2 CurrentCameraPosition = new Vector2(0,0);
    }

    internal class SimulationScreen : GameScreen {
        private readonly float CAMERA_SPEED = 400.0f;

        private World world;
        private SimulationState simState;

        public SimulationScreen(Game game) : base(game) {
            simState = new SimulationState { CurrentWorldSpace = WorldSpace.Anthill };
            world = new WorldBuilder()
                .AddSystem(new SimulationRenderSystem(GraphicsDevice, simState))
                .Build();
            Game.Components.Add(world);

            initTestMap();
        }

        public override void LoadContent() {
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            world.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {
            var keyboardState = Keyboard.GetState();
            var dt = gameTime.GetElapsedSeconds();

            if(keyboardState.IsKeyDown(Keys.F1)) {
                simState.CurrentWorldSpace = WorldSpace.Anthill;
            }
            if (keyboardState.IsKeyDown(Keys.F2)) {
                simState.CurrentWorldSpace = WorldSpace.Garden;
            }

            if (keyboardState.IsKeyDown(Keys.Left)) {
                simState.CurrentCameraPosition -= new Vector2(1, 0) * dt * CAMERA_SPEED;
            }
            if (keyboardState.IsKeyDown(Keys.Right)) {
                simState.CurrentCameraPosition += new Vector2(1, 0) * dt * CAMERA_SPEED;
            }
            if (keyboardState.IsKeyDown(Keys.Up)) {
                simState.CurrentCameraPosition -= new Vector2(0, 1) * dt * CAMERA_SPEED;
            }
            if (keyboardState.IsKeyDown(Keys.Down)) {
                simState.CurrentCameraPosition += new Vector2(0, 1) * dt * CAMERA_SPEED;
            }

            world.Update(gameTime);
        }

        /// <summary>
        /// Initialize a test map with every kind of entity
        /// </summary>
        private void initTestMap() {
            var r = new Random();

            // Init rocks in the garden
            var rockPositions = new List<Point>() { new(87, 120), new(340, 234), new(620, 435), new(300, 420), new(500, 120)};
            foreach(var pos in rockPositions) {
                var rock = world.CreateEntity();
                rock.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
                rock.Attach(new Renderable {
                    RenderItem = new PolygonRenderable {
                        Color = Color.DarkGray,
                        Polygon = new Polygon(ShapeUtils.GenerateConvexPolygon(r.Next(5, 10), r.Next(30, 70)))
                    }
                });
            }
        }
    }
}
