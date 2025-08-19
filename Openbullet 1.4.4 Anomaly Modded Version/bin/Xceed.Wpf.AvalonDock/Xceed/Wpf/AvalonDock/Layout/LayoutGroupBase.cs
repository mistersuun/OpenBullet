// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutGroupBase
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public abstract class LayoutGroupBase : LayoutElement
{
  protected virtual void OnChildrenCollectionChanged()
  {
    if (this.ChildrenCollectionChanged == null)
      return;
    this.ChildrenCollectionChanged((object) this, EventArgs.Empty);
  }

  protected void NotifyChildrenTreeChanged(ChildrenTreeChange change)
  {
    this.OnChildrenTreeChanged(change);
    if (!(this.Parent is LayoutGroupBase parent))
      return;
    parent.NotifyChildrenTreeChanged(ChildrenTreeChange.TreeChanged);
  }

  protected virtual void OnChildrenTreeChanged(ChildrenTreeChange change)
  {
    if (this.ChildrenTreeChanged == null)
      return;
    this.ChildrenTreeChanged((object) this, new ChildrenTreeChangedEventArgs(change));
  }

  [field: XmlIgnore]
  [field: NonSerialized]
  public event EventHandler ChildrenCollectionChanged;

  [field: XmlIgnore]
  [field: NonSerialized]
  public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;
}
