/* Generated by MyraPad at 23.10.2019 22:56:55 */
using Myra.Graphics2D.UI;
using System.Linq;

namespace Wanderers.MapEditor.UI
{
	public partial class NewGeneratedMapDialog
	{
		public NewGeneratedMapDialog()
		{
			BuildUI();

			var generators = (from c in TJ.Module.Generators.Values orderby c.Id select c).ToArray();
			foreach (var g in generators)
			{
				_comboGenerator.Items.Add(new ListItem(g.Id, null, g));
			}

			if (generators.Length > 0)
			{
				_comboGenerator.SelectedIndex = 0;
			}

			UpdateEnabled();

			_textId.TextChanged += (s, a) => UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			ButtonOk.Enabled = _comboGenerator.SelectedIndex != null && !string.IsNullOrEmpty(_textId.Text);
		}
	}
}