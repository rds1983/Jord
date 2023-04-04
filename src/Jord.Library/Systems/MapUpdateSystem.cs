using DefaultEcs.System;

namespace Jord.Services
{
	public class MapUpdateSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; }

		public void Dispose()
		{
		}

		public void Update(float state)
		{
			var map = TJ.Map;

			// Reset IsInFov for the whole map
			for (var x = 0; x < map.Width; ++x)
			{
				for (var y = 0; y < map.Height; ++y)
				{
					map[x, y].IsInFov = false;
				}
			}

			var playerPosition = TJ.PlayerPosition;
			map.FieldOfView.Calculate(playerPosition.X, playerPosition.Y, 12);

			foreach (var coord in map.FieldOfView.CurrentFOV)
			{
				var tile = map[coord];

				tile.IsInFov = true;
				if (!tile.IsExplored)
				{
					tile.IsExplored = true;
				}

/*				if (tile.Creature != null)
				{
					var asNpc = tile.Creature as NonPlayer;
					if (asNpc != null && asNpc.Info.CreatureType == CreatureType.Enemy && asNpc.AttackTarget == null)
					{
						TJ.GameLog(Strings.BuildRushesToAttack(asNpc.Info.Name));
						asNpc.AttackTarget = TJ.Player;
					}
				}*/
			}
		}
	}
}
