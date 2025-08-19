// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.HybridMapping`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public sealed class HybridMapping<T>
{
  private Dictionary<int, object> _dict = new Dictionary<int, object>();
  private readonly object _synchObject = new object();
  private readonly int _offset;
  private int _current;
  public const int SIZE = 4096 /*0x1000*/;
  private const int MIN_RANGE = 2048 /*0x0800*/;

  public HybridMapping()
    : this(0)
  {
  }

  public HybridMapping(int offset)
  {
    this._offset = offset >= 0 && 4096 /*0x1000*/ - offset >= 2048 /*0x0800*/ ? offset : throw new InvalidOperationException("HybridMapping is full");
    this._current = offset;
  }

  private void NextKey()
  {
    if (++this._current < 4096 /*0x1000*/)
      return;
    this._current = this._offset;
  }

  public int WeakAdd(T value)
  {
    lock (this._synchObject)
    {
      int current = this._current;
      while (this._dict.ContainsKey(this._current))
      {
        this.NextKey();
        if (this._current == current)
          throw new InvalidOperationException("HybridMapping is full");
      }
      this._dict.Add(this._current, (object) new WeakObject((object) value));
      return this._current;
    }
  }

  public int StrongAdd(T value, int pos)
  {
    if (pos >= 4096 /*0x1000*/)
      throw new InvalidOperationException($"Size of HybridMapping Exceeded ({(object) 4096 /*0x1000*/})");
    lock (this._synchObject)
    {
      if (pos == -1)
      {
        int current = this._current;
        while (this._dict.ContainsKey(this._current))
        {
          this.NextKey();
          if (this._current == current)
            throw new InvalidOperationException("HybridMapping is full");
        }
        this._dict.Add(this._current, (object) value);
        return this._current;
      }
      if (this._dict.ContainsKey(pos))
        throw new InvalidOperationException($"HybridMapping at pos:{(object) pos} is not empty");
      this._dict.Add(pos, (object) value);
      return pos;
    }
  }

  public T GetObjectFromId(int id)
  {
    object obj;
    if (!this._dict.TryGetValue(id, out obj))
      return default (T);
    switch (obj)
    {
      case WeakObject weakObject:
        return (T) weakObject.Target;
      case T objectFromId:
        return objectFromId;
      default:
        throw new InvalidOperationException("Unexpected dictionary content: type " + (object) obj.GetType());
    }
  }

  public int GetIdFromObject(T value)
  {
    lock (this._synchObject)
    {
      foreach (KeyValuePair<int, object> keyValuePair in this._dict)
      {
        object obj1 = keyValuePair.Value;
        if (obj1 != null)
        {
          if (!(obj1 is WeakObject weakObject))
          {
            if (obj1 is T obj2 && obj2.Equals((object) value))
              return keyValuePair.Key;
          }
          else
          {
            object target = weakObject.Target;
            if (target != null && target.Equals((object) value))
              return keyValuePair.Key;
          }
        }
      }
    }
    return -1;
  }

  public void RemoveOnId(int id)
  {
    lock (this._synchObject)
    {
      this._dict.Remove(id);
      if (id < this._offset || id >= this._current)
        return;
      this._current = id;
    }
  }

  public void RemoveOnObject(T value)
  {
    try
    {
      this.RemoveOnId(this.GetIdFromObject(value));
    }
    catch
    {
    }
  }
}
