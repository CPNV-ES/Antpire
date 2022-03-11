using Myra.Graphics2D.UI;
using Myra;

namespace Antpire.Screens.Windows;

public partial class PauseWindow {
	private Desktop desktop;
	private SimulationScreen simulationScreen;

	public PauseWindow(Desktop desktop, SimulationScreen simulationScreen) {
		BuildUI();

		this.desktop = desktop;
		this.simulationScreen = simulationScreen;

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

		Save.Click += (s, a) => {
			var success = simulationScreen.QuickSave();
			if(!success) {
				var saveAsWindow = new GameSaveWindow(simulationScreen);
				desktop.Widgets.Add(saveAsWindow);
			}
		};

		Load.Click += (s, a) => {
			var loadWindow = new MainMenuScreenGameLoadWindow(simulationScreen);
			desktop.Widgets.Add(loadWindow);
		};

		SaveAs.Click += (s, a) => {
			var saveAsWindow = new GameSaveWindow(simulationScreen);
			desktop.Widgets.Add(saveAsWindow);
		};

		SaveAndQuit.Click += (s, a) => {
			var success = simulationScreen.QuickSave();
			if(!success) {
				var saveAsWindow = new GameSaveWindow(simulationScreen);
				desktop.Widgets.Add(saveAsWindow);
				saveAsWindow.Closed += (sender, args) => goToMainMenu();
			}
			else {
				goToMainMenu();
			}
		};		

		Quit.Click += (s, a) => goToMainMenu();
	}
}