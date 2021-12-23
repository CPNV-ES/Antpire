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
        public float ZoomCamera = 1f;

    }

    internal class SimulationScreen : GameScreen {
        private readonly float CAMERA_SPEED = 400.0f;

        private World world;
        private SimulationState simState;
        
        // Textures
        private Texture2D aphidAliveTexture;
        private Texture2D aphidDeadTexture;
        private Texture2D antAliveTexture;
        private Texture2D antDeadTexture;
        private Texture2D anthillTexture;

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
            antDeadTexture = Content.Load<Texture2D>("ant/alive");
            anthillTexture = Content.Load<Texture2D>("anthill/anthill");

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
            if (keyboardState.IsKeyDown(Keys.F3)) {
                simState.ZoomCamera += 0.05f;
            }
            if (keyboardState.IsKeyDown(Keys.F4)) {
                if (simState.ZoomCamera >= 0.5f) {
                    simState.ZoomCamera -= 0.05f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.F5)) {
                simState.ZoomCamera = 1f;
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
            var aphids = new List<Point>() { new(600, 0), new(800, 0), new(200, 200), new(300, 300) };
            foreach (var pos in aphids) {
                var aphid = world.CreateEntity();
                aphid.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

                //50% that the aphid appears as deadborn
                if (r.Next(0, 2) != 0) {
                    aphid.Attach(new Renderable {
                        RenderItem = new SpriteRenderable(100, aphidAliveTexture)
                    });
                }
                else {
                    aphid.Attach(new Renderable {
                        RenderItem = new SpriteRenderable(100, aphidDeadTexture)
                    });
                }
            }


            // Init ants inside the garden
            var ants = new List<Point>() { new(400, 400), new(420, 400), new(440, 400), new(460, 400) };

            foreach (var pos in ants) {
                var ant = world.CreateEntity();
                ant.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

                //50% that the aphid appears as deadborn
                if (r.Next(0, 2) != 0) {
                    ant.Attach(new Renderable {
                        RenderItem = new SpriteRenderable(100, antAliveTexture)
                    });
                }
                else {
                    ant.Attach(new Renderable {
                        RenderItem = new SpriteRenderable(100, antDeadTexture)
                    });
                }
            }

            // Init the anthill
            var anthills = new List<Point>() { new(500, 400) };

            foreach (var pos in anthills) {
                var anthill = world.CreateEntity();
                anthill.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

                anthill.Attach(new Renderable {
                    RenderItem = new SpriteRenderable(500, anthillTexture)
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

            // Init bushes
            var bushesPositions = new Point[] { new (180, 560), new(410, 70), new(1150, 500) };
            foreach (var pos in bushesPositions) {
                // Generate the bush's leaves
                var leavesPositions = new List<Vector2>() { new(0, 0) };
                var fruitsPositions = new List<Vector2>();

                for (int i = 0; i < r.Next(6, 18); i++) {
                    leavesPositions.Add(ShapeUtils.GetRandomPointInCircle(50));
                    
                    // Generate fruits
                    for (int f = 0; f < r.Next(0, 5); f++) {
                        fruitsPositions.Add(ShapeUtils.GetRandomPointInCircle(40) + leavesPositions.Last());                
                    }
                }
                
                var bush = world.CreateEntity();
                var fruits = world.CreateEntity();
                bush.Attach(new SimulationPosition { Position = pos, WorldSpace= WorldSpace.Garden });
                bush.Attach(new Renderable {
                    RenderItem = new RenderablesGroup {
                        Children = leavesPositions.Select(x => 
                            (
                                new CircleRenderable { Sides = 32, Color = Color.DarkGreen, Thickness = 50.0f, Radius = 50 - (int)Math.Sqrt(x.X*x.X + x.Y*x.Y)/3 } as IRenderable,
                                x.ToPoint()
                            )
                        ).ToArray()
                    }
                });
                var colors = new Color[] { Color.Red, Color.GreenYellow, Color.OrangeRed };
                fruits.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
                fruits.Attach(new Renderable {
                    RenderItem = new RenderablesGroup {
                        Children = fruitsPositions.Select(x =>
                            (
                                new CircleRenderable { Sides = 32, Color = colors[r.Next(0, 3)], Thickness = 8.0f, Radius = r.Next(4, 8) } as IRenderable,
                                x.ToPoint()
                            )
                        ).ToArray()
                    }
                });
            }
        }
    }
}
