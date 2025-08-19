// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.WeakDictionary`2
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class WeakDictionary<K, V> where K : class
{
  private List<WeakReference> _keys = new List<WeakReference>();
  private List<V> _values = new List<V>();

  public V this[K key]
  {
    get
    {
      V v;
      if (!this.GetValue(key, out v))
        throw new ArgumentException();
      return v;
    }
    set => this.SetValue(key, value);
  }

  public bool ContainsKey(K key)
  {
    this.CollectGarbage();
    return -1 != this._keys.FindIndex((Predicate<WeakReference>) (k => (object) k.GetValueOrDefault<K>() == (object) key));
  }

  public void SetValue(K key, V value)
  {
    this.CollectGarbage();
    int index = this._keys.FindIndex((Predicate<WeakReference>) (k => (object) k.GetValueOrDefault<K>() == (object) key));
    if (index > -1)
    {
      this._values[index] = value;
    }
    else
    {
      this._values.Add(value);
      this._keys.Add(new WeakReference((object) key));
    }
  }

  public bool GetValue(K key, out V value)
  {
    this.CollectGarbage();
    int index = this._keys.FindIndex((Predicate<WeakReference>) (k => (object) k.GetValueOrDefault<K>() == (object) key));
    value = default (V);
    if (index == -1)
      return false;
    value = this._values[index];
    return true;
  }

  private void CollectGarbage()
  {
    int num = 0;
    do
    {
      num = this._keys.FindIndex(num, (Predicate<WeakReference>) (k => !k.IsAlive));
      if (num >= 0)
      {
        this._keys.RemoveAt(num);
        this._values.RemoveAt(num);
      }
    }
    while (num >= 0);
  }
}
