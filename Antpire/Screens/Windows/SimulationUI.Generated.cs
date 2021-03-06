/* Generated by MyraPad at 04.02.2022 09:29:07 */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
#endif

namespace Antpire.Screens.Windows
{
	partial class SimulationUI: Panel
	{
		private void BuildUI()
		{
			Pause = new ImageTextButton();
			Pause.Text = "Pause";
			Pause.LabelHorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
			Pause.Padding = new Thickness(5);
			Pause.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			Pause.Id = "Pause";

			var panel1 = new Panel();
			panel1.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Top;
			panel1.Padding = new Thickness(5);
			panel1.Background = new SolidBrush("#202020FF");
			panel1.Widgets.Add(Pause);

			
			Widgets.Add(panel1);
		}

		
		public ImageTextButton Pause;
	}
}
