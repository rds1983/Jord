using Jord.Generation;
using System;

namespace Jord.Core
{
	public class DungeonRecord
	{
		public int MinimumLevel { get; }
		public BaseGenerator Generator { get; }

		public DungeonRecord(int minimumLevel, BaseGenerator generator)
		{
			if (minimumLevel < 0 || minimumLevel >= 100)
			{
				throw new ArgumentOutOfRangeException(nameof(minimumLevel));
			}

			MinimumLevel = minimumLevel;
			Generator = generator ?? throw new ArgumentNullException(nameof(generator));
		}
	}
}
