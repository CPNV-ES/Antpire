using System.IO;
using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Thickness = Myra.Graphics2D.Thickness;

namespace Antpire.Screens.Windows; 

public partial class MainMenuScreenGameLoadWindow {
	private SimulationScreen simulationScreen;
	
	public MainMenuScreenGameLoadWindow(SimulationScreen simulationScreen) {
		BuildUI();
		this.simulationScreen = simulationScreen;

		foreach(var f in simulationScreen.GetSaveNames()) {
			var saveName = Path.GetFileNameWithoutExtension(f.Name);
			var imageTextButton = new ImageTextButton();
			imageTextButton.Text = $"{saveName} - {f.LastWriteTime}";
			imageTextButton.Padding = new Thickness(5);
			imageTextButton.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			imageTextButton.Click += (sender, args) => {
				simulationScreen.LoadWorld(saveName);
				this.Close();
			};
		
			container.Widgets.Add(imageTextButton);
		}
	}
}