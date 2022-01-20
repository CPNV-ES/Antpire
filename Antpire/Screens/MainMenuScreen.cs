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
using Myra;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
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
