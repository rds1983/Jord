using System;

namespace RogueSharp.MapCreation
{
	/// <summary>
	/// The BorderOnlyMapCreationStrategy creates a Map of the specified type by making an empty map with only the outermost border being solid walls
	/// </summary>
	/// <typeparam name="TMap">The type of IMap that will be created</typeparam>
	/// <typeparam name="TCell">The type of ICell that the Map will use</typeparam>
	public class BorderOnlyMapCreationStrategy<TMap, TCell> : IMapCreationStrategy<TMap, TCell> where TMap : IMap<TCell>, new() where TCell : ICell
	{
		private readonly ICellInfo _space, _wall;
		private readonly int _height;
		private readonly int _width;

		/// <summary>
		/// Constructs a new BorderOnlyMapCreationStrategy with the specified parameters
		/// </summary>
		/// <param name="space"></param>
		/// <param name="wall"></param>
		/// <param name="width">The width of the Map to be created</param>
		/// <param name="height">The height of the Map to be created</param>
		public BorderOnlyMapCreationStrategy(ICellInfo space, ICellInfo wall, int width, int height)
		{
			if (space == null)
			{
				throw new ArgumentNullException(nameof(space));
			}

			if (wall == null)
			{
				throw new ArgumentNullException(nameof(wall));
			}

			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			_space = space;
			_wall = wall;
			_width = width;
			_height = height;
		}

		/// <summary>
		/// Creates a Map of the specified type by making an empty map with only the outermost border being solid walls
		/// </summary>
		/// <returns>An IMap of the specified type</returns>
		public TMap CreateMap()
		{
			var map = new TMap();
			map.Initialize(_width, _height);
			map.Clear(_space);

			foreach (TCell cell in map.GetCellsInRows(0, _height - 1))
			{
				map.SetCellInfo(cell.X, cell.Y, _wall);
			}

			foreach (TCell cell in map.GetCellsInColumns(0, _width - 1))
			{
				map.SetCellInfo(cell.X, cell.Y, _wall);
			}

			return map;
		}
	}
}