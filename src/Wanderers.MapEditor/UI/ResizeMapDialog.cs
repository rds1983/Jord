using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Wanderers.MapEditor.UI
{
	public partial class ResizeMapDialog : Dialog
	{
		public ResizeMapDialog()
		{
			BuildUI();

			foreach(var tileInfo in TJ.Module.TileInfos)
			{
				var text = tileInfo.Key;
				_comboFiller.Items.Add(new ListItem(text, null, tileInfo.Value));
			}

			_comboFiller.SelectedIndex = 0;
		}
	}
}