using Myra.Graphics2D.TextureAtlases;
using System.Collections.Generic;

namespace Jord.Core
{
	public class TileSet: BaseObject
	{
		public TextureRegionAtlas TextureAtlas;
		public readonly Dictionary<string, Appearance> Appearances = new Dictionary<string, Appearance>();
	}
}
