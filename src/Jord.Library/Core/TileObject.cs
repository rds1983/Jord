namespace Jord.Core
{
	public enum TileObjectType
	{
		TanningBench,
		CraftingBench
	}

	public class TileObject: BaseMapObject
	{
		public TileObjectType Type { get; set; }
	}
}
