using Myra.Graphics2D.TextureAtlases;
using System.Collections.Generic;

namespace Jord.Core
{
	public class Tileset: BaseObject
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public TextureRegionAtlas TextureAtlas { get; set; }

		public Dictionary<string, TextureRegion> TileImages { get; } = new Dictionary<string, TextureRegion>();
	}
}
