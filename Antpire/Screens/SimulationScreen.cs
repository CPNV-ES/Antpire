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
using Myra.Graphics2D.UI;
using Antpire.Screens.Windows;
using Microsoft.Xna.Framework.Content;
using static Antpire.Antpire;

namespace Antpire.Screens {
    internal record CameraState {
        private float zoom = 1.0f;
        public Vector2 Position { get; set; } = new();
        public float Zoom {
            get => zoom;
            set {
                if (value <= 4.5f && value >= 0.5f)
                    zoom = value;
            }
        }
    }

    internal class SimulationState {
        public CameraState GardenCameraState { get; set; } = new();
        public CameraState AnthillCameraState { get; set; } = new();
        public CameraState CurrentCameraState => CurrentWorldSpace == WorldSpace.Anthill ? AnthillCameraState : GardenCameraState;
        public WorldSpace CurrentWorldSpace;
        public AnthillInteriorGridMap AnthillInteriorGridMap { get; set; }
        public float TimeScale { get; set; } = 1.0f;
    }

    internal class SimulationScreen : GameScreen {
        private World world;
        private Desktop desktop;
        private Window mainPauseWindow;
        private SimulationUI ui;
        private Panel mainPanel;
        private ContentProvider contentProvider;

        public SimulationState SimulationState;
        
        public SimulationScreen(Game game) : base(game) {
            SimulationState = new SimulationState { CurrentWorldSpace = WorldSpace.Garden };
            world = new WorldBuilder()
                .AddSystem(new SimulationRenderSystem(GraphicsDevice, SimulationState))
                .AddSystem(new UserInputsSystem(SimulationState))
                .Build();
            contentProvider = game.Services.GetService<ContentProvider>();
        }

        public override void LoadContent() {
            base.LoadContent();
            
            SimulationState.AnthillInteriorGridMap = new AnthillInteriorGridMap {
                Grid = new AnthillInteriorGridMap.TileState[256, 256],
                TilesRenderables = new Dictionary<AnthillInteriorGridMap.TileState, IRenderable> {
                    { AnthillInteriorGridMap.TileState.Dug, new SpriteRenderable(64, contentProvider.Get<Texture2D>("anthill_interior/dug_tile")) },
                    { AnthillInteriorGridMap.TileState.Wall, new SpriteRenderable(64, contentProvider.Get<Texture2D>("anthill_interior/wall_tile")) },
                },
                TileWidth = 64
            };

            desktop = new Desktop();
            mainPanel = new Panel();
            desktop.Root = mainPanel;

            mainPauseWindow = new PauseWindow(desktop);
            mainPauseWindow.ZIndex = 1;
            mainPanel.AddChild(mainPauseWindow);

            ui = new SimulationUI(desktop);
            ui.ZIndex = 2;
            mainPanel.AddChild(ui);

            mainPauseWindow.Visible = false;
        }

        public override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            world.Draw(gameTime);
            desktop.Render();
        }

        public override void Update(GameTime gameTime) {
            var keyboardState = Keyboard.GetState();
            var dt = gameTime.GetElapsedSeconds();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                if (!mainPauseWindow.Visible)
                    mainPauseWindow.Visible = true;
            }

            SimulationState.TimeScale = mainPauseWindow.Visible ? 0 : SimulationState.TimeScale;

