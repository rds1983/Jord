using DefaultEcs;
using Jord.Utils;
using Microsoft.Xna.Framework;

namespace Jord.Core
{
	public static class EntityExtensions
	{
		private const int MovementDurationInMs = 50;

		public static bool Move(this Entity entity, Point delta)
		{
			if (delta == Point.Zero)
			{
				return false;
			}

			var location = entity.Get<Location>();

			var newPosition = location.Position + delta;
			var map = TJ.Map;
			if (newPosition.X < 0 || newPosition.Y < 0 || newPosition.X >= map.Width || newPosition.Y >= map.Height)
			{
				return false;
			}

			var newTile = map[newPosition];
			if (!newTile.Info.Passable || newTile.Object != null)
			{
				return false;
			}

			var currentTile = map[location.Position];

			var oldPosition = location.Position.ToVector2();

			location.Position = newPosition;
			entity.Set(location);

			void onUpdate(float part)
			{
				location.DisplayPosition = oldPosition + (newPosition.ToVector2() - oldPosition) * part;
				entity.Set(location);
			}

			TJ.ActivityService.AddParallelActivity(onUpdate, () =>
			{
				location.DisplayPosition = newPosition.ToVector2();
				entity.Set(location);
			}, MovementDurationInMs);

			return true;
		}
	}
}
