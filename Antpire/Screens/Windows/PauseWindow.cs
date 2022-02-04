using Myra.Graphics2D.UI;
using Myra;

namespace Antpire.Screens.Windows;

public partial class PauseWindow {
	private Desktop desktop;

	public PauseWindow(Desktop desktop) {
		BuildUI();

		this.desktop = desktop;

		CloseKey = Microsoft.Xna.Framework.Input.Keys.F24;
		CloseButton.Visible = false;

		var goToMainMenu = () => {
			var g = (Antpire)MyraEnvironment.Game;
			g.GoToMainMenu();
		};

		Continue.Click += (s, a) => Close();

		Closing += (s, a) => {
			a.Cancel = true;
			Visible = false;
		};

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