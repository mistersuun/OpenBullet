// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchPanelAdorner
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

internal class SearchPanelAdorner : Adorner
{
  private SearchPanel panel;

  public SearchPanelAdorner(TextArea textArea, SearchPanel panel)
    : base((UIElement) textArea)
  {
    this.panel = panel;
    this.AddVisualChild((Visual) panel);
  }

  protected override int VisualChildrenCount => 1;

  protected override Visual GetVisualChild(int index)
  {
    if (index != 0)
      throw new ArgumentOutOfRangeException();
    return (Visual) this.panel;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    this.panel.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
    return new Size(this.panel.ActualWidth, this.panel.ActualHeight);
  }
}
