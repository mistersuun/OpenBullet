// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorableControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorableControl : Control
{
  public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof (Model), typeof (LayoutAnchorable), typeof (LayoutAnchorableControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutAnchorableControl.OnModelChanged)));
  private static readonly DependencyPropertyKey LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof (LayoutItem), typeof (LayoutItem), typeof (LayoutAnchorableControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty LayoutItemProperty = LayoutAnchorableControl.LayoutItemPropertyKey.DependencyProperty;

  static LayoutAnchorableControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAnchorableControl)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (LayoutAnchorableControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  }

  public LayoutAnchorable Model
  {
    get => (LayoutAnchorable) this.GetValue(LayoutAnchorableControl.ModelProperty);
    set => this.SetValue(LayoutAnchorableControl.ModelProperty, (object) value);
  }

  private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableControl) d).OnModelChanged(e);
  }

  protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      ((LayoutElement) e.OldValue).PropertyChanged -= new PropertyChangedEventHandler(this.Model_PropertyChanged);
    if (this.Model != null)
    {
      this.Model.PropertyChanged += new PropertyChangedEventHandler(this.Model_PropertyChanged);
      this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel((LayoutContent) this.Model));
    }
    else
      this.SetLayoutItem((LayoutItem) null);
  }

  private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!(e.PropertyName == "IsEnabled") || this.Model == null)
      return;
    this.IsEnabled = this.Model.IsEnabled;
    if (this.IsEnabled || !this.Model.IsActive || this.Model.Parent == null || !(this.Model.Parent is LayoutAnchorablePane))
      return;
    ((LayoutAnchorablePane) this.Model.Parent).SetNextSelectedIndex();
  }

  public LayoutItem LayoutItem
  {
    get => (LayoutItem) this.GetValue(LayoutAnchorableControl.LayoutItemProperty);
  }

  protected void SetLayoutItem(LayoutItem value)
  {
    this.SetValue(LayoutAnchorableControl.LayoutItemPropertyKey, (object) value);
  }

  protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    if (this.Model != null)
      this.Model.IsActive = true;
    base.OnGotKeyboardFocus(e);
  }
}
