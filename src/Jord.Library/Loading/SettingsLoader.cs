using FontStashSharp;
using Jord.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Jord.Loading
{
	class SettingsLoader : BaseObjectLoader<Settings>
	{
		public static readonly SettingsLoader Instance = new SettingsLoader();

		protected override Settings CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var fontSystem = new FontSystem();
			var path = Path.Combine(Path.GetDirectoryName(source), data.EnsureString("Font"));
			fontSystem.AddFont(File.ReadAllBytes(path));
			var font = fontSystem.GetFont(data.EnsureInt("FontSize"));

			var symbolStr = data.EnsureString("PlayerSymbol");
			if (symbolStr.Length != 1)
			{
				RaiseError($"Unable to read '{symbolStr}' as symbol.");
			}

			var color = data.EnsureColor("PlayerColor");
			var playerAppearance = new Appearance(symbolStr, color, null);

			var result = new Settings
			{
				Id = data.EnsureId(),
				PlayerAppearance = playerAppearance,
				FontSystem = fontSystem,
				Font = font,
				BasePurchasePercentage = data.EnsureInt("BasePurchasePercentage"),
				BaseSellPercentage = data.EnsureInt("BaseSellPercentage")
			};

			secondRunAction = db => SecondRun(result, data, db);

			return result;
		}

		private void SecondRun(Settings result, JObject data, Database db)
		{
			var tilesetId = data.OptionalString("Tileset");

			if (!string.IsNullOrEmpty(tilesetId))
			{
				result.Tileset = db.Tilesets[tilesetId];
			}
		}
	}
}
