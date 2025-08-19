// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ListGenericWrapper`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

public class ListGenericWrapper<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  private IList<object> _value;

  public ListGenericWrapper(IList<object> value) => this._value = value;

  public int IndexOf(T item) => this._value.IndexOf((object) item);

  public void Insert(int index, T item) => this._value.Insert(index, (object) item);

  public void RemoveAt(int index) => this._value.RemoveAt(index);

  public T this[int index]
  {
    get => (T) this._value[index];
    set => this._value[index] = (object) value;
  }

  public void Add(T item) => this._value.Add((object) item);

  public void Clear() => this._value.Clear();

  public bool Contains(T item) => this._value.Contains((object) item);

  public void CopyTo(T[] array, int arrayIndex)
  {
    for (int index = 0; index < this._value.Count; ++index)
      array[arrayIndex + index] = (T) this._value[index];
  }

  public int Count => this._value.Count;

  public bool IsReadOnly => this._value.IsReadOnly;

  public bool Remove(T item) => this._value.Remove((object) item);

  public IEnumerator<T> GetEnumerator()
  {
    return (IEnumerator<T>) new IEnumeratorOfTWrapper<T>((IEnumerator) this._value.GetEnumerator());
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._value.GetEnumerator();
}
