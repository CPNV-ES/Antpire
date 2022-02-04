using MonoGame.Extended.Screens;
using Myra;
using Myra.Graphics2D.UI;
using Antpire.Screens.Windows;

namespace Antpire.Screens {
    internal class MainMenuScreen : GameScreen {
        private Desktop desktop;
        private MainMenuScreenMainMenuWindow mainWindow;

        public MainMenuScreen(Game game) : base(game) {
            MyraEnvironment.Game = game;
        }

        public override void LoadContent() {
            base.LoadContent();

            desktop = new Desktop();
            CreateMainWindow();
        }

        private void CreateMainWindow() {
            mainWindow = new MainMenuScreenMainMenuWindow(desktop);
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
}
