// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CacheDict`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class CacheDict<TKey, TValue>
{
  private readonly Dictionary<TKey, CacheDict<TKey, TValue>.KeyInfo> _dict = new Dictionary<TKey, CacheDict<TKey, TValue>.KeyInfo>();
  private readonly LinkedList<TKey> _list = new LinkedList<TKey>();
  private readonly int _maxSize;

  public CacheDict(int maxSize) => this._maxSize = maxSize;

  public bool TryGetValue(TKey key, out TValue value)
  {
    CacheDict<TKey, TValue>.KeyInfo keyInfo;
    if (this._dict.TryGetValue(key, out keyInfo))
    {
      LinkedListNode<TKey> list = keyInfo.List;
      if (list.Previous != null)
      {
        this._list.Remove(list);
        this._list.AddFirst(list);
      }
      value = keyInfo.Value;
      return true;
    }
    value = default (TValue);
    return false;
  }

  public void Add(TKey key, TValue value)
  {
    CacheDict<TKey, TValue>.KeyInfo keyInfo;
    if (this._dict.TryGetValue(key, out keyInfo))
      this._list.Remove(keyInfo.List);
    else if (this._list.Count == this._maxSize)
    {
      LinkedListNode<TKey> last = this._list.Last;
      this._list.RemoveLast();
      this._dict.Remove(last.Value);
    }
    LinkedListNode<TKey> linkedListNode = new LinkedListNode<TKey>(key);
    this._list.AddFirst(linkedListNode);
    this._dict[key] = new CacheDict<TKey, TValue>.KeyInfo(value, linkedListNode);
  }

  public TValue this[TKey key]
  {
    get
    {
      TValue obj;
      if (this.TryGetValue(key, out obj))
        return obj;
      throw new KeyNotFoundException();
    }
    set => this.Add(key, value);
  }

  private struct KeyInfo
  {
    internal readonly TValue Value;
    internal readonly LinkedListNode<TKey> List;

    internal KeyInfo(TValue value, LinkedListNode<TKey> list)
    {
      this.Value = value;
      this.List = list;
    }
  }
}
