using Jord.Core;
using Jord.Utils;
using System;

namespace Jord.Generation
{
	public class CityGenerator : BaseGenerator
	{
		public CityGenerator(int width, int height) : base(width, height)
		{
		}

		public override Map Generate()
		{
			var result = new Map(Width, Height);

			var ground = TJ.Database.TileInfos["Ground"];

			// Fill with ground
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					result[x, y].Info = ground;
				}
			}

			Step();

			// Water
			var water = TJ.Database.TileInfos["Water"];
			var n = MathUtils.Random.NextDouble();
			/*			for (var y = 0; y < Height; ++y)
						{
							var nw = (int)(Math.Sin(n) * 10.0) + 14 + MathUtils.Random.Next(1, 7);
							for (var x = 0; x < nw; ++x)
							{
								result[x, y].Info = water;
							}

							n += 0.1;
						}*/
			for (var x = 0; x < Width; ++x)
			{
				var nw = (int)(Math.Sin(n) * 10.0) + 12 + MathUtils.Random.Next(1, 5);
				for (var y = 0; y < nw; ++y)
				{
					result[x, Height - y - 1].Info = water;
				}

				n += 0.1;
			}

			return result;
		}
	}
}
