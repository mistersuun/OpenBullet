// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutGroup`1
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public abstract class LayoutGroup<T> : 
  LayoutGroupBase,
  ILayoutContainer,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutGroup,
  IXmlSerializable
  where T : class, ILayoutElement
{
  private ObservableCollection<T> _children = new ObservableCollection<T>();
  private bool _isVisible = true;

  internal LayoutGroup()
  {
    this._children.CollectionChanged += new NotifyCollectionChangedEventHandler(this._children_CollectionChanged);
  }

  public ObservableCollection<T> Children => this._children;

  public bool IsVisible
  {
    get => this._isVisible;
    protected set
    {
      if (this._isVisible == value)
        return;
      this.RaisePropertyChanging(nameof (IsVisible));
      this._isVisible = value;
      this.OnIsVisibleChanged();
      this.RaisePropertyChanged(nameof (IsVisible));
    }
  }

  public int ChildrenCount => this._children.Count;

  protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
    base.OnParentChanged(oldValue, newValue);
    this.ComputeVisibility();
  }

  public void ComputeVisibility() => this.IsVisible = this.GetVisibility();

  public void MoveChild(int oldIndex, int newIndex)
  {
    if (oldIndex == newIndex)
      return;
    this._children.Move(oldIndex, newIndex);
    this.ChildMoved(oldIndex, newIndex);
  }

  public void RemoveChildAt(int childIndex) => this._children.RemoveAt(childIndex);

  public int IndexOfChild(ILayoutElement element)
  {
    return this._children.Cast<ILayoutElement>().ToList<ILayoutElement>().IndexOf(element);
  }

  public void InsertChildAt(int index, ILayoutElement element)
  {
    this._children.Insert(index, (T) element);
  }

  public void RemoveChild(ILayoutElement element) => this._children.Remove((T) element);

  public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
  {
    int index = this._children.IndexOf((T) oldElement);
    this._children.Insert(index, (T) newElement);
    this._children.RemoveAt(index + 1);
  }

  public void ReplaceChildAt(int index, ILayoutElement element)
  {
    this._children[index] = (T) element;
  }

  public XmlSchema GetSchema() => (XmlSchema) null;

  public virtual void ReadXml(XmlReader reader)
  {
    int content = (int) reader.MoveToContent();
    if (reader.IsEmptyElement)
    {
      reader.Read();
      this.ComputeVisibility();
    }
    else
    {
      string localName = reader.LocalName;
      reader.Read();
      while (!(reader.LocalName == localName) || reader.NodeType != XmlNodeType.EndElement)
      {
        if (reader.NodeType == XmlNodeType.Whitespace)
        {
          reader.Read();
        }
        else
        {
          XmlSerializer xmlSerializer;
          if (reader.LocalName == "LayoutAnchorablePaneGroup")
            xmlSerializer = new XmlSerializer(typeof (LayoutAnchorablePaneGroup));
          else if (reader.LocalName == "LayoutAnchorablePane")
            xmlSerializer = new XmlSerializer(typeof (LayoutAnchorablePane));
          else if (reader.LocalName == "LayoutAnchorable")
            xmlSerializer = new XmlSerializer(typeof (LayoutAnchorable));
          else if (reader.LocalName == "LayoutDocumentPaneGroup")
            xmlSerializer = new XmlSerializer(typeof (LayoutDocumentPaneGroup));
          else if (reader.LocalName == "LayoutDocumentPane")
            xmlSerializer = new XmlSerializer(typeof (LayoutDocumentPane));
          else if (reader.LocalName == "LayoutDocument")
            xmlSerializer = new XmlSerializer(typeof (LayoutDocument));
          else if (reader.LocalName == "LayoutAnchorGroup")
            xmlSerializer = new XmlSerializer(typeof (LayoutAnchorGroup));
          else if (reader.LocalName == "LayoutPanel")
          {
            xmlSerializer = new XmlSerializer(typeof (LayoutPanel));
          }
          else
          {
            Type type = this.FindType(reader.LocalName);
            xmlSerializer = !(type == (Type) null) ? new XmlSerializer(type) : throw new ArgumentException("AvalonDock.LayoutGroup doesn't know how to deserialize " + reader.LocalName);
          }
          this.Children.Add((T) xmlSerializer.Deserialize(reader));
        }
      }
      reader.ReadEndElement();
    }
  }

  public virtual void WriteXml(XmlWriter writer)
  {
    foreach (T child in (Collection<T>) this.Children)
      new XmlSerializer(child.GetType()).Serialize(writer, (object) child);
  }

  protected virtual void OnIsVisibleChanged() => this.UpdateParentVisibility();

  protected abstract bool GetVisibility();

  protected virtual void ChildMoved(int oldIndex, int newIndex)
  {
  }

  private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
    {
      foreach (LayoutElement oldItem in (IEnumerable) e.OldItems)
      {
        if (oldItem.Parent == this)
          oldItem.Parent = (ILayoutContainer) null;
      }
    }
    if ((e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
    {
      foreach (LayoutElement newItem in (IEnumerable) e.NewItems)
      {
        if (newItem.Parent != this)
        {
          if (newItem.Parent != null)
            newItem.Parent.RemoveChild((ILayoutElement) newItem);
          newItem.Parent = (ILayoutContainer) this;
        }
      }
    }
    this.ComputeVisibility();
    this.OnChildrenCollectionChanged();
    this.NotifyChildrenTreeChanged(ChildrenTreeChange.DirectChildrenChanged);
    this.RaisePropertyChanged("ChildrenCount");
  }

  private void UpdateParentVisibility()
  {
    if (!(this.Parent is ILayoutElementWithVisibility parent))
      return;
    parent.ComputeVisibility();
  }

  private Type FindType(string name)
  {
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (type.Name.Equals(name))
          return type;
      }
    }
    return (Type) null;
  }

  IEnumerable<ILayoutElement> ILayoutContainer.Children => this._children.Cast<ILayoutElement>();
}
