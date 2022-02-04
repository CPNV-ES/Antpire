using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra;
using MonoGame.Extended.Screens;

namespace Antpire.Screens.Windows;

public partial class PauseWindow {
	private Desktop desktop;

	public PauseWindow(Desktop desktop) {
		BuildUI();

		this.desktop = desktop;

		var hide = () => {
			Visible = false;
		};

		var goToMainMenu = () => {
			var g = (Antpire)MyraEnvironment.Game;
			g.GoToMainMenu();
		};

		Continue.Click += (s, a) => hide();

		Closed += (s, a) => hide();

		Load.Click += (s, a) => {
			var test = new MainMenuScreenGameLoadWindow();
			desktop.Widgets.Add(test);
		};

		SaveAs.Click += (s, a) => {
			var test = new GameSaveWindow();
			desktop.Widgets.Add(test);
		};

		SaveAndQuit.Click += (s, a) => goToMainMenu();

		Quit.Click += (s, a) => goToMainMenu();
	}
}