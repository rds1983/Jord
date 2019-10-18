using System;
using Microsoft.Xna.Framework;

namespace Wanderers.Core
{
	public enum CreatureState
	{
		Idle,
		Moving,
		Fighting
	}

	public abstract class Creature
	{
		private const int TargetAttackDelayInMs = 1000;
		private const int AttackMoveInMs = 200;
		private const int TwoAttackMoveInMs = AttackMoveInMs * 2;

		private CreatureState _state;
		private const int _movementInterval = 100;
		private Point[] _movementPoints;
		private DateTime _actionStart;
		private DateTime? _attackStart;
		private Vector2 _attackPosition;
		private Action _movementFinished;
		private readonly Inventory _inventory = new Inventory();

		public Map Map { get; set; }
		public Vector2 Position { get; set; }

		public Tile Tile
		{
			get
			{
				if (Map == null)
				{
					return null;
				}

				return Map.GetTileAt(Position);
			}
		}

		public abstract Appearance Image { get; }
		public string Name { get; set; }
		public int Gold { get; set; }

		private CreatureState State
		{
			get
			{
				return _state;
			}

			set
			{
				if (value == _state)
				{
					return;
				}

				_state = value;

				if (_state == CreatureState.Idle)
				{
					TJ.Session.RemoveActiveCreature(this);
				} else
				{
					TJ.Session.AddActiveCreature(this);
				}
			}
		}

		public Inventory Inventory
		{
			get
			{
				return _inventory;
			}
		}

		public Creature AttackTarget { get; set; }
		public int AttackDelayInMs { get; set; }

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

		private void ProcessFighting()
		{
			if (!IsAttackable(AttackTarget))
			{
				// End fighting
				State = CreatureState.Idle;
				AttackTarget = null;
				return;
			}

			var now = DateTime.Now;
			var span = now - _actionStart;
			if (span.TotalMilliseconds >= AttackDelayInMs)
			{
				_attackStart = now;
				_actionStart = now;
				AttackDelayInMs = 3000;

				TJ.GameLog("{0} hits {1}", Name, AttackTarget.Name);
			}

			if (_attackStart != null)
			{
				span = now - _attackStart.Value;
				var delta = new Vector2(AttackTarget._attackPosition.X - _attackPosition.X,
					AttackTarget._attackPosition.Y - _attackPosition.Y) / 2;

				if (span.TotalMilliseconds < AttackMoveInMs)
				{
					// First phase
					Position = new Vector2(
						_attackPosition.X + delta.X * (float)span.TotalMilliseconds / AttackMoveInMs,
						_attackPosition.Y + delta.Y * (float)span.TotalMilliseconds / AttackMoveInMs);
				} else if (span.TotalMilliseconds < TwoAttackMoveInMs)
				{
					// Second phase
					var f = (TwoAttackMoveInMs - (float)span.TotalMilliseconds);
					Position = new Vector2(
						_attackPosition.X + delta.X * f / TwoAttackMoveInMs,
						_attackPosition.Y + delta.Y * f / TwoAttackMoveInMs);
				} else
				{
					// Set position back
					Position = _attackPosition;
					_attackStart = null;
				}
			}
		}

		public void OnTimer()
		{
			switch (State)
			{
				case CreatureState.Idle:
					// Nothing
					break;
				case CreatureState.Moving:
					ProcessMovement();
					break;
				case CreatureState.Fighting:
					ProcessFighting();
					break;
			}
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

		private bool IsAttackable(Creature target)
		{
			if (Math.Abs(Position.X - target.Position.X) > 1 ||
				Math.Abs(Position.Y - target.Position.Y) > 1)
			{
				// Too far away
				return false;
			}

			return true;
		}

		public bool Attack(Creature target)
		{
			if (!IsAttackable(target))
			{
				return false;
			}

			var now = DateTime.Now;

			_actionStart = now;
			State = CreatureState.Fighting;
			AttackTarget = target;
			AttackDelayInMs = 0;
			_attackPosition = Position;

			target._actionStart = now;
			target.AttackTarget = this;
			target.State = CreatureState.Fighting;
			target.AttackDelayInMs = TargetAttackDelayInMs;
			target._attackPosition = target.Position;

			TJ.GameLog("{0} attacked {1}...", Name, target.Name);

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