// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorableFloatingWindowControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Commands;
using Xceed.Wpf.AvalonDock.Converters;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorableFloatingWindowControl : LayoutFloatingWindowControl, IOverlayWindowHost
{
  private LayoutAnchorableFloatingWindow _model;
  private OverlayWindow _overlayWindow;
  private List<IDropArea> _dropAreas;
  public static readonly DependencyProperty SingleContentLayoutItemProperty = DependencyProperty.Register(nameof (SingleContentLayoutItem), typeof (LayoutItem), typeof (LayoutAnchorableFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutAnchorableFloatingWindowControl.OnSingleContentLayoutItemChanged)));

  static LayoutAnchorableFloatingWindowControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAnchorableFloatingWindowControl)));
  }

  internal LayoutAnchorableFloatingWindowControl(
    LayoutAnchorableFloatingWindow model,
    bool isContentImmutable)
    : base((ILayoutElement) model, isContentImmutable)
  {
    this._model = model;
    this.HideWindowCommand = (ICommand) new RelayCommand((Action<object>) (p => this.OnExecuteHideWindowCommand(p)), (Predicate<object>) (p => this.CanExecuteHideWindowCommand(p)));
    this.CloseWindowCommand = (ICommand) new RelayCommand((Action<object>) (p => this.OnExecuteCloseWindowCommand(p)), (Predicate<object>) (p => this.CanExecuteCloseWindowCommand(p)));
    this.UpdateThemeResources((Theme) null);
  }

  internal LayoutAnchorableFloatingWindowControl(LayoutAnchorableFloatingWindow model)
    : base((ILayoutElement) model, false)
  {
  }

  public LayoutItem SingleContentLayoutItem
  {
    get
    {
      return (LayoutItem) this.GetValue(LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty);
    }
    set
    {
      this.SetValue(LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty, (object) value);
    }
  }

  private static void OnSingleContentLayoutItemChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableFloatingWindowControl) d).OnSingleContentLayoutItemChanged(e);
  }

  protected virtual void OnSingleContentLayoutItemChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public override ILayoutElement Model => (ILayoutElement) this._model;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    this.Content = (object) this._model.Root.Manager.CreateUIElementForModel((ILayoutElement) this._model.RootPanel);
    this.IsVisibleChanged += (DependencyPropertyChangedEventHandler) ((s, args) =>
    {
      BindingExpression bindingExpression = this.GetBindingExpression(UIElement.VisibilityProperty);
      if (!this.IsVisible || bindingExpression != null)
        return;
      this.SetBinding(UIElement.VisibilityProperty, (BindingBase) new Binding("IsVisible")
      {
        Source = (object) this._model,
        Converter = (IValueConverter) new BoolToVisibilityConverter(),
        Mode = BindingMode.OneWay,
        ConverterParameter = (object) Visibility.Hidden
      });
    });
    this.SetBinding(LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty, (BindingBase) new Binding("Model.SinglePane.SelectedContent")
    {
      Source = (object) this,
      Converter = (IValueConverter) new LayoutItemFromLayoutModelConverter()
    });
    this._model.PropertyChanged += new PropertyChangedEventHandler(this._model_PropertyChanged);
  }

  protected override void OnClosed(EventArgs e)
  {
    ILayoutRoot root = this.Model.Root;
    if (root != null)
    {
      root.Manager.RemoveFloatingWindow((LayoutFloatingWindowControl) this);
      root.CollectGarbage();
    }
    if (this._overlayWindow != null)
    {
      this._overlayWindow.Close();
      this._overlayWindow = (OverlayWindow) null;
    }
    base.OnClosed(e);
    if (!this.CloseInitiatedByUser && root != null)
      root.FloatingWindows.Remove((LayoutFloatingWindow) this._model);
    this._model.PropertyChanged -= new PropertyChangedEventHandler(this._model_PropertyChanged);
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    if (this.CloseInitiatedByUser && !this.KeepContentVisibleOnClose)
    {
      e.Cancel = true;
      ((IEnumerable<LayoutAnchorable>) this._model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>()).ForEach<LayoutAnchorable>((Action<LayoutAnchorable>) (a => a.Hide()));
    }
    base.OnClosing(e);
  }

  protected override IntPtr FilterMessage(
    IntPtr hwnd,
    int msg,
    IntPtr wParam,
    IntPtr lParam,
    ref bool handled)
  {
    switch (msg)
    {
      case 161:
        if (wParam.ToInt32() == 2)
        {
          this._model.Descendents().OfType<LayoutAnchorablePane>().First<LayoutAnchorablePane>((Func<LayoutAnchorablePane, bool>) (p => p.ChildrenCount > 0 && p.SelectedContent != null)).SelectedContent.IsActive = true;
          handled = true;
          break;
        }
        break;
      case 165:
        if (wParam.ToInt32() == 2)
        {
          if (this.OpenContextMenu())
            handled = true;
          if (this._model.Root.Manager.ShowSystemMenu)
          {
            WindowChrome.GetWindowChrome((Window) this).ShowSystemMenu = !handled;
            break;
          }
          WindowChrome.GetWindowChrome((Window) this).ShowSystemMenu = false;
          break;
        }
        break;
    }
    return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
  }

  internal override void UpdateThemeResources(Theme oldTheme = null)
  {
    base.UpdateThemeResources(oldTheme);
    if (this._overlayWindow == null)
      return;
    this._overlayWindow.UpdateThemeResources(oldTheme);
  }

  private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!(e.PropertyName == "RootPanel") || this._model.RootPanel != null)
      return;
    this.InternalClose();
  }

  private void CreateOverlayWindow()
  {
    if (this._overlayWindow == null)
      this._overlayWindow = new OverlayWindow((IOverlayWindowHost) this);
    Rect rect = new Rect(this.PointToScreenDPIWithoutFlowDirection(new Point()), this.TransformActualSizeToAncestor());
    this._overlayWindow.Left = rect.Left;
    this._overlayWindow.Top = rect.Top;
    this._overlayWindow.Width = rect.Width;
    this._overlayWindow.Height = rect.Height;
  }

  private bool OpenContextMenu()
  {
    ContextMenu anchorableContextMenu = this._model.Root.Manager.AnchorableContextMenu;
    if (anchorableContextMenu == null || this.SingleContentLayoutItem == null)
      return false;
    anchorableContextMenu.PlacementTarget = (UIElement) null;
    anchorableContextMenu.Placement = PlacementMode.MousePoint;
    anchorableContextMenu.DataContext = (object) this.SingleContentLayoutItem;
    anchorableContextMenu.IsOpen = true;
    return true;
  }

  private bool IsContextMenuOpen()
  {
    ContextMenu anchorableContextMenu = this._model.Root.Manager.AnchorableContextMenu;
    return anchorableContextMenu != null && this.SingleContentLayoutItem != null && anchorableContextMenu.IsOpen;
  }

  public ICommand HideWindowCommand { get; private set; }

  private bool CanExecuteHideWindowCommand(object parameter)
  {
    if (this.Model == null)
      return false;
    ILayoutRoot root = this.Model.Root;
    if (root == null)
      return false;
    DockingManager manager = root.Manager;
    if (manager == null)
      return false;
    bool flag = false;
    foreach (LayoutAnchorable content in this.Model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
    {
      if (!content.CanHide)
      {
        flag = false;
        break;
      }
      if (!(manager.GetLayoutItemFromModel((LayoutContent) content) is LayoutAnchorableItem layoutItemFromModel) || layoutItemFromModel.HideCommand == null || !layoutItemFromModel.HideCommand.CanExecute(parameter))
      {
        flag = false;
        break;
      }
      flag = true;
    }
    return flag;
  }

  private void OnExecuteHideWindowCommand(object parameter)
  {
    DockingManager manager = this.Model.Root.Manager;
    foreach (LayoutAnchorable content in this.Model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
      (manager.GetLayoutItemFromModel((LayoutContent) content) as LayoutAnchorableItem).HideCommand.Execute(parameter);
  }

  public ICommand CloseWindowCommand { get; private set; }

  private bool CanExecuteCloseWindowCommand(object parameter)
  {
    if (this.Model == null)
      return false;
    ILayoutRoot root = this.Model.Root;
    if (root == null)
      return false;
    DockingManager manager = root.Manager;
    if (manager == null)
      return false;
    bool flag = false;
    foreach (LayoutAnchorable content in this.Model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
    {
      if (!content.CanClose)
      {
        flag = false;
        break;
      }
      if (!(manager.GetLayoutItemFromModel((LayoutContent) content) is LayoutAnchorableItem layoutItemFromModel) || layoutItemFromModel.CloseCommand == null || !layoutItemFromModel.CloseCommand.CanExecute(parameter))
      {
        flag = false;
        break;
      }
      flag = true;
    }
    return flag;
  }

  private void OnExecuteCloseWindowCommand(object parameter)
  {
    DockingManager manager = this.Model.Root.Manager;
    foreach (LayoutAnchorable content in this.Model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
      (manager.GetLayoutItemFromModel((LayoutContent) content) as LayoutAnchorableItem).CloseCommand.Execute(parameter);
  }

  bool IOverlayWindowHost.HitTest(Point dragPoint)
  {
    return new Rect(this.PointToScreenDPIWithoutFlowDirection(new Point()), this.TransformActualSizeToAncestor()).Contains(dragPoint);
  }

  DockingManager IOverlayWindowHost.Manager => this._model.Root.Manager;

  IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
  {
    this.CreateOverlayWindow();
    this._overlayWindow.Owner = (Window) draggingWindow;
    this._overlayWindow.EnableDropTargets();
    this._overlayWindow.Show();
    return (IOverlayWindow) this._overlayWindow;
  }

  void IOverlayWindowHost.HideOverlayWindow()
  {
    this._dropAreas = (List<IDropArea>) null;
    this._overlayWindow.Owner = (Window) null;
    this._overlayWindow.HideDropTargets();
  }

  IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
  {
    if (this._dropAreas != null)
      return (IEnumerable<IDropArea>) this._dropAreas;
    this._dropAreas = new List<IDropArea>();
    if (draggingWindow.Model is LayoutDocumentFloatingWindow)
      return (IEnumerable<IDropArea>) this._dropAreas;
    Visual rootVisual = (this.Content as LayoutFloatingWindowControl.FloatingWindowContentHost).RootVisual;
    foreach (LayoutAnchorablePaneControl visualChild in rootVisual.FindVisualChildren<LayoutAnchorablePaneControl>())
      this._dropAreas.Add((IDropArea) new DropArea<LayoutAnchorablePaneControl>(visualChild, DropAreaType.AnchorablePane));
    foreach (LayoutDocumentPaneControl visualChild in rootVisual.FindVisualChildren<LayoutDocumentPaneControl>())
      this._dropAreas.Add((IDropArea) new DropArea<LayoutDocumentPaneControl>(visualChild, DropAreaType.DocumentPane));
    return (IEnumerable<IDropArea>) this._dropAreas;
  }
}
