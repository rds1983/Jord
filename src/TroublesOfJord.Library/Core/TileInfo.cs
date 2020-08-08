namespace TroublesOfJord.Core
{
	public class TileInfo: BaseMapObject
	{
		public bool Passable { get; set; }

		public bool IsTransparent => Passable;

		public bool IsWalkable => Passable;
	}
}
