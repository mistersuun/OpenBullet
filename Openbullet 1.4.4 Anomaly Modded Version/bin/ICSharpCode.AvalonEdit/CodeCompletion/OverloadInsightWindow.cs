// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.OverloadInsightWindow
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Editing;
using System.Windows;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class OverloadInsightWindow : InsightWindow
{
  private OverloadViewer overloadViewer = new OverloadViewer();

  public OverloadInsightWindow(TextArea textArea)
    : base(textArea)
  {
    this.overloadViewer.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
    this.Content = (object) this.overloadViewer;
  }

  public IOverloadProvider Provider
  {
    get => this.overloadViewer.Provider;
    set => this.overloadViewer.Provider = value;
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (e.Handled || this.Provider == null || this.Provider.Count <= 1)
      return;
    switch (e.Key)
    {
      case Key.Up:
        e.Handled = true;
        this.overloadViewer.ChangeIndex(-1);
        break;
      case Key.Down:
        e.Handled = true;
        this.overloadViewer.ChangeIndex(1);
        break;
    }
    if (!e.Handled)
      return;
    this.UpdateLayout();
    this.UpdatePosition();
  }
}
