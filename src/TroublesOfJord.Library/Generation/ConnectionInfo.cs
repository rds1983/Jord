using System.Collections.Generic;

namespace TroublesOfJord.Generation
{
	internal class ConnectionInfo
	{
		private readonly List<HashSet<int>> _connections = new List<HashSet<int>>();

		private HashSet<int> FindQueue(int index)
		{
			foreach(var q in _connections)
			{
				if (q.Contains(index))
				{
					return q;
				}
			}

			return null;
		}

		public void Connect(int from, int to)
		{
			var fromQueue = FindQueue(from);
			var toQueue = FindQueue(to);

			if (fromQueue != null && toQueue != null)
			{
				if (fromQueue == toQueue)
				{
					// Already connected
					return;
				}

				// Merge queues
				_connections.Remove(toQueue);
				foreach(var i in toQueue)
				{
					fromQueue.Add(i);
				}
			} else if (fromQueue != null)
			{
				fromQueue.Add(to);
			} else if (toQueue != null)
			{
				toQueue.Add(from);
			} else
			{
				var newQueue = new HashSet<int>
				{
					from,
					to
				};

				_connections.Add(newQueue);
			}
		}

		public bool IsConnected(int from, int to)
		{
			var fromQueue = FindQueue(from);

			return fromQueue != null && fromQueue.Contains(to);
		}
	}
}
