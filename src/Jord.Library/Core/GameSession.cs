using Jord.Core.Abilities;
using Jord.Storage;
using Jord.UI;
using Jord.Utils;

namespace Jord.Core
{
	public enum OperateType
	{
		Enter
	}

	public class GameSession
	{
		public Slot Slot { get; }
		public Player Player { get; }

		public MapNavigationBase MapNavigationBase;

		public GameSession(int slotIndex)
		{
			Slot = TJ.StorageService.Slots[slotIndex];

			Player = Slot.PlayerData.CreateCharacter();

			// Spawn player
			var map = TJ.Module.Maps[Slot.PlayerData.StartingMapId];
			Player.Place(map, map.SpawnSpot.Value);
			Player.Stats.Life.Restore();
		}

		private void WorldAct()
		{
			var map = Player.Map;
			foreach (var creature in map.Creatures)
			{
				creature.RegenTurn();
				var npc = creature as NonPlayer;
				if (npc == null)
				{
					continue;
				}

				// Let npcs act
				npc.Act();
			}

			UpdateTilesVisibility();
		}

		public void WaitPlayer()
		{
			WorldAct();
		}

		public bool MovePlayer(MovementDirection direction, bool isRunning)
		{
			var result = Player.MoveTo(direction.GetDelta());
			if (result)
			{
				WorldAct();
			}

			return result;
		}

		public void UseAbility(AbilityInfo ability)
		{
			if (Player.Stats.Life.CurrentMana < ability.Mana)
			{
				TJ.GameLog(Strings.NotEnoughEnergy);
				return;
			}

			var success = true;

			foreach(var effect in ability.Effects)
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
					} else if (Player.Stats.Life.CurrentHP + amount > Player.Stats.Life.MaximumHP)
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
			}
		}

		public OperateType? GetPlayerOperate()
		{
			if (Player.CanEnter())
			{
				return OperateType.Enter;
			}

			return null;
		}

		public void PlayerOperate()
		{
			if (Player.Enter())
			{
				if (Player.Map.DungeonLevel == null)
				{
					TJ.GameLog(Strings.BuildEnteredMap(Player.Map.Name));
				} else
				{
					TJ.GameLog(Strings.BuildEnteredMap(Player.Map.Name + ", " + Player.Map.DungeonLevel.Value));
				}
				UpdateTilesVisibility(true);
			}
		}

		public void UpdateTilesVisibility(bool refreshMap = false)
		{
			var map = Player.Map;

			// Reset IsInFov for the whole map
			for(var x = 0; x < map.Width; ++x)
			{
				for(var y = 0; y < map.Height; ++y)
				{
					map[x, y].IsInFov = false;
				}
			}

			map.FieldOfView.Calculate(Player.Position.X, Player.Position.Y, 12);

			foreach(var coord in map.FieldOfView.CurrentFOV)
			{
				var tile = map[coord];

				tile.IsInFov = true;
				if (!tile.IsExplored)
				{
					refreshMap = true;
					tile.IsExplored = true;
				}

				if (tile.Creature != null)
				{
					var asNpc = tile.Creature as NonPlayer;
					if (asNpc != null && asNpc.Info.CreatureType == CreatureType.Enemy && asNpc.AttackTarget == null)
					{
						TJ.GameLog(Strings.BuildRushesToAttack(asNpc.Info.Name));
						asNpc.AttackTarget = TJ.Player;
					}
				}
			}

			if (refreshMap)
			{
				MapNavigationBase.InvalidateImage();
			}
		}
	}
}