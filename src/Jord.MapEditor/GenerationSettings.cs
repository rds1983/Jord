using Myra.Attributes;
using Myra.Graphics2D.UI.File;

namespace Jord.MapEditor
{
	public class GenerationSettings
	{
		[FilePath(FileDialogMode.OpenFile, "*.bin", true)]
		public string InputFile { get; set; }
	}
}
