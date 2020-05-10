using Myra.Graphics2D.UI;
using System.Linq;

namespace TroublesOfJord.MapEditor.UI
{
	public partial class ChooseMapDialog
	{
		public ChooseMapDialog()
		{
			BuildUI();

			_listMaps.SelectedIndexChanged += (s, a) => UpdateEnabled();

			var maps = (from c in TJ.Module.Maps.Values orderby c.Id select c).ToArray();

			int? selectedIndex = null;
			for(var i = 0; i < maps.Length; ++i)
			{
				var m = maps[i];
				_listMaps.Items.Add(new ListItem(m.Id, null, m));

				if (Studio.Instance.Map == m)
				{
					selectedIndex = i;
				}
			}

			_listMaps.SelectedIndex = selectedIndex;
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			ButtonOk.Enabled = _listMaps.SelectedIndex != null;
		}
	}
}