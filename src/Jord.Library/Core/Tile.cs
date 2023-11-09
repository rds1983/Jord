using GoRogue.GameFramework;
using Jord.Utils;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;
using System;

namespace Jord.Core
{
	public class Tile
	{
		private TileInfo _info;
		private readonly Map _map;
		private Appearance _appearance;

		public TileInfo Info
		{
			get
			{
				return _info;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_info = value;
				ResetAppearance();
			}
		}

		public bool IsExitUp => Info.Id == "ExitUp";
		public bool IsExitDown => Info.Id == "ExitDown";
		public bool IsExit => IsExitUp || IsExitDown;

		public TileObject Object { get; set; }


		public Creature Creature
		{
			get;
			internal set;
		}

		public int X { get; set; }
		public int Y { get; set; }

		public Point Position
		{
			get
			{
				return new Point(X, Y);
			}

			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public bool IsInFov { get; set; }
		public bool IsExplored { get; set; }

		public bool Highlighted { get; set; }

		public string Sign { get; set; }

		public readonly Inventory Inventory = new Inventory();

		public Map Map => _map;

		public Appearance Appearance
		{
			get
			{
				if (_appearance != null)
				{
					return _appearance;
				}

				TextureRegion appearance = null;
				var tileAppearance = Info.TileAppearance;
				if (tileAppearance != null)
				{
					appearance = tileAppearance.Default;
					if (X > 0 && Y > 0 && X < Map.Width - 1 && Y < Map.Height - 1)
					{
						foreach (var choice in tileAppearance.Choices)
						{
							var fits = true;
							foreach (var condition in choice.Conditions)
							{
								var delta = condition.Direction.GetDelta();
								var nx = X + delta.X;
								var ny = Y + delta.Y;

								if ((condition.Is && Map[nx, ny].Info.Id != condition.TileInfoId) ||
									(!condition.Is && Map[nx, ny].Info.Id == condition.TileInfoId))
								{
									fits = false;
									break;
								}
							}

							if (fits)
							{
								appearance = choice.Image;
								break;
							}
						}
					}
				}

				_appearance = new Appearance(Info.Image.Symbol, Info.Image.Color, appearance);

				return _appearance;
			}
		}

		public Tile(Map map)
		{
			_map = map ?? throw new ArgumentNullException(nameof(map));
		}

		public void ResetAppearance()
		{
			_appearance = null;
		}
	}
}
