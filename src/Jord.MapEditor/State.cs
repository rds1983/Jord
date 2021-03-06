﻿using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Jord.MapEditor
{
	public class State
	{
		public const string StateFileName = "Jord.MapEditor.config";

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