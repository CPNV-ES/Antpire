using Antpire.Utils;
using Myra;

namespace Antpire.Screens.Windows;

public partial class MainMenuScreenGameConfigWindow {
	public MainMenuScreenGameConfigWindow() {
		BuildUI();
		ConfirmGameParamsButton.Click += (o, e) => {
			var g = (Antpire)MyraEnvironment.Game;
			g.StartNewGame(new GardenGenerator.GardenGenerationOptions());
		};
	}
}