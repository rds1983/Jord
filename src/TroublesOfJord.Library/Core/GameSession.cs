using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TroublesOfJord.Storage;
using TroublesOfJord.UI;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	public class GameSession
	{
		private class AnimationTask
		{
			public Action<float> Update;
			public Action Finished;
		}

		private DateTime? _delayStarted;
		private int _delayInMs = 0;
		private readonly List<AnimationTask> _animations = new List<AnimationTask>();

		public Slot Slot { get; }
		public Character Character { get; }
		public Player Player { get { return Character.Player; } }

		public MapRender MapRender;
		public MapNavigationBase MapNavigationBase;

		public bool AcceptsInput
		{
			get
			{
				return _delayStarted == null;
			}
		}

		public GameSession(int slotIndex)
		{
			Slot = TJ.StorageService.Slots[slotIndex];

			Character = Slot.CharacterData.CreateCharacter();

			// Spawn player
			var map = TJ.Module.Maps[Slot.CharacterData.StartingMapId];
			Player.Place(map, map.SpawnSpot.Value);
			Player.Stats.Life.Restore();
		}

		public bool MovePlayer(MovementDirection direction, bool isRunning)
		{
			var result = Player.MoveTo(direction.GetDelta());

			if (result)
			{
				// Let npcs act
				var map = Player.Map;
				for (var x = 0; x < map.Size.X; ++x)
				{
					for (var y = 0; y < map.Size.Y; ++y)
					{
						var npc = map[x, y].Creature as NonPlayer;
						if (npc == null)
						{
							continue;
						}

						npc.Act();
					}
				}

				_delayStarted = DateTime.Now;
				if (!isRunning)
				{
					_delayInMs = Config.TurnDelayInMs;
				}
				else
				{
					_delayInMs = Config.TurnDelayInMs / 2;
				}

				UpdateTilesVisibility();
			}

			return result;
		}

		public void Update()
		{
			if (_delayStarted == null)
			{
				return;
			}

			var passed = (DateTime.Now - _delayStarted.Value).TotalMilliseconds;

			if (passed < _delayInMs)
			{
				var part = (float)passed / _delayInMs;
				foreach (var task in _animations)
				{
					task?.Update(part);
				}
			}
			else if (passed >= _delayInMs)
			{
				foreach (var task in _animations)
				{
					task?.Finished();
				}
				_animations.Clear();

				_delayStarted = null;
			}
		}

		internal void AddAnimationTask(Action<float> onUpdate, Action onFinished)
		{
			_animations.Add(new AnimationTask
			{
				Update = onUpdate,
				Finished = onFinished
			});
		}

		public void UpdateTilesVisibility()
		{
			var map = Player.Map;
			/*			for(var x = 0; x < map.Size.X; ++x)
						{
							for(var y = 0; y < map.Size.Y; ++y)
							{
								map[x, y].Visible = false;
							}
						}*/

			for (var y = (int)MapRender.TopLeft.Y;
				y <= Math.Min(MapRender.TopLeft.Y + MapRender.GridSize.Y, map.Size.Y - 1);
				++y)
			{
				for (var x = (int)MapRender.TopLeft.X;
					x <= Math.Min(MapRender.TopLeft.X + MapRender.GridSize.X, map.Size.X - 1);
					++x)
				{
					if (map[x, y].Visible)
					{
						continue;
					}

					if (Player.Position == new Point(x, y))
					{
						continue;
					}

					var toTile = new Vector2(x - Player.Position.X, y - Player.Position.Y);
					toTile.Normalize();

					var pos = Player.Position.ToVector2();
					pos.X += 0.5f;
					pos.Y += 0.5f;

					var lastPassable = true;
					for (; ; )
					{
						if (pos.X < 0 || pos.X >= map.Size.X ||
							pos.Y < 0 || pos.Y >= map.Size.Y)
						{
							break;
						}

						var tile = map[(int)pos.X, (int)pos.Y];
						if (!tile.Info.Passable && !lastPassable)
						{
							break;
						}

						tile.Visible = true;
						lastPassable = tile.Info.Passable;

						if (pos.ToPoint() == new Point(x, y))
						{
							break;
						}

						pos += toTile;
					}
				}
			}

			MapNavigationBase.InvalidateImage();
		}
	}
}