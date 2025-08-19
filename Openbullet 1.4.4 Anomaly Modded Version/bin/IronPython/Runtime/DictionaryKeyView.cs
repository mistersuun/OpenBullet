// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryKeyView
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

[PythonType("dict_keys")]
public sealed class DictionaryKeyView : 
  ICollection<object>,
  IEnumerable<object>,
  IEnumerable,
  ICodeFormattable
{
  internal readonly PythonDictionary _dict;

  internal DictionaryKeyView(PythonDictionary dict) => this._dict = dict;

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator GetEnumerator() => this._dict.iterkeys();

  IEnumerator<object> IEnumerable<object>.GetEnumerator()
  {
    return (IEnumerator<object>) new DictionaryKeyEnumerator(this._dict._storage);
  }

  void ICollection<object>.Add(object key)
  {
    throw new NotSupportedException("Collection is read-only");
  }

  void ICollection<object>.Clear() => throw new NotSupportedException("Collection is read-only");

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(object key) => this._dict.__contains__(key);

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(object[] array, int arrayIndex)
  {
    int num = arrayIndex;
    foreach (object obj in this)
    {
      array[num++] = obj;
      if (num >= array.Length)
        break;
    }
  }

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get => this._dict.Count;
  }

  public bool IsReadOnly
  {
    [PythonHidden(new PlatformID[] {})] get => true;
  }

  bool ICollection<object>.Remove(object item)
  {
    throw new NotSupportedException("Collection is read-only");
  }

  public static SetCollection operator |(DictionaryKeyView x, IEnumerable y)
  {
    return new SetCollection(SetStorage.Union(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator |(IEnumerable y, DictionaryKeyView x)
  {
    return new SetCollection(SetStorage.Union(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator &(DictionaryKeyView x, IEnumerable y)
  {
    return new SetCollection(SetStorage.Intersection(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator &(IEnumerable y, DictionaryKeyView x)
  {
    return new SetCollection(SetStorage.Intersection(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator ^(DictionaryKeyView x, IEnumerable y)
  {
    return new SetCollection(SetStorage.SymmetricDifference(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator ^(IEnumerable y, DictionaryKeyView x)
  {
    return new SetCollection(SetStorage.SymmetricDifference(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator -(DictionaryKeyView x, IEnumerable y)
  {
    return new SetCollection(SetStorage.Difference(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public static SetCollection operator -(IEnumerable y, DictionaryKeyView x)
  {
    return new SetCollection(SetStorage.Difference(SetStorage.GetItemsWorker(x.GetEnumerator()), SetStorage.GetItems((object) y)));
  }

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    if ((object) (obj as DictionaryKeyView) != null)
      return this == (DictionaryKeyView) obj;
    if ((object) (obj as DictionaryItemView) != null)
      return this == (DictionaryItemView) obj;
    switch (obj)
    {
      case SetCollection _:
        return this == (SetCollection) obj;
      case FrozenSetCollection _:
        return this == (FrozenSetCollection) obj;
      default:
        return false;
    }
  }

  public static bool operator ==(DictionaryKeyView x, DictionaryKeyView y)
  {
    if (x._dict == y._dict)
      return true;
    SetStorage itemsWorker1 = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage itemsWorker2 = SetStorage.GetItemsWorker(y.GetEnumerator());
    return itemsWorker1.Count == itemsWorker2.Count && itemsWorker1.IsSubset(itemsWorker2);
  }

  public static bool operator !=(DictionaryKeyView x, DictionaryKeyView y)
  {
    if (x._dict == y._dict)
      return false;
    SetStorage itemsWorker1 = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage itemsWorker2 = SetStorage.GetItemsWorker(y.GetEnumerator());
    return itemsWorker1.Count != itemsWorker2.Count || !itemsWorker1.IsSubset(itemsWorker2);
  }

  public static bool operator >(DictionaryKeyView x, DictionaryKeyView y)
  {
    if (x._dict == y._dict)
      return false;
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return SetStorage.GetItemsWorker(y.GetEnumerator()).IsStrictSubset(itemsWorker);
  }

  public static bool operator <(DictionaryKeyView x, DictionaryKeyView y)
  {
    return x._dict != y._dict && SetStorage.GetItemsWorker(x.GetEnumerator()).IsStrictSubset(SetStorage.GetItemsWorker(y.GetEnumerator()));
  }

  public static bool operator >=(DictionaryKeyView x, DictionaryKeyView y)
  {
    if (x._dict == y._dict)
      return true;
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return SetStorage.GetItemsWorker(y.GetEnumerator()).IsSubset(itemsWorker);
  }

  public static bool operator <=(DictionaryKeyView x, DictionaryKeyView y)
  {
    return x._dict == y._dict || SetStorage.GetItemsWorker(x.GetEnumerator()).IsSubset(SetStorage.GetItemsWorker(y.GetEnumerator()));
  }

  public static bool operator ==(DictionaryKeyView x, DictionaryItemView y)
  {
    if (x._dict == y._dict)
      return false;
    SetStorage itemsWorker1 = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage itemsWorker2 = SetStorage.GetItemsWorker(y.GetEnumerator());
    return itemsWorker1.Count == itemsWorker2.Count && itemsWorker1.IsSubset(itemsWorker2);
  }

  public static bool operator !=(DictionaryKeyView x, DictionaryItemView y)
  {
    if (x._dict == y._dict)
      return true;
    SetStorage itemsWorker1 = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage itemsWorker2 = SetStorage.GetItemsWorker(y.GetEnumerator());
    return itemsWorker1.Count != itemsWorker2.Count || !itemsWorker1.IsSubset(itemsWorker2);
  }

  public static bool operator >(DictionaryKeyView x, DictionaryItemView y)
  {
    if (x._dict == y._dict)
      return true;
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return SetStorage.GetItemsWorker(y.GetEnumerator()).IsStrictSubset(itemsWorker);
  }

  public static bool operator <(DictionaryKeyView x, DictionaryItemView y)
  {
    return x._dict == y._dict || SetStorage.GetItemsWorker(x.GetEnumerator()).IsStrictSubset(SetStorage.GetItemsWorker(y.GetEnumerator()));
  }

  public static bool operator >=(DictionaryKeyView x, DictionaryItemView y)
  {
    if (x._dict == y._dict)
      return false;
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return SetStorage.GetItemsWorker(y.GetEnumerator()).IsSubset(itemsWorker);
  }

  public static bool operator <=(DictionaryKeyView x, DictionaryItemView y)
  {
    return x._dict != y._dict && SetStorage.GetItemsWorker(x.GetEnumerator()).IsSubset(SetStorage.GetItemsWorker(y.GetEnumerator()));
  }

  public static bool operator ==(DictionaryKeyView x, SetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage items = y._items;
    return itemsWorker.Count == items.Count && itemsWorker.IsSubset(items);
  }

  public static bool operator !=(DictionaryKeyView x, SetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage items = y._items;
    return itemsWorker.Count != items.Count || !itemsWorker.IsSubset(items);
  }

  public static bool operator >(DictionaryKeyView x, SetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return y._items.IsStrictSubset(itemsWorker);
  }

  public static bool operator <(DictionaryKeyView x, SetCollection y)
  {
    return SetStorage.GetItemsWorker(x.GetEnumerator()).IsStrictSubset(y._items);
  }

  public static bool operator >=(DictionaryKeyView x, SetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return y._items.IsSubset(itemsWorker);
  }

  public static bool operator <=(DictionaryKeyView x, SetCollection y)
  {
    return SetStorage.GetItemsWorker(x.GetEnumerator()).IsSubset(y._items);
  }

  public static bool operator ==(DictionaryKeyView x, FrozenSetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage items = y._items;
    return itemsWorker.Count == items.Count && itemsWorker.IsSubset(items);
  }

  public static bool operator !=(DictionaryKeyView x, FrozenSetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    SetStorage items = y._items;
    return itemsWorker.Count != items.Count || !itemsWorker.IsSubset(items);
  }

  public static bool operator >(DictionaryKeyView x, FrozenSetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return y._items.IsStrictSubset(itemsWorker);
  }

  public static bool operator <(DictionaryKeyView x, FrozenSetCollection y)
  {
    return SetStorage.GetItemsWorker(x.GetEnumerator()).IsStrictSubset(y._items);
  }

  public static bool operator >=(DictionaryKeyView x, FrozenSetCollection y)
  {
    SetStorage itemsWorker = SetStorage.GetItemsWorker(x.GetEnumerator());
    return y._items.IsSubset(itemsWorker);
  }

  public static bool operator <=(DictionaryKeyView x, FrozenSetCollection y)
  {
    return SetStorage.GetItemsWorker(x.GetEnumerator()).IsSubset(y._items);
  }

  public string __repr__(CodeContext context)
  {
    StringBuilder stringBuilder = new StringBuilder(20);
    stringBuilder.Append("dict_keys([");
    string str = "";
    foreach (object o in this)
    {
      stringBuilder.Append(str);
      str = ", ";
      stringBuilder.Append(PythonOps.Repr(context, o));
    }
    stringBuilder.Append("])");
    return stringBuilder.ToString();
  }

  public override int GetHashCode() => base.GetHashCode();
}
