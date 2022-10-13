namespace Jord.Core
{
	public class TileInfo : BaseMapObject
	{
		public bool Passable { get; set; }

		public bool IsTransparent => Passable;

		public bool IsWalkable => Passable;

		public TilesetTileInfo TileAppearance { get; set; }

		public override void UpdateAppearance(Tileset tileset)
		{
			base.UpdateAppearance(tileset);

			if (tileset != null)
			{
				TilesetTileInfo image;
				if (tileset.TileImages.TryGetValue(Id, out image))
				{
					TileAppearance = image;
				}
				else
				{
					Image.TextureRegion = null;
				}
			}
			else
			{
				Image.TextureRegion = null;
			}
		}
	}
}
