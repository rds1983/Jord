using System;
using System.Collections.Generic;

namespace Jord
{
	public class ActivityService
	{
		private class Activity
		{
			private DateTime? _start;

			public Activity(Action<float> onUpdate, Action onEnd, int durationInMs)
			{
				OnUpdate = onUpdate;
				OnEnd = onEnd;
				DurationInMs = durationInMs;
			}

			public DateTime Start
			{
				get
				{
					if (_start != null)
					{
						return _start.Value;
					}

					_start = DateTime.Now;
					return _start.Value;
				}
			}
			public Action<float> OnUpdate { get; }
			public Action OnEnd { get; }
			public int DurationInMs { get; }
		}

		private readonly List<Activity> _parallelActivities = new List<Activity>();
		private readonly List<Activity> _orderedActivities = new List<Activity>();
		private readonly List<Activity> _toDelete = new List<Activity>();

		public bool IsBusy => _parallelActivities.Count > 0 || _orderedActivities.Count > 0;

		public void AddParallelActivity(Action<float> onUpdate, Action onEnd, int durationInMs)
		{
			_parallelActivities.Add(new Activity(onUpdate, onEnd, durationInMs));
		}

		public void AddOrderedActivity(Action<float> onUpdate, Action onEnd, int durationInMs)
		{
			_orderedActivities.Add(new Activity(onUpdate, onEnd, durationInMs));
		}

		public void Update()
		{
			foreach (var activity in _parallelActivities)
			{
				var passed = (int)(DateTime.Now - activity.Start).TotalMilliseconds;
				if (passed >= activity.DurationInMs)
				{
					activity.OnEnd?.Invoke();
					_toDelete.Add(activity);
				}
				else
				{
					activity.OnUpdate?.Invoke((float)passed / activity.DurationInMs);
				}
			}

			foreach (var activity in _toDelete)
			{
				_parallelActivities.Remove(activity);
			}

			// Only one ordered activity(first) is executed in the time
			if (_orderedActivities.Count > 0)
			{
				var activity = _orderedActivities[0];
				var passed = (int)(DateTime.Now - activity.Start).TotalMilliseconds;
				if (passed >= activity.DurationInMs)
				{
					activity.OnEnd?.Invoke();
					_orderedActivities.RemoveAt(0);
				}
				else
				{
					activity.OnUpdate?.Invoke((float)passed / activity.DurationInMs);
				}
			}
		}
	}
}
