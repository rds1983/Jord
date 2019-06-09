using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Wanderers.MapEditor
{
	public class State
	{
		public const string StateFileName = "Wanderers.MapEditor.config";

		public static string StateFilePath
		{
			get
			{
				var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), StateFileName);
				return result;
			}
		}

		public Point Size { get; set; }
		public float TopSplitterPosition { get; set; }
		public float RightSplitterPosition { get; set; }
		public string EditedFile { get; set; }
		public int[] CustomColors { get; set; }
		public string LastFolder
		{
			get; set;
		}

		public void Save()
		{
			using (var stream = new StreamWriter(StateFilePath, false))
			{
				var serializer = new XmlSerializer(typeof (State));
				serializer.Serialize(stream, this);
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
				var serializer = new XmlSerializer(typeof (State));
				state = (State) serializer.Deserialize(stream);
			}

			return state;
		}

		public override string ToString()
		{
			return string.Format("Size = {0}\n" +
			                     "TopSplitter = {1:0.##}\n" +
			                     "RightSplitter= {2:0.##}\n" +
			                     "EditedFile = {3}\n" +
			                     "CustomColors = {4}\n",
								 "LastFolder = {5}",
				Size,
				TopSplitterPosition,
				RightSplitterPosition,
				EditedFile,
				CustomColors,
				LastFolder);
		}
	}
}