using System.Collections.Generic;
using System.Text;

namespace Wanderers.Generator
{
	public class LandGeneratorConfig
	{
		public int WorldSize { get; set; }
		public int HeightMapVariability { get; set; }
		public float LandPart { get; set; }
		public float MountainPart { get; set; }
		public float ForestPart { get; set; }
		public bool SurroundedByWater { get; set; }
		public bool Smooth { get; set; }
		public bool RemoveSmallIslands { get; set; }
		public bool RemoveSmallLakes { get; set; }

		public LandGeneratorConfig()
		{
			WorldSize = 1024;
			LandPart = 0.6f;
			MountainPart = 0.1f;
			ForestPart = 0.1f;
			HeightMapVariability = 5;
			Smooth = true;
			RemoveSmallIslands = true;
			RemoveSmallLakes = true;
			//			SurroundedByWater = true;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append("WorldSize=" + WorldSize + ",\n");
			sb.Append("HeightMapVariability=" + HeightMapVariability + ",\n");
			sb.Append("LandPart=" + (int)(LandPart * 100.0f) + "%,\n");
			sb.Append("MountainPart=" + (int)(MountainPart * 100.0f) + "%,\n");
			sb.Append("ForestPart=" + (int)(ForestPart * 100.0f) + "%,\n");
			sb.Append("SurroundedByWater=" + SurroundedByWater + ",\n");
			sb.Append("Smooth=" + Smooth + ",\n");
			sb.Append("RemoveSmallIslands=" + RemoveSmallIslands + ",\n");
			sb.Append("RemoveSmallLakes=" + RemoveSmallLakes);

			return sb.ToString();
		}
	}
}