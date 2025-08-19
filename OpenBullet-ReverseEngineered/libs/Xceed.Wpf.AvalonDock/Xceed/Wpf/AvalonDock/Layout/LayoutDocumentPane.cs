// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutDocumentPane
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Children")]
[Serializable]
public class LayoutDocumentPane : 
  LayoutPositionableGroup<LayoutContent>,
  ILayoutDocumentPane,
  ILayoutPanelElement,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutPane,
  ILayoutContainer,
  ILayoutElementWithVisibility,
  ILayoutPositionableElement,
  ILayoutElementForFloatingWindow,
  ILayoutContentSelector,
  ILayoutPaneSerializable
{
  private bool _showHeader = true;
  private int _selectedIndex = -1;
  private string _id;

  public LayoutDocumentPane()
  {
  }

  public LayoutDocumentPane(LayoutContent firstChild) => this.Children.Add(firstChild);

  public bool ShowHeader
  {
    get => this._showHeader;
    set
    {
      if (value == this._showHeader)
        return;
      this._showHeader = value;
      this.RaisePropertyChanged(nameof (ShowHeader));
    }
  }

  public int SelectedContentIndex
  {
    get => this._selectedIndex;
    set
    {
      if (value < 0 || value >= this.Children.Count)
        value = -1;
      if (this._selectedIndex == value)
        return;
      this.RaisePropertyChanging(nameof (SelectedContentIndex));
      this.RaisePropertyChanging("SelectedContent");
      if (this._selectedIndex >= 0 && this._selectedIndex < this.Children.Count)
        this.Children[this._selectedIndex].IsSelected = false;
      this._selectedIndex = value;
      if (this._selectedIndex >= 0 && this._selectedIndex < this.Children.Count)
        this.Children[this._selectedIndex].IsSelected = true;
      this.RaisePropertyChanged(nameof (SelectedContentIndex));
      this.RaisePropertyChanged("SelectedContent");
    }
  }

  public LayoutContent SelectedContent
  {
    get => this._selectedIndex != -1 ? this.Children[this._selectedIndex] : (LayoutContent) null;
  }

  public IEnumerable<LayoutContent> ChildrenSorted
  {
    get
    {
      List<LayoutContent> list = this.Children.ToList<LayoutContent>();
      list.Sort();
      return (IEnumerable<LayoutContent>) list;
    }
  }

  protected override bool GetVisibility()
  {
    if (!(this.Parent is LayoutDocumentPaneGroup))
      return true;
    return this.ChildrenCount > 0 && this.Children.Any<LayoutContent>((Func<LayoutContent, bool>) (c => c is LayoutDocument && ((LayoutDocument) c).IsVisible || c is LayoutAnchorable));
  }

  protected override void ChildMoved(int oldIndex, int newIndex)
  {
    if (this._selectedIndex == oldIndex)
    {
      this.RaisePropertyChanging("SelectedContentIndex");
      this._selectedIndex = newIndex;
      this.RaisePropertyChanged("SelectedContentIndex");
    }
    base.ChildMoved(oldIndex, newIndex);
  }

  protected override void OnChildrenCollectionChanged()
  {
    if (this.SelectedContentIndex >= this.ChildrenCount)
      this.SelectedContentIndex = this.Children.Count - 1;
    if (this.SelectedContentIndex == -1 && this.ChildrenCount > 0)
    {
      if (this.Root == null)
      {
        this.SetNextSelectedIndex();
      }
      else
      {
        LayoutContent layoutContent = this.Children.OrderByDescending<LayoutContent, DateTime>((Func<LayoutContent, DateTime>) (c => c.LastActivationTimeStamp.GetValueOrDefault())).First<LayoutContent>();
        this.SelectedContentIndex = this.Children.IndexOf(layoutContent);
        layoutContent.IsActive = true;
      }
    }
    base.OnChildrenCollectionChanged();
    this.RaisePropertyChanged("ChildrenSorted");
  }

  protected override void OnIsVisibleChanged()
  {
    this.UpdateParentVisibility();
    base.OnIsVisibleChanged();
  }

  public override void WriteXml(XmlWriter writer)
  {
    if (this._id != null)
      writer.WriteAttributeString("Id", this._id);
    if (!this._showHeader)
      writer.WriteAttributeString("ShowHeader", this._showHeader.ToString());
    base.WriteXml(writer);
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Id"))
      this._id = reader.Value;
    if (reader.MoveToAttribute("ShowHeader"))
      this._showHeader = bool.Parse(reader.Value);
    base.ReadXml(reader);
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("DocumentPane()");
    foreach (LayoutElement child in (Collection<LayoutContent>) this.Children)
      child.ConsoleDump(tab + 1);
  }

  public int IndexOf(LayoutContent content) => this.Children.IndexOf(content);

  internal void SetNextSelectedIndex()
  {
    this.SelectedContentIndex = -1;
    for (int index = 0; index < this.Children.Count; ++index)
    {
      if (this.Children[index].IsEnabled)
      {
        this.SelectedContentIndex = index;
        break;
      }
    }
  }

  private void UpdateParentVisibility()
  {
    if (!(this.Parent is ILayoutElementWithVisibility parent))
      return;
    parent.ComputeVisibility();
  }

  string ILayoutPaneSerializable.Id
  {
    get => this._id;
    set => this._id = value;
  }
}
