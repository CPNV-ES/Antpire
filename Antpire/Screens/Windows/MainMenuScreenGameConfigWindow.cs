using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using Antpire.Utils;
using MonoGame.Extended;
using MoreLinq.Extensions;
using Myra;

namespace Antpire.Screens.Windows;

public partial class MainMenuScreenGameConfigWindow {
	private struct AnthillConfigData {
		public string Name = "New Anthill";
		public Color Color = Color.Azure;
		
		[Category("Ants")] public int Scouts = 8; 
		[Category("Ants")] public int Soldiers = 4; 
		[Category("Ants")] public int Workers = 16;  
		[Category("Ants")] public int Farmers = 16;
		
		public override string ToString() => Name;
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

	private enum Size {
		Small,
		Medium,
		Big
	}

	private enum Frequency {
		Desert,
		Sparse,
		Normal,
		Plentiful,
		Extreme,
	}

	private struct ConfigData {
		public ConfigData() { }

		public GardenSize GardenSize = GardenSize.Medium;
		public string Seed = getSeed();
        
		[Category("Obstacles")] public Frequency RocksFrequency = Frequency.Sparse;
		[Category("Obstacles")] public Size RockSize = Size.Medium;
        [Category("Obstacles")] public Frequency TrunksFrequency = Frequency.Desert;
		[Category("Obstacles")] public Size TrunkSize = Size.Medium;
        
        [Category("Food Resources")] public Frequency BushesFrequency = Frequency.Normal;
		[Category("Food Resources")] public Size BushSize = Size.Medium;
        
        [Category("Construction Resources")] public Frequency TwigsFrequency = Frequency.Normal;
		[Category("Construction Resources")] public Size TwigSize = Size.Medium;
        
        public List<AnthillConfigData> Anthills { get; } = new List<AnthillConfigData> {
	        new AnthillConfigData { Name = "Player Anthill" },
	        new AnthillConfigData { Name = "Opponent Anthill 1" },
        };
        
        private static string getSeed() => Guid.NewGuid().GetHashCode().ToString();
	}

	public MainMenuScreenGameConfigWindow(SimulationScreen simulationScreen) {
		BuildUI();

		propertyGrid.Object = new ConfigData();

		ConfirmGameParamsButton.Click += (o, e) => {
			var config = (ConfigData)propertyGrid.Object;
			var chunkCount = getGardenChunkCountFromSize(config.GardenSize);
			var rockSize = getRangeFromSize(config.RockSize);

			simulationScreen.InitializeNewGame(new GardenGenerator.GardenGenerationOptions {
				Seed = config.Seed,
				Height = chunkCount,
				Width = chunkCount,
				ChunkSize = 1024,
				RockSize = new(rockSize.Min*5, rockSize.Max*10),
				TrunkSize = getRangeFromSize(config.TrunkSize),
				BushSize = getRangeFromSize(config.BushSize),
				TwigSize = getRangeFromSize(config.TwigSize),
				BushesPerChunk = getRangeFromFrequency(config.BushesFrequency),
				TwigsPerChunk = getRangeFromFrequency(config.TwigsFrequency),
				RocksPerChunk = getRangeFromFrequency(config.RocksFrequency),
				TrunksPerChunk = getRangeFromFrequency(config.TrunksFrequency),
				Anthills = config.Anthills.Select(x => new GardenGenerator.AnthillOptions {
					Farmers	= x.Farmers,
					Scouts = x.Scouts,
					Soldiers = x.Soldiers,
					Workers = x.Workers,
				}).ToList(),
			});
		};
	}

	private int getGardenChunkCountFromSize(GardenSize size) =>
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

	private Range<int> getRangeFromSize(Size size) =>
		size switch {
			Size.Small => new(3, 5),
			Size.Medium => new(7, 13),
			Size.Big => new(8, 20),
			_ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
		};
	
	private Range<int> getRangeFromFrequency(Frequency frequency) =>
		frequency switch {
			Frequency.Desert => new(0, 2),
			Frequency.Sparse=> new(0, 3),
			Frequency.Normal => new(1, 3),
			Frequency.Plentiful => new(3, 4),
			Frequency.Extreme => new(4, 5),
			_ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null)
		};
}