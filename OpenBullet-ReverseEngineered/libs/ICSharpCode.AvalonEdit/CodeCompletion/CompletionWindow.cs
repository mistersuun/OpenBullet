// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.CompletionWindow
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionWindow : CompletionWindowBase
{
  private readonly CompletionList completionList = new CompletionList();
  private System.Windows.Controls.ToolTip toolTip = new System.Windows.Controls.ToolTip();

  public CompletionList CompletionList => this.completionList;

  public CompletionWindow(TextArea textArea)
    : base(textArea)
  {
    this.CloseAutomatically = true;
    this.SizeToContent = SizeToContent.Height;
    this.MaxHeight = 300.0;
    this.Width = 175.0;
    this.Content = (object) this.completionList;
    this.MinHeight = 15.0;
    this.MinWidth = 30.0;
    this.toolTip.PlacementTarget = (UIElement) this;
    this.toolTip.Placement = PlacementMode.Right;
    this.toolTip.Closed += new RoutedEventHandler(this.toolTip_Closed);
    this.AttachEvents();
  }

  private void toolTip_Closed(object sender, RoutedEventArgs e)
  {
    if (this.toolTip == null)
      return;
    this.toolTip.Content = (object) null;
  }

  private void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    ICompletionData selectedItem = this.completionList.SelectedItem;
    if (selectedItem == null)
      return;
    object description = selectedItem.Description;
    if (description != null)
    {
      if (description is string str)
        this.toolTip.Content = (object) new TextBlock()
        {
          Text = str,
          TextWrapping = TextWrapping.Wrap
        };
      else
        this.toolTip.Content = description;
      this.toolTip.IsOpen = true;
    }
    else
      this.toolTip.IsOpen = false;
  }

  private void completionList_InsertionRequested(object sender, EventArgs e)
  {
    this.Close();
    this.completionList.SelectedItem?.Complete(this.TextArea, (ISegment) new AnchorSegment(this.TextArea.Document, this.StartOffset, this.EndOffset - this.StartOffset), e);
  }

  private void AttachEvents()
  {
    this.completionList.InsertionRequested += new EventHandler(this.completionList_InsertionRequested);
    this.completionList.SelectionChanged += new SelectionChangedEventHandler(this.completionList_SelectionChanged);
    this.TextArea.Caret.PositionChanged += new EventHandler(this.CaretPositionChanged);
    this.TextArea.MouseWheel += new MouseWheelEventHandler(this.textArea_MouseWheel);
    this.TextArea.PreviewTextInput += new TextCompositionEventHandler(this.textArea_PreviewTextInput);
  }

  protected override void DetachEvents()
  {
    this.completionList.InsertionRequested -= new EventHandler(this.completionList_InsertionRequested);
    this.completionList.SelectionChanged -= new SelectionChangedEventHandler(this.completionList_SelectionChanged);
    this.TextArea.Caret.PositionChanged -= new EventHandler(this.CaretPositionChanged);
    this.TextArea.MouseWheel -= new MouseWheelEventHandler(this.textArea_MouseWheel);
    this.TextArea.PreviewTextInput -= new TextCompositionEventHandler(this.textArea_PreviewTextInput);
    base.DetachEvents();
  }

  protected override void OnClosed(EventArgs e)
  {
    base.OnClosed(e);
    if (this.toolTip == null)
      return;
    this.toolTip.IsOpen = false;
    this.toolTip = (System.Windows.Controls.ToolTip) null;
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (e.Handled)
      return;
    this.completionList.HandleKey(e);
  }

  private void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
  {
    e.Handled = CompletionWindowBase.RaiseEventPair((UIElement) this, UIElement.PreviewTextInputEvent, UIElement.TextInputEvent, (RoutedEventArgs) new TextCompositionEventArgs(e.Device, e.TextComposition));
  }

  private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
  {
    e.Handled = CompletionWindowBase.RaiseEventPair(this.GetScrollEventTarget(), UIElement.PreviewMouseWheelEvent, UIElement.MouseWheelEvent, (RoutedEventArgs) new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
  }

  private UIElement GetScrollEventTarget()
  {
    return this.completionList == null ? (UIElement) this : (UIElement) this.completionList.ScrollViewer ?? (UIElement) this.completionList.ListBox ?? (UIElement) this.completionList;
  }

  public bool CloseAutomatically { get; set; }

  protected override bool CloseOnFocusLost => this.CloseAutomatically;

  public bool CloseWhenCaretAtBeginning { get; set; }

  private void CaretPositionChanged(object sender, EventArgs e)
  {
    int offset = this.TextArea.Caret.Offset;
    if (offset == this.StartOffset)
    {
      if (this.CloseAutomatically && this.CloseWhenCaretAtBeginning)
        this.Close();
      else
        this.completionList.SelectItem(string.Empty);
    }
    else if (offset < this.StartOffset || offset > this.EndOffset)
    {
      if (!this.CloseAutomatically)
        return;
      this.Close();
    }
    else
    {
      TextDocument document = this.TextArea.Document;
      if (document == null)
        return;
      this.completionList.SelectItem(document.GetText(this.StartOffset, offset - this.StartOffset));
    }
  }
}
