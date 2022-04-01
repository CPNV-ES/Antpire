using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
using MonoGame.Extended.Collections;
using Newtonsoft.Json;
using static Antpire.Antpire;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Antpire.Screens;
public class SimulationScreen : GameScreen {
    private World world;
    private Desktop desktop;
    private Window mainPauseWindow;
    private SimulationUI ui;
    private Panel mainPanel;
    private ContentProvider contentProvider;
    private string lastSaveFileName;
    private Antpire antpire;
    
    public SimulationState SimulationState;

    public SimulationScreen(Antpire game) : base(game) {
        antpire = game;
        SimulationState = new SimulationState { CurrentWorldSpace = WorldSpace.Garden };
        initWorld();
        contentProvider = game.Services.GetService<ContentProvider>();
    }

    private void initWorld() {
        world = new WorldBuilder()
            .AddSystem(new SimulationRenderSystem(GraphicsDevice, SimulationState))
            .AddSystem(new UserInputsSystem(SimulationState))
            .AddSystem(new WalkingSystem(SimulationState))
            .AddSystem(new SightSystem(SimulationState))
            .AddSystem(new AntLogicSystem(SimulationState))
            .AddSystem(new CollisionSystem(SimulationState))
            .Build();
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

        mainPauseWindow = new PauseWindow(desktop, this);
        mainPauseWindow.ZIndex = 1;
        mainPanel.AddChild(mainPauseWindow);

        ui = new SimulationUI(desktop) {
            ZIndex = 2
        };
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

        SimulationState.Paused = mainPauseWindow.Visible;

        
        // TODO: Make it easier for Systems to use the SimulationState's TimeScale
        //       Maybe inject TimeScale into a new 'ScaledGameTime' object, so that systems 
        //       can use a single property to scale on delta time AND the time scale. 
        world.Update(gameTime);
    }

    public void InitProcGen(GardenGenerator.GardenGenerationOptions options = default) {
        SimulationState.GardenGenerationOptions = options;
        initWorld();
        var gg = new GardenGenerator(Game, SimulationState.GardenGenerationOptions);
        gg.GenerateGarden(world);
    }

    public void InitTestMaps() {
        initTestMapAnthill();
        initTestMapGarden();
    }

    [Serializable]
    public struct SaveFormat {
        public SimulationState SimulationState;
        public List<List<object>> Entities;
    }
    
