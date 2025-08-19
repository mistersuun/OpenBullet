// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutDocumentPaneControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutDocumentPaneControl : TabControl, ILayoutControl
{
  private List<object> _logicalChildren = new List<object>();
  private LayoutDocumentPane _model;

  static LayoutDocumentPaneControl()
  {
    UIElement.FocusableProperty.OverrideMetadata(typeof (LayoutDocumentPaneControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  }

  internal LayoutDocumentPaneControl(LayoutDocumentPane model)
  {
    this._model = model != null ? model : throw new ArgumentNullException(nameof (model));
    this.SetBinding(ItemsControl.ItemsSourceProperty, (BindingBase) new Binding("Model.Children")
    {
      Source = (object) this
    });
    this.SetBinding(FrameworkElement.FlowDirectionProperty, (BindingBase) new Binding("Model.Root.Manager.FlowDirection")
    {
      Source = (object) this
    });
    this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
  }

  public ILayoutElement Model => (ILayoutElement) this._model;

  protected override IEnumerator LogicalChildren
  {
    get => (IEnumerator) this._logicalChildren.GetEnumerator();
  }

  protected override void OnSelectionChanged(SelectionChangedEventArgs e)
  {
    base.OnSelectionChanged(e);
    if (this._model.SelectedContent == null)
      return;
    this._model.SelectedContent.IsActive = true;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonDown(e);
    if (e.Handled || this._model.SelectedContent == null)
      return;
    this._model.SelectedContent.IsActive = true;
  }

  protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseRightButtonDown(e);
    if (e.Handled || this._model.SelectedContent == null)
      return;
    this._model.SelectedContent.IsActive = true;
  }

  private void OnLayoutUpdated(object sender, EventArgs e)
  {
    LayoutDocumentPane model = this._model;
    ((ILayoutPositionableElementWithActualSize) model).ActualWidth = this.ActualWidth;
    ((ILayoutPositionableElementWithActualSize) model).ActualHeight = this.ActualHeight;
  }
}
