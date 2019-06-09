using System;

namespace Wanderers.Core
{
	public interface IItemWithId
	{
		string Id { get; }

		event EventHandler IdChanged;
	}
}
