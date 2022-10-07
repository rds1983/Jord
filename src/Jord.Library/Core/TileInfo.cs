using Myra.Graphics2D.TextureAtlases;

namespace Jord.Core
{
	public class TileInfo: BaseMapObject
	{
		public bool Passable { get; set; }

		public bool IsTransparent => Passable;

		public bool IsWalkable => Passable;

		public override void UpdateAppearance(Tileset tileset)
		{
			base.UpdateAppearance(tileset);

			TextureRegion image;
			if (tileset.TileImages.TryGetValue(Id, out image))
			{
				Image.TextureRegion = image;
			}
		}
	}
}
