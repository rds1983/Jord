namespace Jord.MapEditor.Generation
{
	public enum HeightType
	{
		DeepWater = 1,
		ShallowWater = 2,
		Shore = 3,
		Sand = 4,
		Grass = 5,
		Forest = 6,
		Rock = 7,
		Snow = 8,
		River = 9
	}

	public enum HeatType
	{
		Coldest = 0,
		Colder = 1,
		Cold = 2,
		Warm = 3,
		Warmer = 4,
		Warmest = 5
	}

	public enum MoistureType
	{
		Wettest = 5,
		Wetter = 4,
		Wet = 3,
		Dry = 2,
		Dryer = 1,
		Dryest = 0
	}

	public enum BiomeType
	{
		Desert,
		Savanna,
		TropicalRainforest,
		Grassland,
		Woodland,
		SeasonalForest,
		TemperateRainforest,
		BorealForest,
		Tundra,
		Ice
	}


	public class InputTile
	{
		private static readonly BiomeType[,] BiomeTable = new BiomeType[6, 6]
		{
			//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
			{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
			{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
			{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
			{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
			{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
			{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
		};

		public HeightType HeightType { get; set; }
		public HeatType HeatType { get; set; }
		public MoistureType MoistureType { get; set; }

		public float HeightValue { get; set; }
		public float HeatValue { get; set; }
		public float MoistureValue { get; set; }

		public bool Collidable => HeightType != HeightType.DeepWater && HeightType != HeightType.ShallowWater && HeightType != HeightType.River;

		public BiomeType BiomeType
		{
			get
			{
				if (!Collidable) return BiomeType.Desert;

				return BiomeTable[(int)MoistureType, (int)HeatType];
			}
		}
	}
}
