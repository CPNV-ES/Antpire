using Myra;
using Myra.Graphics2D.UI;

namespace Antpire.Screens.Windows; 

public partial class MainMenuScreenMainMenuWindow {
	private Desktop desktop;
	private SimulationScreen simulationScreen;
	
	public MainMenuScreenMainMenuWindow(Desktop desktop, SimulationScreen simulationScreen) {
		BuildUI();
			
		this.desktop = desktop;
		this.simulationScreen = simulationScreen;

		NewGameBtn.Click += (s, a) => {
			var test = new MainMenuScreenGameConfigWindow(simulationScreen);
			desktop.Widgets.Add(test);
		};
            
		LoadGameBtn.Click += (s, a) => {
			var test = new MainMenuScreenGameLoadWindow(simulationScreen);
			desktop.Widgets.Add(test);
		};

		QuitBtn.Click += (sender, args) => {
			MyraEnvironment.Game.Exit();
		};
	}
}