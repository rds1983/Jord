using DefaultEcs.System;
using Jord.Components;
using Jord.Core;
using Jord.Services;
using Jord.Utils;

namespace Jord
{
	public static class PlayerService
	{
		private static SequentialSystem<float> _systems = new SequentialSystem<float>(
			new MapUpdateSystem()
		);

		private static void WorldAct()
		{
			_systems.Update(0);
		}

		public static bool MovePlayer(MovementDirection direction, bool isRunning)
		{
			var result = TJ.PlayerEntity.Move(direction.GetDelta());
			if (result)
			{
				WorldAct();
			}

			return result;
		}
	}
}
