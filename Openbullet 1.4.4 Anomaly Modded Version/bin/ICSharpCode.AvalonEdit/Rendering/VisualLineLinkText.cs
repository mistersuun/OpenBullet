// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineLinkText
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineLinkText : VisualLineText
{
  public Uri NavigateUri { get; set; }

  public string TargetName { get; set; }

  public bool RequireControlModifierForClick { get; set; }

  public VisualLineLinkText(VisualLine parentVisualLine, int length)
    : base(parentVisualLine, length)
  {
    this.RequireControlModifierForClick = true;
  }

  public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
  {
    this.TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
    this.TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
    if (context.TextView.LinkTextUnderline)
      this.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
    return base.CreateTextRun(startVisualColumn, context);
  }

  protected virtual bool LinkIsClickable()
  {
    if (this.NavigateUri == (Uri) null)
      return false;
    return !this.RequireControlModifierForClick || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
  }

  protected internal override void OnQueryCursor(QueryCursorEventArgs e)
  {
    if (!this.LinkIsClickable())
      return;
    e.Handled = true;
    e.Cursor = Cursors.Hand;
  }

  protected internal override void OnMouseDown(MouseButtonEventArgs e)
  {
    if (e.ChangedButton != MouseButton.Left || e.Handled || !this.LinkIsClickable())
      return;
    RequestNavigateEventArgs e1 = new RequestNavigateEventArgs(this.NavigateUri, this.TargetName);
    e1.RoutedEvent = Hyperlink.RequestNavigateEvent;
    if (e.Source is FrameworkElement source)
      source.RaiseEvent((RoutedEventArgs) e1);
    if (!e1.Handled)
    {
      try
      {
        Process.Start(this.NavigateUri.ToString());
      }
      catch
      {
      }
    }
    e.Handled = true;
  }

  protected override VisualLineText CreateInstance(int length)
  {
    return (VisualLineText) new VisualLineLinkText(this.ParentVisualLine, length)
    {
      NavigateUri = this.NavigateUri,
      TargetName = this.TargetName,
      RequireControlModifierForClick = this.RequireControlModifierForClick
    };
  }
}
