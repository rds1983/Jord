using Jord.Utils;
using Microsoft.Xna.Framework;

namespace Jord.Components
{
	public struct Location
	{
		public Point Position;
		public Vector2 DisplayPosition;

		public Location(Point pos)
		{
			Position = pos;
			DisplayPosition = pos.ToVector2();
		}
	}
}
