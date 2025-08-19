// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.CompletionListBox
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows.Controls;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionListBox : ListBox
{
  internal ScrollViewer scrollViewer;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.scrollViewer = (ScrollViewer) null;
    if (this.VisualChildrenCount <= 0 || !(this.GetVisualChild(0) is Border visualChild))
      return;
    this.scrollViewer = visualChild.Child as ScrollViewer;
  }

  public int FirstVisibleItem
  {
    get
    {
      return this.scrollViewer == null || this.scrollViewer.ExtentHeight == 0.0 ? 0 : (int) ((double) this.Items.Count * this.scrollViewer.VerticalOffset / this.scrollViewer.ExtentHeight);
    }
    set
    {
      value = value.CoerceValue(0, this.Items.Count - this.VisibleItemCount);
      if (this.scrollViewer == null)
        return;
      this.scrollViewer.ScrollToVerticalOffset((double) value / (double) this.Items.Count * this.scrollViewer.ExtentHeight);
    }
  }

  public int VisibleItemCount
  {
    get
    {
      return this.scrollViewer == null || this.scrollViewer.ExtentHeight == 0.0 ? 10 : Math.Max(3, (int) Math.Ceiling((double) this.Items.Count * this.scrollViewer.ViewportHeight / this.scrollViewer.ExtentHeight));
    }
  }

  public void ClearSelection() => this.SelectedIndex = -1;

  public void SelectIndex(int index)
  {
    if (index >= this.Items.Count)
      index = this.Items.Count - 1;
    if (index < 0)
      index = 0;
    this.SelectedIndex = index;
    this.ScrollIntoView(this.SelectedItem);
  }

  public void CenterViewOn(int index) => this.FirstVisibleItem = index - this.VisibleItemCount / 2;
}
