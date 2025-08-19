// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.MultipleCopiesCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class MultipleCopiesCollection : 
  IList,
  ICollection,
  IEnumerable,
  INotifyCollectionChanged,
  INotifyPropertyChanged
{
  private const string CountName = "Count";
  private const string IndexerName = "Item[]";
  private object _item;
  private int _count;

  internal MultipleCopiesCollection(object item, int count)
  {
    this.CopiedItem = item;
    this._count = count;
  }

  internal void MirrorCollectionChange(NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        this.Insert(e.NewStartingIndex);
        break;
      case NotifyCollectionChangedAction.Remove:
        this.RemoveAt(e.OldStartingIndex);
        break;
      case NotifyCollectionChangedAction.Replace:
        this.OnReplace(this.CopiedItem, this.CopiedItem, e.NewStartingIndex);
        break;
      case NotifyCollectionChangedAction.Move:
        this.Move(e.OldStartingIndex, e.NewStartingIndex);
        break;
      case NotifyCollectionChangedAction.Reset:
        this.Reset();
        break;
    }
  }

  internal void SyncToCount(int newCount)
  {
    int repeatCount = this.RepeatCount;
    if (newCount == repeatCount)
      return;
    if (newCount > repeatCount)
    {
      this.InsertRange(repeatCount, newCount - repeatCount);
    }
    else
    {
      int count = repeatCount - newCount;
      this.RemoveRange(repeatCount - count, count);
    }
  }

  internal object CopiedItem
  {
    get => this._item;
    set
    {
      if (value == CollectionView.NewItemPlaceholder)
        value = DataGrid.NewItemPlaceholder;
      if (this._item == value)
        return;
      object oldItem = this._item;
      this._item = value;
      this.OnPropertyChanged("Item[]");
      for (int index = 0; index < this._count; ++index)
        this.OnReplace(oldItem, this._item, index);
    }
  }

  private int RepeatCount
  {
    get => this._count;
    set
    {
      if (this._count == value)
        return;
      this._count = value;
      this.OnPropertyChanged("Count");
      this.OnPropertyChanged("Item[]");
    }
  }

  private void Insert(int index)
  {
    ++this.RepeatCount;
    this.OnCollectionChanged(NotifyCollectionChangedAction.Add, this.CopiedItem, index);
  }

  private void InsertRange(int index, int count)
  {
    for (int index1 = 0; index1 < count; ++index1)
    {
      this.Insert(index);
      ++index;
    }
  }

  private void Move(int oldIndex, int newIndex)
  {
    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this.CopiedItem, newIndex, oldIndex));
  }

  private void RemoveAt(int index)
  {
    --this.RepeatCount;
    this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, this.CopiedItem, index);
  }

  private void RemoveRange(int index, int count)
  {
    for (int index1 = 0; index1 < count; ++index1)
      this.RemoveAt(index);
  }

  private void OnReplace(object oldItem, object newItem, int index)
  {
    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
  }

  private void Reset()
  {
    this.RepeatCount = 0;
    this.OnCollectionReset();
  }

  public int Add(object value)
  {
    throw new NotSupportedException(SR.Get(SRID.DataGrid_ReadonlyCellsItemsSource));
  }

  public void Clear()
  {
    throw new NotSupportedException(SR.Get(SRID.DataGrid_ReadonlyCellsItemsSource));
  }

  public bool Contains(object value)
  {
    if (value == null)
      throw new ArgumentNullException(nameof (value));
    return this._item == value;
  }

  public int IndexOf(object value)
  {
    if (value == null)
      throw new ArgumentNullException(nameof (value));
    return this._item != value ? -1 : 0;
  }

  public void Insert(int index, object value)
  {
    throw new NotSupportedException(SR.Get(SRID.DataGrid_ReadonlyCellsItemsSource));
  }

  public bool IsFixedSize => false;

  public bool IsReadOnly => true;

  public void Remove(object value)
  {
    throw new NotSupportedException(SR.Get(SRID.DataGrid_ReadonlyCellsItemsSource));
  }

  void IList.RemoveAt(int index)
  {
    throw new NotSupportedException(SR.Get(SRID.DataGrid_ReadonlyCellsItemsSource));
  }

  public object this[int index]
  {
    get
    {
      if (index >= 0 && index < this.RepeatCount)
        return this._item;
      throw new ArgumentOutOfRangeException(nameof (index));
    }
    set => throw new InvalidOperationException();
  }

  public void CopyTo(Array array, int index) => throw new NotSupportedException();

  public int Count => this.RepeatCount;

  public bool IsSynchronized => false;

  public object SyncRoot => (object) this;

  public IEnumerator GetEnumerator()
  {
    return (IEnumerator) new MultipleCopiesCollection.MultipleCopiesCollectionEnumerator(this);
  }

  public event NotifyCollectionChangedEventHandler CollectionChanged;

  private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
  {
    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
  }

  private void OnCollectionReset()
  {
    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
  }

  private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
  {
    if (this.CollectionChanged == null)
      return;
    this.CollectionChanged((object) this, e);
  }

  public event PropertyChangedEventHandler PropertyChanged;

  private void OnPropertyChanged(string propertyName)
  {
    this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
  }

  private void OnPropertyChanged(PropertyChangedEventArgs e)
  {
    if (this.PropertyChanged == null)
      return;
    this.PropertyChanged((object) this, e);
  }

  private class MultipleCopiesCollectionEnumerator : IEnumerator
  {
    private object _item;
    private int _count;
    private int _current;
    private MultipleCopiesCollection _collection;

    public MultipleCopiesCollectionEnumerator(MultipleCopiesCollection collection)
    {
      this._collection = collection;
      this._item = this._collection.CopiedItem;
      this._count = this._collection.RepeatCount;
      this._current = -1;
    }

    object IEnumerator.Current
    {
      get
      {
        if (this._current < 0)
          return (object) null;
        if (this._current < this._count)
          return this._item;
        throw new InvalidOperationException();
      }
    }

    bool IEnumerator.MoveNext()
    {
      if (!this.IsCollectionUnchanged)
        throw new InvalidOperationException();
      int num = this._current + 1;
      if (num >= this._count)
        return false;
      this._current = num;
      return true;
    }

    void IEnumerator.Reset()
    {
      if (!this.IsCollectionUnchanged)
        throw new InvalidOperationException();
      this._current = -1;
    }

    private bool IsCollectionUnchanged
    {
      get
      {
        return this._collection.RepeatCount == this._count && this._collection.CopiedItem == this._item;
      }
    }
  }
}
