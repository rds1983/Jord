using DefaultEcs;
using Jord.Components;
using Microsoft.Xna.Framework;
using System.Xml.XPath;

namespace Jord.Core
{
	public static class Factory
	{
		public static Entity CreateMob(CreatureInfo info, Point location)
		{
			var result = TJ.World.CreateEntity();

			result.Set(new Location(location));
			result.Set(info.Image);

			return result;
		}

		public static Entity CreatePlayer(Point location)
		{
			var result = TJ.World.CreateEntity();

			result.Set(new Location(location));
			result.Set(TJ.Settings.PlayerAppearance);
			result.Set(new PlayerMarker());
			return result;

		}
	}
}
