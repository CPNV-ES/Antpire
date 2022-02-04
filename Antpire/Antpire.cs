using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Linq;

namespace Antpire;
public class Antpire : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private readonly ScreenManager screenManager;
    private Screens.SimulationScreen simulationScreen;
    private Screens.MainMenuScreen mainMenuScreen;

    public Antpire() {
        Window.AllowUserResizing = true;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        screenManager = new ScreenManager();
        Services.AddService(screenManager);
    }

    protected override void Initialize() {
        base.Initialize();

        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        graphics.ApplyChanges();

        simulationScreen = new Screens.SimulationScreen(this);
        mainMenuScreen = new Screens.MainMenuScreen(this);

        var args = System.Environment.GetCommandLineArgs()[1..];
        if(args.Contains("--start=test_anthill")) {
            loadSimulationScreen();
            simulationScreen.SimulationState.CurrentWorldSpace = WorldSpace.Anthill;
        }
        if (args.Contains("--start=test_garden")) {
            loadSimulationScreen();
            simulationScreen.SimulationState.CurrentWorldSpace = WorldSpace.Garden;
        }
        else {
            screenManager.LoadScreen(mainMenuScreen);
        }
    }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);

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

    private void loadSimulationScreen() {
        screenManager.LoadScreen(simulationScreen, new FadeTransition(GraphicsDevice, Color.Black));
    }

    public void GoToMainMenu() {
        screenManager.LoadScreen(mainMenuScreen, new FadeTransition(GraphicsDevice, Color.Black, .33f));
    }
}