            world.Update(gameTime);
        }

        public void InitProcGen() {
            var genOpts = new GardenGenerator.GardenGenerationOptions(); // TODO: Get this from game config window
            var gg = new GardenGenerator(Game, genOpts); 

            gg.GenerateGarden(world);
        }

        public void InitTestMaps() {
            initTestMapAnthill();
            initTestMapGarden();
        }
        
        // Initialize the garden part of the test map with every kind of entity
        private void initTestMapAnthill() {
            var r = new Random();

            var sky = world.CreateEntity();
            sky.Attach(new SimulationPosition { Position = new(0, -500), WorldSpace = WorldSpace.Anthill });
            sky.Attach(new Renderable {
                RenderItem = new RectangleRenderable(new(10000, 500), 0.0f, Color.SkyBlue, 300.9f)
            });

            var gridmap = world.CreateEntity();
            gridmap.Attach(new SimulationPosition { Position = new(0, 0), WorldSpace = WorldSpace.Anthill });
            gridmap.Attach(new Renderable {
                RenderItem = new AnthillInteriorGridmapRenderable { GridMap = SimulationState.AnthillInteriorGridMap }
            });

            // Entrance hall
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(8, 3), new(5, 3)), AnthillInteriorGridMap.TileState.Dug);

            // Entrance tunnel
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(10, 0), new(1, 3)), AnthillInteriorGridMap.TileState.Dug);

            // Main tunnel
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(10, 6), new(1, 6)), AnthillInteriorGridMap.TileState.Dug);

            // Warehouse
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(14, 3), new(5, 3)), AnthillInteriorGridMap.TileState.Dug);

            // Food in warehouse
            var foodPositions = new Point[] { new Point(14 * 64 + 32, 3 * 64 + 32), new Point(15 * 64 + 32, 3 * 64 + 32), new Point(16 * 64 + 32, 3 * 64 + 32), new Point(17 * 64 + 32, 3 * 64 + 32) };
            foreach (var foodPos in foodPositions) {
                var food = world.CreateEntity();
                var radius = r.Next(4, 28);
                food.Attach(new SimulationPosition { Position = new Point(foodPos.X, foodPos.Y), WorldSpace = WorldSpace.Anthill });
                food.Attach(new Renderable {
                    RenderItem = new CircleRenderable { Sides = 32, Color = Color.DarkOliveGreen, Thickness = radius, Radius = radius }
                });
            }

            // Materials in warehouse
            var materialsPositions = new Point[] { new Point(18 * 64 + 32, 3 * 64 + 32), new Point(18 * 64 + 32, 4 * 64 + 32), new Point(17 * 64 + 32, 4 * 64 + 32) };
            foreach (var materialPos in materialsPositions) {
                var food = world.CreateEntity();
                var radius = r.Next(4, 28);
                food.Attach(new SimulationPosition { Position = new Point(materialPos.X, materialPos.Y), WorldSpace = WorldSpace.Anthill });
                food.Attach(new Renderable {
                    RenderItem = new CircleRenderable { Sides = 32, Color = Color.SaddleBrown, Thickness = radius, Radius = radius }
                });
            }

            // Entrance to warehouse
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(13, 4), new(1, 1)), AnthillInteriorGridMap.TileState.Dug);

            // Queens room
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(15, 7), new(7, 6)), AnthillInteriorGridMap.TileState.Dug);

            // Queen in queens room
            var queen = world.CreateEntity();
            queen.Attach(new SimulationPosition { Position = new Point(17*64, 9*64), WorldSpace = WorldSpace.Anthill });
            queen.Attach(new Renderable {
                RenderItem = new SpriteRenderable(150, contentProvider.Get<Texture2D>("queen/alive"))
            });

            // Entrance to warehouse
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(11, 8), new(4, 1)), AnthillInteriorGridMap.TileState.Dug);

            // Nursery 
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(12, 10), new(2, 3)), AnthillInteriorGridMap.TileState.Dug);

            // Entrance to nursery
            SimulationState.AnthillInteriorGridMap.FillRectangle(new Rectangle(new(14, 11), new(1, 1)), AnthillInteriorGridMap.TileState.Dug);

            // Egg in nursery
            var eggPositions = new Point[] { new Point(12 * 64, 10 * 64), new Point(12 * 64 + 16, 10 * 64), new Point(12 * 64, 10 * 64 + 16), new Point(13 * 64, 10 * 64) };
            foreach(var eggPos in eggPositions) {
                var egg = world.CreateEntity();
                egg.Attach(new SimulationPosition { Position = new Point(eggPos.X, eggPos.Y), WorldSpace = WorldSpace.Anthill });
                egg.Attach(new Renderable {
                    RenderItem = new SpriteRenderable(20, contentProvider.Get<Texture2D>("anthill_interior/egg"))
                });
            }

            // Ant 
            var ant = world.CreateEntity();
            ant.Attach(new SimulationPosition { Position = new Point(10 * 64, 8 * 64), WorldSpace = WorldSpace.Anthill });
            ant.Attach(new Renderable {
                RenderItem = new SpriteRenderable(75, contentProvider.Get<Texture2D>("ant/alive"))
            });
        }

        // Initialize the garden part of the test map with every kind of entity
        private void initTestMapGarden() {
            var r = new Random();

            // Init rocks in the garden
            var rockPositions = new List<Point>() { new(87, 120), new(340, 234), new(620, 680), new(1000, 320), new(1000, 120)};
            foreach(var pos in rockPositions) {
                var rock = world.CreateEntity();
                rock.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
                rock.Attach(new Renderable {
                    RenderItem = new PolygonRenderable {
                        Color = Color.DarkGray,
                        Polygon = new Polygon(ShapeUtils.GenerateConvexPolygon(r.Next(5, 10), r.Next(30, 70))),
                        Thickness = 5.0f
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
                    RenderItem = new RectangleRenderable(
                        size: new(trunkWidth, (float)(trunkWidth * (r.NextDouble()*3+2))), 
                        rotation: (float)(r.NextDouble()*Math.PI*2), 
                        color: Color.SaddleBrown,
                        thickness: 15.0f
                    )
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
                        RenderItem = new SpriteRenderable(100, contentProvider.Get<Texture2D>("aphid/alive"))
                    });
                }
                else {
                    aphid.Attach(new Renderable {
                        RenderItem = new SpriteRenderable(100, contentProvider.Get<Texture2D>("aphid/dead"))
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
                        RenderItem = new SpriteRenderable(100, contentProvider.Get<Texture2D>("ant/alive"))
                    });
                }
                else {
                    ant.Attach(new Renderable {
                        RenderItem = new SpriteRenderable(100, contentProvider.Get<Texture2D>("ant/alive"))
                    });
                }
            }

            // Init the anthill
            var anthills = new List<Point>() { new(500, 400) };

            foreach (var pos in anthills) {
                var anthill = world.CreateEntity();
                anthill.Attach(new SimulationPosition { Position = new Point(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

                anthill.Attach(new Renderable {
                    RenderItem = new SpriteRenderable(500, contentProvider.Get<Texture2D>("anthill/Anthill"))
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
