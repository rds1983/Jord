using DefaultEcs;
using Microsoft.Xna.Framework;

namespace Jord.Core
{
	public interface ISpawnable
	{
		Entity Spawn(Point position);
	}
}
