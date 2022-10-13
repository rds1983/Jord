using Myra.Graphics2D.TextureAtlases;
using System.Collections.Generic;

namespace Jord.Core
{
	public class TilesetTileChoiceCondition
	{
		public MovementDirection Direction { get; set; }
		public bool Is { get; set; }
		public string TileInfoId { get; set; }
	}

	public class TilesetTileChoice
	{
		public TilesetTileChoiceCondition[] Conditions { get; set; }
		public TextureRegion Image { get; set; }
	}

	public class TilesetTileInfo
	{
		public TextureRegion Default { get; set; }

		public List<TilesetTileChoice> Choices { get; } = new List<TilesetTileChoice>();
	}

	public class Tileset : BaseObject
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public TextureRegionAtlas TextureAtlas { get; set; }
		public Dictionary<string, TilesetTileInfo> TileImages { get; } = new Dictionary<string, TilesetTileInfo>();
	}
}
