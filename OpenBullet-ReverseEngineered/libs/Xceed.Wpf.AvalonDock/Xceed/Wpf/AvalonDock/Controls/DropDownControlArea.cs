// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DropDownControlArea
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class DropDownControlArea : UserControl
{
  public static readonly DependencyProperty DropDownContextMenuProperty = DependencyProperty.Register(nameof (DropDownContextMenu), typeof (ContextMenu), typeof (DropDownControlArea), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty DropDownContextMenuDataContextProperty = DependencyProperty.Register(nameof (DropDownContextMenuDataContext), typeof (object), typeof (DropDownControlArea), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));

  public ContextMenu DropDownContextMenu
  {
    get => (ContextMenu) this.GetValue(DropDownControlArea.DropDownContextMenuProperty);
    set => this.SetValue(DropDownControlArea.DropDownContextMenuProperty, (object) value);
  }

  public object DropDownContextMenuDataContext
  {
    get => this.GetValue(DropDownControlArea.DropDownContextMenuDataContextProperty);
    set => this.SetValue(DropDownControlArea.DropDownContextMenuDataContextProperty, value);
  }

  protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseRightButtonDown(e);
  }

  protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
  {
    base.OnPreviewMouseRightButtonUp(e);
    if (e.Handled || this.DropDownContextMenu == null)
      return;
    this.DropDownContextMenu.PlacementTarget = (UIElement) null;
    this.DropDownContextMenu.Placement = PlacementMode.MousePoint;
    this.DropDownContextMenu.DataContext = this.DropDownContextMenuDataContext;
    this.DropDownContextMenu.IsOpen = true;
  }
}
