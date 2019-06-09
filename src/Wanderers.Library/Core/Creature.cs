using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Wanderers.Utils;

namespace Wanderers.Core
{
	public abstract class Creature
	{
		private bool _moving;
		private const int _movementInterval = 100;
		private Point[] _movementPoints;
		private DateTime _movementStart;
		private Action _movementFinished;
		private readonly Inventory _inventory = new Inventory();

		public Map Map
		{
			get; set;
		}

		public Vector2 Position
		{
			get; set;
		}

		public abstract Appearance Image
		{
			get;
		}

		public string Name
		{
			get; set;
		}

		public int Gold
		{
			get; set;
		}

		public Inventory Inventory
		{
			get
			{
				return _inventory;
			}
		}

		public bool IsPlaceable(Map map, Vector2 pos)
		{
			if (pos.X < 0 || pos.X >= map.Size.X ||
				pos.Y < 0 || pos.Y >= map.Size.Y)
			{
				// Out of range
				return false;
			}

			var tile = map.GetTileAt(pos);
			if (tile.Creature != null || tile.Info == null || !tile.Info.Passable)
			{
				return false;
			}

			return true;
		}

		public void SetPosition(Vector2 position)
		{
			var currentTile = Map.GetTileAt(Position);
			var newTile = Map.GetTileAt(position);

			if (currentTile != newTile)
			{
				newTile.Creature = currentTile.Creature;
				currentTile.Creature = null;
			}

			Position = position;
		}

		public bool IsMoveable(Map map, Vector2 pos)
		{
			var result = false;
			var x = (int)pos.X;
			var y = (int)pos.Y;

			if (x >= 0 && y >= 0 && x < map.Size.X && y < map.Size.Y)
			{
				var tile = map.GetTileAt(pos);
				if (tile.Info.Passable &&
					(tile.Creature == null ||
					 (tile.Creature == this)))
				{
					result = true;
				}
			}

			return result;
		}

//		private static Random _random = new Random();

		public void OnTimer()
		{
			if (!_moving)
			{
/*				if (Map == null)
				{
					return;
				}

				var npc = this as NonPlayer;
				if (npc == null)
				{
					return;
				}

				Point? dest = null;
				for (var i = 0; i < 100; ++i)
				{
					var t = new Point(_random.Next(Map.Size.X), _random.Next(Map.Size.Y));
					if (TilePassable(t))
					{
						dest = t;
						break;
					}
				}

				if (dest != null)
				{
					InitiateMovement(dest.Value);
				}*/

				return;
			}

			var span = DateTime.Now - _movementStart;
			var totalPart = (float)span.TotalMilliseconds / _movementInterval;

			Point p1, p2;
			var part = 0.0f;

			if (totalPart >= _movementPoints.Length - 1)
			{
				// Finished
				p1 = p2 = _movementPoints[_movementPoints.Length - 1];
				_moving = false;

				if (_movementFinished != null)
				{
					_movementFinished();
				}
			}
			else
			{
				// Between second and other steps
				var i = (int)totalPart;
				p1 = _movementPoints[i];
				p2 = _movementPoints[i + 1];
				part = totalPart - (int)totalPart;
			}

			var d = p2;
			d.X -= p1.X;
			d.Y -= p1.Y;
			var p = Vector2.Zero;
			p.X = p1.X + part * d.X;
			p.Y = p1.Y + part * d.Y;

			SetPosition(p);
		}

		private bool TilePassable(Point pos)
		{
			var tile = Map.GetTileAt(pos);

			return tile.Info.Passable &&
				   (tile.Creature == null ||
					tile.Creature == this);
		}

		public bool InitiateMovement(Point pos, float distance = 0.0f, Action movementFinished = null)
		{
			if (_moving)
			{
				return false;
			}

			if (Map == null)
			{
				return false;
			}

			var vpos = pos.ToVector2();
			var pathFinder = new PathFinder(new Point((int)Position.X, (int)Position.Y),
											pos,
											Map.Size,
											TilePassable,
											p =>
											{
												return Vector2.Distance(p.ToVector2(), vpos) <= distance;
											});

			var result = pathFinder.Process();
			if (result == null || result.Length == 0)
			{
				return false;
			}

			_movementPoints = new Point[result.Length + 1];
			_movementPoints[0] = pathFinder.Start;
			for (var i = 1; i < _movementPoints.Length; ++i)
			{
				_movementPoints[i] = result[i - 1].Position;
			}

			_moving = true;
			_movementStart = DateTime.Now;
			_movementFinished = movementFinished;

			return true;
		}

		public void Place(Map map, Vector2 position)
		{
			if (position.X < 0 || position.X >= map.Size.X ||
				position.Y < 0 || position.Y >= map.Size.Y)
			{
				throw new ArgumentOutOfRangeException("position");
			}

			Map = map;
			Position = position;

			var tile = map.GetTileAt(position);
			tile.Creature = this;
		}

		public bool Remove()
		{
			if (Map == null)
			{
				return false;
			}

			var tile = Map.GetTileAt(Position);
			tile.Creature = null;

			Map = null;

			return true;
		}
	}
}
