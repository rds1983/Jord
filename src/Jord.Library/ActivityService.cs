using System;
using System.Collections.Generic;

namespace Jord
{
	public class ActivityService
	{
		private class Activity
		{
			public Activity(Action<int> onUpdate, Action onEnd, int durationInMs)
			{
				Start = DateTime.Now;
				OnUpdate = onUpdate;
				OnEnd = onEnd;
				DurationInMs = durationInMs;
			}

			public DateTime Start { get; }
			public Action<int> OnUpdate { get; }
			public Action OnEnd { get; }
			public int DurationInMs { get; }
		}

		private readonly List<Activity> _activities = new List<Activity>();
		private readonly List<Activity> _toDelete = new List<Activity>();

		public bool IsBusy => _activities.Count > 0;

		public void AddActivity(Action<int> onUpdate, Action onEnd, int durationInMs)
		{
			_activities.Add(new Activity(onUpdate, onEnd, durationInMs));
		}

		public void Update()
		{
			foreach (var activity in _activities)
			{
				var passed = (int)(DateTime.Now - activity.Start).TotalMilliseconds;

				if (passed >= activity.DurationInMs)
				{
					activity.OnEnd?.Invoke();
					_toDelete.Add(activity);
				}
				else
				{
					activity.OnUpdate?.Invoke(passed);
				}
			}

			foreach (var activity in _toDelete)
			{
				_activities.Remove(activity);
			}
		}
	}
}
