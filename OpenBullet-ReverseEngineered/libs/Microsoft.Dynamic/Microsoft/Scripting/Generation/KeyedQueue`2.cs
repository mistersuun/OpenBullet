// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.KeyedQueue`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal sealed class KeyedQueue<K, V>
{
  private readonly Dictionary<K, Queue<V>> _data;

  internal KeyedQueue() => this._data = new Dictionary<K, Queue<V>>();

  internal void Enqueue(K key, V value)
  {
    Queue<V> vQueue;
    if (!this._data.TryGetValue(key, out vQueue))
      this._data.Add(key, vQueue = new Queue<V>());
    vQueue.Enqueue(value);
  }

  internal V Dequeue(K key)
  {
    Queue<V> vQueue;
    if (!this._data.TryGetValue(key, out vQueue))
      throw Error.QueueEmpty();
    V v = vQueue.Dequeue();
    if (vQueue.Count != 0)
      return v;
    this._data.Remove(key);
    return v;
  }

  internal bool TryDequeue(K key, out V value)
  {
    Queue<V> vQueue;
    if (this._data.TryGetValue(key, out vQueue) && vQueue.Count > 0)
    {
      value = vQueue.Dequeue();
      if (vQueue.Count == 0)
        this._data.Remove(key);
      return true;
    }
    value = default (V);
    return false;
  }

  internal V Peek(K key)
  {
    Queue<V> vQueue;
    if (!this._data.TryGetValue(key, out vQueue))
      throw Error.QueueEmpty();
    return vQueue.Peek();
  }

  internal int GetCount(K key)
  {
    Queue<V> vQueue;
    return !this._data.TryGetValue(key, out vQueue) ? 0 : vQueue.Count;
  }

  internal void Clear() => this._data.Clear();
}
