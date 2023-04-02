using FontStashSharp;

namespace Jord.Core
{
	public class Settings : BaseObject
	{
		public Appearance PlayerAppearance { get; set; }
		public string StartingMapId { get; set; }
		public Tileset Tileset { get; set; }
		public SpriteFontBase Font { get; set; }
		public int BasePurchasePercentage { get; set; }
		public int BaseSellPercentage { get; set; }
	}
}
