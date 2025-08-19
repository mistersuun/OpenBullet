// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.WeakCollectionChangedWrapper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

internal class WeakCollectionChangedWrapper : 
  IList,
  ICollection,
  IEnumerable,
  INotifyCollectionChanged
{
  private WeakEventListener<NotifyCollectionChangedEventArgs> _innerListListener;
  private IList _innerList;

  public WeakCollectionChangedWrapper(IList sourceList)
  {
    this._innerList = sourceList;
    if (!(this._innerList is INotifyCollectionChanged innerList))
      return;
    this._innerListListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnInnerCollectionChanged));
    CollectionChangedEventManager.AddListener(innerList, (IWeakEventListener) this._innerListListener);
  }

  public event NotifyCollectionChangedEventHandler CollectionChanged;

  private void OnInnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
  {
    if (this.CollectionChanged == null)
      return;
    this.CollectionChanged((object) this, args);
  }

  internal void ReleaseEvents()
  {
    if (this._innerListListener == null)
      return;
    CollectionChangedEventManager.RemoveListener((INotifyCollectionChanged) this._innerList, (IWeakEventListener) this._innerListListener);
    this._innerListListener = (WeakEventListener<NotifyCollectionChangedEventArgs>) null;
  }

  int IList.Add(object value) => this._innerList.Add(value);

  void IList.Clear() => this._innerList.Clear();

  bool IList.Contains(object value) => this._innerList.Contains(value);

  int IList.IndexOf(object value) => this._innerList.IndexOf(value);

  void IList.Insert(int index, object value) => this._innerList.Insert(index, value);

  bool IList.IsFixedSize => this._innerList.IsFixedSize;

  bool IList.IsReadOnly => this._innerList.IsReadOnly;

  void IList.Remove(object value) => this._innerList.Remove(value);

  void IList.RemoveAt(int index) => this._innerList.RemoveAt(index);

  object IList.this[int index]
  {
    get => this._innerList[index];
    set => this._innerList[index] = value;
  }

  void ICollection.CopyTo(Array array, int index) => this._innerList.CopyTo(array, index);

  int ICollection.Count => this._innerList.Count;

  bool ICollection.IsSynchronized => this._innerList.IsSynchronized;

  object ICollection.SyncRoot => this._innerList.SyncRoot;

  IEnumerator IEnumerable.GetEnumerator() => this._innerList.GetEnumerator();
}
