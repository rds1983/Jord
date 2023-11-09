using Jord.Generation;
using Jord.Utils;
using Microsoft.Xna.Framework;

namespace Jord.Core
{
	partial class Creature
	{
		private const int MovementDurationInMs = 50;

		public void SetPosition(Point position)
		{
			var currentTile = Map[Position];
			var newTile = Map[position];

			if (currentTile != newTile)
			{
				newTile.Creature = currentTile.Creature;
				currentTile.Creature = null;
			}

			var oldPosition = Position.ToVector2();
			Position = position;

			void onUpdate(float part)
			{
				DisplayPosition = oldPosition + (Position.ToVector2() - oldPosition) * part;
			}

			TJ.ActivityService.AddParallelActivity(onUpdate, () => DisplayPosition = Position.ToVector2(), MovementDurationInMs);
		}

		public bool MoveTo(Point delta)
		{
			if (delta == Point.Zero)
			{
				return false;
			}

			var newPosition = Position + delta;

			if (newPosition.X < 0 || newPosition.Y < 0 || newPosition.X >= Map.Width || newPosition.Y >= Map.Height)
			{
				return false;
			}

			var newTile = Map[newPosition];
			var creature = newTile.Creature;
			if (creature != null)
			{
				var asNonPlayer = creature as NonPlayer;
				if (asNonPlayer != null && asNonPlayer.Info.CreatureType == CreatureType.Enemy)
				{
					Attack(creature);

					return true;
				}
			}

			if (!newTile.Info.Passable || newTile.Object != null)
			{
				return false;
			}

			SetPosition(newPosition);

			return true;
		}

		public bool CanEnter()
		{
			return Tile != null && Tile.IsExit;
		}

		public bool Enter()
		{
			if (Tile == null || !Tile.IsExit)
			{
				return false;
			}

			Tile exitTile = null;

			Map map;

			if (Tile.IsExitUp)
			{
				map = MapGeneration.Generate(Map.DungeonLevel - 1);
			} else
			{
				map = MapGeneration.Generate(Map.DungeonLevel + 1);
			}

			Remove();
			Place(map, exitTile.Position);

			OnEntered();

			return true;
		}

		protected virtual void OnEntered()
		{
		}
	}
}
