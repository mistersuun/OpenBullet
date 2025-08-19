// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.OverlayWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class OverlayWindow : Window, IOverlayWindow
{
  private ResourceDictionary currentThemeResourceDictionary;
  private Canvas _mainCanvasPanel;
  private Grid _gridDockingManagerDropTargets;
  private Grid _gridAnchorablePaneDropTargets;
  private Grid _gridDocumentPaneDropTargets;
  private Grid _gridDocumentPaneFullDropTargets;
  private FrameworkElement _dockingManagerDropTargetBottom;
  private FrameworkElement _dockingManagerDropTargetTop;
  private FrameworkElement _dockingManagerDropTargetLeft;
  private FrameworkElement _dockingManagerDropTargetRight;
  private FrameworkElement _anchorablePaneDropTargetBottom;
  private FrameworkElement _anchorablePaneDropTargetTop;
  private FrameworkElement _anchorablePaneDropTargetLeft;
  private FrameworkElement _anchorablePaneDropTargetRight;
  private FrameworkElement _anchorablePaneDropTargetInto;
  private FrameworkElement _documentPaneDropTargetBottom;
  private FrameworkElement _documentPaneDropTargetTop;
  private FrameworkElement _documentPaneDropTargetLeft;
  private FrameworkElement _documentPaneDropTargetRight;
  private FrameworkElement _documentPaneDropTargetInto;
  private FrameworkElement _documentPaneDropTargetBottomAsAnchorablePane;
  private FrameworkElement _documentPaneDropTargetTopAsAnchorablePane;
  private FrameworkElement _documentPaneDropTargetLeftAsAnchorablePane;
  private FrameworkElement _documentPaneDropTargetRightAsAnchorablePane;
  private FrameworkElement _documentPaneFullDropTargetBottom;
  private FrameworkElement _documentPaneFullDropTargetTop;
  private FrameworkElement _documentPaneFullDropTargetLeft;
  private FrameworkElement _documentPaneFullDropTargetRight;
  private FrameworkElement _documentPaneFullDropTargetInto;
  private Path _previewBox;
  private IOverlayWindowHost _host;
  private LayoutFloatingWindowControl _floatingWindow;
  private List<IDropArea> _visibleAreas = new List<IDropArea>();

  static OverlayWindow()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (OverlayWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (OverlayWindow)));
    Window.AllowsTransparencyProperty.OverrideMetadata(typeof (OverlayWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    Window.WindowStyleProperty.OverrideMetadata(typeof (OverlayWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) WindowStyle.None));
    Window.ShowInTaskbarProperty.OverrideMetadata(typeof (OverlayWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    Window.ShowActivatedProperty.OverrideMetadata(typeof (OverlayWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    UIElement.VisibilityProperty.OverrideMetadata(typeof (OverlayWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Hidden));
  }

  internal OverlayWindow(IOverlayWindowHost host)
  {
    this._host = host;
    this.UpdateThemeResources();
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._mainCanvasPanel = this.GetTemplateChild("PART_DropTargetsContainer") as Canvas;
    this._gridDockingManagerDropTargets = this.GetTemplateChild("PART_DockingManagerDropTargets") as Grid;
    this._gridAnchorablePaneDropTargets = this.GetTemplateChild("PART_AnchorablePaneDropTargets") as Grid;
    this._gridDocumentPaneDropTargets = this.GetTemplateChild("PART_DocumentPaneDropTargets") as Grid;
    this._gridDocumentPaneFullDropTargets = this.GetTemplateChild("PART_DocumentPaneFullDropTargets") as Grid;
    this._gridDockingManagerDropTargets.Visibility = Visibility.Hidden;
    this._gridAnchorablePaneDropTargets.Visibility = Visibility.Hidden;
    this._gridDocumentPaneDropTargets.Visibility = Visibility.Hidden;
    if (this._gridDocumentPaneFullDropTargets != null)
      this._gridDocumentPaneFullDropTargets.Visibility = Visibility.Hidden;
    this._dockingManagerDropTargetBottom = this.GetTemplateChild("PART_DockingManagerDropTargetBottom") as FrameworkElement;
    this._dockingManagerDropTargetTop = this.GetTemplateChild("PART_DockingManagerDropTargetTop") as FrameworkElement;
    this._dockingManagerDropTargetLeft = this.GetTemplateChild("PART_DockingManagerDropTargetLeft") as FrameworkElement;
    this._dockingManagerDropTargetRight = this.GetTemplateChild("PART_DockingManagerDropTargetRight") as FrameworkElement;
    this._anchorablePaneDropTargetBottom = this.GetTemplateChild("PART_AnchorablePaneDropTargetBottom") as FrameworkElement;
    this._anchorablePaneDropTargetTop = this.GetTemplateChild("PART_AnchorablePaneDropTargetTop") as FrameworkElement;
    this._anchorablePaneDropTargetLeft = this.GetTemplateChild("PART_AnchorablePaneDropTargetLeft") as FrameworkElement;
    this._anchorablePaneDropTargetRight = this.GetTemplateChild("PART_AnchorablePaneDropTargetRight") as FrameworkElement;
    this._anchorablePaneDropTargetInto = this.GetTemplateChild("PART_AnchorablePaneDropTargetInto") as FrameworkElement;
    this._documentPaneDropTargetBottom = this.GetTemplateChild("PART_DocumentPaneDropTargetBottom") as FrameworkElement;
    this._documentPaneDropTargetTop = this.GetTemplateChild("PART_DocumentPaneDropTargetTop") as FrameworkElement;
    this._documentPaneDropTargetLeft = this.GetTemplateChild("PART_DocumentPaneDropTargetLeft") as FrameworkElement;
    this._documentPaneDropTargetRight = this.GetTemplateChild("PART_DocumentPaneDropTargetRight") as FrameworkElement;
    this._documentPaneDropTargetInto = this.GetTemplateChild("PART_DocumentPaneDropTargetInto") as FrameworkElement;
    this._documentPaneDropTargetBottomAsAnchorablePane = this.GetTemplateChild("PART_DocumentPaneDropTargetBottomAsAnchorablePane") as FrameworkElement;
    this._documentPaneDropTargetTopAsAnchorablePane = this.GetTemplateChild("PART_DocumentPaneDropTargetTopAsAnchorablePane") as FrameworkElement;
    this._documentPaneDropTargetLeftAsAnchorablePane = this.GetTemplateChild("PART_DocumentPaneDropTargetLeftAsAnchorablePane") as FrameworkElement;
    this._documentPaneDropTargetRightAsAnchorablePane = this.GetTemplateChild("PART_DocumentPaneDropTargetRightAsAnchorablePane") as FrameworkElement;
    this._documentPaneFullDropTargetBottom = this.GetTemplateChild("PART_DocumentPaneFullDropTargetBottom") as FrameworkElement;
    this._documentPaneFullDropTargetTop = this.GetTemplateChild("PART_DocumentPaneFullDropTargetTop") as FrameworkElement;
    this._documentPaneFullDropTargetLeft = this.GetTemplateChild("PART_DocumentPaneFullDropTargetLeft") as FrameworkElement;
    this._documentPaneFullDropTargetRight = this.GetTemplateChild("PART_DocumentPaneFullDropTargetRight") as FrameworkElement;
    this._documentPaneFullDropTargetInto = this.GetTemplateChild("PART_DocumentPaneFullDropTargetInto") as FrameworkElement;
    this._previewBox = this.GetTemplateChild("PART_PreviewBox") as Path;
  }

  protected override void OnClosing(CancelEventArgs e) => base.OnClosing(e);

  internal void UpdateThemeResources(Theme oldTheme = null)
  {
    if (oldTheme != null)
    {
      if (oldTheme is DictionaryTheme)
      {
        if (this.currentThemeResourceDictionary != null)
        {
          this.Resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
          this.currentThemeResourceDictionary = (ResourceDictionary) null;
        }
      }
      else
      {
        ResourceDictionary resourceDictionary = this.Resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((Func<ResourceDictionary, bool>) (r => r.Source == oldTheme.GetResourceUri()));
        if (resourceDictionary != null)
          this.Resources.MergedDictionaries.Remove(resourceDictionary);
      }
    }
    if (this._host.Manager.Theme == null)
      return;
    if (this._host.Manager.Theme is DictionaryTheme)
    {
      this.currentThemeResourceDictionary = ((DictionaryTheme) this._host.Manager.Theme).ThemeResourceDictionary;
      this.Resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
    }
    else
      this.Resources.MergedDictionaries.Add(new ResourceDictionary()
      {
        Source = this._host.Manager.Theme.GetResourceUri()
      });
  }

  internal void EnableDropTargets()
  {
    if (this._mainCanvasPanel == null)
      return;
    this._mainCanvasPanel.Visibility = Visibility.Visible;
  }

  internal void HideDropTargets()
  {
    if (this._mainCanvasPanel == null)
      return;
    this._mainCanvasPanel.Visibility = Visibility.Hidden;
  }

  private void SetDropTargetIntoVisibility(ILayoutPositionableElement positionableElement)
  {
    if (positionableElement is LayoutAnchorablePane)
      this._anchorablePaneDropTargetInto.Visibility = Visibility.Visible;
    else if (positionableElement is LayoutDocumentPane)
      this._documentPaneDropTargetInto.Visibility = Visibility.Visible;
    if (positionableElement == null || this._floatingWindow.Model == null || positionableElement.AllowDuplicateContent)
      return;
    List<LayoutContent> allLayoutContents = this.GetAllLayoutContents((object) positionableElement);
    foreach (LayoutContent allLayoutContent in this.GetAllLayoutContents((object) this._floatingWindow.Model))
    {
      LayoutContent content = allLayoutContent;
      if (allLayoutContents.Any<LayoutContent>((Func<LayoutContent, bool>) (item => item.Title == content.Title && item.ContentId == content.ContentId)))
      {
        switch (positionableElement)
        {
          case LayoutAnchorablePane _:
            this._anchorablePaneDropTargetInto.Visibility = Visibility.Hidden;
            return;
          case LayoutDocumentPane _:
            this._documentPaneDropTargetInto.Visibility = Visibility.Hidden;
            return;
          default:
            return;
        }
      }
    }
  }

  private List<LayoutContent> GetAllLayoutContents(object source)
  {
    List<LayoutContent> allLayoutContents = new List<LayoutContent>();
    if (source is LayoutDocumentFloatingWindow documentFloatingWindow)
    {
      foreach (ILayoutElement child in documentFloatingWindow.Children)
        allLayoutContents.AddRange((IEnumerable<LayoutContent>) this.GetAllLayoutContents((object) child));
    }
    if (source is LayoutAnchorableFloatingWindow anchorableFloatingWindow)
    {
      foreach (ILayoutElement child in anchorableFloatingWindow.Children)
        allLayoutContents.AddRange((IEnumerable<LayoutContent>) this.GetAllLayoutContents((object) child));
    }
    if (source is LayoutDocumentPaneGroup documentPaneGroup)
    {
      foreach (ILayoutDocumentPane child in (Collection<ILayoutDocumentPane>) documentPaneGroup.Children)
        allLayoutContents.AddRange((IEnumerable<LayoutContent>) this.GetAllLayoutContents((object) child));
    }
    if (source is LayoutAnchorablePaneGroup anchorablePaneGroup)
    {
      foreach (ILayoutAnchorablePane child in (Collection<ILayoutAnchorablePane>) anchorablePaneGroup.Children)
        allLayoutContents.AddRange((IEnumerable<LayoutContent>) this.GetAllLayoutContents((object) child));
    }
    if (source is LayoutDocumentPane layoutDocumentPane)
    {
      foreach (LayoutContent child in (Collection<LayoutContent>) layoutDocumentPane.Children)
        allLayoutContents.Add(child);
    }
    if (source is LayoutAnchorablePane layoutAnchorablePane)
    {
      foreach (LayoutAnchorable child in (Collection<LayoutAnchorable>) layoutAnchorablePane.Children)
        allLayoutContents.Add((LayoutContent) child);
    }
    if (source is LayoutDocument layoutDocument)
      allLayoutContents.Add((LayoutContent) layoutDocument);
    if (source is LayoutAnchorable layoutAnchorable)
      allLayoutContents.Add((LayoutContent) layoutAnchorable);
    return allLayoutContents;
  }

  IEnumerable<IDropTarget> IOverlayWindow.GetTargets()
  {
    List<IDropArea>.Enumerator enumerator1 = this._visibleAreas.GetEnumerator();
    while (enumerator1.MoveNext())
    {
      IDropArea current1 = enumerator1.Current;
      Rect screenArea1;
      switch (current1.Type)
      {
        case DropAreaType.DockingManager:
          DropArea<DockingManager> dropAreaDockingManager = current1 as DropArea<DockingManager>;
          yield return (IDropTarget) new DockingManagerDropTarget(dropAreaDockingManager.AreaElement, this._dockingManagerDropTargetLeft.GetScreenArea(), DropTargetType.DockingManagerDockLeft);
          yield return (IDropTarget) new DockingManagerDropTarget(dropAreaDockingManager.AreaElement, this._dockingManagerDropTargetTop.GetScreenArea(), DropTargetType.DockingManagerDockTop);
          yield return (IDropTarget) new DockingManagerDropTarget(dropAreaDockingManager.AreaElement, this._dockingManagerDropTargetBottom.GetScreenArea(), DropTargetType.DockingManagerDockBottom);
          yield return (IDropTarget) new DockingManagerDropTarget(dropAreaDockingManager.AreaElement, this._dockingManagerDropTargetRight.GetScreenArea(), DropTargetType.DockingManagerDockRight);
          dropAreaDockingManager = (DropArea<DockingManager>) null;
          continue;
        case DropAreaType.DocumentPane:
          DropArea<LayoutDocumentPaneControl> dropAreaDocumentPane;
          LayoutDocumentPane parentPaneModel1;
          LayoutDocumentTabItem lastAreaTabItem1;
          IEnumerator<LayoutDocumentTabItem> enumerator2;
          if (this._floatingWindow.Model is LayoutAnchorableFloatingWindow && this._gridDocumentPaneFullDropTargets != null)
          {
            dropAreaDocumentPane = current1 as DropArea<LayoutDocumentPaneControl>;
            if (this._documentPaneFullDropTargetLeft.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneFullDropTargetLeft.GetScreenArea(), DropTargetType.DocumentPaneDockLeft);
            if (this._documentPaneFullDropTargetTop.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneFullDropTargetTop.GetScreenArea(), DropTargetType.DocumentPaneDockTop);
            if (this._documentPaneFullDropTargetRight.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneFullDropTargetRight.GetScreenArea(), DropTargetType.DocumentPaneDockRight);
            if (this._documentPaneFullDropTargetBottom.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneFullDropTargetBottom.GetScreenArea(), DropTargetType.DocumentPaneDockBottom);
            if (this._documentPaneFullDropTargetInto.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneFullDropTargetInto.GetScreenArea(), DropTargetType.DocumentPaneDockInside);
            parentPaneModel1 = dropAreaDocumentPane.AreaElement.Model as LayoutDocumentPane;
            lastAreaTabItem1 = (LayoutDocumentTabItem) null;
            enumerator2 = dropAreaDocumentPane.AreaElement.FindVisualChildren<LayoutDocumentTabItem>().GetEnumerator();
            while (enumerator2.MoveNext())
            {
              LayoutDocumentTabItem current2 = enumerator2.Current;
              LayoutContent model = current2.Model;
              LayoutDocumentTabItem layoutDocumentTabItem;
              if (lastAreaTabItem1 != null)
              {
                screenArea1 = lastAreaTabItem1.GetScreenArea();
                double right1 = screenArea1.Right;
                screenArea1 = current2.GetScreenArea();
                double right2 = screenArea1.Right;
                if (right1 >= right2)
                {
                  layoutDocumentTabItem = lastAreaTabItem1;
                  goto label_30;
                }
              }
              layoutDocumentTabItem = current2;
label_30:
              lastAreaTabItem1 = layoutDocumentTabItem;
              int tabIndex = parentPaneModel1.Children.IndexOf(model);
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, current2.GetScreenArea(), DropTargetType.DocumentPaneDockInside, tabIndex);
            }
            enumerator2 = (IEnumerator<LayoutDocumentTabItem>) null;
            if (lastAreaTabItem1 != null)
            {
              Rect screenArea2 = lastAreaTabItem1.GetScreenArea();
              Rect detectionRect = new Rect(screenArea2.TopRight, new Point(screenArea2.Right + screenArea2.Width, screenArea2.Bottom));
              double right3 = detectionRect.Right;
              screenArea1 = dropAreaDocumentPane.AreaElement.GetScreenArea();
              double right4 = screenArea1.Right;
              if (right3 < right4)
                yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, detectionRect, DropTargetType.DocumentPaneDockInside, parentPaneModel1.Children.Count);
            }
            if (this._documentPaneDropTargetLeftAsAnchorablePane.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropAsAnchorableTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetLeftAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableLeft);
            if (this._documentPaneDropTargetTopAsAnchorablePane.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropAsAnchorableTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetTopAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableTop);
            if (this._documentPaneDropTargetRightAsAnchorablePane.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropAsAnchorableTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetRightAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableRight);
            if (this._documentPaneDropTargetBottomAsAnchorablePane.IsVisible)
              yield return (IDropTarget) new DocumentPaneDropAsAnchorableTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetBottomAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableBottom);
            dropAreaDocumentPane = (DropArea<LayoutDocumentPaneControl>) null;
            parentPaneModel1 = (LayoutDocumentPane) null;
            lastAreaTabItem1 = (LayoutDocumentTabItem) null;
            continue;
          }
          dropAreaDocumentPane = current1 as DropArea<LayoutDocumentPaneControl>;
          if (this._documentPaneDropTargetLeft.IsVisible)
            yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetLeft.GetScreenArea(), DropTargetType.DocumentPaneDockLeft);
          if (this._documentPaneDropTargetTop.IsVisible)
            yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetTop.GetScreenArea(), DropTargetType.DocumentPaneDockTop);
          if (this._documentPaneDropTargetRight.IsVisible)
            yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetRight.GetScreenArea(), DropTargetType.DocumentPaneDockRight);
          if (this._documentPaneDropTargetBottom.IsVisible)
            yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetBottom.GetScreenArea(), DropTargetType.DocumentPaneDockBottom);
          if (this._documentPaneDropTargetInto.IsVisible)
            yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, this._documentPaneDropTargetInto.GetScreenArea(), DropTargetType.DocumentPaneDockInside);
          parentPaneModel1 = dropAreaDocumentPane.AreaElement.Model as LayoutDocumentPane;
          lastAreaTabItem1 = (LayoutDocumentTabItem) null;
          enumerator2 = dropAreaDocumentPane.AreaElement.FindVisualChildren<LayoutDocumentTabItem>().GetEnumerator();
          while (enumerator2.MoveNext())
          {
            LayoutDocumentTabItem current3 = enumerator2.Current;
            LayoutContent model = current3.Model;
            LayoutDocumentTabItem layoutDocumentTabItem;
            if (lastAreaTabItem1 != null)
            {
              screenArea1 = lastAreaTabItem1.GetScreenArea();
              double right5 = screenArea1.Right;
              screenArea1 = current3.GetScreenArea();
              double right6 = screenArea1.Right;
              if (right5 >= right6)
              {
                layoutDocumentTabItem = lastAreaTabItem1;
                goto label_59;
              }
            }
            layoutDocumentTabItem = current3;
label_59:
            lastAreaTabItem1 = layoutDocumentTabItem;
            int tabIndex = parentPaneModel1.Children.IndexOf(model);
            yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, current3.GetScreenArea(), DropTargetType.DocumentPaneDockInside, tabIndex);
          }
          enumerator2 = (IEnumerator<LayoutDocumentTabItem>) null;
          if (lastAreaTabItem1 != null)
          {
            Rect screenArea3 = lastAreaTabItem1.GetScreenArea();
            Rect detectionRect = new Rect(screenArea3.TopRight, new Point(screenArea3.Right + screenArea3.Width, screenArea3.Bottom));
            double right7 = detectionRect.Right;
            screenArea1 = dropAreaDocumentPane.AreaElement.GetScreenArea();
            double right8 = screenArea1.Right;
            if (right7 < right8)
              yield return (IDropTarget) new DocumentPaneDropTarget(dropAreaDocumentPane.AreaElement, detectionRect, DropTargetType.DocumentPaneDockInside, parentPaneModel1.Children.Count);
          }
          dropAreaDocumentPane = (DropArea<LayoutDocumentPaneControl>) null;
          parentPaneModel1 = (LayoutDocumentPane) null;
          lastAreaTabItem1 = (LayoutDocumentTabItem) null;
          continue;
        case DropAreaType.DocumentPaneGroup:
          DropArea<LayoutDocumentPaneGroupControl> dropArea = current1 as DropArea<LayoutDocumentPaneGroupControl>;
          if (this._documentPaneDropTargetInto.IsVisible)
          {
            yield return (IDropTarget) new DocumentPaneGroupDropTarget(dropArea.AreaElement, this._documentPaneDropTargetInto.GetScreenArea(), DropTargetType.DocumentPaneGroupDockInside);
            continue;
          }
          continue;
        case DropAreaType.AnchorablePane:
          DropArea<LayoutAnchorablePaneControl> dropAreaAnchorablePane = current1 as DropArea<LayoutAnchorablePaneControl>;
          yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, this._anchorablePaneDropTargetLeft.GetScreenArea(), DropTargetType.AnchorablePaneDockLeft);
          yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, this._anchorablePaneDropTargetTop.GetScreenArea(), DropTargetType.AnchorablePaneDockTop);
          yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, this._anchorablePaneDropTargetRight.GetScreenArea(), DropTargetType.AnchorablePaneDockRight);
          yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, this._anchorablePaneDropTargetBottom.GetScreenArea(), DropTargetType.AnchorablePaneDockBottom);
          if (this._anchorablePaneDropTargetInto.IsVisible)
            yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, this._anchorablePaneDropTargetInto.GetScreenArea(), DropTargetType.AnchorablePaneDockInside);
          LayoutAnchorablePane parentPaneModel2 = dropAreaAnchorablePane.AreaElement.Model as LayoutAnchorablePane;
          LayoutAnchorableTabItem lastAreaTabItem2 = (LayoutAnchorableTabItem) null;
          IEnumerator<LayoutAnchorableTabItem> enumerator3 = dropAreaAnchorablePane.AreaElement.FindVisualChildren<LayoutAnchorableTabItem>().GetEnumerator();
          while (enumerator3.MoveNext())
          {
            LayoutAnchorableTabItem current4 = enumerator3.Current;
            LayoutAnchorable model = current4.Model as LayoutAnchorable;
            lastAreaTabItem2 = lastAreaTabItem2 == null || lastAreaTabItem2.GetScreenArea().Right < current4.GetScreenArea().Right ? current4 : lastAreaTabItem2;
            int tabIndex = parentPaneModel2.Children.IndexOf(model);
            yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, current4.GetScreenArea(), DropTargetType.AnchorablePaneDockInside, tabIndex);
          }
          enumerator3 = (IEnumerator<LayoutAnchorableTabItem>) null;
          if (lastAreaTabItem2 != null)
          {
            Rect screenArea4 = lastAreaTabItem2.GetScreenArea();
            Rect detectionRect = new Rect(screenArea4.TopRight, new Point(screenArea4.Right + screenArea4.Width, screenArea4.Bottom));
            double right9 = detectionRect.Right;
            screenArea1 = dropAreaAnchorablePane.AreaElement.GetScreenArea();
            double right10 = screenArea1.Right;
            if (right9 < right10)
              yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, detectionRect, DropTargetType.AnchorablePaneDockInside, parentPaneModel2.Children.Count);
          }
          AnchorablePaneTitle element = dropAreaAnchorablePane.AreaElement.FindVisualChildren<AnchorablePaneTitle>().FirstOrDefault<AnchorablePaneTitle>();
          if (element != null)
            yield return (IDropTarget) new AnchorablePaneDropTarget(dropAreaAnchorablePane.AreaElement, element.GetScreenArea(), DropTargetType.AnchorablePaneDockInside);
          dropAreaAnchorablePane = (DropArea<LayoutAnchorablePaneControl>) null;
          parentPaneModel2 = (LayoutAnchorablePane) null;
          lastAreaTabItem2 = (LayoutAnchorableTabItem) null;
          continue;
        default:
          continue;
      }
    }
    enumerator1 = new List<IDropArea>.Enumerator();
  }

  void IOverlayWindow.DragEnter(LayoutFloatingWindowControl floatingWindow)
  {
    this._floatingWindow = floatingWindow;
    this.EnableDropTargets();
  }

  void IOverlayWindow.DragLeave(LayoutFloatingWindowControl floatingWindow)
  {
    this.Visibility = Visibility.Hidden;
    this._floatingWindow = (LayoutFloatingWindowControl) null;
  }

  void IOverlayWindow.DragEnter(IDropArea area)
  {
    DockingManager manager = this._floatingWindow.Model.Root.Manager;
    this._visibleAreas.Add(area);
    FrameworkElement element1;
    switch (area.Type)
    {
      case DropAreaType.DockingManager:
        if ((area as DropArea<DockingManager>).AreaElement != manager)
        {
          this._visibleAreas.Remove(area);
          return;
        }
        element1 = (FrameworkElement) this._gridDockingManagerDropTargets;
        break;
      case DropAreaType.DocumentPaneGroup:
        element1 = (FrameworkElement) this._gridDocumentPaneDropTargets;
        if (((((area as DropArea<LayoutDocumentPaneGroupControl>).AreaElement.Model as LayoutDocumentPaneGroup).Children.First<ILayoutDocumentPane>() as LayoutDocumentPane).Parent as LayoutDocumentPaneGroup).Root.Manager != manager)
        {
          this._visibleAreas.Remove(area);
          return;
        }
        this._documentPaneDropTargetLeft.Visibility = Visibility.Hidden;
        this._documentPaneDropTargetRight.Visibility = Visibility.Hidden;
        this._documentPaneDropTargetTop.Visibility = Visibility.Hidden;
        this._documentPaneDropTargetBottom.Visibility = Visibility.Hidden;
        break;
      case DropAreaType.AnchorablePane:
        element1 = (FrameworkElement) this._gridAnchorablePaneDropTargets;
        LayoutAnchorablePane model1 = (area as DropArea<LayoutAnchorablePaneControl>).AreaElement.Model as LayoutAnchorablePane;
        if (model1.Root.Manager != manager)
        {
          this._visibleAreas.Remove(area);
          return;
        }
        this.SetDropTargetIntoVisibility((ILayoutPositionableElement) model1);
        break;
      default:
        if (this._floatingWindow.Model is LayoutAnchorableFloatingWindow && this._gridDocumentPaneFullDropTargets != null)
        {
          element1 = (FrameworkElement) this._gridDocumentPaneFullDropTargets;
          LayoutDocumentPane model2 = (area as DropArea<LayoutDocumentPaneControl>).AreaElement.Model as LayoutDocumentPane;
          LayoutDocumentPaneGroup parent = model2.Parent as LayoutDocumentPaneGroup;
          if (model2.Root.Manager != manager)
          {
            this._visibleAreas.Remove(area);
            return;
          }
          this.SetDropTargetIntoVisibility((ILayoutPositionableElement) model2);
          if (parent != null && parent.Children.Where<ILayoutDocumentPane>((Func<ILayoutDocumentPane, bool>) (c => c.IsVisible)).Count<ILayoutDocumentPane>() > 1)
          {
            if (!parent.Root.Manager.AllowMixedOrientation)
            {
              this._documentPaneFullDropTargetLeft.Visibility = parent.Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Hidden;
              this._documentPaneFullDropTargetRight.Visibility = parent.Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Hidden;
              this._documentPaneFullDropTargetTop.Visibility = parent.Orientation == Orientation.Vertical ? Visibility.Visible : Visibility.Hidden;
              this._documentPaneFullDropTargetBottom.Visibility = parent.Orientation == Orientation.Vertical ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
              this._documentPaneFullDropTargetLeft.Visibility = Visibility.Visible;
              this._documentPaneFullDropTargetRight.Visibility = Visibility.Visible;
              this._documentPaneFullDropTargetTop.Visibility = Visibility.Visible;
              this._documentPaneFullDropTargetBottom.Visibility = Visibility.Visible;
            }
          }
          else if (parent == null && model2 != null && model2.ChildrenCount == 0)
          {
            this._documentPaneFullDropTargetLeft.Visibility = Visibility.Hidden;
            this._documentPaneFullDropTargetRight.Visibility = Visibility.Hidden;
            this._documentPaneFullDropTargetTop.Visibility = Visibility.Hidden;
            this._documentPaneFullDropTargetBottom.Visibility = Visibility.Hidden;
          }
          else
          {
            this._documentPaneFullDropTargetLeft.Visibility = Visibility.Visible;
            this._documentPaneFullDropTargetRight.Visibility = Visibility.Visible;
            this._documentPaneFullDropTargetTop.Visibility = Visibility.Visible;
            this._documentPaneFullDropTargetBottom.Visibility = Visibility.Visible;
          }
          if (parent != null && parent.Children.Where<ILayoutDocumentPane>((Func<ILayoutDocumentPane, bool>) (c => c.IsVisible)).Count<ILayoutDocumentPane>() > 1)
          {
            int num = parent.Children.Where<ILayoutDocumentPane>((Func<ILayoutDocumentPane, bool>) (ch => ch.IsVisible)).ToList<ILayoutDocumentPane>().IndexOf((ILayoutDocumentPane) model2);
            bool flag1 = num == 0;
            bool flag2 = num == parent.ChildrenCount - 1;
            if (!parent.Root.Manager.AllowMixedOrientation)
            {
              this._documentPaneDropTargetBottomAsAnchorablePane.Visibility = parent.Orientation == Orientation.Vertical ? (flag2 ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden;
              this._documentPaneDropTargetTopAsAnchorablePane.Visibility = parent.Orientation == Orientation.Vertical ? (flag1 ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden;
              this._documentPaneDropTargetLeftAsAnchorablePane.Visibility = parent.Orientation == Orientation.Horizontal ? (flag1 ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden;
              this._documentPaneDropTargetRightAsAnchorablePane.Visibility = parent.Orientation == Orientation.Horizontal ? (flag2 ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden;
              break;
            }
            this._documentPaneDropTargetBottomAsAnchorablePane.Visibility = Visibility.Visible;
            this._documentPaneDropTargetLeftAsAnchorablePane.Visibility = Visibility.Visible;
            this._documentPaneDropTargetRightAsAnchorablePane.Visibility = Visibility.Visible;
            this._documentPaneDropTargetTopAsAnchorablePane.Visibility = Visibility.Visible;
            break;
          }
          this._documentPaneDropTargetBottomAsAnchorablePane.Visibility = Visibility.Visible;
          this._documentPaneDropTargetLeftAsAnchorablePane.Visibility = Visibility.Visible;
          this._documentPaneDropTargetRightAsAnchorablePane.Visibility = Visibility.Visible;
          this._documentPaneDropTargetTopAsAnchorablePane.Visibility = Visibility.Visible;
          break;
        }
        element1 = (FrameworkElement) this._gridDocumentPaneDropTargets;
        LayoutDocumentPane model3 = (area as DropArea<LayoutDocumentPaneControl>).AreaElement.Model as LayoutDocumentPane;
        LayoutDocumentPaneGroup parent1 = model3.Parent as LayoutDocumentPaneGroup;
        if (model3.Root.Manager != manager)
        {
          this._visibleAreas.Remove(area);
          return;
        }
        this.SetDropTargetIntoVisibility((ILayoutPositionableElement) model3);
        if (parent1 != null && parent1.Children.Where<ILayoutDocumentPane>((Func<ILayoutDocumentPane, bool>) (c => c.IsVisible)).Count<ILayoutDocumentPane>() > 1)
        {
          if (!parent1.Root.Manager.AllowMixedOrientation)
          {
            this._documentPaneDropTargetLeft.Visibility = parent1.Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this._documentPaneDropTargetRight.Visibility = parent1.Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this._documentPaneDropTargetTop.Visibility = parent1.Orientation == Orientation.Vertical ? Visibility.Visible : Visibility.Hidden;
            this._documentPaneDropTargetBottom.Visibility = parent1.Orientation == Orientation.Vertical ? Visibility.Visible : Visibility.Hidden;
            break;
          }
          this._documentPaneDropTargetLeft.Visibility = Visibility.Visible;
          this._documentPaneDropTargetRight.Visibility = Visibility.Visible;
          this._documentPaneDropTargetTop.Visibility = Visibility.Visible;
          this._documentPaneDropTargetBottom.Visibility = Visibility.Visible;
          break;
        }
        if (parent1 == null && model3 != null && model3.ChildrenCount == 0)
        {
          this._documentPaneDropTargetLeft.Visibility = Visibility.Hidden;
          this._documentPaneDropTargetRight.Visibility = Visibility.Hidden;
          this._documentPaneDropTargetTop.Visibility = Visibility.Hidden;
          this._documentPaneDropTargetBottom.Visibility = Visibility.Hidden;
          break;
        }
        this._documentPaneDropTargetLeft.Visibility = Visibility.Visible;
        this._documentPaneDropTargetRight.Visibility = Visibility.Visible;
        this._documentPaneDropTargetTop.Visibility = Visibility.Visible;
        this._documentPaneDropTargetBottom.Visibility = Visibility.Visible;
        break;
    }
    Canvas.SetLeft((UIElement) element1, area.DetectionRect.Left - this.Left);
    FrameworkElement element2 = element1;
    Rect detectionRect = area.DetectionRect;
    double length = detectionRect.Top - this.Top;
    Canvas.SetTop((UIElement) element2, length);
    FrameworkElement frameworkElement1 = element1;
    detectionRect = area.DetectionRect;
    double width = detectionRect.Width;
    frameworkElement1.Width = width;
    FrameworkElement frameworkElement2 = element1;
    detectionRect = area.DetectionRect;
    double height = detectionRect.Height;
    frameworkElement2.Height = height;
    element1.Visibility = Visibility.Visible;
  }

  void IOverlayWindow.DragLeave(IDropArea area)
  {
    this._visibleAreas.Remove(area);
    FrameworkElement frameworkElement;
    switch (area.Type)
    {
      case DropAreaType.DockingManager:
        frameworkElement = (FrameworkElement) this._gridDockingManagerDropTargets;
        break;
      case DropAreaType.DocumentPaneGroup:
        frameworkElement = (FrameworkElement) this._gridDocumentPaneDropTargets;
        break;
      case DropAreaType.AnchorablePane:
        frameworkElement = (FrameworkElement) this._gridAnchorablePaneDropTargets;
        break;
      default:
        frameworkElement = !(this._floatingWindow.Model is LayoutAnchorableFloatingWindow) || this._gridDocumentPaneFullDropTargets == null ? (FrameworkElement) this._gridDocumentPaneDropTargets : (FrameworkElement) this._gridDocumentPaneFullDropTargets;
        break;
    }
    frameworkElement.Visibility = Visibility.Hidden;
  }

  void IOverlayWindow.DragEnter(IDropTarget target)
  {
    Geometry previewPath = target.GetPreviewPath(this, this._floatingWindow.Model as LayoutFloatingWindow);
    if (previewPath == null)
      return;
    this._previewBox.Data = previewPath;
    this._previewBox.Visibility = Visibility.Visible;
  }

  void IOverlayWindow.DragLeave(IDropTarget target)
  {
    this._previewBox.Visibility = Visibility.Hidden;
  }

  void IOverlayWindow.DragDrop(IDropTarget target)
  {
    target.Drop(this._floatingWindow.Model as LayoutFloatingWindow);
  }
}
