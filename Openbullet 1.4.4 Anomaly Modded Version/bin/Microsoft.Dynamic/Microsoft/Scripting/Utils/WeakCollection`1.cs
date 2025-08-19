// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.WeakCollection`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal sealed class WeakCollection<T> : IEnumerable<T>, IEnumerable where T : class
{
  private const int DefaultCapacity = 4;
  private WeakReference[] _items = new WeakReference[4];
  private int _size;

  public void Add(T t)
  {
    this.EnsureCapacity(this._size + 1);
    this._items[this._size++] = new WeakReference((object) t);
  }

  private void EnsureCapacity(int size)
  {
    if (size <= this._items.Length)
      return;
    this.Compact();
    if (size <= this._items.Length)
      return;
    int length = this._items.Length * 2;
    if (length < size)
      length = size;
    WeakReference[] weakReferenceArray = new WeakReference[length];
    this._items.CopyTo((Array) weakReferenceArray, 0);
    this._items = weakReferenceArray;
  }

  private void Compact()
  {
    int index1 = 0;
    for (int index2 = 0; index2 < this._size; ++index2)
    {
      if (this._items[index2].IsAlive)
      {
        if (index1 < index2)
          this._items[index1] = this._items[index2];
        ++index1;
      }
    }
    for (int index3 = index1; index3 < this._size; ++index3)
      this._items[index3] = (WeakReference) null;
    this._size = index1;
  }

  public IEnumerator<T> GetEnumerator()
  {
    for (int i = 0; i < this._size; ++i)
    {
      T target = (T) this._items[i].Target;
      if ((object) target != null)
        yield return target;
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
