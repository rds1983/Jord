using Microsoft.Xna.Framework;
using System;

namespace Wanderers.Core
{
	partial class Creature
	{
		private const int _movementInterval = 100;
		private Point[] _movementPoints;
		private Action _movementFinished;

		private void ProcessMovement()
		{
			var span = DateTime.Now - _actionStart;
			var totalPart = (float)span.TotalMilliseconds / _movementInterval;

			Point p1, p2;
			var part = 0.0f;

			if (totalPart >= _movementPoints.Length - 1)
			{
				// Finished
				p1 = p2 = _movementPoints[_movementPoints.Length - 1];
				State = CreatureState.Idle;
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

			State = CreatureState.Moving;
			_actionStart = DateTime.Now;
			_movementFinished = movementFinished;

			return true;
		}
	}
}
