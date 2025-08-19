// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridItemAttachedStorage
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Collections.Generic;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class DataGridItemAttachedStorage
{
  private Dictionary<object, Dictionary<DependencyProperty, object>> _itemStorageMap;

  public void SetValue(object item, DependencyProperty property, object value)
  {
    this.EnsureItem(item)[property] = value;
  }

  public bool TryGetValue(object item, DependencyProperty property, out object value)
  {
    value = (object) null;
    this.EnsureItemStorageMap();
    Dictionary<DependencyProperty, object> dictionary;
    return this._itemStorageMap.TryGetValue(item, out dictionary) && dictionary.TryGetValue(property, out value);
  }

  public void ClearValue(object item, DependencyProperty property)
  {
    this.EnsureItemStorageMap();
    Dictionary<DependencyProperty, object> dictionary;
    if (!this._itemStorageMap.TryGetValue(item, out dictionary))
      return;
    dictionary.Remove(property);
  }

  public void ClearItem(object item)
  {
    this.EnsureItemStorageMap();
    this._itemStorageMap.Remove(item);
  }

  public void Clear()
  {
    this._itemStorageMap = (Dictionary<object, Dictionary<DependencyProperty, object>>) null;
  }

  private void EnsureItemStorageMap()
  {
    if (this._itemStorageMap != null)
      return;
    this._itemStorageMap = new Dictionary<object, Dictionary<DependencyProperty, object>>();
  }

  private Dictionary<DependencyProperty, object> EnsureItem(object item)
  {
    this.EnsureItemStorageMap();
    Dictionary<DependencyProperty, object> dictionary;
    if (!this._itemStorageMap.TryGetValue(item, out dictionary))
    {
      dictionary = new Dictionary<DependencyProperty, object>();
      this._itemStorageMap[item] = dictionary;
    }
    return dictionary;
  }
}
