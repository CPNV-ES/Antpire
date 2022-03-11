using MonoGame.Extended.Screens;
using Myra;
using Myra.Graphics2D.UI;
using Antpire.Screens.Windows;

namespace Antpire.Screens; 

internal class MainMenuScreen : GameScreen {
    private Desktop desktop;
    private MainMenuScreenMainMenuWindow mainWindow;
    private SimulationScreen simulationScreen;

    public MainMenuScreen(Game game, SimulationScreen simulationScreen) : base(game) {
        MyraEnvironment.Game = game;
        this.simulationScreen = simulationScreen;
    }

    public override void LoadContent() {
        base.LoadContent();

        desktop = new Desktop();
        CreateMainWindow();
    }

    private void CreateMainWindow() {
        mainWindow = new MainMenuScreenMainMenuWindow(desktop, simulationScreen);
        mainWindow.Closed += (sender, args) => {
            CreateMainWindow();
        };
        desktop.Root = mainWindow;
    }

    public override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);
        desktop.Render();
    }

    public override void Update(GameTime gameTime) {
            
    }
}