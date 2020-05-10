using System.Collections.Generic;

namespace Wanderers.Generator
{
	public class LocationsGeneratorConfig
	{
		private readonly List<LocationConfig> _locations = new List<LocationConfig>();

		public List<LocationConfig> Locations
		{
			get
			{
				return _locations;
			}
		}

		public LocationsGeneratorConfig()
		{
			_locations.Add(new LocationConfig { Name = "Capital" });
			_locations.Add(new LocationConfig { Name = "Ur" });
			_locations.Add(new LocationConfig { Name = "Wanderers" });
		}
	}
}