using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TroublesOfJord.Compiling
{
	public class CompilerContext
	{
		private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();

		public Dictionary<string, Color> Colors
		{
			get
			{
				return _colors;
			}
		}

		public Core.Module Module { get; } = new Core.Module();

		public Color EnsureColor(string id, string source)
		{
			Color result;
			if (!_colors.TryGetValue(id, out result))
			{
				throw new Exception(string.Format("Could not find color '{0}'. Source: {1}", id, source));
			}

			return result;
		}
	}
}
