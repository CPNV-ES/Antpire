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
        
        // Textures
        private Texture2D aphidAliveTexture;
        private Texture2D aphidDeadTexture;
        private Texture2D antAliveTexture;

        public SimulationScreen(Game game) : base(game) {
            simState = new SimulationState { CurrentWorldSpace = WorldSpace.Garden };
            world = new WorldBuilder()
                .AddSystem(new SimulationRenderSystem(GraphicsDevice, simState))
                .Build();
            Game.Components.Add(world);
        }

        public override void LoadContent() {
            aphidAliveTexture = Content.Load<Texture2D>("aphid/alive");
            aphidDeadTexture = Content.Load<Texture2D>("aphid/dead");
            antAliveTexture = Content.Load<Texture2D>("ant/alive");
            initTestMap();
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            world.Draw(gameTime);

            System.Diagnostics.Debug.WriteLine(Mouse.GetState().Position.ToString());
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
            var rockPositions = new List<Point>() { new(87, 120), new(340, 234), new(620, 680), new(1000, 320), new(1000, 120)};
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

            // Init trunks in the garden
            var treeTrunks = new List<Point>() { new(120, 650), new(140, 370), new(320, 335), new(950, 620), new(800, 220) };
            foreach (var pos in treeTrunks) {
                var trunk = world.CreateEntity();
                var trunkWidth = r.Next(20, 30);
                trunk.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
                trunk.Attach(new Renderable {
                    RenderItem = new RectangleRenderable(size: new(trunkWidth, (float)(trunkWidth * (r.NextDouble()*3+2))), (float)(r.NextDouble()*Math.PI*2), Color.Brown)
                });
            }

            // Init aphids inside the garden
            var aphids = new List<Point>() { new(600, 0), new(800, 0), new(200, 200), new(300, 300)  };
            foreach (var pos in aphids)
            {
                var aphid = world.CreateEntity();
                aphid.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

                //50% that the aphid appears as deadborn
                if (r.Next(0, 2) != 0)
                {
                    aphid.Attach(new Renderable
                    {
                        RenderItem = new SpriteRenderable(100, aphidAliveTexture)
                    });
                }
                else
                {
                    aphid.Attach(new Renderable
                    {
                        RenderItem = new SpriteRenderable(100, aphidDeadTexture)
                    });
                }
            }


            // Init ants inside the garden
            var ants = new List<Point>() { new(400, 400), new(420, 400), new(440, 400), new(460, 400) };

            foreach (var pos in ants)
            {
                var ant = world.CreateEntity();
                ant.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });


                ant.Attach(new Renderable
                {
                    RenderItem = new SpriteRenderable(100, antAliveTexture)
                });
            }

            // Init river
            var riverSegments = new Vector2[] { new(550, 250), new(510, 330), new(520, 430), new(490, 500), new(480, 610), new(530, 720) };
            var river = world.CreateEntity();
            river.Attach(new SimulationPosition { Position = new Point(0, 0), WorldSpace = WorldSpace.Garden });
            river.Attach(new Renderable { RenderItem = new PathRenderable { Color = Color.Blue, Segments = riverSegments, Thickness = 15 } });

            // Init piles of twigs
            var twigStackSegments = new Vector2[] { new(0, 0), new(30, 0), new(0, 4), new(30, 4), new(0, 8), new(30, 8), new(0, 12), new(30, 12) };
            var twigPositions = new Point[] { new(140, 160), new(300, 300), new(850, 300) };

            foreach(var pos in twigPositions) {
                var t = world.CreateEntity();
                t.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
                t.Attach(new Renderable { RenderItem = new LineStackRenderable { Segments = ShapeUtils.GenerateLineStack(20, 25), Color = Color.SandyBrown, Thickness = 1.0f } });
            }
        }
    }
}
