/* Generated by MyraPad at 20.01.2022 13:17:55 */

using Myra;
using Myra.Graphics2D.UI;

namespace Antpire.Screens.Windows; 

public partial class MainMenuScreenMainMenuWindow {
	private Desktop desktop;
	public MainMenuScreenMainMenuWindow(Desktop desktop) {
		BuildUI();
			
		this.desktop = desktop;

		NewGameBtn.Click += (s, a) => {
			var test = new MainMenuScreenGameConfigWindow();
			desktop.Widgets.Add(test);
		};
            
		LoadGameBtn.Click += (s, a) => {
			var test = new MainMenuScreenGameLoadWindow();
			desktop.Widgets.Add(test);
		};

		QuitBtn.Click += (sender, args) => {
			MyraEnvironment.Game.Exit();
		};
	}
}