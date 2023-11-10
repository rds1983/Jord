using Jord.Core;
using System.Collections.Generic;

namespace Jord.Generation
{
	public abstract class BaseGenerator : BaseObject
	{
		private readonly List<Map> _steps = new List<Map>();
		private Map[] _stepsArray;

		public int Width { get; }
		public int Height { get; }
		public abstract Map CurrentResult { get; }
		public Map[] Steps => _stepsArray;
		protected BaseGenerator(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public Map Generate()
		{
			_steps.Clear();
			var result = InternalGenerate();
			_stepsArray = _steps.ToArray();

			return result;
		}
		protected abstract Map InternalGenerate();

		private Map CloneResult()
		{
			var result = new Map(Width, Height);

			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					result[x, y].Info = CurrentResult[x, y].Info;
				}
			}

			return result;
		}

		public void Step()
		{
			_steps.Add(CloneResult());
		}
	}
}