    /// <summary>
    /// Saves the game state to a file.
    /// </summary>
    /// <param name="saveName"></param>
    public void SaveWorld(string saveName) {
        // Init property, field and method info
        var pi_entityManager = typeof(World).GetProperty("EntityManager", BindingFlags.Instance | BindingFlags.NonPublic);
        var pi_componentManager = typeof(Entity).GetField("_componentManager", BindingFlags.Instance | BindingFlags.NonPublic);
        var fi_componentTypes = typeof(ComponentManager).GetField("_componentTypes", BindingFlags.Instance | BindingFlags.NonPublic);
        var mi_getMapper = typeof(ComponentManager).GetMethods().First(x => x.Name == "GetMapper" && !x.IsGenericMethod);
        
        var filePath = Path.Combine(SaveDataDir, $"{saveName}.json");
        using var sf = File.CreateText(filePath);
        
        var output = new SaveFormat { SimulationState = this.SimulationState, Entities = new List<List<object>>() }; 
        
        // Get our World's EntityManager
        var entityManager = pi_entityManager.GetValue(world) as EntityManager;
        
        foreach(var entityId in entityManager.Entities) {
            var entity = world.GetEntity(entityId);
            
            if(entity == null) continue;
            
            var componentsList = new List<object>();    // List of components we're going to serialize
            
            var componentManager = pi_componentManager.GetValue(entity) as ComponentManager;
            var componentTypes = fi_componentTypes.GetValue(componentManager) as Dictionary<Type, int>;

            int c = -1;
            foreach(var id in componentTypes.Values) {
                c++;
                if((entity.ComponentBits.Data >> c & 1) == 0) continue;
              
                // Get a ComponentMapper<T> where T is the type of the component
                dynamic mapper = mi_getMapper.Invoke(componentManager, new object[] { id }); 
                componentsList.Add(mapper.Components[entity.Id]);
            }
            output.Entities.Add(componentsList);
        }
        
        var serialized = JsonConvert.SerializeObject(output, Formatting.Indented , new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        sf.Write(serialized);
        lastSaveFileName = saveName;
    }

    /// <summary>
    /// Save the game state to the latest used save file.
    /// If the current game hasn't been saved yet, cancels the operation and returns false.                                                         
    /// </summary>
    /// <returns>False if there was no last save file to write to</returns>
    public bool QuickSave() {
        if(String.IsNullOrEmpty(lastSaveFileName)) return false;
        SaveWorld(lastSaveFileName);
        return true;
    }

    /// <summary>
    /// Loads a game state from the specified file.
    /// </summary>
    /// <param name="saveName"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public void LoadWorld(string saveName) {
        var mi_attach = typeof(Entity).GetMethods().First(x => x.Name == "Attach" && x.IsGenericMethod);
        
        var filePath = Path.Combine(SaveDataDir, $"{saveName}.json");
        if(!File.Exists(filePath)) throw new FileNotFoundException("The save file could not be found.");
        
        var s = File.ReadAllText(filePath);
        SaveFormat save = JsonConvert.DeserializeObject<SaveFormat>(s, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        // Load the simulation state
        SimulationState = save.SimulationState;
        
        // Reset the world and its systems
        initWorld();

        // Load the entities
        foreach(var entity in save.Entities) {
            var e = world.CreateEntity();
            foreach(var component in entity) {
                // Call implicit generic version of the method because it would be attached as an object otherwise
                mi_attach.MakeGenericMethod(component.GetType()).Invoke(e, new object[] { component });   
                
                // Load the sprite if it's a SpriteRenderable
                if(component is Renderable { RenderItem: SpriteRenderable sr }) {
                    sr.Texture = contentProvider.Get<Texture2D>(sr.TexturePath.Replace("\\", "/"));
                }
            }
        }
        
        lastSaveFileName = saveName;
        antpire.LoadSimulationScreen();
    }
    
    /// <summary>
    /// Gets the all save files' FileInfos
    /// </summary>
    /// <returns></returns>
    public FileInfo[] GetSaveNames() {
        var files = Directory.GetFiles(SaveDataDir, "*.json").Select(x => new FileInfo(x));
        return files.OrderByDescending(x => x.LastWriteTime).ToArray();
    }

    public void InitializeNewGame(GardenGenerator.GardenGenerationOptions options) {
        antpire.LoadSimulationScreen();
        InitProcGen(options);
        SimulationState.CurrentWorldSpace = WorldSpace.Garden;
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
            food.Attach(new SimulationPosition { Position = new(foodPos.X, foodPos.Y), WorldSpace = WorldSpace.Anthill });
            food.Attach(new Renderable {
                RenderItem = new CircleRenderable { Sides = 32, Color = Color.DarkOliveGreen, Thickness = radius, Radius = radius }
            });
        }

        // Materials in warehouse
        var materialsPositions = new Point[] { new Point(18 * 64 + 32, 3 * 64 + 32), new Point(18 * 64 + 32, 4 * 64 + 32), new Point(17 * 64 + 32, 4 * 64 + 32) };
        foreach (var materialPos in materialsPositions) {
            var food = world.CreateEntity();
            var radius = r.Next(4, 28);
            food.Attach(new SimulationPosition { Position = new(materialPos.X, materialPos.Y), WorldSpace = WorldSpace.Anthill });
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
        queen.Attach(new SimulationPosition { Position = new(17*64, 9*64), WorldSpace = WorldSpace.Anthill });
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
            egg.Attach(new SimulationPosition { Position = new(eggPos.X, eggPos.Y), WorldSpace = WorldSpace.Anthill });
            egg.Attach(new Renderable {
                RenderItem = new SpriteRenderable(20, contentProvider.Get<Texture2D>("anthill_interior/egg"))
            });
        }

        // Ant 
        var ant = world.CreateEntity();
        ant.Attach(new SimulationPosition { Position = new(10 * 64, 8 * 64), WorldSpace = WorldSpace.Anthill });
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
            rock.Attach(new SimulationPosition { Position = new (pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
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
            trunk.Attach(new SimulationPosition { Position = new(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
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
            aphid.Attach(new SimulationPosition { Position = new(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
                
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
            ant.Attach(new SimulationPosition { Position = new(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });
            ant.Attach(new Ant());

            //50% that the ant appears as deadborn
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

        // Init one wandering ant
        var wanderingAnts = new List<Point>() { new(600, 600), new(600, 600), new(600, 600), new(600, 600), new(600, 600), new(600, 600), new(600, 600), new(600, 600) };

        foreach (var pos in wanderingAnts)
        {
            var wanderingAnt = world.CreateEntity();
            wanderingAnt.Attach(new Ant());
            wanderingAnt.Attach(new Insect());

            wanderingAnt.Attach(new SimulationPosition { Position = new(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

            wanderingAnt.Attach(new Renderable
            {
                RenderItem = new SpriteRenderable(1.0f, contentProvider.Get<Texture2D>("ant/alivev2"))
            });
        }

        // Init the anthill
        var anthills = new List<Point>() { new(500, 400) };

        foreach (var pos in anthills) {
            var anthill = world.CreateEntity();
            anthill.Attach(new SimulationPosition { Position = new(pos.X, pos.Y), WorldSpace = WorldSpace.Garden });

            anthill.Attach(new Renderable {
                RenderItem = new SpriteRenderable(500, contentProvider.Get<Texture2D>("anthill/Anthill"))
            });
        }

        // Init river
        var riverSegments = new Vector2[] { new(550, 250), new(510, 330), new(520, 430), new(490, 500), new(480, 610), new(530, 720) };
        var river = world.CreateEntity();
        river.Attach(new SimulationPosition { Position = new(0, 0), WorldSpace = WorldSpace.Garden });
        river.Attach(new Renderable { RenderItem = new SmoothPathRenderable { Color = Color.Blue, Segments = riverSegments, Thickness = 15 } });

        // Init piles of twigs
        var twigPositions = new Vector2[] { new(140, 160), new(300, 300), new(850, 300) };

        foreach(var pos in twigPositions) {
            var t = world.CreateEntity();
            t.Attach(new SimulationPosition { Position = pos, WorldSpace = WorldSpace.Garden });
            t.Attach(new Renderable { RenderItem = new LineStackRenderable { Segments = ShapeUtils.GenerateLineStack(20, 25), Color = Color.SandyBrown, Thickness = 1.0f } });
        }

        // Init bushes
        var bushesPositions = new Vector2[] { new (180, 560), new(410, 70), new(1150, 500) };
        foreach(var bushPos in bushesPositions) {
            var pos = bushPos; 
            // Generate the bush's leaves
            var leavesPositions = new List<Vector2>() { new(0, 0) };
            var fruitsPositions = new List<Vector2>();

            var colors = new Color[] { Color.Red, Color.GreenYellow, Color.OrangeRed };
            for (int b = 0; b < 10; b++) {
                leavesPositions.Add(ShapeUtils.GetRandomPointInCircle(50));
            }

            foreach(var leaf in leavesPositions) {
               var leafEntity = world.CreateEntity(); 
               leafEntity.Attach(new SimulationPosition { Position = pos + new Vector2(leaf.X, leaf.Y), WorldSpace = WorldSpace.Garden });
               leafEntity.Attach(new Renderable {
                   RenderItem = new CircleRenderable { Sides = 32, Color = Color.DarkGreen, Thickness = 50.0f, Radius = 50 - (int)Math.Sqrt(leaf.X*leaf.X + leaf.Y*leaf.Y)/3 } as IRenderable,
               });
            
                
               // Generate fruits
               for (int f = 0; f < 2; f++) {
                    fruitsPositions.Add(ShapeUtils.GetRandomPointInCircle(30) + new Vector2(leaf.X, leaf.Y) + new Vector2(60, 60));                
               }
            }
            
            foreach(var fruit in fruitsPositions) {
               var fruitEntity = world.CreateEntity(); 
               fruitEntity.Attach(new SimulationPosition { Position = pos + new Vector2(fruit.X, fruit.Y), WorldSpace = WorldSpace.Garden });
               fruitEntity.Attach(new Renderable {
                   RenderItem = new CircleRenderable { Sides = 32, Color = colors[(int)fruit.X%3], Thickness = 16.0f, Radius = 4 } as IRenderable,
               });
            }
        }
    }
}