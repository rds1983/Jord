using DefaultEcs;
using Jord.Components;
using Microsoft.Xna.Framework;

namespace Jord.Core
{
	public enum TileObjectType
	{
		TanningBench,
		CraftingBench
	}

	public class TileObject: BaseMapObject, ISpawnable
	{
		public TileObjectType Type { get; set; }

		public Entity Spawn(Point location)
		{
			var result = TJ.World.CreateEntity();

			result.Set(new Location(location));
			result.Set(Image);

			return result;
		}
	}
}
