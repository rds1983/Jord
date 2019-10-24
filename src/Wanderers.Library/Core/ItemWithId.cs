using System;
using Wanderers.Compiling;

namespace Wanderers.Core
{
	public class ItemWithId: IItemWithId
	{
		private string _id;

		[IgnoreField]
		public string Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (value == _id)
				{
					return;
				}

				_id = value;
				FireIdChanged();
			}
		}

		[IgnoreField]
		public string Source;

		public event EventHandler IdChanged;

		public override string ToString()
		{
			return Id;
		}

		protected void FireIdChanged()
		{
			var ev = IdChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
