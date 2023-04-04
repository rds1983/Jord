using DefaultEcs;
using Jord.Core;
using Jord.Core.Abilities;
using Jord.Utils;

namespace Jord
{
	partial class TJ
	{
		public static bool MovePlayer(MovementDirection direction, bool isRunning)
		{
			var result = TJ.PlayerEntity.Move(direction.GetDelta());
			if (result)
			{
				WorldAct();
			}

			return result;
		}

		public static bool PlayerCanEnter()
		{
			return PlayerTile != null && PlayerTile.Exit != null;
		}

		public static bool PlayerEnter()
		{
			var playerTile = PlayerTile;
			if (playerTile == null || playerTile.Exit == null)
			{
				return false;
			}

			Tile exitTile = null;

			var previousMap = Map;

			Map map;

			if (TJ.Database.Dungeons.ContainsKey(playerTile.Exit.MapId))
			{
				var dungeon = TJ.Database.Dungeons.Ensure(playerTile.Exit.MapId);

				var dungeonLevel = PlayerTile.Exit.DungeonLevel == null ? 1 : PlayerTile.Exit.DungeonLevel.Value;
				map = dungeon.Generate(dungeonLevel);
				for (var x = 0; x < map.Width; ++x)
				{
					for (var y = 0; y < map.Height; ++y)
					{
						var tile = map[x, y];
						if (tile.Exit == null)
						{
							continue;
						}

						if (tile.Exit.MapId == previousMap.Id && tile.Exit.DungeonLevel == previousMap.DungeonLevel)
						{
							// Found backwards exit
							exitTile = tile;
							goto found;
						}
					}
				}
			found:;
			}
			else
			{
				map = TJ.Database.Maps.Ensure(playerTile.Exit.MapId);

				if (playerTile.Exit.Position != null)
				{
					exitTile = map[playerTile.Exit.Position.Value];
				}
				else
				{
					// If position isnt set explicitly
					// Then we search for a tile that has exit to the current creature map
					exitTile = map.EnsureExitTileById(previousMap.Id);
				}
			}

			// Recreate the world
			TJ.World.Dispose();
			TJ.World = new World();

			TJ.Map = map;

			// Spawn player
			TJ.Player.Spawn(exitTile.Position);

			// Everything else
			map.SpawnAll();

			return true;
		}

		public static void WaitPlayer()
		{
			WorldAct();
		}

		public static void UseAbility(AbilityInfo ability)
		{
			/*			if (Player.Stats.Life.CurrentMana < ability.Mana)
						{
							TJ.GameLog(Strings.NotEnoughEnergy);
							return;
						}

						var success = true;

						foreach (var effect in ability.Effects)
						{
							var asHealSelf = effect as HealSelf;
							if (asHealSelf != null)
							{
								var amount = (float)MathUtils.Random.Next(asHealSelf.Minimum, asHealSelf.Maximum + 1);
								if (Player.Stats.Life.CurrentHP >= Player.Stats.Life.MaximumHP)
								{
									TJ.GameLog(Strings.MaximumHpAlready);
									success = false;
									continue;
								}
								else if (Player.Stats.Life.CurrentHP + amount > Player.Stats.Life.MaximumHP)
								{
									amount = Player.Stats.Life.MaximumHP - Player.Stats.Life.CurrentHP;
								}

								Player.Stats.Life.CurrentHP += amount;

								var message = string.Format(asHealSelf.MessageActivated, amount);
								TJ.GameLog(message);
							}
						}

						if (success)
						{
							Player.Stats.Life.CurrentMana -= ability.Mana;
							WorldAct();
						}*/
		}

		public static void TakeLyingItem(int index, int count)
		{
			var item = PlayerTile.Inventory.Items[index];

			Player.Inventory.Add(item.Item, count);
			PlayerTile.Inventory.Add(item.Item, -count);

			TJ.GameLog(Strings.BuildPickedUp(item.Item.Info.Name, count));
		}
	}
}
