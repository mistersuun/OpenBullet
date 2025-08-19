// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutElement
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public abstract class LayoutElement : 
  DependencyObject,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging
{
  [NonSerialized]
  private ILayoutContainer _parent;
  [NonSerialized]
  private ILayoutRoot _root;

  internal LayoutElement()
  {
  }

  [XmlIgnore]
  public ILayoutContainer Parent
  {
    get => this._parent;
    set
    {
      if (this._parent == value)
        return;
      ILayoutContainer parent = this._parent;
      ILayoutRoot root1 = this._root;
      this.RaisePropertyChanging(nameof (Parent));
      this.OnParentChanging(parent, value);
      this._parent = value;
      this.OnParentChanged(parent, value);
      this._root = this.Root;
      if (root1 != this._root)
        this.OnRootChanged(root1, this._root);
      this.RaisePropertyChanged(nameof (Parent));
      if (!(this.Root is LayoutRoot root2))
        return;
      root2.FireLayoutUpdated();
    }
  }

  public ILayoutRoot Root
  {
    get
    {
      ILayoutContainer parent = this.Parent;
      while (true)
      {
        switch (parent)
        {
          case null:
          case ILayoutRoot _:
            goto label_3;
          default:
            parent = parent.Parent;
            continue;
        }
      }
label_3:
      return parent as ILayoutRoot;
    }
  }

  public virtual void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine(this.ToString());
  }

  protected virtual void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
  }

  protected virtual void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
  }

  protected virtual void OnRootChanged(ILayoutRoot oldRoot, ILayoutRoot newRoot)
  {
    ((LayoutRoot) oldRoot)?.OnLayoutElementRemoved(this);
    ((LayoutRoot) newRoot)?.OnLayoutElementAdded(this);
  }

  protected virtual void RaisePropertyChanged(string propertyName)
  {
    if (this.PropertyChanged == null)
      return;
    this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
  }

  protected virtual void RaisePropertyChanging(string propertyName)
  {
    if (this.PropertyChanging == null)
      return;
    this.PropertyChanging((object) this, new PropertyChangingEventArgs(propertyName));
  }

  [field: XmlIgnore]
  [field: NonSerialized]
  public event PropertyChangedEventHandler PropertyChanged;

  [field: XmlIgnore]
  [field: NonSerialized]
  public event PropertyChangingEventHandler PropertyChanging;
}
