
using Myra.Graphics2D.UI;

namespace Antpire.Screens.Windows;

public partial class SimulationUI {
	private Desktop desktop;

	public SimulationUI(Desktop desktop) {
		BuildUI();

		this.desktop = desktop;

		Pause.Click += (s, a) => {
			desktop.GetWidgetByID("PauseWindow").Visible = true;
		};
	}
}