// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutDocumentFloatingWindowControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Microsoft.Windows.Shell;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutDocumentFloatingWindowControl : LayoutFloatingWindowControl
{
  private LayoutDocumentFloatingWindow _model;

  static LayoutDocumentFloatingWindowControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutDocumentFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutDocumentFloatingWindowControl)));
  }

  internal LayoutDocumentFloatingWindowControl(
    LayoutDocumentFloatingWindow model,
    bool isContentImmutable)
    : base((ILayoutElement) model, isContentImmutable)
  {
    this._model = model;
    this.UpdateThemeResources();
  }

  internal LayoutDocumentFloatingWindowControl(LayoutDocumentFloatingWindow model)
    : base((ILayoutElement) model, false)
  {
  }

  public LayoutItem RootDocumentLayoutItem
  {
    get
    {
      return this._model.Root.Manager.GetLayoutItemFromModel((LayoutContent) this._model.RootDocument);
    }
  }

  public override ILayoutElement Model => (ILayoutElement) this._model;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    if (this._model.RootDocument == null)
    {
      this.InternalClose();
    }
    else
    {
      this.Content = (object) this._model.Root.Manager.CreateUIElementForModel((ILayoutElement) this._model.RootDocument);
      this._model.RootDocumentChanged += new EventHandler(this._model_RootDocumentChanged);
    }
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
        if (wParam.ToInt32() == 2 && this._model.RootDocument != null)
        {
          this._model.RootDocument.IsActive = true;
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

  protected override void OnClosed(EventArgs e)
  {
    ILayoutRoot root = this.Model.Root;
    root.Manager.RemoveFloatingWindow((LayoutFloatingWindowControl) this);
    root.CollectGarbage();
    base.OnClosed(e);
    if (!this.CloseInitiatedByUser)
      root.FloatingWindows.Remove((LayoutFloatingWindow) this._model);
    this._model.RootDocumentChanged -= new EventHandler(this._model_RootDocumentChanged);
  }

  private void _model_RootDocumentChanged(object sender, EventArgs e)
  {
    if (this._model.RootDocument != null)
      return;
    this.InternalClose();
  }

  private bool OpenContextMenu()
  {
    ContextMenu documentContextMenu = this._model.Root.Manager.DocumentContextMenu;
    if (documentContextMenu == null || this.RootDocumentLayoutItem == null)
      return false;
    documentContextMenu.PlacementTarget = (UIElement) null;
    documentContextMenu.Placement = PlacementMode.MousePoint;
    documentContextMenu.DataContext = (object) this.RootDocumentLayoutItem;
    documentContextMenu.IsOpen = true;
    return true;
  }
}
