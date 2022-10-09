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

	public class InputTile
	{
		public HeightType HeightType { get; set; }
		public HeatType HeatType { get; set; }
		public MoistureType MoistureType { get; set; }

		public float HeightValue { get; set; }
		public float HeatValue { get; set; }
		public float MoistureValue { get; set; }
	}
}
