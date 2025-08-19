// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

public static class DictionaryOps
{
  public static string __repr__(CodeContext context, IDictionary<object, object> self)
  {
    List<object> andCheckInfinite = PythonOps.GetAndCheckInfinite((object) self);
    if (andCheckInfinite == null)
      return "{...}";
    int count = andCheckInfinite.Count;
    andCheckInfinite.Add((object) self);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      bool flag = true;
      foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) self)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(", ");
        if (CustomStringDictionary.IsNullObject(keyValuePair.Key))
          stringBuilder.Append("None");
        else
          stringBuilder.Append(PythonOps.Repr(context, keyValuePair.Key));
        stringBuilder.Append(": ");
        stringBuilder.Append(PythonOps.Repr(context, keyValuePair.Value));
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
    finally
    {
      andCheckInfinite.RemoveAt(count);
    }
  }

  public static object get(PythonDictionary self, object key)
  {
    return DictionaryOps.get(self, key, (object) null);
  }

  public static object get(PythonDictionary self, object key, object defaultValue)
  {
    object obj;
    return self.TryGetValueNoMissing(key, out obj) ? obj : defaultValue;
  }

  public static bool has_key(IDictionary<object, object> self, object key) => self.ContainsKey(key);

  public static List items(IDictionary<object, object> self)
  {
    List list = PythonOps.MakeEmptyList(self.Count);
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) self)
      list.AddNoLock((object) PythonTuple.MakeTuple(keyValuePair.Key, keyValuePair.Value));
    return list;
  }

  public static IEnumerator iteritems(IDictionary<object, object> self)
  {
    return DictionaryOps.items(self).GetEnumerator();
  }

  public static IEnumerator iterkeys(IDictionary<object, object> self)
  {
    return DictionaryOps.keys(self).GetEnumerator();
  }

  public static List keys(IDictionary<object, object> self)
  {
    return PythonOps.MakeListFromSequence((object) self.Keys);
  }

  public static object pop(PythonDictionary self, object key)
  {
    object obj;
    if (!self.TryGetValueNoMissing(key, out obj))
      throw PythonOps.KeyError(key);
    self.RemoveDirect(key);
    return obj;
  }

  public static object pop(PythonDictionary self, object key, object defaultValue)
  {
    object obj;
    if (!self.TryGetValueNoMissing(key, out obj))
      return defaultValue;
    self.RemoveDirect(key);
    return obj;
  }

  public static PythonTuple popitem(IDictionary<object, object> self)
  {
    IEnumerator<KeyValuePair<object, object>> enumerator = self.GetEnumerator();
    if (!enumerator.MoveNext())
      throw PythonOps.KeyError("dictionary is empty");
    object key = enumerator.Current.Key;
    object obj = enumerator.Current.Value;
    self.Remove(key);
    return PythonTuple.MakeTuple(key, obj);
  }

  public static object setdefault(PythonDictionary self, object key)
  {
    return DictionaryOps.setdefault(self, key, (object) null);
  }

  public static object setdefault(PythonDictionary self, object key, object defaultValue)
  {
    object obj;
    if (self.TryGetValueNoMissing(key, out obj))
      return obj;
    self.SetItem(key, defaultValue);
    return defaultValue;
  }

  public static void update(CodeContext context, PythonDictionary self, object other)
  {
    if (other is PythonDictionary pythonDictionary)
      pythonDictionary._storage.CopyTo(ref self._storage);
    else
      DictionaryOps.SlowUpdate(context, self, other);
  }

  private static void SlowUpdate(CodeContext context, PythonDictionary self, object other)
  {
    if (other is DictProxy dictProxy)
      DictionaryOps.update(context, self, (object) dictProxy.Type.GetMemberDictionary(context, false));
    else if (other is IDictionary dictionary)
    {
      IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
      while (enumerator.MoveNext())
        self._storage.Add(ref self._storage, enumerator.Key, enumerator.Value);
    }
    else
    {
      object ret;
      if (PythonOps.TryGetBoundAttr(other, "keys", out ret))
      {
        IEnumerator enumerator = PythonOps.GetEnumerator(PythonCalls.Call(context, ret));
        while (enumerator.MoveNext())
          self._storage.Add(ref self._storage, enumerator.Current, PythonOps.GetIndex(context, other, enumerator.Current));
      }
      else
      {
        IEnumerator enumerator = PythonOps.GetEnumerator(other);
        int num = 0;
        while (enumerator.MoveNext())
        {
          if (!DictionaryOps.AddKeyValue(self, enumerator.Current))
            throw PythonOps.ValueError("dictionary update sequence element #{0} has bad length; 2 is required", (object) num);
          ++num;
        }
      }
    }
  }

  internal static bool TryGetValueVirtual(
    CodeContext context,
    PythonDictionary self,
    object key,
    ref object DefaultGetItem,
    out object value)
  {
    if (self is IPythonObject pythonObject)
    {
      PythonType pythonType = pythonObject.PythonType;
      PythonTypeSlot slot;
      if (DefaultGetItem == null)
      {
        TypeCache.Dict.TryLookupSlot(context, "__getitem__", out slot);
        slot.TryGetValue(context, (object) self, TypeCache.Dict, out DefaultGetItem);
      }
      if (pythonType.TryLookupSlot(context, "__getitem__", out slot))
      {
        object obj;
        slot.TryGetValue(context, (object) self, pythonType, out obj);
        if (obj != DefaultGetItem)
        {
          try
          {
            value = self[key];
            return true;
          }
          catch (KeyNotFoundException ex)
          {
            value = (object) null;
            return false;
          }
        }
      }
    }
    value = (object) null;
    return false;
  }

  internal static bool AddKeyValue(PythonDictionary self, object o)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(o);
    if (enumerator.MoveNext())
    {
      object current1 = enumerator.Current;
      if (enumerator.MoveNext())
      {
        object current2 = enumerator.Current;
        self._storage.Add(ref self._storage, current1, current2);
        return !enumerator.MoveNext();
      }
    }
    return false;
  }

  internal static int CompareTo(
    CodeContext context,
    IDictionary<object, object> left,
    IDictionary<object, object> right)
  {
    int count1 = left.Count;
    int count2 = right.Count;
    if (count1 != count2)
      return count1 <= count2 ? -1 : 1;
    List ritems = DictionaryOps.items(right);
    return DictionaryOps.CompareToWorker(context, left, ritems);
  }

  internal static int CompareToWorker(
    CodeContext context,
    IDictionary<object, object> left,
    List ritems)
  {
    List list = DictionaryOps.items(left);
    list.sort(context);
    ritems.sort(context);
    return list.CompareToWorker(ritems);
  }
}
