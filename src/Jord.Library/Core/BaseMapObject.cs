namespace Jord.Core
{
	public class BaseMapObject: BaseObject
	{
		public char Symbol { get; set; }
		public Appearance Image { get; set; }

		public virtual void UpdateAppearance(Tileset tileset)
		{
		}
	}
}
