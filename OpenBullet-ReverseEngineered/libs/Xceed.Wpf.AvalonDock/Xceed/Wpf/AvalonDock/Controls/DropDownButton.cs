// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DropDownButton
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class DropDownButton : ToggleButton
{
  public static readonly DependencyProperty DropDownContextMenuProperty = DependencyProperty.Register(nameof (DropDownContextMenu), typeof (ContextMenu), typeof (DropDownButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DropDownButton.OnDropDownContextMenuChanged)));
  public static readonly DependencyProperty DropDownContextMenuDataContextProperty = DependencyProperty.Register(nameof (DropDownContextMenuDataContext), typeof (object), typeof (DropDownButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));

  public DropDownButton() => this.Unloaded += new RoutedEventHandler(this.DropDownButton_Unloaded);

  public ContextMenu DropDownContextMenu
  {
    get => (ContextMenu) this.GetValue(DropDownButton.DropDownContextMenuProperty);
    set => this.SetValue(DropDownButton.DropDownContextMenuProperty, (object) value);
  }

  private static void OnDropDownContextMenuChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DropDownButton) d).OnDropDownContextMenuChanged(e);
  }

  protected virtual void OnDropDownContextMenuChanged(DependencyPropertyChangedEventArgs e)
  {
    if (!(e.OldValue is ContextMenu oldValue) || !this.IsChecked.GetValueOrDefault())
      return;
    oldValue.Closed -= new RoutedEventHandler(this.OnContextMenuClosed);
  }

  public object DropDownContextMenuDataContext
  {
    get => this.GetValue(DropDownButton.DropDownContextMenuDataContextProperty);
    set => this.SetValue(DropDownButton.DropDownContextMenuDataContextProperty, value);
  }

  protected override void OnClick()
  {
    if (this.DropDownContextMenu != null)
    {
      this.DropDownContextMenu.PlacementTarget = (UIElement) this;
      this.DropDownContextMenu.Placement = PlacementMode.Bottom;
      this.DropDownContextMenu.DataContext = this.DropDownContextMenuDataContext;
      this.DropDownContextMenu.Closed += new RoutedEventHandler(this.OnContextMenuClosed);
      this.DropDownContextMenu.IsOpen = true;
    }
    base.OnClick();
  }

  private void OnContextMenuClosed(object sender, RoutedEventArgs e)
  {
    (sender as ContextMenu).Closed -= new RoutedEventHandler(this.OnContextMenuClosed);
    this.IsChecked = new bool?(false);
  }

  private void DropDownButton_Unloaded(object sender, RoutedEventArgs e)
  {
    if (!this.IsLoaded)
      return;
    this.DropDownContextMenu = (ContextMenu) null;
  }
}
