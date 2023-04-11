using Jord.Core;
using Jord.Generation;
using Myra.Graphics2D.UI;
using System.Linq;

namespace Jord.MapEditor.UI
{
	public partial class GenerateMapDialog
	{
		private ScaledMapRender _mapRender;

		public Map Map => _mapRender.Map;

		public GenerateMapDialog()
		{
			BuildUI();

			_mapRender = new ScaledMapRender();
			_panelResult.Widgets.Add(_mapRender);

			var generators = (from c in TJ.Database.Generators.Values orderby c.Id select c).ToArray();
			foreach (var g in generators)
			{
				_comboGenerator.Items.Add(new ListItem(g.Id, null, g));
			}

			if (generators.Length > 0)
			{
				_comboGenerator.SelectedIndex = 0;
			}

			_buttonGenerate.Click += _buttonGenerate_Click;
			ButtonOk.Enabled = false;
		}

		private void _buttonGenerate_Click(object sender, System.EventArgs e)
		{
			var generator = (BaseGenerator)_comboGenerator.SelectedItem.Tag;

			var newMap = generator.Generate();
			_mapRender.Map = newMap;
			ButtonOk.Enabled = true;
		}
	}
}