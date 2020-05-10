using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	partial class Creature
	{
		private const int _movementInterval = 100;
		private Point _movementTarget;
		private readonly Queue<Point> _movementQueue = new Queue<Point>();

		private Action _movementFinished;

		private void ProcessMovement()
		{
			var now = DateTime.Now;

			var span = now - _actionStart;
			var totalPart = (float)span.TotalMilliseconds / _movementInterval;
			if (totalPart >= 1.0f)
			{
				Map[_movementTarget].Highlighted = false;
				SetPosition(_movementTarget);
				if (_movementQueue.Count == 0)
				{
					// Finished
					State = CreatureState.Idle;
				} else
				{
					// Next target
					_movementTarget = _movementQueue.Dequeue();
					_actionStart = now;
				}
			}
			else
			{
				// Between second and other steps
				var part = totalPart - (int)totalPart;

				var p1 = Position.ToVector2();
				var p2 = _movementTarget.ToVector2();
				var d = p2 - p1;

				var p = Vector2.Zero;
				p.X = p1.X + part * d.X;
				p.Y = p1.Y + part * d.Y;

				DisplayPosition = p;
			}

			if (State == CreatureState.Idle && _movementFinished != null)
			{
				// Movement finished
				_movementFinished();
			}
		}

		public bool InitiateMovement(Point pos, float distance = 0.0f, Action movementFinished = null)
		{
			if (State == CreatureState.Moving)
			{
				return false;
			}

			if (Map == null)
			{
				return false;
			}

			var vpos = pos.ToVector2();
			if (Vector2.Distance(Position.ToVector2(), vpos) <= distance)
			{
				// Already there
				movementFinished?.Invoke();

				return true;
			}

			var pathFinder = new PathFinder(Position,
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

			_movementTarget = result[0].Position;
			for (var i = 1; i < result.Length; ++i)
			{
				_movementQueue.Enqueue(result[i].Position);
				Map[result[i].Position].Highlighted = true;
			}

			State = CreatureState.Moving;
			_actionStart = DateTime.Now;
			_movementFinished = movementFinished;

			return true;
		}

		public bool Enter()
		{
			if (Tile == null || Tile.Exit == null)
			{
				return false;
			}

			Map map = null;
			Tile exitTile = null;

			if (TJ.Module.MapTemplates.ContainsKey(Tile.Exit.MapId))
			{
				var mapTemplate = TJ.Module.EnsureMapTemplate(Tile.Exit.MapId);
				map = mapTemplate.Generate();
				for(var x = 0; x < map.Size.X; ++x)
				{
					for(var y = 0; y < map.Size.Y; ++y)
					{
						var tile = map[x, y];
						if (tile.Exit == null)
						{
							continue;
						}

						if (tile.Exit.MapId == Map.Id)
						{
							// Found backwards exit
							exitTile = tile;
							goto found;
						}
					}
				}
				found:;
			} else
			{
				map = TJ.Module.EnsureMap(Tile.Exit.MapId);

				if (Tile.Exit.Position != null)
				{
					exitTile = map[Tile.Exit.Position.Value];
				} else
				{
					exitTile = map.EnsureExitTileById(Tile.Exit.ExitMapId);
				}
			}

			Remove();
			Place(map, exitTile.Position);

			return true;
		}
	}
}
