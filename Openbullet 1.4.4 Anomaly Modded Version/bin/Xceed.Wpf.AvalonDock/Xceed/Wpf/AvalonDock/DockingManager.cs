// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.DockingManager
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

#nullable disable
namespace Xceed.Wpf.AvalonDock;

[ContentProperty("Layout")]
[TemplatePart(Name = "PART_AutoHideArea")]
public class DockingManager : Control, IOverlayWindowHost
{
  private ResourceDictionary currentThemeResourceDictionary;
  private AutoHideWindowManager _autoHideWindowManager;
  private FrameworkElement _autohideArea;
  private List<LayoutFloatingWindowControl> _fwList = new List<LayoutFloatingWindowControl>();
  private OverlayWindow _overlayWindow;
  private List<IDropArea> _areas;
  private bool _insideInternalSetActiveContent;
  private List<LayoutItem> _layoutItems = new List<LayoutItem>();
  private bool _suspendLayoutItemCreation;
  private DispatcherOperation _collectLayoutItemsOperations;
  private NavigatorWindow _navigatorWindow;
  internal bool SuspendDocumentsSourceBinding;
  internal bool SuspendAnchorablesSourceBinding;
  public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(nameof (Layout), typeof (LayoutRoot), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLayoutChanged), new CoerceValueCallback(DockingManager.CoerceLayoutValue)));
  public static readonly DependencyProperty LayoutUpdateStrategyProperty = DependencyProperty.Register(nameof (LayoutUpdateStrategy), typeof (ILayoutUpdateStrategy), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty DocumentPaneTemplateProperty = DependencyProperty.Register(nameof (DocumentPaneTemplate), typeof (ControlTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentPaneTemplateChanged)));
  public static readonly DependencyProperty AnchorablePaneTemplateProperty = DependencyProperty.Register(nameof (AnchorablePaneTemplate), typeof (ControlTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorablePaneTemplateChanged)));
  public static readonly DependencyProperty AnchorSideTemplateProperty = DependencyProperty.Register(nameof (AnchorSideTemplate), typeof (ControlTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty AnchorGroupTemplateProperty = DependencyProperty.Register(nameof (AnchorGroupTemplate), typeof (ControlTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty AnchorTemplateProperty = DependencyProperty.Register(nameof (AnchorTemplate), typeof (ControlTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty DocumentPaneControlStyleProperty = DependencyProperty.Register(nameof (DocumentPaneControlStyle), typeof (Style), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentPaneControlStyleChanged)));
  public static readonly DependencyProperty AnchorablePaneControlStyleProperty = DependencyProperty.Register(nameof (AnchorablePaneControlStyle), typeof (Style), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorablePaneControlStyleChanged)));
  public static readonly DependencyProperty DocumentHeaderTemplateProperty = DependencyProperty.Register(nameof (DocumentHeaderTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentHeaderTemplateChanged), new CoerceValueCallback(DockingManager.CoerceDocumentHeaderTemplateValue)));
  public static readonly DependencyProperty DocumentHeaderTemplateSelectorProperty = DependencyProperty.Register(nameof (DocumentHeaderTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentHeaderTemplateSelectorChanged), new CoerceValueCallback(DockingManager.CoerceDocumentHeaderTemplateSelectorValue)));
  public static readonly DependencyProperty DocumentTitleTemplateProperty = DependencyProperty.Register(nameof (DocumentTitleTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentTitleTemplateChanged), new CoerceValueCallback(DockingManager.CoerceDocumentTitleTemplateValue)));
  public static readonly DependencyProperty DocumentTitleTemplateSelectorProperty = DependencyProperty.Register(nameof (DocumentTitleTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentTitleTemplateSelectorChanged), new CoerceValueCallback(DockingManager.CoerceDocumentTitleTemplateSelectorValue)));
  public static readonly DependencyProperty AnchorableTitleTemplateProperty = DependencyProperty.Register(nameof (AnchorableTitleTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorableTitleTemplateChanged), new CoerceValueCallback(DockingManager.CoerceAnchorableTitleTemplateValue)));
  public static readonly DependencyProperty AnchorableTitleTemplateSelectorProperty = DependencyProperty.Register(nameof (AnchorableTitleTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorableTitleTemplateSelectorChanged)));
  public static readonly DependencyProperty AnchorableHeaderTemplateProperty = DependencyProperty.Register(nameof (AnchorableHeaderTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorableHeaderTemplateChanged), new CoerceValueCallback(DockingManager.CoerceAnchorableHeaderTemplateValue)));
  public static readonly DependencyProperty AnchorableHeaderTemplateSelectorProperty = DependencyProperty.Register(nameof (AnchorableHeaderTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorableHeaderTemplateSelectorChanged)));
  public static readonly DependencyProperty LayoutRootPanelProperty = DependencyProperty.Register(nameof (LayoutRootPanel), typeof (LayoutPanelControl), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLayoutRootPanelChanged)));
  public static readonly DependencyProperty RightSidePanelProperty = DependencyProperty.Register(nameof (RightSidePanel), typeof (LayoutAnchorSideControl), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnRightSidePanelChanged)));
  public static readonly DependencyProperty LeftSidePanelProperty = DependencyProperty.Register(nameof (LeftSidePanel), typeof (LayoutAnchorSideControl), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLeftSidePanelChanged)));
  public static readonly DependencyProperty TopSidePanelProperty = DependencyProperty.Register(nameof (TopSidePanel), typeof (LayoutAnchorSideControl), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnTopSidePanelChanged)));
  public static readonly DependencyProperty BottomSidePanelProperty = DependencyProperty.Register(nameof (BottomSidePanel), typeof (LayoutAnchorSideControl), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnBottomSidePanelChanged)));
  private List<WeakReference> _logicalChildren = new List<WeakReference>();
  private static readonly DependencyPropertyKey AutoHideWindowPropertyKey = DependencyProperty.RegisterReadOnly(nameof (AutoHideWindow), typeof (LayoutAutoHideWindowControl), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAutoHideWindowChanged)));
  public static readonly DependencyProperty AutoHideWindowProperty = DockingManager.AutoHideWindowPropertyKey.DependencyProperty;
  public static readonly DependencyProperty LayoutItemTemplateProperty = DependencyProperty.Register(nameof (LayoutItemTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLayoutItemTemplateChanged)));
  public static readonly DependencyProperty LayoutItemTemplateSelectorProperty = DependencyProperty.Register(nameof (LayoutItemTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLayoutItemTemplateSelectorChanged)));
  public static readonly DependencyProperty DocumentsSourceProperty = DependencyProperty.Register(nameof (DocumentsSource), typeof (IEnumerable), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentsSourceChanged)));
  public static readonly DependencyProperty DocumentContextMenuProperty = DependencyProperty.Register(nameof (DocumentContextMenu), typeof (ContextMenu), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty AnchorablesSourceProperty = DependencyProperty.Register(nameof (AnchorablesSource), typeof (IEnumerable), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnAnchorablesSourceChanged)));
  public static readonly DependencyProperty ActiveContentProperty = DependencyProperty.Register(nameof (ActiveContent), typeof (object), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnActiveContentChanged)));
  public static readonly DependencyProperty AnchorableContextMenuProperty = DependencyProperty.Register(nameof (AnchorableContextMenu), typeof (ContextMenu), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(nameof (Theme), typeof (Theme), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnThemeChanged)));
  public static readonly DependencyProperty GridSplitterWidthProperty = DependencyProperty.Register(nameof (GridSplitterWidth), typeof (double), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) 6.0));
  public static readonly DependencyProperty GridSplitterHeightProperty = DependencyProperty.Register(nameof (GridSplitterHeight), typeof (double), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) 6.0));
  public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateProperty = DependencyProperty.Register(nameof (DocumentPaneMenuItemHeaderTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentPaneMenuItemHeaderTemplateChanged), new CoerceValueCallback(DockingManager.CoerceDocumentPaneMenuItemHeaderTemplateValue)));
  public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateSelectorProperty = DependencyProperty.Register(nameof (DocumentPaneMenuItemHeaderTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnDocumentPaneMenuItemHeaderTemplateSelectorChanged), new CoerceValueCallback(DockingManager.CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue)));
  public static readonly DependencyProperty IconContentTemplateProperty = DependencyProperty.Register(nameof (IconContentTemplate), typeof (DataTemplate), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty IconContentTemplateSelectorProperty = DependencyProperty.Register(nameof (IconContentTemplateSelector), typeof (DataTemplateSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty LayoutItemContainerStyleProperty = DependencyProperty.Register(nameof (LayoutItemContainerStyle), typeof (Style), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLayoutItemContainerStyleChanged)));
  public static readonly DependencyProperty LayoutItemContainerStyleSelectorProperty = DependencyProperty.Register(nameof (LayoutItemContainerStyleSelector), typeof (StyleSelector), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DockingManager.OnLayoutItemContainerStyleSelectorChanged)));
  public static readonly DependencyProperty ShowSystemMenuProperty = DependencyProperty.Register(nameof (ShowSystemMenu), typeof (bool), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  public static readonly DependencyProperty AllowMixedOrientationProperty = DependencyProperty.Register(nameof (AllowMixedOrientation), typeof (bool), typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));

  static DockingManager()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DockingManager)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (DockingManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
  }

  public DockingManager()
  {
    this.Layout = new LayoutRoot()
    {
      RootPanel = new LayoutPanel((ILayoutPanelElement) new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
    };
    this.Loaded += new RoutedEventHandler(this.DockingManager_Loaded);
    this.Unloaded += new RoutedEventHandler(this.DockingManager_Unloaded);
  }

  public LayoutRoot Layout
  {
    get => (LayoutRoot) this.GetValue(DockingManager.LayoutProperty);
    set => this.SetValue(DockingManager.LayoutProperty, (object) value);
  }

  private static object CoerceLayoutValue(DependencyObject d, object value)
  {
    if (value == null)
      return (object) new LayoutRoot()
      {
        RootPanel = new LayoutPanel((ILayoutPanelElement) new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
      };
    ((DockingManager) d).OnLayoutChanging(value as LayoutRoot);
    return value;
  }

  private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
  }

  protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
  {
    if (oldLayout != null)
    {
      oldLayout.PropertyChanged -= new PropertyChangedEventHandler(this.OnLayoutRootPropertyChanged);
      oldLayout.Updated -= new EventHandler(this.OnLayoutRootUpdated);
    }
    foreach (LayoutFloatingWindowControl floatingWindowControl in this._fwList.ToArray())
    {
      floatingWindowControl.KeepContentVisibleOnClose = true;
      floatingWindowControl.InternalClose();
    }
    this._fwList.Clear();
    this.DetachDocumentsSource(oldLayout, this.DocumentsSource);
    this.DetachAnchorablesSource(oldLayout, this.AnchorablesSource);
    if (oldLayout != null && oldLayout.Manager == this)
      oldLayout.Manager = (DockingManager) null;
    this.ClearLogicalChildrenList();
    this.DetachLayoutItems();
    this.Layout.Manager = this;
    this.AttachLayoutItems();
    this.AttachDocumentsSource(newLayout, this.DocumentsSource);
    this.AttachAnchorablesSource(newLayout, this.AnchorablesSource);
    if (this.IsLoaded)
    {
      this.LayoutRootPanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.RootPanel) as LayoutPanelControl;
      this.LeftSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.LeftSide) as LayoutAnchorSideControl;
      this.TopSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.TopSide) as LayoutAnchorSideControl;
      this.RightSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.RightSide) as LayoutAnchorSideControl;
      this.BottomSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.BottomSide) as LayoutAnchorSideControl;
      foreach (LayoutFloatingWindow model in this.Layout.FloatingWindows.ToArray<LayoutFloatingWindow>())
      {
        if (model.IsValid)
          this._fwList.Add(this.CreateUIElementForModel((ILayoutElement) model) as LayoutFloatingWindowControl);
      }
      foreach (LayoutFloatingWindowControl fw in this._fwList)
        ;
    }
    if (newLayout != null)
    {
      newLayout.PropertyChanged += new PropertyChangedEventHandler(this.OnLayoutRootPropertyChanged);
      newLayout.Updated += new EventHandler(this.OnLayoutRootUpdated);
    }
    if (this.LayoutChanged != null)
      this.LayoutChanged((object) this, EventArgs.Empty);
    CommandManager.InvalidateRequerySuggested();
  }

  public ILayoutUpdateStrategy LayoutUpdateStrategy
  {
    get => (ILayoutUpdateStrategy) this.GetValue(DockingManager.LayoutUpdateStrategyProperty);
    set => this.SetValue(DockingManager.LayoutUpdateStrategyProperty, (object) value);
  }

  public ControlTemplate DocumentPaneTemplate
  {
    get => (ControlTemplate) this.GetValue(DockingManager.DocumentPaneTemplateProperty);
    set => this.SetValue(DockingManager.DocumentPaneTemplateProperty, (object) value);
  }

  private static void OnDocumentPaneTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentPaneTemplateChanged(e);
  }

  protected virtual void OnDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public ControlTemplate AnchorablePaneTemplate
  {
    get => (ControlTemplate) this.GetValue(DockingManager.AnchorablePaneTemplateProperty);
    set => this.SetValue(DockingManager.AnchorablePaneTemplateProperty, (object) value);
  }

  private static void OnAnchorablePaneTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorablePaneTemplateChanged(e);
  }

  protected virtual void OnAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public ControlTemplate AnchorSideTemplate
  {
    get => (ControlTemplate) this.GetValue(DockingManager.AnchorSideTemplateProperty);
    set => this.SetValue(DockingManager.AnchorSideTemplateProperty, (object) value);
  }

  public ControlTemplate AnchorGroupTemplate
  {
    get => (ControlTemplate) this.GetValue(DockingManager.AnchorGroupTemplateProperty);
    set => this.SetValue(DockingManager.AnchorGroupTemplateProperty, (object) value);
  }

  public ControlTemplate AnchorTemplate
  {
    get => (ControlTemplate) this.GetValue(DockingManager.AnchorTemplateProperty);
    set => this.SetValue(DockingManager.AnchorTemplateProperty, (object) value);
  }

  public Style DocumentPaneControlStyle
  {
    get => (Style) this.GetValue(DockingManager.DocumentPaneControlStyleProperty);
    set => this.SetValue(DockingManager.DocumentPaneControlStyleProperty, (object) value);
  }

  private static void OnDocumentPaneControlStyleChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentPaneControlStyleChanged(e);
  }

  protected virtual void OnDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public Style AnchorablePaneControlStyle
  {
    get => (Style) this.GetValue(DockingManager.AnchorablePaneControlStyleProperty);
    set => this.SetValue(DockingManager.AnchorablePaneControlStyleProperty, (object) value);
  }

  private static void OnAnchorablePaneControlStyleChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorablePaneControlStyleChanged(e);
  }

  protected virtual void OnAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public DataTemplate DocumentHeaderTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.DocumentHeaderTemplateProperty);
    set => this.SetValue(DockingManager.DocumentHeaderTemplateProperty, (object) value);
  }

  private static void OnDocumentHeaderTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentHeaderTemplateChanged(e);
  }

  protected virtual void OnDocumentHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceDocumentHeaderTemplateValue(DependencyObject d, object value)
  {
    return value != null && d.GetValue(DockingManager.DocumentHeaderTemplateSelectorProperty) != null ? (object) null : value;
  }

  public DataTemplateSelector DocumentHeaderTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DockingManager.DocumentHeaderTemplateSelectorProperty);
    }
    set => this.SetValue(DockingManager.DocumentHeaderTemplateSelectorProperty, (object) value);
  }

  private static void OnDocumentHeaderTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentHeaderTemplateSelectorChanged(e);
  }

  protected virtual void OnDocumentHeaderTemplateSelectorChanged(
    DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue != null && this.DocumentHeaderTemplate != null)
      this.DocumentHeaderTemplate = (DataTemplate) null;
    if (this.DocumentPaneMenuItemHeaderTemplateSelector != null)
      return;
    this.DocumentPaneMenuItemHeaderTemplateSelector = this.DocumentHeaderTemplateSelector;
  }

  private static object CoerceDocumentHeaderTemplateSelectorValue(DependencyObject d, object value)
  {
    return value;
  }

  public DataTemplate DocumentTitleTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.DocumentTitleTemplateProperty);
    set => this.SetValue(DockingManager.DocumentTitleTemplateProperty, (object) value);
  }

  private static void OnDocumentTitleTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentTitleTemplateChanged(e);
  }

  protected virtual void OnDocumentTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceDocumentTitleTemplateValue(DependencyObject d, object value)
  {
    return value != null && d.GetValue(DockingManager.DocumentTitleTemplateSelectorProperty) != null ? (object) null : value;
  }

  public DataTemplateSelector DocumentTitleTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DockingManager.DocumentTitleTemplateSelectorProperty);
    }
    set => this.SetValue(DockingManager.DocumentTitleTemplateSelectorProperty, (object) value);
  }

  private static void OnDocumentTitleTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentTitleTemplateSelectorChanged(e);
  }

  protected virtual void OnDocumentTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue == null)
      return;
    this.DocumentTitleTemplate = (DataTemplate) null;
  }

  private static object CoerceDocumentTitleTemplateSelectorValue(DependencyObject d, object value)
  {
    return value;
  }

  public DataTemplate AnchorableTitleTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.AnchorableTitleTemplateProperty);
    set => this.SetValue(DockingManager.AnchorableTitleTemplateProperty, (object) value);
  }

  private static void OnAnchorableTitleTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorableTitleTemplateChanged(e);
  }

  protected virtual void OnAnchorableTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceAnchorableTitleTemplateValue(DependencyObject d, object value)
  {
    return value != null && d.GetValue(DockingManager.AnchorableTitleTemplateSelectorProperty) != null ? (object) null : value;
  }

  public DataTemplateSelector AnchorableTitleTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DockingManager.AnchorableTitleTemplateSelectorProperty);
    }
    set => this.SetValue(DockingManager.AnchorableTitleTemplateSelectorProperty, (object) value);
  }

  private static void OnAnchorableTitleTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorableTitleTemplateSelectorChanged(e);
  }

  protected virtual void OnAnchorableTitleTemplateSelectorChanged(
    DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue == null || this.AnchorableTitleTemplate == null)
      return;
    this.AnchorableTitleTemplate = (DataTemplate) null;
  }

  public DataTemplate AnchorableHeaderTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.AnchorableHeaderTemplateProperty);
    set => this.SetValue(DockingManager.AnchorableHeaderTemplateProperty, (object) value);
  }

  private static void OnAnchorableHeaderTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorableHeaderTemplateChanged(e);
  }

  protected virtual void OnAnchorableHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceAnchorableHeaderTemplateValue(DependencyObject d, object value)
  {
    return value != null && d.GetValue(DockingManager.AnchorableHeaderTemplateSelectorProperty) != null ? (object) null : value;
  }

  public DataTemplateSelector AnchorableHeaderTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DockingManager.AnchorableHeaderTemplateSelectorProperty);
    }
    set => this.SetValue(DockingManager.AnchorableHeaderTemplateSelectorProperty, (object) value);
  }

  private static void OnAnchorableHeaderTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorableHeaderTemplateSelectorChanged(e);
  }

  protected virtual void OnAnchorableHeaderTemplateSelectorChanged(
    DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue == null)
      return;
    this.AnchorableHeaderTemplate = (DataTemplate) null;
  }

  public LayoutPanelControl LayoutRootPanel
  {
    get => (LayoutPanelControl) this.GetValue(DockingManager.LayoutRootPanelProperty);
    set => this.SetValue(DockingManager.LayoutRootPanelProperty, (object) value);
  }

  private static void OnLayoutRootPanelChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLayoutRootPanelChanged(e);
  }

  protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      this.InternalRemoveLogicalChild(e.OldValue);
    if (e.NewValue == null)
      return;
    this.InternalAddLogicalChild(e.NewValue);
  }

  public LayoutAnchorSideControl RightSidePanel
  {
    get => (LayoutAnchorSideControl) this.GetValue(DockingManager.RightSidePanelProperty);
    set => this.SetValue(DockingManager.RightSidePanelProperty, (object) value);
  }

  private static void OnRightSidePanelChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnRightSidePanelChanged(e);
  }

  protected virtual void OnRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      this.InternalRemoveLogicalChild(e.OldValue);
    if (e.NewValue == null)
      return;
    this.InternalAddLogicalChild(e.NewValue);
  }

  public LayoutAnchorSideControl LeftSidePanel
  {
    get => (LayoutAnchorSideControl) this.GetValue(DockingManager.LeftSidePanelProperty);
    set => this.SetValue(DockingManager.LeftSidePanelProperty, (object) value);
  }

  private static void OnLeftSidePanelChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLeftSidePanelChanged(e);
  }

  protected virtual void OnLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      this.InternalRemoveLogicalChild(e.OldValue);
    if (e.NewValue == null)
      return;
    this.InternalAddLogicalChild(e.NewValue);
  }

  public LayoutAnchorSideControl TopSidePanel
  {
    get => (LayoutAnchorSideControl) this.GetValue(DockingManager.TopSidePanelProperty);
    set => this.SetValue(DockingManager.TopSidePanelProperty, (object) value);
  }

  private static void OnTopSidePanelChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnTopSidePanelChanged(e);
  }

  protected virtual void OnTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      this.InternalRemoveLogicalChild(e.OldValue);
    if (e.NewValue == null)
      return;
    this.InternalAddLogicalChild(e.NewValue);
  }

  public LayoutAnchorSideControl BottomSidePanel
  {
    get => (LayoutAnchorSideControl) this.GetValue(DockingManager.BottomSidePanelProperty);
    set => this.SetValue(DockingManager.BottomSidePanelProperty, (object) value);
  }

  private static void OnBottomSidePanelChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnBottomSidePanelChanged(e);
  }

  protected virtual void OnBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      this.InternalRemoveLogicalChild(e.OldValue);
    if (e.NewValue == null)
      return;
    this.InternalAddLogicalChild(e.NewValue);
  }

  protected override IEnumerator LogicalChildren
  {
    get
    {
      return (IEnumerator) this._logicalChildren.Select<WeakReference, object>((Func<WeakReference, object>) (ch => ch.GetValueOrDefault<object>())).GetEnumerator();
    }
  }

  public IEnumerator LogicalChildrenPublic => this.LogicalChildren;

  internal void InternalAddLogicalChild(object element)
  {
    if (this._logicalChildren.Select<WeakReference, object>((Func<WeakReference, object>) (ch => ch.GetValueOrDefault<object>())).Contains(element))
      return;
    this._logicalChildren.Add(new WeakReference(element));
    this.AddLogicalChild(element);
  }

  internal void InternalRemoveLogicalChild(object element)
  {
    WeakReference weakReference = this._logicalChildren.FirstOrDefault<WeakReference>((Func<WeakReference, bool>) (ch => ch.GetValueOrDefault<object>() == element));
    if (weakReference != null)
      this._logicalChildren.Remove(weakReference);
    this.RemoveLogicalChild(element);
  }

  private void ClearLogicalChildrenList()
  {
    foreach (object child in this._logicalChildren.Select<WeakReference, object>((Func<WeakReference, object>) (ch => ch.GetValueOrDefault<object>())).ToArray<object>())
      this.RemoveLogicalChild(child);
    this._logicalChildren.Clear();
  }

  public LayoutAutoHideWindowControl AutoHideWindow
  {
    get => (LayoutAutoHideWindowControl) this.GetValue(DockingManager.AutoHideWindowProperty);
  }

  protected void SetAutoHideWindow(LayoutAutoHideWindowControl value)
  {
    this.SetValue(DockingManager.AutoHideWindowPropertyKey, (object) value);
  }

  private static void OnAutoHideWindowChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAutoHideWindowChanged(e);
  }

  protected virtual void OnAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
      this.InternalRemoveLogicalChild(e.OldValue);
    if (e.NewValue == null)
      return;
    this.InternalAddLogicalChild(e.NewValue);
  }

  public IEnumerable<LayoutFloatingWindowControl> FloatingWindows
  {
    get => (IEnumerable<LayoutFloatingWindowControl>) this._fwList;
  }

  public DataTemplate LayoutItemTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.LayoutItemTemplateProperty);
    set => this.SetValue(DockingManager.LayoutItemTemplateProperty, (object) value);
  }

  private static void OnLayoutItemTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLayoutItemTemplateChanged(e);
  }

  protected virtual void OnLayoutItemTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public DataTemplateSelector LayoutItemTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DockingManager.LayoutItemTemplateSelectorProperty);
    set => this.SetValue(DockingManager.LayoutItemTemplateSelectorProperty, (object) value);
  }

  private static void OnLayoutItemTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLayoutItemTemplateSelectorChanged(e);
  }

  protected virtual void OnLayoutItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  public IEnumerable DocumentsSource
  {
    get => (IEnumerable) this.GetValue(DockingManager.DocumentsSourceProperty);
    set => this.SetValue(DockingManager.DocumentsSourceProperty, (object) value);
  }

  private static void OnDocumentsSourceChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentsSourceChanged(e);
  }

  protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
  {
    this.DetachDocumentsSource(this.Layout, e.OldValue as IEnumerable);
    this.AttachDocumentsSource(this.Layout, e.NewValue as IEnumerable);
  }

  public ContextMenu DocumentContextMenu
  {
    get => (ContextMenu) this.GetValue(DockingManager.DocumentContextMenuProperty);
    set => this.SetValue(DockingManager.DocumentContextMenuProperty, (object) value);
  }

  public IEnumerable AnchorablesSource
  {
    get => (IEnumerable) this.GetValue(DockingManager.AnchorablesSourceProperty);
    set => this.SetValue(DockingManager.AnchorablesSourceProperty, (object) value);
  }

  private static void OnAnchorablesSourceChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnAnchorablesSourceChanged(e);
  }

  protected virtual void OnAnchorablesSourceChanged(DependencyPropertyChangedEventArgs e)
  {
    this.DetachAnchorablesSource(this.Layout, e.OldValue as IEnumerable);
    this.AttachAnchorablesSource(this.Layout, e.NewValue as IEnumerable);
  }

  public object ActiveContent
  {
    get => this.GetValue(DockingManager.ActiveContentProperty);
    set => this.SetValue(DockingManager.ActiveContentProperty, value);
  }

  private static void OnActiveContentChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).InternalSetActiveContent(e.NewValue);
    ((DockingManager) d).OnActiveContentChanged(e);
  }

  protected virtual void OnActiveContentChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.ActiveContentChanged == null)
      return;
    this.ActiveContentChanged((object) this, EventArgs.Empty);
  }

  public ContextMenu AnchorableContextMenu
  {
    get => (ContextMenu) this.GetValue(DockingManager.AnchorableContextMenuProperty);
    set => this.SetValue(DockingManager.AnchorableContextMenuProperty, (object) value);
  }

  public Theme Theme
  {
    get => (Theme) this.GetValue(DockingManager.ThemeProperty);
    set => this.SetValue(DockingManager.ThemeProperty, (object) value);
  }

  private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnThemeChanged(e);
  }

  protected virtual void OnThemeChanged(DependencyPropertyChangedEventArgs e)
  {
    Theme oldTheme = e.OldValue as Theme;
    Theme newValue = e.NewValue as Theme;
    ResourceDictionary resources = this.Resources;
    if (oldTheme != null)
    {
      if (oldTheme is DictionaryTheme)
      {
        if (this.currentThemeResourceDictionary != null)
        {
          resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
          this.currentThemeResourceDictionary = (ResourceDictionary) null;
        }
      }
      else
      {
        ResourceDictionary resourceDictionary = resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((Func<ResourceDictionary, bool>) (r => r.Source == oldTheme.GetResourceUri()));
        if (resourceDictionary != null)
          resources.MergedDictionaries.Remove(resourceDictionary);
      }
    }
    if (newValue != null)
    {
      if (newValue is DictionaryTheme)
      {
        this.currentThemeResourceDictionary = ((DictionaryTheme) newValue).ThemeResourceDictionary;
        resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
      }
      else
        resources.MergedDictionaries.Add(new ResourceDictionary()
        {
          Source = newValue.GetResourceUri()
        });
    }
    foreach (LayoutFloatingWindowControl fw in this._fwList)
      fw.UpdateThemeResources(oldTheme);
    if (this._navigatorWindow != null)
      this._navigatorWindow.UpdateThemeResources();
    if (this._overlayWindow == null)
      return;
    this._overlayWindow.UpdateThemeResources();
  }

  public double GridSplitterWidth
  {
    get => (double) this.GetValue(DockingManager.GridSplitterWidthProperty);
    set => this.SetValue(DockingManager.GridSplitterWidthProperty, (object) value);
  }

  public double GridSplitterHeight
  {
    get => (double) this.GetValue(DockingManager.GridSplitterHeightProperty);
    set => this.SetValue(DockingManager.GridSplitterHeightProperty, (object) value);
  }

  public DataTemplate DocumentPaneMenuItemHeaderTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateProperty);
    set => this.SetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateProperty, (object) value);
  }

  private static void OnDocumentPaneMenuItemHeaderTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentPaneMenuItemHeaderTemplateChanged(e);
  }

  protected virtual void OnDocumentPaneMenuItemHeaderTemplateChanged(
    DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceDocumentPaneMenuItemHeaderTemplateValue(
    DependencyObject d,
    object value)
  {
    if (value != null && d.GetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty) != null)
      return (object) null;
    return value == null ? d.GetValue(DockingManager.DocumentHeaderTemplateProperty) : value;
  }

  public DataTemplateSelector DocumentPaneMenuItemHeaderTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty);
    }
    set
    {
      this.SetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty, (object) value);
    }
  }

  private static void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(e);
  }

  protected virtual void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(
    DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue == null || this.DocumentPaneMenuItemHeaderTemplate == null)
      return;
    this.DocumentPaneMenuItemHeaderTemplate = (DataTemplate) null;
  }

  private static object CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue(
    DependencyObject d,
    object value)
  {
    return value;
  }

  public DataTemplate IconContentTemplate
  {
    get => (DataTemplate) this.GetValue(DockingManager.IconContentTemplateProperty);
    set => this.SetValue(DockingManager.IconContentTemplateProperty, (object) value);
  }

  public DataTemplateSelector IconContentTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DockingManager.IconContentTemplateSelectorProperty);
    set => this.SetValue(DockingManager.IconContentTemplateSelectorProperty, (object) value);
  }

  public Style LayoutItemContainerStyle
  {
    get => (Style) this.GetValue(DockingManager.LayoutItemContainerStyleProperty);
    set => this.SetValue(DockingManager.LayoutItemContainerStyleProperty, (object) value);
  }

  private static void OnLayoutItemContainerStyleChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLayoutItemContainerStyleChanged(e);
  }

  protected virtual void OnLayoutItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
  {
    this.AttachLayoutItems();
  }

  public StyleSelector LayoutItemContainerStyleSelector
  {
    get => (StyleSelector) this.GetValue(DockingManager.LayoutItemContainerStyleSelectorProperty);
    set => this.SetValue(DockingManager.LayoutItemContainerStyleSelectorProperty, (object) value);
  }

  private static void OnLayoutItemContainerStyleSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DockingManager) d).OnLayoutItemContainerStyleSelectorChanged(e);
  }

  protected virtual void OnLayoutItemContainerStyleSelectorChanged(
    DependencyPropertyChangedEventArgs e)
  {
    this.AttachLayoutItems();
  }

  public bool ShowSystemMenu
  {
    get => (bool) this.GetValue(DockingManager.ShowSystemMenuProperty);
    set => this.SetValue(DockingManager.ShowSystemMenuProperty, (object) value);
  }

  public bool AllowMixedOrientation
  {
    get => (bool) this.GetValue(DockingManager.AllowMixedOrientationProperty);
    set => this.SetValue(DockingManager.AllowMixedOrientationProperty, (object) value);
  }

  private bool IsNavigatorWindowActive => this._navigatorWindow != null;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._autohideArea = this.GetTemplateChild("PART_AutoHideArea") as FrameworkElement;
  }

  protected override void OnInitialized(EventArgs e) => base.OnInitialized(e);

  protected override Size ArrangeOverride(Size arrangeBounds)
  {
    this._areas = (List<IDropArea>) null;
    return base.ArrangeOverride(arrangeBounds);
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.IsDown && e.Key == Key.Tab && !this.IsNavigatorWindowActive)
    {
      this.ShowNavigatorWindow();
      e.Handled = true;
    }
    base.OnPreviewKeyDown(e);
  }

  public LayoutItem GetLayoutItemFromModel(LayoutContent content)
  {
    return this._layoutItems.FirstOrDefault<LayoutItem>((Func<LayoutItem, bool>) (item => item.LayoutElement == content));
  }

  public LayoutFloatingWindowControl CreateFloatingWindow(
    LayoutContent contentModel,
    bool isContentImmutable)
  {
    LayoutFloatingWindowControl floatingWindow = (LayoutFloatingWindowControl) null;
    if (contentModel is LayoutAnchorable && !(contentModel.Parent is ILayoutPane))
    {
      LayoutAnchorablePane paneModel = new LayoutAnchorablePane(contentModel as LayoutAnchorable);
      paneModel.FloatingTop = contentModel.FloatingTop;
      paneModel.FloatingLeft = contentModel.FloatingLeft;
      paneModel.FloatingWidth = contentModel.FloatingWidth;
      paneModel.FloatingHeight = contentModel.FloatingHeight;
      floatingWindow = this.CreateFloatingWindowForLayoutAnchorableWithoutParent(paneModel, isContentImmutable);
    }
    if (floatingWindow == null)
      floatingWindow = this.CreateFloatingWindowCore(contentModel, isContentImmutable);
    return floatingWindow;
  }

  internal UIElement CreateUIElementForModel(ILayoutElement model)
  {
    switch (model)
    {
      case LayoutPanel _:
        return (UIElement) new LayoutPanelControl(model as LayoutPanel);
      case LayoutAnchorablePaneGroup _:
        return (UIElement) new LayoutAnchorablePaneGroupControl(model as LayoutAnchorablePaneGroup);
      case LayoutDocumentPaneGroup _:
        return (UIElement) new LayoutDocumentPaneGroupControl(model as LayoutDocumentPaneGroup);
      case LayoutAnchorSide _:
        LayoutAnchorSideControl uiElementForModel1 = new LayoutAnchorSideControl(model as LayoutAnchorSide);
        uiElementForModel1.SetBinding(Control.TemplateProperty, (BindingBase) new Binding("AnchorSideTemplate")
        {
          Source = (object) this
        });
        return (UIElement) uiElementForModel1;
      case LayoutAnchorGroup _:
        LayoutAnchorGroupControl uiElementForModel2 = new LayoutAnchorGroupControl(model as LayoutAnchorGroup);
        uiElementForModel2.SetBinding(Control.TemplateProperty, (BindingBase) new Binding("AnchorGroupTemplate")
        {
          Source = (object) this
        });
        return (UIElement) uiElementForModel2;
      case LayoutDocumentPane _:
        LayoutDocumentPaneControl uiElementForModel3 = new LayoutDocumentPaneControl(model as LayoutDocumentPane);
        uiElementForModel3.SetBinding(FrameworkElement.StyleProperty, (BindingBase) new Binding("DocumentPaneControlStyle")
        {
          Source = (object) this
        });
        return (UIElement) uiElementForModel3;
      case LayoutAnchorablePane _:
        LayoutAnchorablePaneControl uiElementForModel4 = new LayoutAnchorablePaneControl(model as LayoutAnchorablePane);
        uiElementForModel4.SetBinding(FrameworkElement.StyleProperty, (BindingBase) new Binding("AnchorablePaneControlStyle")
        {
          Source = (object) this
        });
        return (UIElement) uiElementForModel4;
      case LayoutAnchorableFloatingWindow _:
        if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
          return (UIElement) null;
        LayoutAnchorableFloatingWindow model1 = model as LayoutAnchorableFloatingWindow;
        LayoutAnchorableFloatingWindowControl newFW = new LayoutAnchorableFloatingWindowControl(model1);
        newFW.SetParentToMainWindowOf((Visual) this);
        LayoutAnchorablePane paneInsideFloatingWindow = model1.RootPanel.Children.OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
        if (paneInsideFloatingWindow != null)
        {
          paneInsideFloatingWindow.KeepInsideNearestMonitor();
          newFW.Left = paneInsideFloatingWindow.FloatingLeft;
          newFW.Top = paneInsideFloatingWindow.FloatingTop;
          newFW.Width = paneInsideFloatingWindow.FloatingWidth;
          newFW.Height = paneInsideFloatingWindow.FloatingHeight;
        }
        newFW.ShowInTaskbar = false;
        this.Dispatcher.BeginInvoke((Delegate) (() => newFW.Show()), DispatcherPriority.Send);
        if (paneInsideFloatingWindow != null && paneInsideFloatingWindow.IsMaximized)
          newFW.WindowState = WindowState.Maximized;
        return (UIElement) newFW;
      case LayoutDocumentFloatingWindow _:
        if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
          return (UIElement) null;
        LayoutDocumentFloatingWindow model2 = model as LayoutDocumentFloatingWindow;
        LayoutDocumentFloatingWindowControl window = new LayoutDocumentFloatingWindowControl(model2);
        window.SetParentToMainWindowOf((Visual) this);
        LayoutDocument rootDocument = model2.RootDocument;
        if (rootDocument != null)
        {
          rootDocument.KeepInsideNearestMonitor();
          window.Left = rootDocument.FloatingLeft;
          window.Top = rootDocument.FloatingTop;
          window.Width = rootDocument.FloatingWidth;
          window.Height = rootDocument.FloatingHeight;
        }
        window.ShowInTaskbar = false;
        window.Show();
        if (rootDocument != null && rootDocument.IsMaximized)
          window.WindowState = WindowState.Maximized;
        return (UIElement) window;
      case LayoutDocument _:
        return (UIElement) new LayoutDocumentControl()
        {
          Model = (LayoutContent) (model as LayoutDocument)
        };
      default:
        return (UIElement) null;
    }
  }

  internal void ShowAutoHideWindow(LayoutAnchorControl anchor)
  {
    this._autoHideWindowManager.ShowAutoHideWindow(anchor);
  }

  internal void HideAutoHideWindow(LayoutAnchorControl anchor)
  {
    this._autoHideWindowManager.HideAutoWindow(anchor);
  }

  internal FrameworkElement GetAutoHideAreaElement() => this._autohideArea;

  internal void StartDraggingFloatingWindowForContent(LayoutContent contentModel, bool startDrag = true)
  {
    LayoutFloatingWindowControl fwc = this.CreateFloatingWindow(contentModel, false);
    if (fwc == null)
      return;
    this.Dispatcher.BeginInvoke((Delegate) (() =>
    {
      if (startDrag)
        fwc.AttachDrag();
      fwc.Show();
    }), DispatcherPriority.Send);
  }

  internal void StartDraggingFloatingWindowForPane(LayoutAnchorablePane paneModel)
  {
    LayoutFloatingWindowControl anchorableWithoutParent = this.CreateFloatingWindowForLayoutAnchorableWithoutParent(paneModel, false);
    if (anchorableWithoutParent == null)
      return;
    anchorableWithoutParent.AttachDrag();
    anchorableWithoutParent.Show();
  }

  internal IEnumerable<LayoutFloatingWindowControl> GetFloatingWindowsByZOrder()
  {
    DockingManager dockingManager = this;
    Window window = Window.GetWindow((DependencyObject) dockingManager);
    if (window != null)
    {
      for (IntPtr currentHandle = Win32Helper.GetWindow(new WindowInteropHelper(window).Handle, 0U); currentHandle != IntPtr.Zero; currentHandle = Win32Helper.GetWindow(currentHandle, 2U))
      {
        LayoutFloatingWindowControl floatingWindowControl = dockingManager._fwList.FirstOrDefault<LayoutFloatingWindowControl>((Func<LayoutFloatingWindowControl, bool>) (fw => new WindowInteropHelper((Window) fw).Handle == currentHandle));
        if (floatingWindowControl != null && floatingWindowControl.Model.Root.Manager == dockingManager)
          yield return floatingWindowControl;
      }
    }
  }

  internal void RemoveFloatingWindow(LayoutFloatingWindowControl floatingWindow)
  {
    this._fwList.Remove(floatingWindow);
  }

  internal void _ExecuteCloseCommand(LayoutDocument document)
  {
    if (this.DocumentClosing != null)
    {
      DocumentClosingEventArgs e = new DocumentClosingEventArgs(document);
      this.DocumentClosing((object) this, e);
      if (e.Cancel)
        return;
    }
    if (!document.CloseDocument())
      return;
    this.RemoveViewFromLogicalChild((LayoutContent) document);
    if (this.DocumentClosed == null)
      return;
    this.DocumentClosed((object) this, new DocumentClosedEventArgs(document));
  }

  internal void _ExecuteCloseAllButThisCommand(LayoutContent contentSelected)
  {
    foreach (LayoutContent contentToClose in this.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((Func<LayoutContent, bool>) (d =>
    {
      if (d == contentSelected)
        return false;
      return d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow;
    })).ToArray<LayoutContent>())
      this.Close(contentToClose);
  }

  internal void _ExecuteCloseAllCommand(LayoutContent contentSelected)
  {
    foreach (LayoutContent contentToClose in this.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((Func<LayoutContent, bool>) (d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).ToArray<LayoutContent>())
      this.Close(contentToClose);
  }

  internal void _ExecuteCloseCommand(LayoutAnchorable anchorable)
  {
    LayoutAnchorable layoutAnchorable = anchorable;
    if (layoutAnchorable == null)
      return;
    layoutAnchorable.CloseAnchorable();
    this.RemoveViewFromLogicalChild((LayoutContent) anchorable);
  }

  internal void _ExecuteHideCommand(LayoutAnchorable anchorable) => anchorable?.Hide();

  internal void _ExecuteAutoHideCommand(LayoutAnchorable _anchorable)
  {
    _anchorable.ToggleAutoHide();
  }

  internal void _ExecuteFloatCommand(LayoutContent contentToFloat) => contentToFloat.Float();

  internal void _ExecuteDockCommand(LayoutAnchorable anchorable) => anchorable.Dock();

  internal void _ExecuteDockAsDocumentCommand(LayoutContent content) => content.DockAsDocument();

  internal void _ExecuteContentActivateCommand(LayoutContent content) => content.IsActive = true;

  private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == "RootPanel")
    {
      if (!this.IsInitialized)
        return;
      this.LayoutRootPanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.RootPanel) as LayoutPanelControl;
    }
    else
    {
      if (!(e.PropertyName == "ActiveContent"))
        return;
      if (this.Layout.ActiveContent != null && this.Layout.ActiveContent != null)
        FocusElementManager.SetFocusOnLastElement((ILayoutElement) this.Layout.ActiveContent);
      if (this._insideInternalSetActiveContent || this.Layout.ActiveContent == null)
        return;
      this.ActiveContent = this.Layout.ActiveContent.Content;
    }
  }

  private void OnLayoutRootUpdated(object sender, EventArgs e)
  {
    CommandManager.InvalidateRequerySuggested();
  }

  private void OnLayoutChanging(LayoutRoot newLayout)
  {
    if (this.LayoutChanging == null)
      return;
    this.LayoutChanging((object) this, EventArgs.Empty);
  }

  private void DockingManager_Loaded(object sender, RoutedEventArgs e)
  {
    if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
      return;
    if (this.Layout.Manager == this)
    {
      this.LayoutRootPanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.RootPanel) as LayoutPanelControl;
      this.LeftSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.LeftSide) as LayoutAnchorSideControl;
      this.TopSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.TopSide) as LayoutAnchorSideControl;
      this.RightSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.RightSide) as LayoutAnchorSideControl;
      this.BottomSidePanel = this.CreateUIElementForModel((ILayoutElement) this.Layout.BottomSide) as LayoutAnchorSideControl;
    }
    this.SetupAutoHideWindow();
    foreach (ILayoutElement model in this.Layout.FloatingWindows.Where<LayoutFloatingWindow>((Func<LayoutFloatingWindow, bool>) (fw => !this._fwList.Any<LayoutFloatingWindowControl>((Func<LayoutFloatingWindowControl, bool>) (fwc => fwc.Model == fw)))))
      this._fwList.Add(this.CreateUIElementForModel(model) as LayoutFloatingWindowControl);
    if (this.IsVisible)
      this.CreateOverlayWindow();
    FocusElementManager.SetupFocusManagement(this);
  }

  private void DockingManager_Unloaded(object sender, RoutedEventArgs e)
  {
    if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
      return;
    if (this._autoHideWindowManager != null)
      this._autoHideWindowManager.HideAutoWindow();
    if (this.AutoHideWindow != null)
      this.AutoHideWindow.Dispose();
    foreach (LayoutFloatingWindowControl window in this._fwList.ToArray())
    {
      window.SetParentWindowToNull();
      window.KeepContentVisibleOnClose = true;
      window.Close();
    }
    this._fwList.Clear();
    this.DestroyOverlayWindow();
    FocusElementManager.FinalizeFocusManagement(this);
  }

  private void SetupAutoHideWindow()
  {
    if (this._autoHideWindowManager != null)
      this._autoHideWindowManager.HideAutoWindow();
    else
      this._autoHideWindowManager = new AutoHideWindowManager(this);
    if (this.AutoHideWindow != null)
      this.AutoHideWindow.Dispose();
    this.SetAutoHideWindow(new LayoutAutoHideWindowControl());
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

  private void DestroyOverlayWindow()
  {
    if (this._overlayWindow == null)
      return;
    this._overlayWindow.Close();
    this._overlayWindow = (OverlayWindow) null;
  }

  private void AttachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
  {
    if (documentsSource == null || layout == null)
      return;
    object[] array = layout.Descendents().OfType<LayoutDocument>().Select<LayoutDocument, object>((Func<LayoutDocument, object>) (d => d.Content)).ToArray<object>();
    List<object> objectList = new List<object>(documentsSource.OfType<object>());
    foreach (object obj in objectList.ToArray())
    {
      if (Extensions.Contains(array, obj))
        objectList.Remove(obj);
    }
    LayoutDocumentPane destinationContainer = (LayoutDocumentPane) null;
    if (layout.LastFocusedDocument != null)
      destinationContainer = layout.LastFocusedDocument.Parent as LayoutDocumentPane;
    if (destinationContainer == null)
      destinationContainer = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
    this._suspendLayoutItemCreation = true;
    foreach (object obj in objectList)
    {
      LayoutDocument layoutDocument1 = new LayoutDocument();
      layoutDocument1.Content = obj;
      LayoutDocument layoutDocument2 = layoutDocument1;
      bool flag = false;
      if (this.LayoutUpdateStrategy != null)
        flag = this.LayoutUpdateStrategy.BeforeInsertDocument(layout, layoutDocument2, (ILayoutContainer) destinationContainer);
      if (!flag)
      {
        if (destinationContainer == null)
          throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
        destinationContainer.Children.Add((LayoutContent) layoutDocument2);
      }
      if (this.LayoutUpdateStrategy != null)
        this.LayoutUpdateStrategy.AfterInsertDocument(layout, layoutDocument2);
      this.CreateDocumentLayoutItem(layoutDocument2);
    }
    this._suspendLayoutItemCreation = false;
    if (!(documentsSource is INotifyCollectionChanged collectionChanged))
      return;
    collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.documentsSourceElementsChanged);
  }

  private void documentsSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (this.Layout == null || this.SuspendDocumentsSourceBinding)
      return;
    if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
    {
      foreach (LayoutDocument element in this.Layout.Descendents().OfType<LayoutDocument>().Where<LayoutDocument>((Func<LayoutDocument, bool>) (d => e.OldItems.Contains(d.Content))).ToArray<LayoutDocument>())
      {
        element.Parent.RemoveChild((ILayoutElement) element);
        this.RemoveViewFromLogicalChild((LayoutContent) element);
      }
    }
    if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
    {
      LayoutDocumentPane destinationContainer = (LayoutDocumentPane) null;
      if (this.Layout.LastFocusedDocument != null)
        destinationContainer = this.Layout.LastFocusedDocument.Parent as LayoutDocumentPane;
      if (destinationContainer == null)
        destinationContainer = this.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
      this._suspendLayoutItemCreation = true;
      foreach (object newItem in (IEnumerable) e.NewItems)
      {
        LayoutDocument layoutDocument1 = new LayoutDocument();
        layoutDocument1.Content = newItem;
        LayoutDocument layoutDocument2 = layoutDocument1;
        bool flag = false;
        if (this.LayoutUpdateStrategy != null)
          flag = this.LayoutUpdateStrategy.BeforeInsertDocument(this.Layout, layoutDocument2, (ILayoutContainer) destinationContainer);
        if (!flag)
        {
          if (destinationContainer == null)
            throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
          destinationContainer.Children.Add((LayoutContent) layoutDocument2);
        }
        if (this.LayoutUpdateStrategy != null)
          this.LayoutUpdateStrategy.AfterInsertDocument(this.Layout, layoutDocument2);
        ILayoutRoot root = layoutDocument2.Root;
        if (root != null && root.Manager == this)
          this.CreateDocumentLayoutItem(layoutDocument2);
      }
      this._suspendLayoutItemCreation = false;
    }
    if (e.Action == NotifyCollectionChangedAction.Reset)
    {
      foreach (LayoutDocument element in this.Layout.Descendents().OfType<LayoutDocument>().ToArray<LayoutDocument>())
      {
        element.Parent.RemoveChild((ILayoutElement) element);
        this.RemoveViewFromLogicalChild((LayoutContent) element);
      }
    }
    if (this.Layout == null)
      return;
    this.Layout.CollectGarbage();
  }

  private void DetachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
  {
    if (documentsSource == null || layout == null)
      return;
    foreach (LayoutDocument element in layout.Descendents().OfType<LayoutDocument>().Where<LayoutDocument>((Func<LayoutDocument, bool>) (d => documentsSource.Contains(d.Content))).ToArray<LayoutDocument>())
    {
      element.Parent.RemoveChild((ILayoutElement) element);
      this.RemoveViewFromLogicalChild((LayoutContent) element);
    }
    if (!(documentsSource is INotifyCollectionChanged collectionChanged))
      return;
    collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.documentsSourceElementsChanged);
  }

  private void Close(LayoutContent contentToClose)
  {
    if (!contentToClose.CanClose)
      return;
    LayoutItem layoutItemFromModel = this.GetLayoutItemFromModel(contentToClose);
    if (layoutItemFromModel.CloseCommand != null)
    {
      if (!layoutItemFromModel.CloseCommand.CanExecute((object) null))
        return;
      layoutItemFromModel.CloseCommand.Execute((object) null);
    }
    else
    {
      switch (contentToClose)
      {
        case LayoutDocument _:
          this._ExecuteCloseCommand(contentToClose as LayoutDocument);
          break;
        case LayoutAnchorable _:
          this._ExecuteCloseCommand(contentToClose as LayoutAnchorable);
          break;
      }
    }
  }

  private void AttachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
  {
    if (anchorablesSource == null || layout == null)
      return;
    object[] array = layout.Descendents().OfType<LayoutAnchorable>().Select<LayoutAnchorable, object>((Func<LayoutAnchorable, object>) (d => d.Content)).ToArray<object>();
    List<object> objectList = new List<object>(anchorablesSource.OfType<object>());
    foreach (object obj in objectList.ToArray())
    {
      if (Extensions.Contains(array, obj))
        objectList.Remove(obj);
    }
    LayoutAnchorablePane destinationContainer = (LayoutAnchorablePane) null;
    if (layout.ActiveContent != null)
      destinationContainer = layout.ActiveContent.Parent as LayoutAnchorablePane;
    if (destinationContainer == null)
      destinationContainer = layout.Descendents().OfType<LayoutAnchorablePane>().Where<LayoutAnchorablePane>((Func<LayoutAnchorablePane, bool>) (pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right)).FirstOrDefault<LayoutAnchorablePane>();
    if (destinationContainer == null)
      destinationContainer = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
    this._suspendLayoutItemCreation = true;
    foreach (object obj in objectList)
    {
      LayoutAnchorable layoutAnchorable1 = new LayoutAnchorable();
      layoutAnchorable1.Content = obj;
      LayoutAnchorable layoutAnchorable2 = layoutAnchorable1;
      bool flag = false;
      if (this.LayoutUpdateStrategy != null)
        flag = this.LayoutUpdateStrategy.BeforeInsertAnchorable(layout, layoutAnchorable2, (ILayoutContainer) destinationContainer);
      if (!flag)
      {
        if (destinationContainer == null)
        {
          LayoutPanel layoutPanel = new LayoutPanel()
          {
            Orientation = Orientation.Horizontal
          };
          if (layout.RootPanel != null)
            layoutPanel.Children.Add((ILayoutPanelElement) layout.RootPanel);
          layout.RootPanel = layoutPanel;
          LayoutAnchorablePane layoutAnchorablePane = new LayoutAnchorablePane();
          layoutAnchorablePane.DockWidth = new GridLength(200.0, GridUnitType.Pixel);
          destinationContainer = layoutAnchorablePane;
          layoutPanel.Children.Add((ILayoutPanelElement) destinationContainer);
        }
        destinationContainer.Children.Add(layoutAnchorable2);
      }
      if (this.LayoutUpdateStrategy != null)
        this.LayoutUpdateStrategy.AfterInsertAnchorable(layout, layoutAnchorable2);
      this.CreateAnchorableLayoutItem(layoutAnchorable2);
    }
    this._suspendLayoutItemCreation = false;
    if (!(anchorablesSource is INotifyCollectionChanged collectionChanged))
      return;
    collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.anchorablesSourceElementsChanged);
  }

  private void anchorablesSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (this.Layout == null || this.SuspendAnchorablesSourceBinding)
      return;
    if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
    {
      foreach (LayoutAnchorable element in this.Layout.Descendents().OfType<LayoutAnchorable>().Where<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (d => e.OldItems.Contains(d.Content))).ToArray<LayoutAnchorable>())
      {
        element.Content = (object) null;
        element.Parent.RemoveChild((ILayoutElement) element);
        this.RemoveViewFromLogicalChild((LayoutContent) element);
      }
    }
    if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
    {
      LayoutAnchorablePane destinationContainer = (LayoutAnchorablePane) null;
      if (this.Layout.ActiveContent != null)
        destinationContainer = this.Layout.ActiveContent.Parent as LayoutAnchorablePane;
      if (destinationContainer == null)
        destinationContainer = this.Layout.Descendents().OfType<LayoutAnchorablePane>().Where<LayoutAnchorablePane>((Func<LayoutAnchorablePane, bool>) (pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right)).FirstOrDefault<LayoutAnchorablePane>();
      if (destinationContainer == null)
        destinationContainer = this.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
      this._suspendLayoutItemCreation = true;
      foreach (object newItem in (IEnumerable) e.NewItems)
      {
        LayoutAnchorable layoutAnchorable1 = new LayoutAnchorable();
        layoutAnchorable1.Content = newItem;
        LayoutAnchorable layoutAnchorable2 = layoutAnchorable1;
        bool flag = false;
        if (this.LayoutUpdateStrategy != null)
          flag = this.LayoutUpdateStrategy.BeforeInsertAnchorable(this.Layout, layoutAnchorable2, (ILayoutContainer) destinationContainer);
        if (!flag)
        {
          if (destinationContainer == null)
          {
            LayoutPanel layoutPanel = new LayoutPanel()
            {
              Orientation = Orientation.Horizontal
            };
            if (this.Layout.RootPanel != null)
              layoutPanel.Children.Add((ILayoutPanelElement) this.Layout.RootPanel);
            this.Layout.RootPanel = layoutPanel;
            LayoutAnchorablePane layoutAnchorablePane = new LayoutAnchorablePane();
            layoutAnchorablePane.DockWidth = new GridLength(200.0, GridUnitType.Pixel);
            destinationContainer = layoutAnchorablePane;
            layoutPanel.Children.Add((ILayoutPanelElement) destinationContainer);
          }
          destinationContainer.Children.Add(layoutAnchorable2);
        }
        if (this.LayoutUpdateStrategy != null)
          this.LayoutUpdateStrategy.AfterInsertAnchorable(this.Layout, layoutAnchorable2);
        ILayoutRoot root = layoutAnchorable2.Root;
        if (root != null && root.Manager == this)
          this.CreateAnchorableLayoutItem(layoutAnchorable2);
      }
      this._suspendLayoutItemCreation = false;
    }
    if (e.Action == NotifyCollectionChangedAction.Reset)
    {
      foreach (LayoutAnchorable element in this.Layout.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
      {
        element.Parent.RemoveChild((ILayoutElement) element);
        this.RemoveViewFromLogicalChild((LayoutContent) element);
      }
    }
    if (this.Layout == null)
      return;
    this.Layout.CollectGarbage();
  }

  private void DetachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
  {
    if (anchorablesSource == null || layout == null)
      return;
    foreach (LayoutAnchorable element in layout.Descendents().OfType<LayoutAnchorable>().Where<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (d => anchorablesSource.Contains(d.Content))).ToArray<LayoutAnchorable>())
    {
      element.Parent.RemoveChild((ILayoutElement) element);
      this.RemoveViewFromLogicalChild((LayoutContent) element);
    }
    if (!(anchorablesSource is INotifyCollectionChanged collectionChanged))
      return;
    collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.anchorablesSourceElementsChanged);
  }

  private void RemoveViewFromLogicalChild(LayoutContent layoutContent)
  {
    if (layoutContent == null)
      return;
    LayoutItem layoutItemFromModel = this.GetLayoutItemFromModel(layoutContent);
    if (layoutItemFromModel == null || !layoutItemFromModel.IsViewExists())
      return;
    this.InternalRemoveLogicalChild((object) layoutItemFromModel.View);
  }

  private void InternalSetActiveContent(object contentObject)
  {
    LayoutContent layoutContent = this.Layout.Descendents().OfType<LayoutContent>().FirstOrDefault<LayoutContent>((Func<LayoutContent, bool>) (lc => lc == contentObject || lc.Content == contentObject));
    this._insideInternalSetActiveContent = true;
    this.Layout.ActiveContent = layoutContent;
    this._insideInternalSetActiveContent = false;
  }

  private void DetachLayoutItems()
  {
    if (this.Layout == null)
      return;
    this._layoutItems.ForEach<LayoutItem>((Action<LayoutItem>) (i => i.Detach()));
    this._layoutItems.Clear();
    this.Layout.ElementAdded -= new EventHandler<LayoutElementEventArgs>(this.Layout_ElementAdded);
    this.Layout.ElementRemoved -= new EventHandler<LayoutElementEventArgs>(this.Layout_ElementRemoved);
  }

  private void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
  {
    if (this._suspendLayoutItemCreation)
      return;
    this.CollectLayoutItemsDeleted();
  }

  private void Layout_ElementAdded(object sender, LayoutElementEventArgs e)
  {
    if (this._suspendLayoutItemCreation)
      return;
    foreach (LayoutContent contentToAttach in this.Layout.Descendents().OfType<LayoutContent>())
    {
      if (contentToAttach is LayoutDocument)
        this.CreateDocumentLayoutItem(contentToAttach as LayoutDocument);
      else
        this.CreateAnchorableLayoutItem(contentToAttach as LayoutAnchorable);
    }
    this.CollectLayoutItemsDeleted();
  }

  private void CollectLayoutItemsDeleted()
  {
    if (this._collectLayoutItemsOperations != null)
      return;
    this._collectLayoutItemsOperations = this.Dispatcher.BeginInvoke((Delegate) (() =>
    {
      this._collectLayoutItemsOperations = (DispatcherOperation) null;
      foreach (LayoutItem layoutItem in this._layoutItems.Where<LayoutItem>((Func<LayoutItem, bool>) (item => item.LayoutElement.Root != this.Layout)).ToArray<LayoutItem>())
      {
        if (layoutItem != null && layoutItem.Model != null)
        {
          UIElement model = layoutItem.Model as UIElement;
        }
        layoutItem.Detach();
        this._layoutItems.Remove(layoutItem);
      }
    }));
  }

  private void AttachLayoutItems()
  {
    if (this.Layout == null)
      return;
    foreach (LayoutDocument contentToAttach in this.Layout.Descendents().OfType<LayoutDocument>().ToArray<LayoutDocument>())
      this.CreateDocumentLayoutItem(contentToAttach);
    foreach (LayoutAnchorable contentToAttach in this.Layout.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
      this.CreateAnchorableLayoutItem(contentToAttach);
    this.Layout.ElementAdded += new EventHandler<LayoutElementEventArgs>(this.Layout_ElementAdded);
    this.Layout.ElementRemoved += new EventHandler<LayoutElementEventArgs>(this.Layout_ElementRemoved);
  }

  private void ApplyStyleToLayoutItem(LayoutItem layoutItem)
  {
    layoutItem._ClearDefaultBindings();
    if (this.LayoutItemContainerStyle != null)
      layoutItem.Style = this.LayoutItemContainerStyle;
    else if (this.LayoutItemContainerStyleSelector != null)
      layoutItem.Style = this.LayoutItemContainerStyleSelector.SelectStyle(layoutItem.Model, (DependencyObject) layoutItem);
    layoutItem._SetDefaultBindings();
  }

  private void CreateAnchorableLayoutItem(LayoutAnchorable contentToAttach)
  {
    if (this._layoutItems.Any<LayoutItem>((Func<LayoutItem, bool>) (item => item.LayoutElement == contentToAttach)))
    {
      foreach (LayoutItem layoutItem in this._layoutItems)
        this.ApplyStyleToLayoutItem(layoutItem);
    }
    else
    {
      LayoutAnchorableItem layoutAnchorableItem = new LayoutAnchorableItem();
      layoutAnchorableItem.Attach((LayoutContent) contentToAttach);
      this.ApplyStyleToLayoutItem((LayoutItem) layoutAnchorableItem);
      this._layoutItems.Add((LayoutItem) layoutAnchorableItem);
      if (contentToAttach == null || contentToAttach.Content == null || !(contentToAttach.Content is UIElement))
        return;
      this.InternalAddLogicalChild(contentToAttach.Content);
    }
  }

  private void CreateDocumentLayoutItem(LayoutDocument contentToAttach)
  {
    if (this._layoutItems.Any<LayoutItem>((Func<LayoutItem, bool>) (item => item.LayoutElement == contentToAttach)))
    {
      foreach (LayoutItem layoutItem in this._layoutItems)
        this.ApplyStyleToLayoutItem(layoutItem);
    }
    else
    {
      LayoutDocumentItem layoutDocumentItem = new LayoutDocumentItem();
      layoutDocumentItem.Attach((LayoutContent) contentToAttach);
      this.ApplyStyleToLayoutItem((LayoutItem) layoutDocumentItem);
      this._layoutItems.Add((LayoutItem) layoutDocumentItem);
      if (contentToAttach == null || contentToAttach.Content == null || !(contentToAttach.Content is UIElement))
        return;
      this.InternalAddLogicalChild(contentToAttach.Content);
    }
  }

  private void ShowNavigatorWindow()
  {
    if (this._navigatorWindow == null)
    {
      NavigatorWindow navigatorWindow = new NavigatorWindow(this);
      navigatorWindow.Owner = Window.GetWindow((DependencyObject) this);
      navigatorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      this._navigatorWindow = navigatorWindow;
    }
    this._navigatorWindow.ShowDialog();
    this._navigatorWindow = (NavigatorWindow) null;
  }

  private LayoutFloatingWindowControl CreateFloatingWindowForLayoutAnchorableWithoutParent(
    LayoutAnchorablePane paneModel,
    bool isContentImmutable)
  {
    if (paneModel.Children.Any<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (c => !c.CanFloat)))
      return (LayoutFloatingWindowControl) null;
    ILayoutPositionableElement positionableElement = (ILayoutPositionableElement) paneModel;
    ILayoutPositionableElementWithActualSize elementWithActualSize = (ILayoutPositionableElementWithActualSize) paneModel;
    double num1 = positionableElement.FloatingWidth;
    double num2 = positionableElement.FloatingHeight;
    double floatingLeft = positionableElement.FloatingLeft;
    double floatingTop = positionableElement.FloatingTop;
    if (num1 == 0.0)
      num1 = elementWithActualSize.ActualWidth + 10.0;
    if (num2 == 0.0)
      num2 = elementWithActualSize.ActualHeight + 10.0;
    LayoutAnchorablePane layoutAnchorablePane = new LayoutAnchorablePane();
    layoutAnchorablePane.DockWidth = positionableElement.DockWidth;
    layoutAnchorablePane.DockHeight = positionableElement.DockHeight;
    layoutAnchorablePane.DockMinHeight = positionableElement.DockMinHeight;
    layoutAnchorablePane.DockMinWidth = positionableElement.DockMinWidth;
    layoutAnchorablePane.FloatingLeft = positionableElement.FloatingLeft;
    layoutAnchorablePane.FloatingTop = positionableElement.FloatingTop;
    layoutAnchorablePane.FloatingWidth = positionableElement.FloatingWidth;
    layoutAnchorablePane.FloatingHeight = positionableElement.FloatingHeight;
    LayoutAnchorablePane firstChild = layoutAnchorablePane;
    bool flag = paneModel.FindParent<LayoutFloatingWindow>() == null;
    int selectedContentIndex = paneModel.SelectedContentIndex;
    while (paneModel.Children.Count > 0)
    {
      LayoutAnchorable child = paneModel.Children[paneModel.Children.Count - 1];
      if (flag)
      {
        ((ILayoutPreviousContainer) child).PreviousContainer = (ILayoutContainer) paneModel;
        child.PreviousContainerIndex = paneModel.Children.Count - 1;
      }
      paneModel.RemoveChildAt(paneModel.Children.Count - 1);
      firstChild.Children.Insert(0, child);
    }
    if (firstChild.Children.Count > 0)
      firstChild.SelectedContentIndex = selectedContentIndex;
    LayoutAnchorableFloatingWindow anchorableFloatingWindow = new LayoutAnchorableFloatingWindow();
    LayoutAnchorablePaneGroup anchorablePaneGroup = new LayoutAnchorablePaneGroup(firstChild);
    anchorablePaneGroup.DockHeight = firstChild.DockHeight;
    anchorablePaneGroup.DockWidth = firstChild.DockWidth;
    anchorablePaneGroup.DockMinHeight = firstChild.DockMinHeight;
    anchorablePaneGroup.DockMinWidth = firstChild.DockMinWidth;
    anchorableFloatingWindow.RootPanel = anchorablePaneGroup;
    LayoutFloatingWindow model = (LayoutFloatingWindow) anchorableFloatingWindow;
    this.Layout.FloatingWindows.Add(model);
    LayoutAnchorableFloatingWindowControl floatingWindowControl = new LayoutAnchorableFloatingWindowControl(model as LayoutAnchorableFloatingWindow, isContentImmutable);
    floatingWindowControl.Width = num1;
    floatingWindowControl.Height = num2;
    floatingWindowControl.Top = floatingTop;
    floatingWindowControl.Left = floatingLeft;
    LayoutFloatingWindowControl anchorableWithoutParent = (LayoutFloatingWindowControl) floatingWindowControl;
    this._fwList.Add(anchorableWithoutParent);
    this.Layout.CollectGarbage();
    this.InvalidateArrange();
    return anchorableWithoutParent;
  }

  private LayoutFloatingWindowControl CreateFloatingWindowCore(
    LayoutContent contentModel,
    bool isContentImmutable)
  {
    if (!contentModel.CanFloat)
      return (LayoutFloatingWindowControl) null;
    if (contentModel is LayoutAnchorable layoutAnchorable && layoutAnchorable.IsAutoHidden)
      layoutAnchorable.ToggleAutoHide();
    ILayoutPane parent1 = contentModel.Parent as ILayoutPane;
    ILayoutPositionableElement parent2 = contentModel.Parent as ILayoutPositionableElement;
    ILayoutPositionableElementWithActualSize parent3 = contentModel.Parent as ILayoutPositionableElementWithActualSize;
    int childIndex = parent1.Children.ToList<ILayoutElement>().IndexOf((ILayoutElement) contentModel);
    if (contentModel.FindParent<LayoutFloatingWindow>() == null)
    {
      ((ILayoutPreviousContainer) contentModel).PreviousContainer = (ILayoutContainer) parent1;
      contentModel.PreviousContainerIndex = childIndex;
    }
    parent1.RemoveChildAt(childIndex);
    double num1 = contentModel.FloatingWidth;
    double num2 = contentModel.FloatingHeight;
    if (num1 == 0.0)
      num1 = parent2.FloatingWidth;
    if (num2 == 0.0)
      num2 = parent2.FloatingHeight;
    if (num1 == 0.0)
      num1 = parent3.ActualWidth + 10.0;
    if (num2 == 0.0)
      num2 = parent3.ActualHeight + 10.0;
    LayoutFloatingWindowControl floatingWindowCore;
    if (contentModel is LayoutAnchorable)
    {
      LayoutAnchorable anchorable = contentModel as LayoutAnchorable;
      LayoutAnchorableFloatingWindow anchorableFloatingWindow = new LayoutAnchorableFloatingWindow();
      LayoutAnchorablePane firstChild = new LayoutAnchorablePane(anchorable);
      firstChild.DockWidth = parent2.DockWidth;
      firstChild.DockHeight = parent2.DockHeight;
      firstChild.DockMinHeight = parent2.DockMinHeight;
      firstChild.DockMinWidth = parent2.DockMinWidth;
      firstChild.FloatingLeft = parent2.FloatingLeft;
      firstChild.FloatingTop = parent2.FloatingTop;
      firstChild.FloatingWidth = parent2.FloatingWidth;
      firstChild.FloatingHeight = parent2.FloatingHeight;
      anchorableFloatingWindow.RootPanel = new LayoutAnchorablePaneGroup(firstChild);
      LayoutFloatingWindow model = (LayoutFloatingWindow) anchorableFloatingWindow;
      this.Layout.FloatingWindows.Add(model);
      LayoutAnchorableFloatingWindowControl floatingWindowControl = new LayoutAnchorableFloatingWindowControl(model as LayoutAnchorableFloatingWindow, isContentImmutable);
      floatingWindowControl.Width = num1;
      floatingWindowControl.Height = num2;
      floatingWindowControl.Left = contentModel.FloatingLeft;
      floatingWindowControl.Top = contentModel.FloatingTop;
      floatingWindowCore = (LayoutFloatingWindowControl) floatingWindowControl;
    }
    else
    {
      LayoutDocument layoutDocument = contentModel as LayoutDocument;
      LayoutFloatingWindow model = (LayoutFloatingWindow) new LayoutDocumentFloatingWindow()
      {
        RootDocument = layoutDocument
      };
      this.Layout.FloatingWindows.Add(model);
      LayoutDocumentFloatingWindowControl floatingWindowControl = new LayoutDocumentFloatingWindowControl(model as LayoutDocumentFloatingWindow, isContentImmutable);
      floatingWindowControl.Width = num1;
      floatingWindowControl.Height = num2;
      floatingWindowControl.Left = contentModel.FloatingLeft;
      floatingWindowControl.Top = contentModel.FloatingTop;
      floatingWindowCore = (LayoutFloatingWindowControl) floatingWindowControl;
    }
    this._fwList.Add(floatingWindowCore);
    this.Layout.CollectGarbage();
    this.UpdateLayout();
    return floatingWindowCore;
  }

  public event EventHandler LayoutChanged;

  public event EventHandler LayoutChanging;

  public event EventHandler<DocumentClosingEventArgs> DocumentClosing;

  public event EventHandler<DocumentClosedEventArgs> DocumentClosed;

  public event EventHandler ActiveContentChanged;

  bool IOverlayWindowHost.HitTest(Point dragPoint)
  {
    return new Rect(this.PointToScreenDPIWithoutFlowDirection(new Point()), this.TransformActualSizeToAncestor()).Contains(dragPoint);
  }

  DockingManager IOverlayWindowHost.Manager => this;

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
    this._areas = (List<IDropArea>) null;
    this._overlayWindow.Owner = (Window) null;
    this._overlayWindow.HideDropTargets();
  }

  IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
  {
    if (this._areas != null)
      return (IEnumerable<IDropArea>) this._areas;
    int num = draggingWindow.Model is LayoutDocumentFloatingWindow ? 1 : 0;
    this._areas = new List<IDropArea>();
    if (num == 0)
    {
      this._areas.Add((IDropArea) new DropArea<DockingManager>(this, DropAreaType.DockingManager));
      foreach (LayoutAnchorablePaneControl visualChild in this.FindVisualChildren<LayoutAnchorablePaneControl>())
      {
        if (visualChild.Model.Descendents().Any<ILayoutElement>())
          this._areas.Add((IDropArea) new DropArea<LayoutAnchorablePaneControl>(visualChild, DropAreaType.AnchorablePane));
      }
    }
    foreach (LayoutDocumentPaneControl visualChild in this.FindVisualChildren<LayoutDocumentPaneControl>())
      this._areas.Add((IDropArea) new DropArea<LayoutDocumentPaneControl>(visualChild, DropAreaType.DocumentPane));
    foreach (LayoutDocumentPaneGroupControl visualChild in this.FindVisualChildren<LayoutDocumentPaneGroupControl>())
    {
      if ((visualChild.Model as LayoutDocumentPaneGroup).Children.Where<ILayoutDocumentPane>((Func<ILayoutDocumentPane, bool>) (c => c.IsVisible)).Count<ILayoutDocumentPane>() == 0)
        this._areas.Add((IDropArea) new DropArea<LayoutDocumentPaneGroupControl>(visualChild, DropAreaType.DocumentPaneGroup));
    }
    return (IEnumerable<IDropArea>) this._areas;
  }
}
