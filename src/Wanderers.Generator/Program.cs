using System;
using System.IO;
using System.Text;

namespace Wanderers.Generator
{
	class Program
	{
		private const char WaterTileId = 'w';
		private const char LandTileId = '.';
		private const char MountainTileId = 'M';
		private const char ForestTileId = 'T';
		private const char RoadTileId = '0';
		private const char LocationTileId = '#';

		private static string DataToString(char[,] data)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < data.GetLength(0); ++i)
			{
				sb.Append("\"");
				for (var j = 0; j < data.GetLength(1); ++j)
				{
					sb.Append(data[i, j]);
				}

				sb.Append("\"");
				if (i < data.GetLength(0) - 1)
				{
					sb.Append(",\n");
				}
			}

			return sb.ToString();
		}

		static void Main(string[] args)
		{
			try
			{
				TJ.LogInfo(Config.Instance.ToString());

				var heightMapGenerator = new HeightMapGenerator();

				var generationResult = heightMapGenerator.Generate();

				var locationGenerator = new LocationsGenerator(generationResult);
				locationGenerator.Generate();

				TJ.LogInfo("Writing to {0}", Config.Instance.OutputFile);

				var result = Resources.Template;

				result = result.Replace("$width$", Config.Instance.WorldSize.ToString());
				result = result.Replace("$height$", Config.Instance.WorldSize.ToString());

				var data = new char[generationResult.Height, generationResult.Width];

				for (var i = 0; i < generationResult.Height; ++i)
				{
					for (var j = 0; j < generationResult.Width; ++j)
					{
						var tileType = generationResult.GetWorldMapTileType(j, i);
						char tileId = ' ';

						switch (tileType)
						{
							case WorldMapTileType.Water:
								tileId = WaterTileId;
								break;
							case WorldMapTileType.Mountain:
								tileId = MountainTileId;
								break;
							case WorldMapTileType.Forest:
								tileId = ForestTileId;
								break;
							case WorldMapTileType.Road:
								tileId = RoadTileId;
								break;
							case WorldMapTileType.Wall:
								tileId = LocationTileId;
								break;
							default:
								tileId = LandTileId;
								break;
						}
						data[i, j] = tileId;
					}
				}

				result = result.Replace("$data$", DataToString(data));

				File.WriteAllText(Config.Instance.OutputFile, result);

				TJ.LogInfo("Done.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}