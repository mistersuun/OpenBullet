// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CopyOnWriteList`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class CopyOnWriteList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  private List<T> _list = new List<T>();

  private List<T> GetNewListForWrite()
  {
    List<T> list = this._list;
    List<T> newListForWrite = new List<T>(list.Count + 1);
    newListForWrite.AddRange((IEnumerable<T>) list);
    return newListForWrite;
  }

  public List<T> GetCopyForRead() => this._list;

  public int IndexOf(T item) => this._list.IndexOf(item);

  public void Insert(int index, T item)
  {
    List<T> list;
    List<T> newListForWrite;
    do
    {
      list = this._list;
      newListForWrite = this.GetNewListForWrite();
      newListForWrite.Insert(index, item);
    }
    while (Interlocked.CompareExchange<List<T>>(ref this._list, newListForWrite, list) != list);
  }

  public void RemoveAt(int index)
  {
    List<T> list;
    List<T> newListForWrite;
    do
    {
      list = this._list;
      newListForWrite = this.GetNewListForWrite();
      newListForWrite.RemoveAt(index);
    }
    while (Interlocked.CompareExchange<List<T>>(ref this._list, newListForWrite, list) != list);
  }

  public T this[int index]
  {
    get => this._list[index];
    set
    {
      List<T> list;
      List<T> newListForWrite;
      do
      {
        list = this._list;
        newListForWrite = this.GetNewListForWrite();
        newListForWrite[index] = value;
      }
      while (Interlocked.CompareExchange<List<T>>(ref this._list, newListForWrite, list) != list);
    }
  }

  public void Add(T item)
  {
    List<T> list;
    List<T> newListForWrite;
    do
    {
      list = this._list;
      newListForWrite = this.GetNewListForWrite();
      newListForWrite.Add(item);
    }
    while (Interlocked.CompareExchange<List<T>>(ref this._list, newListForWrite, list) != list);
  }

  public void Clear() => this._list = new List<T>();

  public bool Contains(T item) => this._list.Contains(item);

  public void CopyTo(T[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

  public int Count => this._list.Count;

  public bool IsReadOnly => false;

  public bool Remove(T item)
  {
    List<T> list;
    List<T> newListForWrite;
    bool flag;
    do
    {
      list = this._list;
      newListForWrite = this.GetNewListForWrite();
      flag = newListForWrite.Remove(item);
    }
    while (Interlocked.CompareExchange<List<T>>(ref this._list, newListForWrite, list) != list);
    return flag;
  }

  public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this._list.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this._list).GetEnumerator();
}
