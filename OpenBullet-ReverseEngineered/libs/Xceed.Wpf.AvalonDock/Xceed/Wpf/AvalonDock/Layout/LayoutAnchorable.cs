// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public class LayoutAnchorable : LayoutContent
{
  private double _autohideWidth;
  private double _autohideMinWidth = 100.0;
  private double _autohideHeight;
  private double _autohideMinHeight = 100.0;
  private bool _canHide = true;
  private bool _canAutoHide = true;
  private bool _canDockAsTabbedDocument = true;
  private bool _canCloseValueBeforeInternalSet;

  public LayoutAnchorable() => this._canClose = false;

  public double AutoHideWidth
  {
    get => this._autohideWidth;
    set
    {
      if (this._autohideWidth == value)
        return;
      this.RaisePropertyChanging(nameof (AutoHideWidth));
      value = Math.Max(value, this._autohideMinWidth);
      this._autohideWidth = value;
      this.RaisePropertyChanged(nameof (AutoHideWidth));
    }
  }

  public double AutoHideMinWidth
  {
    get => this._autohideMinWidth;
    set
    {
      if (this._autohideMinWidth == value)
        return;
      this.RaisePropertyChanging(nameof (AutoHideMinWidth));
      this._autohideMinWidth = value >= 0.0 ? value : throw new ArgumentException(nameof (value));
      this.RaisePropertyChanged(nameof (AutoHideMinWidth));
    }
  }

  public double AutoHideHeight
  {
    get => this._autohideHeight;
    set
    {
      if (this._autohideHeight == value)
        return;
      this.RaisePropertyChanging(nameof (AutoHideHeight));
      value = Math.Max(value, this._autohideMinHeight);
      this._autohideHeight = value;
      this.RaisePropertyChanged(nameof (AutoHideHeight));
    }
  }

  public double AutoHideMinHeight
  {
    get => this._autohideMinHeight;
    set
    {
      if (this._autohideMinHeight == value)
        return;
      this.RaisePropertyChanging(nameof (AutoHideMinHeight));
      this._autohideMinHeight = value >= 0.0 ? value : throw new ArgumentException(nameof (value));
      this.RaisePropertyChanged(nameof (AutoHideMinHeight));
    }
  }

  public bool CanHide
  {
    get => this._canHide;
    set
    {
      if (this._canHide == value)
        return;
      this._canHide = value;
      this.RaisePropertyChanged(nameof (CanHide));
    }
  }

  public bool CanAutoHide
  {
    get => this._canAutoHide;
    set
    {
      if (this._canAutoHide == value)
        return;
      this._canAutoHide = value;
      this.RaisePropertyChanged(nameof (CanAutoHide));
    }
  }

  public bool CanDockAsTabbedDocument
  {
    get => this._canDockAsTabbedDocument;
    set
    {
      if (this._canDockAsTabbedDocument == value)
        return;
      this._canDockAsTabbedDocument = value;
      this.RaisePropertyChanged(nameof (CanDockAsTabbedDocument));
    }
  }

  public bool IsAutoHidden => this.Parent != null && this.Parent is LayoutAnchorGroup;

  [XmlIgnore]
  public bool IsHidden => this.Parent is LayoutRoot;

  [XmlIgnore]
  public bool IsVisible
  {
    get => this.Parent != null && !(this.Parent is LayoutRoot);
    set
    {
      if (value)
        this.Show();
      else
        this.Hide();
    }
  }

  public event EventHandler IsVisibleChanged;

  public event EventHandler<CancelEventArgs> Hiding;

  protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
    this.UpdateParentVisibility();
    this.RaisePropertyChanged("IsVisible");
    this.NotifyIsVisibleChanged();
    this.RaisePropertyChanged("IsHidden");
    this.RaisePropertyChanged("IsAutoHidden");
    base.OnParentChanged(oldValue, newValue);
  }

  protected override void InternalDock()
  {
    LayoutRoot root = this.Root as LayoutRoot;
    LayoutAnchorablePane destinationContainer = (LayoutAnchorablePane) null;
    if (root.ActiveContent != null && root.ActiveContent != this)
      destinationContainer = root.ActiveContent.Parent as LayoutAnchorablePane;
    if (destinationContainer == null)
      destinationContainer = root.Descendents().OfType<LayoutAnchorablePane>().Where<LayoutAnchorablePane>((Func<LayoutAnchorablePane, bool>) (pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right)).FirstOrDefault<LayoutAnchorablePane>();
    if (destinationContainer == null)
      destinationContainer = root.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
    bool flag = false;
    if (root.Manager.LayoutUpdateStrategy != null)
      flag = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root, this, (ILayoutContainer) destinationContainer);
    if (!flag)
    {
      if (destinationContainer == null)
      {
        LayoutPanel layoutPanel = new LayoutPanel()
        {
          Orientation = Orientation.Horizontal
        };
        if (root.RootPanel != null)
          layoutPanel.Children.Add((ILayoutPanelElement) root.RootPanel);
        root.RootPanel = layoutPanel;
        LayoutAnchorablePane layoutAnchorablePane = new LayoutAnchorablePane();
        layoutAnchorablePane.DockWidth = new GridLength(200.0, GridUnitType.Pixel);
        destinationContainer = layoutAnchorablePane;
        layoutPanel.Children.Add((ILayoutPanelElement) destinationContainer);
      }
      destinationContainer.Children.Add(this);
    }
    if (root.Manager.LayoutUpdateStrategy != null)
      root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root, this);
    base.InternalDock();
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("CanHide"))
      this.CanHide = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("CanAutoHide"))
      this.CanAutoHide = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("AutoHideWidth"))
      this.AutoHideWidth = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("AutoHideHeight"))
      this.AutoHideHeight = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("AutoHideMinWidth"))
      this.AutoHideMinWidth = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("AutoHideMinHeight"))
      this.AutoHideMinHeight = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("CanDockAsTabbedDocument"))
      this.CanDockAsTabbedDocument = bool.Parse(reader.Value);
    base.ReadXml(reader);
  }

  public override void WriteXml(XmlWriter writer)
  {
    if (!this.CanHide)
      writer.WriteAttributeString("CanHide", this.CanHide.ToString());
    if (!this.CanAutoHide)
      writer.WriteAttributeString("CanAutoHide", this.CanAutoHide.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.AutoHideWidth > 0.0)
      writer.WriteAttributeString("AutoHideWidth", this.AutoHideWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.AutoHideHeight > 0.0)
      writer.WriteAttributeString("AutoHideHeight", this.AutoHideHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.AutoHideMinWidth != 25.0)
      writer.WriteAttributeString("AutoHideMinWidth", this.AutoHideMinWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.AutoHideMinHeight != 25.0)
      writer.WriteAttributeString("AutoHideMinHeight", this.AutoHideMinHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (!this.CanDockAsTabbedDocument)
      writer.WriteAttributeString("CanDockAsTabbedDocument", this.CanDockAsTabbedDocument.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    base.WriteXml(writer);
  }

  public override void Close() => this.CloseAnchorable();

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("Anchorable()");
  }

  public void Hide(bool cancelable = true)
  {
    if (!this.IsVisible)
    {
      this.IsSelected = true;
      this.IsActive = true;
    }
    else
    {
      if (cancelable)
      {
        CancelEventArgs args = new CancelEventArgs();
        this.OnHiding(args);
        if (args.Cancel)
          return;
      }
      this.RaisePropertyChanging("IsHidden");
      this.RaisePropertyChanging("IsVisible");
      ILayoutGroup parent = this.Parent as ILayoutGroup;
      this.PreviousContainer = (ILayoutContainer) parent;
      this.PreviousContainerIndex = parent.IndexOfChild((ILayoutElement) this);
      this.Root.Hidden.Add(this);
      this.RaisePropertyChanged("IsVisible");
      this.RaisePropertyChanged("IsHidden");
      this.NotifyIsVisibleChanged();
    }
  }

  public void Show()
  {
    if (this.IsVisible)
      return;
    if (!this.IsHidden)
      throw new InvalidOperationException();
    this.RaisePropertyChanging("IsHidden");
    this.RaisePropertyChanging("IsVisible");
    bool flag = false;
    ILayoutRoot root = this.Root;
    if (root != null && root.Manager != null && root.Manager.LayoutUpdateStrategy != null)
      flag = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root as LayoutRoot, this, this.PreviousContainer);
    if (!flag && this.PreviousContainer != null)
    {
      ILayoutGroup previousContainer = this.PreviousContainer as ILayoutGroup;
      if (this.PreviousContainerIndex < previousContainer.ChildrenCount)
        previousContainer.InsertChildAt(this.PreviousContainerIndex, (ILayoutElement) this);
      else
        previousContainer.InsertChildAt(previousContainer.ChildrenCount, (ILayoutElement) this);
      this.IsSelected = true;
      this.IsActive = true;
    }
    if (root != null && root.Manager != null && root.Manager.LayoutUpdateStrategy != null)
      root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root as LayoutRoot, this);
    this.PreviousContainer = (ILayoutContainer) null;
    this.PreviousContainerIndex = -1;
    this.RaisePropertyChanged("IsVisible");
    this.RaisePropertyChanged("IsHidden");
    this.NotifyIsVisibleChanged();
  }

  public void AddToLayout(DockingManager manager, AnchorableShowStrategy strategy)
  {
    if (this.IsVisible || this.IsHidden)
      throw new InvalidOperationException();
    bool flag1 = (strategy & AnchorableShowStrategy.Most) == AnchorableShowStrategy.Most;
    bool flag2 = (strategy & AnchorableShowStrategy.Left) == AnchorableShowStrategy.Left;
    bool flag3 = (strategy & AnchorableShowStrategy.Right) == AnchorableShowStrategy.Right;
    bool flag4 = (strategy & AnchorableShowStrategy.Top) == AnchorableShowStrategy.Top;
    bool flag5 = (strategy & AnchorableShowStrategy.Bottom) == AnchorableShowStrategy.Bottom;
    if (!flag1)
    {
      AnchorSide side = AnchorSide.Left;
      if (flag2)
        side = AnchorSide.Left;
      if (flag3)
        side = AnchorSide.Right;
      if (flag4)
        side = AnchorSide.Top;
      if (flag5)
        side = AnchorSide.Bottom;
      LayoutAnchorablePane layoutAnchorablePane = manager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>((Func<LayoutAnchorablePane, bool>) (p => p.GetSide() == side));
      if (layoutAnchorablePane != null)
        layoutAnchorablePane.Children.Add(this);
      else
        flag1 = true;
    }
    if (!flag1)
      return;
    if (manager.Layout.RootPanel == null)
      manager.Layout.RootPanel = new LayoutPanel()
      {
        Orientation = flag2 | flag3 ? Orientation.Horizontal : Orientation.Vertical
      };
    if (flag2 | flag3)
    {
      if (manager.Layout.RootPanel.Orientation == Orientation.Vertical && manager.Layout.RootPanel.ChildrenCount > 1)
        manager.Layout.RootPanel = new LayoutPanel((ILayoutPanelElement) manager.Layout.RootPanel);
      manager.Layout.RootPanel.Orientation = Orientation.Horizontal;
      if (flag2)
        manager.Layout.RootPanel.Children.Insert(0, (ILayoutPanelElement) new LayoutAnchorablePane(this));
      else
        manager.Layout.RootPanel.Children.Add((ILayoutPanelElement) new LayoutAnchorablePane(this));
    }
    else
    {
      if (manager.Layout.RootPanel.Orientation == Orientation.Horizontal && manager.Layout.RootPanel.ChildrenCount > 1)
        manager.Layout.RootPanel = new LayoutPanel((ILayoutPanelElement) manager.Layout.RootPanel);
      manager.Layout.RootPanel.Orientation = Orientation.Vertical;
      if (flag4)
        manager.Layout.RootPanel.Children.Insert(0, (ILayoutPanelElement) new LayoutAnchorablePane(this));
      else
        manager.Layout.RootPanel.Children.Add((ILayoutPanelElement) new LayoutAnchorablePane(this));
    }
  }

  public void ToggleAutoHide()
  {
    if (this.IsAutoHidden)
    {
      LayoutAnchorGroup parentGroup = this.Parent as LayoutAnchorGroup;
      LayoutAnchorSide parent1 = parentGroup.Parent as LayoutAnchorSide;
      if (!(((ILayoutPreviousContainer) parentGroup).PreviousContainer is LayoutAnchorablePane layoutAnchorablePane))
      {
        switch ((parentGroup.Parent as LayoutAnchorSide).Side)
        {
          case AnchorSide.Left:
            if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
            {
              layoutAnchorablePane = new LayoutAnchorablePane();
              layoutAnchorablePane.DockMinWidth = this.AutoHideMinWidth;
              parentGroup.Root.RootPanel.Children.Insert(0, (ILayoutPanelElement) layoutAnchorablePane);
              break;
            }
            layoutAnchorablePane = new LayoutAnchorablePane();
            LayoutPanel layoutPanel1 = new LayoutPanel()
            {
              Orientation = Orientation.Horizontal
            };
            LayoutRoot root1 = parentGroup.Root as LayoutRoot;
            LayoutPanel rootPanel1 = parentGroup.Root.RootPanel;
            LayoutPanel layoutPanel2 = layoutPanel1;
            root1.RootPanel = layoutPanel2;
            layoutPanel1.Children.Add((ILayoutPanelElement) layoutAnchorablePane);
            layoutPanel1.Children.Add((ILayoutPanelElement) rootPanel1);
            break;
          case AnchorSide.Top:
            if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
            {
              layoutAnchorablePane = new LayoutAnchorablePane();
              layoutAnchorablePane.DockMinHeight = this.AutoHideMinHeight;
              parentGroup.Root.RootPanel.Children.Insert(0, (ILayoutPanelElement) layoutAnchorablePane);
              break;
            }
            layoutAnchorablePane = new LayoutAnchorablePane();
            LayoutPanel layoutPanel3 = new LayoutPanel()
            {
              Orientation = Orientation.Vertical
            };
            LayoutRoot root2 = parentGroup.Root as LayoutRoot;
            LayoutPanel rootPanel2 = parentGroup.Root.RootPanel;
            LayoutPanel layoutPanel4 = layoutPanel3;
            root2.RootPanel = layoutPanel4;
            layoutPanel3.Children.Add((ILayoutPanelElement) layoutAnchorablePane);
            layoutPanel3.Children.Add((ILayoutPanelElement) rootPanel2);
            break;
          case AnchorSide.Right:
            if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
            {
              layoutAnchorablePane = new LayoutAnchorablePane();
              layoutAnchorablePane.DockMinWidth = this.AutoHideMinWidth;
              parentGroup.Root.RootPanel.Children.Add((ILayoutPanelElement) layoutAnchorablePane);
              break;
            }
            layoutAnchorablePane = new LayoutAnchorablePane();
            LayoutPanel layoutPanel5 = new LayoutPanel()
            {
              Orientation = Orientation.Horizontal
            };
            LayoutRoot root3 = parentGroup.Root as LayoutRoot;
            LayoutPanel rootPanel3 = parentGroup.Root.RootPanel;
            LayoutPanel layoutPanel6 = layoutPanel5;
            root3.RootPanel = layoutPanel6;
            layoutPanel5.Children.Add((ILayoutPanelElement) rootPanel3);
            layoutPanel5.Children.Add((ILayoutPanelElement) layoutAnchorablePane);
            break;
          case AnchorSide.Bottom:
            if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
            {
              layoutAnchorablePane = new LayoutAnchorablePane();
              layoutAnchorablePane.DockMinHeight = this.AutoHideMinHeight;
              parentGroup.Root.RootPanel.Children.Add((ILayoutPanelElement) layoutAnchorablePane);
              break;
            }
            layoutAnchorablePane = new LayoutAnchorablePane();
            LayoutPanel layoutPanel7 = new LayoutPanel()
            {
              Orientation = Orientation.Vertical
            };
            LayoutRoot root4 = parentGroup.Root as LayoutRoot;
            LayoutPanel rootPanel4 = parentGroup.Root.RootPanel;
            LayoutPanel layoutPanel8 = layoutPanel7;
            root4.RootPanel = layoutPanel8;
            layoutPanel7.Children.Add((ILayoutPanelElement) rootPanel4);
            layoutPanel7.Children.Add((ILayoutPanelElement) layoutAnchorablePane);
            break;
        }
      }
      else
      {
        foreach (ILayoutPreviousContainer previousContainer in (parentGroup.Root as LayoutRoot).Descendents().OfType<ILayoutPreviousContainer>().Where<ILayoutPreviousContainer>((Func<ILayoutPreviousContainer, bool>) (c => c.PreviousContainer == parentGroup)))
          previousContainer.PreviousContainer = (ILayoutContainer) layoutAnchorablePane;
      }
      foreach (LayoutAnchorable layoutAnchorable in parentGroup.Children.ToArray<LayoutAnchorable>())
        layoutAnchorablePane.Children.Add(layoutAnchorable);
      parent1.Children.Remove(parentGroup);
      for (LayoutGroupBase parent2 = layoutAnchorablePane.Parent as LayoutGroupBase; parent2 != null; parent2 = parent2.Parent as LayoutGroupBase)
      {
        if (parent2 is LayoutGroup<ILayoutPanelElement>)
          ((LayoutGroup<ILayoutPanelElement>) parent2).ComputeVisibility();
      }
    }
    else
    {
      if (!(this.Parent is LayoutAnchorablePane))
        return;
      ILayoutRoot root = this.Root;
      LayoutAnchorablePane parent = this.Parent as LayoutAnchorablePane;
      LayoutAnchorGroup layoutAnchorGroup = new LayoutAnchorGroup();
      ((ILayoutPreviousContainer) layoutAnchorGroup).PreviousContainer = (ILayoutContainer) parent;
      foreach (LayoutAnchorable layoutAnchorable in parent.Children.ToArray<LayoutAnchorable>())
        layoutAnchorGroup.Children.Add(layoutAnchorable);
      switch (parent.GetSide())
      {
        case AnchorSide.Left:
          if (root.LeftSide == null)
            break;
          root.LeftSide.Children.Add(layoutAnchorGroup);
          break;
        case AnchorSide.Top:
          if (root.TopSide == null)
            break;
          root.TopSide.Children.Add(layoutAnchorGroup);
          break;
        case AnchorSide.Right:
          if (root.RightSide == null)
            break;
          root.RightSide.Children.Add(layoutAnchorGroup);
          break;
        case AnchorSide.Bottom:
          if (root.BottomSide == null)
            break;
          root.BottomSide.Children.Add(layoutAnchorGroup);
          break;
      }
    }
  }

  protected virtual void OnHiding(CancelEventArgs args)
  {
    if (this.Hiding == null)
      return;
    this.Hiding((object) this, args);
  }

  internal void CloseAnchorable()
  {
    if (!this.TestCanClose())
      return;
    if (this.IsAutoHidden)
      this.ToggleAutoHide();
    this.CloseInternal();
  }

  internal void SetCanCloseInternal(bool canClose)
  {
    this._canCloseValueBeforeInternalSet = this._canClose;
    this._canClose = canClose;
  }

  internal void ResetCanCloseInternal() => this._canClose = this._canCloseValueBeforeInternalSet;

  private void NotifyIsVisibleChanged()
  {
    if (this.IsVisibleChanged == null)
      return;
    this.IsVisibleChanged((object) this, EventArgs.Empty);
  }

  private void UpdateParentVisibility()
  {
    if (!(this.Parent is ILayoutElementWithVisibility parent))
      return;
    parent.ComputeVisibility();
  }
}
