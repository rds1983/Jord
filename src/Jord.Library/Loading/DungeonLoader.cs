using Newtonsoft.Json.Linq;
using Jord.Core;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class DungeonLoader : BaseObjectLoader<Dungeon>
	{
		public static readonly DungeonLoader Instance = new DungeonLoader();

		protected override Dungeon CreateObject(string source, JObject dataObj, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new Dungeon
			{
				GeneratorId = dataObj.EnsureString("GeneratorId"),
				Name = dataObj.EnsureString("Name"),
				Levels = dataObj.EnsureInt("Levels")
			};

			var exitObj = dataObj.EnsureJObject("Exit");
			result.Exit = new Exit
			{
				MapId = exitObj.EnsureString("MapId"),
				TileInfoId = exitObj.EnsureString("TileInfoId"),
			};

			if (result.Levels > 1)
			{
				result.ExitDownTileInfoId = dataObj.EnsureString("ExitDownTileInfoId");
			}

			secondRunAction = db => SecondRun(result, db);

			return result;
		}

		private void SecondRun(Dungeon result, Database database)
		{
			if (database.Maps.ContainsKey(result.Id))
			{
				RaiseError($"There's already Map with id '{result.Id}'");
			}
		}
	}
}