/* Generated by MyraPad at 4/10/2023 8:36:33 PM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jord.MapEditor.UI
{
	partial class GenerateMapDialog: Dialog
	{
		private void BuildUI()
		{
			_comboGenerator = new ComboBox();
			_comboGenerator.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			_comboGenerator.Id = "_comboGenerator";

			_buttonGenerate = new TextButton();
			_buttonGenerate.Text = "Generate...";
			_buttonGenerate.Id = "_buttonGenerate";

			var horizontalStackPanel1 = new HorizontalStackPanel();
			horizontalStackPanel1.Spacing = 8;
			horizontalStackPanel1.Widgets.Add(_comboGenerator);
			horizontalStackPanel1.Widgets.Add(_buttonGenerate);

			_panelResult = new Panel();
			_panelResult.Id = "_panelResult";

			var verticalStackPanel1 = new VerticalStackPanel();
			verticalStackPanel1.Spacing = 8;
			verticalStackPanel1.Proportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			verticalStackPanel1.Proportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			verticalStackPanel1.Width = 800;
			verticalStackPanel1.Widgets.Add(horizontalStackPanel1);
			verticalStackPanel1.Widgets.Add(_panelResult);

			
			Title = "Generate Map";
			Left = 553;
			Top = 71;
			Height = 600;
			Content = verticalStackPanel1;
		}

		
		public ComboBox _comboGenerator;
		public TextButton _buttonGenerate;
		public Panel _panelResult;
	}
}
