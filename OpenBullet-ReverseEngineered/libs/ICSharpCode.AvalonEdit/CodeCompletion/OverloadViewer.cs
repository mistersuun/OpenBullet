// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.OverloadViewer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class OverloadViewer : Control
{
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (OverloadViewer));
  public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(nameof (Provider), typeof (IOverloadProvider), typeof (OverloadViewer));

  static OverloadViewer()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (OverloadViewer), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (OverloadViewer)));
  }

  public string Text
  {
    get => (string) this.GetValue(OverloadViewer.TextProperty);
    set => this.SetValue(OverloadViewer.TextProperty, (object) value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    ((ButtonBase) this.Template.FindName("PART_UP", (FrameworkElement) this)).Click += (RoutedEventHandler) ((sender, e) =>
    {
      e.Handled = true;
      this.ChangeIndex(-1);
    });
    ((ButtonBase) this.Template.FindName("PART_DOWN", (FrameworkElement) this)).Click += (RoutedEventHandler) ((sender, e) =>
    {
      e.Handled = true;
      this.ChangeIndex(1);
    });
  }

  public IOverloadProvider Provider
  {
    get => (IOverloadProvider) this.GetValue(OverloadViewer.ProviderProperty);
    set => this.SetValue(OverloadViewer.ProviderProperty, (object) value);
  }

  public void ChangeIndex(int relativeIndexChange)
  {
    IOverloadProvider provider = this.Provider;
    if (provider == null)
      return;
    int num = provider.SelectedIndex + relativeIndexChange;
    if (num < 0)
      num = provider.Count - 1;
    if (num >= provider.Count)
      num = 0;
    provider.SelectedIndex = num;
  }
}
