using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Antpire.Utils;
using MonoGame.Extended;
using Myra;

namespace Antpire.Screens.Windows;

public partial class MainMenuScreenGameConfigWindow {
	private struct AnthillConfigData {
		public bool IsPlayer;
		public Color Color;
		
		public int Scouts; 
		public int Soldiers; 
		public int Workers;  
		public int Farmers;

		public override string ToString() => "Super Anthill";
	}

	private enum GardenSize {
		Tiny,
		Small,
		Medium,
		Big,
		Huge,
		Gigantic,
		Colossal,
	}

	private struct ConfigData {
		public ConfigData() { }

		public GardenSize GardenSize = GardenSize.Medium;
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
        
        [Category("Build Resources")] public float TwigsFrequency = 10f;
        [Category("Build Resources")] public int TwigsMinSize = 10;
        [Category("Build Resources")] public int TwigsMaxSize = 15;
        
        public List<AnthillConfigData> Anthills { get; } = new List<AnthillConfigData>();
        
        private static string getSeed() => Guid.NewGuid().GetHashCode().ToString();
	}

	public MainMenuScreenGameConfigWindow(SimulationScreen simulationScreen) {
		BuildUI();

		propertyGrid.Object = new ConfigData();

		ConfirmGameParamsButton.Click += (o, e) => {
			var config = (ConfigData)propertyGrid.Object;
			var chunkCount = getGardenChunkCount(config.GardenSize); 
			simulationScreen.InitializeNewGame(new GardenGenerator.GardenGenerationOptions {
				Seed = config.Seed,
				Height = chunkCount,
				Width = chunkCount,
				ChunkSize = 768,
				RockSize = new Range<int>(config.MinRockSize, config.MaxRockSize),
				
			});
		};
	}

	private int getGardenChunkCount(GardenSize size) =>
		size switch {
			GardenSize.Tiny => 3,
			GardenSize.Small => 4,
			GardenSize.Medium => 5,
			GardenSize.Big => 8,
			GardenSize.Huge => 10,
			GardenSize.Gigantic => 12,
			GardenSize.Colossal => 15,
			_ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
		};
}