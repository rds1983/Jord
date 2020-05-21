using RogueSharp.MapCreation;
using TroublesOfJord.Core;

namespace TroublesOfJord.Generation
{
	public class BaseGenerator: BaseObject
	{
		public IMapCreationStrategy<Map, Tile> MapCreationStrategy;

		public Map Generate()
		{
			return MapCreationStrategy.CreateMap();
		}
	}
}
