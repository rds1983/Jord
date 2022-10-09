using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Myra.Utility;

namespace Jord.MapEditor
{
	public class State
	{
		public const string StateFileName = "Jord.MapEditor.config";

		public static string StateFilePath
		{
			get
			{
				var result = Path.Combine(PathUtils.ExecutingAssemblyDirectory, StateFileName);
				return result;
			}
		}

		public Point Size { get; set; }
		public float TopSplitterPosition { get; set; }
		public float RightSplitterPosition { get; set; }
		public string ModulePath { get; set; }
		public string MapId { get; set; }
		public int[] CustomColors { get; set; }
		public string LastFolder
		{
			get; set;
		}

		public int LastNewMapType
		{
			get; set;
		}

		public GenerationSettings GenerationSettings { get; set; }

		public void Save()
		{
			using (var fileStream = File.Create(StateFilePath))
			{
				var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8)
				{
					Formatting = Formatting.Indented
				};
				var serializer = new XmlSerializer(typeof(State));
				serializer.Serialize(xmlWriter, this);
			}
		}

		public static State Load()
		{
			if (!File.Exists(StateFilePath))
			{
				return null;
			}

			State state;
			using (var stream = new StreamReader(StateFilePath))
			{
				var serializer = new XmlSerializer(typeof(State));
				state = (State)serializer.Deserialize(stream);
			}

			return state;
		}

		public override string ToString()
		{
			return string.Format("Size = {0}\n" +
								 "TopSplitter = {1:0.##}\n" +
								 "RightSplitter= {2:0.##}\n" +
								 "ModulePath = {3}\n" +
								 "MapId = {4}\n" +
								 "CustomColors = {5}\n" +
								 "LastFolder = {6}\n" +
								 "LastNewMapType = {7}",
				Size,
				TopSplitterPosition,
				RightSplitterPosition,
				ModulePath,
				MapId,
				CustomColors,
				LastFolder,
				LastNewMapType);
		}
	}
}