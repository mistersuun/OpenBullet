// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ColumnHeaderCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class ColumnHeaderCollection : IEnumerable, INotifyCollectionChanged, IDisposable
{
  private ObservableCollection<DataGridColumn> _columns;

  public ColumnHeaderCollection(ObservableCollection<DataGridColumn> columns)
  {
    this._columns = columns;
    if (this._columns == null)
      return;
    this._columns.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnColumnsChanged);
  }

  public DataGridColumn ColumnFromIndex(int index)
  {
    return index >= 0 && index < this._columns.Count ? this._columns[index] : (DataGridColumn) null;
  }

  internal void NotifyHeaderPropertyChanged(
    DataGridColumn column,
    DependencyPropertyChangedEventArgs e)
  {
    this.FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewValue, e.OldValue, this._columns.IndexOf(column)));
  }

  public void Dispose()
  {
    GC.SuppressFinalize((object) this);
    if (this._columns == null)
      return;
    this._columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnColumnsChanged);
  }

  public IEnumerator GetEnumerator()
  {
    return (IEnumerator) new ColumnHeaderCollection.ColumnHeaderCollectionEnumerator(this._columns);
  }

  public event NotifyCollectionChangedEventHandler CollectionChanged;

  private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    NotifyCollectionChangedEventArgs args;
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        args = new NotifyCollectionChangedEventArgs(e.Action, (IList) ColumnHeaderCollection.HeadersFromColumns(e.NewItems), e.NewStartingIndex);
        break;
      case NotifyCollectionChangedAction.Remove:
        args = new NotifyCollectionChangedEventArgs(e.Action, (IList) ColumnHeaderCollection.HeadersFromColumns(e.OldItems), e.OldStartingIndex);
        break;
      case NotifyCollectionChangedAction.Replace:
        args = new NotifyCollectionChangedEventArgs(e.Action, (IList) ColumnHeaderCollection.HeadersFromColumns(e.NewItems), (IList) ColumnHeaderCollection.HeadersFromColumns(e.OldItems), e.OldStartingIndex);
        break;
      case NotifyCollectionChangedAction.Move:
        args = new NotifyCollectionChangedEventArgs(e.Action, (IList) ColumnHeaderCollection.HeadersFromColumns(e.OldItems), e.NewStartingIndex, e.OldStartingIndex);
        break;
      default:
        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        break;
    }
    this.FireCollectionChanged(args);
  }

  private void FireCollectionChanged(NotifyCollectionChangedEventArgs args)
  {
    if (this.CollectionChanged == null)
      return;
    this.CollectionChanged((object) this, args);
  }

  private static object[] HeadersFromColumns(IList columns)
  {
    object[] objArray = new object[columns.Count];
    for (int index = 0; index < columns.Count; ++index)
      objArray[index] = !(columns[index] is DataGridColumn column) ? (object) null : column.Header;
    return objArray;
  }

  private class ColumnHeaderCollectionEnumerator : IEnumerator, IDisposable
  {
    private int _current;
    private bool _columnsChanged;
    private ObservableCollection<DataGridColumn> _columns;

    public ColumnHeaderCollectionEnumerator(ObservableCollection<DataGridColumn> columns)
    {
      if (columns != null)
      {
        this._columns = columns;
        this._columns.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnColumnsChanged);
      }
      this._current = -1;
    }

    public object Current
    {
      get
      {
        if (!this.IsValid)
          throw new InvalidOperationException();
        return this._columns[this._current]?.Header;
      }
    }

    public bool MoveNext()
    {
      if (this.HasChanged)
        throw new InvalidOperationException();
      if (this._columns == null || this._current >= this._columns.Count - 1)
        return false;
      ++this._current;
      return true;
    }

    public void Reset()
    {
      if (this.HasChanged)
        throw new InvalidOperationException();
      this._current = -1;
    }

    public void Dispose()
    {
      GC.SuppressFinalize((object) this);
      if (this._columns == null)
        return;
      this._columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnColumnsChanged);
    }

    private bool HasChanged => this._columnsChanged;

    private bool IsValid
    {
      get
      {
        return this._columns != null && this._current >= 0 && this._current < this._columns.Count && !this.HasChanged;
      }
    }

    private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this._columnsChanged = true;
    }
  }
}
