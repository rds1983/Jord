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

		public static void PlayerEnter()
		{
			//			Player.Enter();
		}

		public static void WaitPlayer()
		{
			WorldAct();
		}

		public static bool PlayerCanEnter() => PlayerTile.Exit != null;

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
