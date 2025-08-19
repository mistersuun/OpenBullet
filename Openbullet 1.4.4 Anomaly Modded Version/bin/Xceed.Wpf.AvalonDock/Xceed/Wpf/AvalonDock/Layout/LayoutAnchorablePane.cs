// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutAnchorablePane
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Children")]
[Serializable]
public class LayoutAnchorablePane : 
  LayoutPositionableGroup<LayoutAnchorable>,
  ILayoutAnchorablePane,
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
  private int _selectedIndex = -1;
  [XmlIgnore]
  private bool _autoFixSelectedContent = true;
  private string _name;
  private string _id;

  public LayoutAnchorablePane()
  {
  }

  public LayoutAnchorablePane(LayoutAnchorable anchorable) => this.Children.Add(anchorable);

  public bool CanHide
  {
    get => this.Children.All<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (a => a.CanHide));
  }

  public bool CanClose
  {
    get => this.Children.All<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (a => a.CanClose));
  }

  public bool IsHostedInFloatingWindow => this.FindParent<LayoutFloatingWindow>() != null;

  public string Name
  {
    get => this._name;
    set
    {
      if (!(this._name != value))
        return;
      this._name = value;
      this.RaisePropertyChanged(nameof (Name));
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
    get
    {
      return this._selectedIndex != -1 ? (LayoutContent) this.Children[this._selectedIndex] : (LayoutContent) null;
    }
  }

  protected override bool GetVisibility()
  {
    return this.Children.Count > 0 && this.Children.Any<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (c => c.IsVisible));
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
    this.AutoFixSelectedContent();
    for (int index = 0; index < this.Children.Count; ++index)
    {
      if (this.Children[index].IsSelected)
      {
        this.SelectedContentIndex = index;
        break;
      }
    }
    this.RaisePropertyChanged("CanClose");
    this.RaisePropertyChanged("CanHide");
    this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
    base.OnChildrenCollectionChanged();
  }

  protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
    if (oldValue is ILayoutGroup layoutGroup1)
      layoutGroup1.ChildrenCollectionChanged -= new EventHandler(this.OnParentChildrenCollectionChanged);
    this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
    if (newValue is ILayoutGroup layoutGroup2)
      layoutGroup2.ChildrenCollectionChanged += new EventHandler(this.OnParentChildrenCollectionChanged);
    base.OnParentChanged(oldValue, newValue);
  }

  public override void WriteXml(XmlWriter writer)
  {
    if (this._id != null)
      writer.WriteAttributeString("Id", this._id);
    if (this._name != null)
      writer.WriteAttributeString("Name", this._name);
    base.WriteXml(writer);
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Id"))
      this._id = reader.Value;
    if (reader.MoveToAttribute("Name"))
      this._name = reader.Value;
    this._autoFixSelectedContent = false;
    base.ReadXml(reader);
    this._autoFixSelectedContent = true;
    this.AutoFixSelectedContent();
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("AnchorablePane()");
    foreach (LayoutElement child in (Collection<LayoutAnchorable>) this.Children)
      child.ConsoleDump(tab + 1);
  }

  public int IndexOf(LayoutContent content)
  {
    return !(content is LayoutAnchorable layoutAnchorable) ? -1 : this.Children.IndexOf(layoutAnchorable);
  }

  public bool IsDirectlyHostedInFloatingWindow
  {
    get
    {
      LayoutAnchorableFloatingWindow parent = this.FindParent<LayoutAnchorableFloatingWindow>();
      return parent != null && parent.IsSinglePane;
    }
  }

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

  internal void UpdateIsDirectlyHostedInFloatingWindow()
  {
    this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
  }

  private void AutoFixSelectedContent()
  {
    if (!this._autoFixSelectedContent)
      return;
    if (this.SelectedContentIndex >= this.ChildrenCount)
      this.SelectedContentIndex = this.Children.Count - 1;
    if (this.SelectedContentIndex != -1 || this.ChildrenCount <= 0)
      return;
    this.SetNextSelectedIndex();
  }

  private void OnParentChildrenCollectionChanged(object sender, EventArgs e)
  {
    this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
  }

  string ILayoutPaneSerializable.Id
  {
    get => this._id;
    set => this._id = value;
  }
}
