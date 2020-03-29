using System.Windows;
using System.Windows.Controls;

namespace Daliboris.Statistiky.UI.WPF.Controls.TreeListView
{
  public class DragDropScrollViewer : ScrollViewer
  {
    protected override void OnPreviewDragOver(System.Windows.DragEventArgs e)
    {
      base.OnPreviewDragOver(e);
      double hScrollHeight = (ExtentWidth > ActualWidth) ? SystemParameters.HorizontalScrollBarHeight : 0;
      var p = e.GetPosition(this);
      if (p.Y - HeaderHeight < DragScrollThreshold)
      {
        ScrollToVerticalOffset(VerticalOffset - DragScrollIncrement);
      }
      else if (ActualHeight - p.Y - hScrollHeight < DragScrollThreshold)
      {
        ScrollToVerticalOffset(VerticalOffset + DragScrollIncrement);
      }
    }

    // this is a hack
    const double HeaderHeight = 20.0;

    public const double DragScrollThreshold = 10.0;
    public const double DragScrollIncrement = 48.0;
  }
}
