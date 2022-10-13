using Jord.Core;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Jord.Loading
{
	class TilesetLoader : BaseObjectLoader<Tileset>
	{
		private static readonly MovementDirection[] MaskOrder = new MovementDirection[]
		{
			MovementDirection.Left, MovementDirection.UpLeft, MovementDirection.Up, MovementDirection.UpRight,
			MovementDirection.Right, MovementDirection.DownRight, MovementDirection.Down, MovementDirection.DownLeft
		};

		public static readonly TilesetLoader Instance = new TilesetLoader();


		protected override Tileset CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			secondRunAction = null;

			var result = new Tileset
			{
				Id = data.EnsureId(),
				Width = data.EnsureInt("Width"),
				Height = data.EnsureInt("Height"),
			};

			var folder = Path.GetDirectoryName(source);
			var atlasFile = Path.ChangeExtension(source, "xmat");

			result.TextureAtlas = TextureRegionAtlas.FromXml(File.ReadAllText(atlasFile),
				fileName =>
				{
					var path = Path.Combine(folder, fileName);

					using (var stream = File.OpenRead(path))
					{
						return Texture2D.FromStream(TJ.GraphicsDevice, stream);
					}
				});

			var tileImages = (JObject)data["TileImages"];
			foreach (var pair in tileImages)
			{
				var tileInfo = new TilesetTileInfo();
				var defaultId = string.Empty;
				var asObject = pair.Value as JObject;
				if (asObject != null)
				{
					foreach (var pair2 in asObject)
					{
						if (pair2.Value is JValue)
						{
							defaultId = pair2.Key;
							continue;
						}

						var choice = new TilesetTileChoice
						{
							Image = result.TextureAtlas[pair2.Key]
						};

						var conditions = new List<TilesetTileChoiceCondition>();
						foreach (var pair3 in (JObject)pair2.Value)
						{
							var condition = new TilesetTileChoiceCondition
							{
								Direction = (MovementDirection)Enum.Parse(typeof(MovementDirection), pair3.Key)
							};

							var id = pair3.Value.ToString();
							if (id.StartsWith("!"))
							{
								condition.Is = false;
								condition.TileInfoId = id.Substring(1);
							}
							else
							{
								condition.Is = true;
								condition.TileInfoId = id;
							}

							conditions.Add(condition);
						}

						choice.Conditions = conditions.ToArray();
						tileInfo.Choices.Add(choice);
					}
				}
				else
				{
					defaultId = pair.Value.ToString();
				}

				tileInfo.Default = result.TextureAtlas[defaultId];

				result.TileImages[pair.Key] = tileInfo;
			}

			return result;
		}
	}
}
