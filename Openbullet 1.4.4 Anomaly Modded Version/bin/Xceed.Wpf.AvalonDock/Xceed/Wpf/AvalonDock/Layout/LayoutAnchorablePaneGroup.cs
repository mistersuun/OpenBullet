// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutAnchorablePaneGroup
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Children")]
[Serializable]
public class LayoutAnchorablePaneGroup : 
  LayoutPositionableGroup<ILayoutAnchorablePane>,
  ILayoutAnchorablePane,
  ILayoutPanelElement,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutPane,
  ILayoutContainer,
  ILayoutElementWithVisibility,
  ILayoutOrientableGroup,
  ILayoutGroup
{
  private Orientation _orientation;

  public LayoutAnchorablePaneGroup()
  {
  }

  public LayoutAnchorablePaneGroup(LayoutAnchorablePane firstChild)
  {
    this.Children.Add((ILayoutAnchorablePane) firstChild);
  }

  public Orientation Orientation
  {
    get => this._orientation;
    set
    {
      if (this._orientation == value)
        return;
      this.RaisePropertyChanging(nameof (Orientation));
      this._orientation = value;
      this.RaisePropertyChanged(nameof (Orientation));
    }
  }

  protected override bool GetVisibility()
  {
    return this.Children.Count > 0 && this.Children.Any<ILayoutAnchorablePane>((Func<ILayoutAnchorablePane, bool>) (c => c.IsVisible));
  }

  protected override void OnIsVisibleChanged()
  {
    this.UpdateParentVisibility();
    base.OnIsVisibleChanged();
  }

  protected override void OnDockWidthChanged()
  {
    if (this.DockWidth.IsAbsolute && this.ChildrenCount == 1)
      ((ILayoutPositionableElement) this.Children[0]).DockWidth = this.DockWidth;
    base.OnDockWidthChanged();
  }

  protected override void OnDockHeightChanged()
  {
    if (this.DockHeight.IsAbsolute && this.ChildrenCount == 1)
      ((ILayoutPositionableElement) this.Children[0]).DockHeight = this.DockHeight;
    base.OnDockHeightChanged();
  }

  protected override void OnChildrenCollectionChanged()
  {
    if (this.DockWidth.IsAbsolute && this.ChildrenCount == 1)
      ((ILayoutPositionableElement) this.Children[0]).DockWidth = this.DockWidth;
    if (this.DockHeight.IsAbsolute && this.ChildrenCount == 1)
      ((ILayoutPositionableElement) this.Children[0]).DockHeight = this.DockHeight;
    base.OnChildrenCollectionChanged();
  }

  public override void WriteXml(XmlWriter writer)
  {
    writer.WriteAttributeString("Orientation", this.Orientation.ToString());
    base.WriteXml(writer);
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Orientation"))
      this.Orientation = (Orientation) Enum.Parse(typeof (Orientation), reader.Value, true);
    base.ReadXml(reader);
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine($"AnchorablePaneGroup({this.Orientation})");
    foreach (LayoutElement child in (Collection<ILayoutAnchorablePane>) this.Children)
      child.ConsoleDump(tab + 1);
  }

  private void UpdateParentVisibility()
  {
    if (!(this.Parent is ILayoutElementWithVisibility parent))
      return;
    parent.ComputeVisibility();
  }
}
