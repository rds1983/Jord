namespace Jord.Storage
{
	public class StorageService
	{
		public const int SlotsCount = 10;
		private readonly Slot[] _slots = new Slot[SlotsCount];

		public Slot[] Slots
		{
			get { return _slots; }
		}

		public StorageService()
		{
			for (var i = 0; i < _slots.Length; ++i)
			{
				_slots[i] = new Slot(i);
			}
		}
	}
}