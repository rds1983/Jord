using Microsoft.Xna.Framework;
using Wanderers.Core.Items;

namespace Wanderers.Core
{
	public class Player : Creature
	{
		private readonly Appearance _playerAppearance = new Appearance('@', Color.White);
		private readonly Equipment _equipment = new Equipment();

		public override Appearance Image
		{
			get
			{
				return _playerAppearance;
			}
		}

		public Equipment Equipment
		{
			get
			{
				return _equipment;
			}
		}
	}
}