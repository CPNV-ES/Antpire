global using System;
global using System.Text;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using Microsoft.Xna.Framework;
using System.IO;
using Antpire.Utils;
using CommandLine;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MoreLinq;

namespace Antpire;
public class Antpire : Game {
    private GraphicsDeviceManager graphics;
    private readonly ScreenManager screenManager;
    private Screens.SimulationScreen simulationScreen;
    private Screens.MainMenuScreen mainMenuScreen;
    
    public static string DataDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Antpire");
    public static string SaveDataDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Antpire/SaveData");

    public Antpire() {
        Directory.CreateDirectory(DataDir);
        Directory.CreateDirectory(SaveDataDir);
        
        Window.AllowUserResizing = true;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        screenManager = new ScreenManager();
        Services.AddService(screenManager);
    }

    public class ContentProvider {
        private Dictionary<string, object> content;

        public ContentProvider(Dictionary<string, object> content) {
            this.content = content;
        } 
        
        public T Get<T>(string key) {
            return (T)content[key];
        }
        
        public void Add(string key, object value) {
            content.Add(key, value);
        }
    }

    public class Options {
        [Option("loadSaveFile", Required = false)]
        public string LoadSaveFile { get; set; } 
        
        [Option("testProcGen", Required = false)]
        public bool TestProcGen { get; set; }
    }

    protected override void Initialize() {
        base.Initialize();
        
        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        graphics.ApplyChanges();
        
        simulationScreen = new Screens.SimulationScreen(this);
        mainMenuScreen = new Screens.MainMenuScreen(this, simulationScreen);
        
        var hasLoadedScreen = false;
        Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs()[1..])
            .WithParsed<Options>(options => {
                if (!String.IsNullOrEmpty(options.LoadSaveFile)) {
                    simulationScreen.LoadWorld(options.LoadSaveFile);
                    LoadSimulationScreen();
                    hasLoadedScreen = true;
                }
                else if (options.TestProcGen) {
                    simulationScreen.InitProcGen();
                    simulationScreen.SimulationState.CurrentWorldSpace = WorldSpace.Garden;
                    LoadSimulationScreen();
                    hasLoadedScreen = true;
                }
            });
        
        if(!hasLoadedScreen)
            screenManager.LoadScreen(mainMenuScreen);
    }

    protected override void LoadContent() {
        var folders = new [] { "", "ant", "anthill", "anthill_interior", "aphid", "queen" };
        var dict = new Dictionary<string, object>();

        foreach(var f in folders) {
            dict = dict.Concat(Content.LoadContent(f)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        Services.AddService(new ContentProvider(dict));
    }

    protected override void Update(GameTime gameTime) {
        screenManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        screenManager.Draw(gameTime);

        base.Draw(gameTime);
    }

    public void LoadSimulationScreen() {
        screenManager.LoadScreen(simulationScreen, new FadeTransition(GraphicsDevice, Color.Black));
    }
    
    public void GoToMainMenu() {
        screenManager.LoadScreen(mainMenuScreen, new FadeTransition(GraphicsDevice, Color.Black, .33f));
    }
}
