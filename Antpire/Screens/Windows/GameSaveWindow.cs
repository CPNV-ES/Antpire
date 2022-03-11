using System.IO;
using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Thickness = Myra.Graphics2D.Thickness;

namespace Antpire.Screens.Windows; 

public partial class GameSaveWindow {
	public GameSaveWindow(SimulationScreen simulationScreen) {
		BuildUI();

		foreach(var f in simulationScreen.GetSaveNames()) {
			var saveName = Path.GetFileNameWithoutExtension(f.Name);
			var imageTextButton = new ImageTextButton();
			imageTextButton.Text = $"{saveName} - {f.LastWriteTime}";
			imageTextButton.Padding = new Thickness(5);
			imageTextButton.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			imageTextButton.Click += (sender, args) => {
				saveNameInput.Text = saveName;
			};
		
			container.Widgets.Add(imageTextButton);
		}

		saveBtn.Click += (sender, args) => {
			if(saveNameInput.Text.Trim().Length == 0) return;
			
			simulationScreen.SaveWorld(saveNameInput.Text);
			Close();
		};
	}
}