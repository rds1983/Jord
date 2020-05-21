using RogueSharp;

namespace TroublesOfJord.Core
{
	public class TileInfo: BaseMapObject, ICellInfo
	{
		public bool Passable { get; set; }

		public bool IsTransparent => Passable;

		public bool IsWalkable => Passable;
	}
}
