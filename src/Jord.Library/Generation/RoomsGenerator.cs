using GoRogue;
using GoRogue.MapGeneration;
using GoRogue.MapViews;
using Jord.Core;

namespace Jord.Generation
{
	public class RoomsGenerator: BaseGenerator
	{
		private class MapSetter : ISettableMapView<bool>
		{
			private readonly RoomsGenerator _generator;

			public Map Map { get; }

			public bool this[Coord pos]
			{
				get => Map[pos].Info == _generator.Space;
				set => Map[pos].Info = value ? _generator.Space : _generator.Wall;
			}

			public bool this[int index1D]
			{
				get => Map[index1D].Info == _generator.Space;
				set => Map[index1D].Info = value ? _generator.Space : _generator.Wall;
			}

			public bool this[int x, int y]
			{
				get => Map[x, y].Info == _generator.Space;
				set => Map[x, y].Info = value ? _generator.Space : _generator.Wall;
			}

			bool IMapView<bool>.this[Coord pos] => this[pos];

			bool IMapView<bool>.this[int index1D] => this[index1D];

			bool IMapView<bool>.this[int x, int y] => this[x, y];

			public int Height => Map.Width;

			public int Width => Map.Height;

			public MapSetter(RoomsGenerator generator)
			{
				_generator = generator;
				Map = new Map(generator.Width, generator.Height);
			}
		}

		public TileInfo Space { get; set; }
		public TileInfo Wall { get; set; }

		public int MaximumRoomsCount { get; }
		public int MinimumRoomWidth { get; }
		public int MaximumRoomWidth { get; }

		public RoomsGenerator(int width, int height,
			int maximumRoomsCount, 
			int minimumRoomWidth, int maximumRoomWidth): base(width, height)
		{
			MaximumRoomsCount = maximumRoomsCount;
			MinimumRoomWidth = minimumRoomWidth;
			MaximumRoomWidth = maximumRoomWidth;
		}

		public override Map Generate()
		{
			var setter = new MapSetter(this);
			QuickGenerators.GenerateRandomRoomsMap(setter, 
				MaximumRoomsCount,
				MinimumRoomWidth, 
				MaximumRoomWidth);

			return setter.Map;
		}
	}
}
