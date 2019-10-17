using Myra.Graphics2D.UI;
using System;
using static Myra.Graphics2D.UI.Grid;

namespace Wanderers.UI
{
	public class LogView : EndlessContainer<Panel>
	{
		private const int LogMoveUpInMs = 300;

		private Grid _gridLog;
		private DateTime? _logStarted;
		private int _moveHeight;

		public LogView()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			InternalChild = new Panel();

			_gridLog = new Grid();
			InternalChild.Widgets.Add(_gridLog);
		}

		private int CalculateTotalHeight(int start = 0)
		{
			var totalHeight = 0;
			for (var i = start; i < _gridLog.Widgets.Count; ++i)
			{
				var widget = _gridLog.Widgets[i];
				totalHeight += widget.ActualBounds.Height;

				if (i < _gridLog.Widgets.Count - 1)
				{
					totalHeight += _gridLog.RowSpacing;
				}
			}

			return totalHeight;
		}

		public void Log(string message)
		{
			// Add to the end
			var textBlock = new Label
			{
				Text = message,
				Wrap = true
			};

			_gridLog.Widgets.Add(textBlock);

			_gridLog.RowsProportions.Add(new Proportion());

			UpdateGridLogPositions();

			Desktop.UpdateLayout();

			// Recalculate total height
			var totalHeight = CalculateTotalHeight();

			if (totalHeight > ActualBounds.Height)
			{
				// Initiate log movement
				if (_logStarted == null)
				{
					_logStarted = DateTime.Now;
				}

				// Determine amount of log strings that needs to be removed
				var start = 1;
				while (start < _gridLog.Widgets.Count && totalHeight > ActualBounds.Height)
				{
					totalHeight = CalculateTotalHeight(start);
					if (totalHeight <= ActualBounds.Height)
					{
						break;
					}

					++start;
				}

				// Calculate move height
				_moveHeight = 0;
				for (var i = 0; i < start; ++i)
				{
					var widget = _gridLog.Widgets[i];

					_moveHeight += widget.ActualBounds.Height;

					if (i < start - 1)
					{
						_moveHeight += _gridLog.RowSpacing;
					}
				}

			}

			ProcessLog();
		}

		private void UpdateGridLogPositions()
		{
			for (var i = 0; i < _gridLog.Widgets.Count; ++i)
			{
				_gridLog.Widgets[i].GridRow = i;
			}
		}

		private void ProcessLog()
		{
			if (_logStarted == null)
			{
				return;
			}

			var now = DateTime.Now;
			var elapsed = now - _logStarted.Value;

			if (elapsed.TotalMilliseconds >= LogMoveUpInMs)
			{
				while (_gridLog.Widgets.Count > 0 && CalculateTotalHeight() > ActualBounds.Height)
				{
					_gridLog.Widgets.RemoveAt(0);
					_gridLog.RowsProportions.RemoveAt(0);
				}

				UpdateGridLogPositions();

				_gridLog.Top = 0;
				_logStarted = null;
				return;
			}

			var y = -(int)(elapsed.TotalMilliseconds * _moveHeight / LogMoveUpInMs);
			_gridLog.Top = y;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			ProcessLog();
		}
	}
}