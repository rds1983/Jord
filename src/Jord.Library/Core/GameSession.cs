using Jord.Components;
using Jord.Core.Abilities;
using Jord.Storage;
using Jord.UI;
using Jord.Utils;

namespace Jord.Core
{
	public class GameSession
	{
		public Slot Slot { get; }
		public Player Player { get; }

		public MapNavigationBase MapNavigationBase { get; set; }

		public GameSession(int slotIndex)
		{
			Slot = TJ.StorageService.Slots[slotIndex];

			Player = Slot.PlayerData.CreateCharacter();

			// Spawn player
			var map = TJ.Database.Maps[Slot.PlayerData.StartingMapId];
			Player.Stats.Life.Restore();

			Factory.CreatePlayer(map.SpawnSpot.Value);
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
		}

		public void PlayerEnter()
		{
			Player.Enter();
		}

		public void WaitPlayer()
		{
			WorldAct();
		}

		public bool MovePlayer(MovementDirection direction, bool isRunning)
		{
			var result = TJ.PlayerEntity.Move(direction.GetDelta());
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
	}
}