using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Wanderers.Core
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
			}

			State = CreatureState.Moving;
			_actionStart = DateTime.Now;
			_movementFinished = movementFinished;

			return true;
		}
	}
}
