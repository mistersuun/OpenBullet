// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutDocumentControl
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

public class LayoutDocumentControl : Control
{
  public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof (Model), typeof (LayoutContent), typeof (LayoutDocumentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutDocumentControl.OnModelChanged)));
  private static readonly DependencyPropertyKey LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof (LayoutItem), typeof (LayoutItem), typeof (LayoutDocumentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty LayoutItemProperty = LayoutDocumentControl.LayoutItemPropertyKey.DependencyProperty;

  static LayoutDocumentControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutDocumentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutDocumentControl)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (LayoutDocumentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  }

  public LayoutContent Model
  {
    get => (LayoutContent) this.GetValue(LayoutDocumentControl.ModelProperty);
    set => this.SetValue(LayoutDocumentControl.ModelProperty, (object) value);
  }

  private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutDocumentControl) d).OnModelChanged(e);
  }

  protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      ((LayoutElement) e.OldValue).PropertyChanged -= new PropertyChangedEventHandler(this.Model_PropertyChanged);
    if (this.Model != null)
    {
      this.Model.PropertyChanged += new PropertyChangedEventHandler(this.Model_PropertyChanged);
      this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel(this.Model));
    }
    else
      this.SetLayoutItem((LayoutItem) null);
  }

  private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!(e.PropertyName == "IsEnabled") || this.Model == null)
      return;
    this.IsEnabled = this.Model.IsEnabled;
    if (this.IsEnabled || !this.Model.IsActive || this.Model.Parent == null || !(this.Model.Parent is LayoutDocumentPane))
      return;
    ((LayoutDocumentPane) this.Model.Parent).SetNextSelectedIndex();
  }

  public LayoutItem LayoutItem
  {
    get => (LayoutItem) this.GetValue(LayoutDocumentControl.LayoutItemProperty);
  }

  protected void SetLayoutItem(LayoutItem value)
  {
    this.SetValue(LayoutDocumentControl.LayoutItemPropertyKey, (object) value);
  }

  protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    this.SetIsActive();
    base.OnPreviewGotKeyboardFocus(e);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    this.SetIsActive();
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
  {
    this.SetIsActive();
    base.OnMouseLeftButtonDown(e);
  }

  private void SetIsActive()
  {
    if (this.Model == null)
      return;
    this.Model.IsActive = true;
  }
}
