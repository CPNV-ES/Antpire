using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Antpire.Utils;
using MonoGame.Extended;
using Myra;

namespace Antpire.Screens.Windows;

public partial class MainMenuScreenGameConfigWindow {
	public struct AnthillConfigData {
		public bool IsPlayer;
		public Color Color;
		
		public int Scouts; 
		public int Soldiers; 
		public int Workers;  
		public int Farmers;

		public override string ToString() => "Super Anthill";
	}
	
	public struct ConfigData {
		public ConfigData() { }
		
        public int ChunkSize = 768;
        public int Width = 3;
        public int Height = 3;
        public string Seed = getSeed();
        
        [Category("Obstacles")] public float RocksFrequency = 7.0f;
        [Category("Obstacles")] public int MinRockSize = 30;
        [Category("Obstacles")] public int MaxRockSize = 120;
        [Category("Obstacles")] public float TrunksFrequency = 3.0f;
        [Category("Obstacles")] public int MinTrunkSize = 40;
        [Category("Obstacles")] public int MaxTrunkSize = 70;
        
        [Category("Food Resources")] public float BushesFrequency = 10f;
        [Category("Food Resources")] public int MinBushSize = 25;
        [Category("Food Resources")] public int MaxBushSize = 50;
        [Category("Food Resources")] public float FruitsFrequency = 33f;
        
        [Category("Build Resources")] public float TwigsFrequency = 10f;
        [Category("Build Resources")] public int TwigsMinSize = 10;
        [Category("Build Resources")] public int TwigsMaxSize = 15;
        
        public List<AnthillConfigData> Anthills { get; } = new List<AnthillConfigData>();
        
        private static string getSeed() => Guid.NewGuid().GetHashCode().ToString();
	}

	public MainMenuScreenGameConfigWindow() {
		BuildUI();

		propertyGrid.Object = new ConfigData();
		
		ConfirmGameParamsButton.Click += (o, e) => {
			var g = (Antpire)MyraEnvironment.Game;
			g.StartNewGame(new GardenGenerator.GardenGenerationOptions());
		};
	}
}