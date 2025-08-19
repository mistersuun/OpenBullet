// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.Deque`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public sealed class Deque<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
  private T[] arr = Empty<T>.Array;
  private int size;
  private int head;
  private int tail;

  public int Count => this.size;

  public void Clear()
  {
    this.arr = Empty<T>.Array;
    this.size = 0;
    this.head = 0;
    this.tail = 0;
  }

  public T this[int index]
  {
    get
    {
      ThrowUtil.CheckInRangeInclusive(index, nameof (index), 0, this.size - 1);
      return this.arr[(this.head + index) % this.arr.Length];
    }
    set
    {
      ThrowUtil.CheckInRangeInclusive(index, nameof (index), 0, this.size - 1);
      this.arr[(this.head + index) % this.arr.Length] = value;
    }
  }

  public void PushBack(T item)
  {
    if (this.size == this.arr.Length)
      this.SetCapacity(Math.Max(4, this.arr.Length * 2));
    this.arr[this.tail++] = item;
    if (this.tail == this.arr.Length)
      this.tail = 0;
    ++this.size;
  }

  public T PopBack()
  {
    if (this.size == 0)
      throw new InvalidOperationException();
    if (this.tail == 0)
      this.tail = this.arr.Length - 1;
    else
      --this.tail;
    T obj = this.arr[this.tail];
    this.arr[this.tail] = default (T);
    --this.size;
    return obj;
  }

  public void PushFront(T item)
  {
    if (this.size == this.arr.Length)
      this.SetCapacity(Math.Max(4, this.arr.Length * 2));
    if (this.head == 0)
      this.head = this.arr.Length - 1;
    else
      --this.head;
    this.arr[this.head] = item;
    ++this.size;
  }

  public T PopFront()
  {
    if (this.size == 0)
      throw new InvalidOperationException();
    T obj = this.arr[this.head];
    this.arr[this.head] = default (T);
    ++this.head;
    if (this.head == this.arr.Length)
      this.head = 0;
    --this.size;
    return obj;
  }

  private void SetCapacity(int capacity)
  {
    T[] array = new T[capacity];
    this.CopyTo(array, 0);
    this.head = 0;
    this.tail = this.size == capacity ? 0 : this.size;
    this.arr = array;
  }

  public IEnumerator<T> GetEnumerator()
  {
    if (this.head < this.tail)
    {
      for (int i = this.head; i < this.tail; ++i)
        yield return this.arr[i];
    }
    else
    {
      for (int i = this.head; i < this.arr.Length; ++i)
        yield return this.arr[i];
      for (int i = 0; i < this.tail; ++i)
        yield return this.arr[i];
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  bool ICollection<T>.IsReadOnly => false;

  void ICollection<T>.Add(T item) => this.PushBack(item);

  public bool Contains(T item)
  {
    EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
    foreach (T y in this)
    {
      if (equalityComparer.Equals(item, y))
        return true;
    }
    return false;
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (this.head < this.tail)
    {
      Array.Copy((Array) this.arr, this.head, (Array) array, arrayIndex, this.tail - this.head);
    }
    else
    {
      int length = this.arr.Length - this.head;
      Array.Copy((Array) this.arr, this.head, (Array) array, arrayIndex, length);
      Array.Copy((Array) this.arr, 0, (Array) array, arrayIndex + length, this.tail);
    }
  }

  bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}
