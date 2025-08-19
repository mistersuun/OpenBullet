// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.FullWeakDictionary`2
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class FullWeakDictionary<K, V> where K : class
{
  private List<WeakReference> _keys = new List<WeakReference>();
  private List<WeakReference> _values = new List<WeakReference>();

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
      this._values[index] = new WeakReference((object) value);
    }
    else
    {
      this._values.Add(new WeakReference((object) value));
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
    value = this._values[index].GetValueOrDefault<V>();
    return true;
  }

  private void CollectGarbage()
  {
    int num1 = 0;
    do
    {
      num1 = this._keys.FindIndex(num1, (Predicate<WeakReference>) (k => !k.IsAlive));
      if (num1 >= 0)
      {
        this._keys.RemoveAt(num1);
        this._values.RemoveAt(num1);
      }
    }
    while (num1 >= 0);
    int num2 = 0;
    do
    {
      num2 = this._values.FindIndex(num2, (Predicate<WeakReference>) (v => !v.IsAlive));
      if (num2 >= 0)
      {
        this._values.RemoveAt(num2);
        this._keys.RemoveAt(num2);
      }
    }
    while (num2 >= 0);
  }
}
