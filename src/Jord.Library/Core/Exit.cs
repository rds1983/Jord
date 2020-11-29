using Microsoft.Xna.Framework;

namespace Jord.Core
{
	public class Exit
	{
		public string MapId;
		public string TileInfoId;

		public Point? Position;
		public int? DungeonLevel;

		public override string ToString()
		{
			if (Position == null)
			{
				return MapId;
			}

			return string.Format("{0}:{1},{2}",
				MapId, Position.Value.X, Position.Value.Y);
		}

		public static Exit FromString(string data)
		{
			var parts = data.Split(':');

			var result = new Exit
			{
				MapId = parts[0]
			};

			if (parts.Length > 1)
			{
				var parts2 = parts[1].Split(',');

				result.Position = new Point(int.Parse(parts2[0]), int.Parse(parts2[1]));
			}

			return result;
		}
	}
}
