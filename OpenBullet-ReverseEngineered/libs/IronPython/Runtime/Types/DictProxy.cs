// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.DictProxy
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("dictproxy")]
public class DictProxy : 
  IDictionary,
  ICollection,
  IEnumerable,
  IDictionary<object, object>,
  ICollection<KeyValuePair<object, object>>,
  IEnumerable<KeyValuePair<object, object>>
{
  private readonly PythonType _dt;

  public DictProxy(PythonType dt) => this._dt = dt;

  public int __len__(CodeContext context) => this._dt.GetMemberDictionary(context, false).Count;

  public bool __contains__(CodeContext context, object value) => this.has_key(context, value);

  public string __str__(CodeContext context)
  {
    return DictionaryOps.__repr__(context, (IDictionary<object, object>) this);
  }

  public bool has_key(CodeContext context, object key)
  {
    return this.TryGetValue(context, key, out object _);
  }

  public object get(CodeContext context, [NotNull] object k, object d = null)
  {
    object obj;
    if (!this.TryGetValue(context, k, out obj))
      obj = d;
    return obj;
  }

  public object keys(CodeContext context)
  {
    return (object) this._dt.GetMemberDictionary(context, false).keys();
  }

  public object values(CodeContext context)
  {
    return (object) this._dt.GetMemberDictionary(context, false).values();
  }

  public List items(CodeContext context) => this._dt.GetMemberDictionary(context, false).items();

  public PythonDictionary copy(CodeContext context) => new PythonDictionary(context, (object) this);

  public IEnumerator iteritems(CodeContext context)
  {
    return (IEnumerator) new DictionaryItemEnumerator(this._dt.GetMemberDictionary(context, false)._storage);
  }

  public IEnumerator iterkeys(CodeContext context)
  {
    return (IEnumerator) new DictionaryKeyEnumerator(this._dt.GetMemberDictionary(context, false)._storage);
  }

  public IEnumerator itervalues(CodeContext context)
  {
    return (IEnumerator) new DictionaryValueEnumerator(this._dt.GetMemberDictionary(context, false)._storage);
  }

  public override bool Equals(object obj)
  {
    return obj is DictProxy dictProxy && dictProxy._dt == this._dt;
  }

  public override int GetHashCode() => ~this._dt.GetHashCode();

  public object this[object key]
  {
    get => this.GetIndex(DefaultContext.Default, key);
    [PythonHidden(new PlatformID[] {})] set
    {
      throw PythonOps.TypeError("cannot assign to dictproxy");
    }
  }

  bool IDictionary.Contains(object key) => this.has_key(DefaultContext.Default, key);

  IEnumerator IEnumerable.GetEnumerator()
  {
    return DictionaryOps.iterkeys((IDictionary<object, object>) this._dt.GetMemberDictionary(DefaultContext.Default, false));
  }

  [PythonHidden(new PlatformID[] {})]
  public void Add(object key, object value) => this[key] = value;

  [PythonHidden(new PlatformID[] {})]
  public void Clear() => throw new InvalidOperationException("dictproxy is read-only");

  IDictionaryEnumerator IDictionary.GetEnumerator()
  {
    return (IDictionaryEnumerator) new PythonDictionary.DictEnumerator(this._dt.GetMemberDictionary(DefaultContext.Default, false).GetEnumerator());
  }

  bool IDictionary.IsFixedSize => true;

  bool IDictionary.IsReadOnly => true;

  ICollection IDictionary.Keys
  {
    get
    {
      ICollection<object> keys = this._dt.GetMemberDictionary(DefaultContext.Default, false).Keys;
      return keys is ICollection collection ? collection : (ICollection) new List<object>((IEnumerable<object>) keys);
    }
  }

  void IDictionary.Remove(object key)
  {
    throw new InvalidOperationException("dictproxy is read-only");
  }

  ICollection IDictionary.Values
  {
    get
    {
      List<object> values = new List<object>();
      foreach (KeyValuePair<object, object> member in this._dt.GetMemberDictionary(DefaultContext.Default, false))
        values.Add(member.Value);
      return (ICollection) values;
    }
  }

  void ICollection.CopyTo(Array array, int index)
  {
    foreach (DictionaryEntry dictionaryEntry in (IDictionary) this)
      array.SetValue((object) dictionaryEntry, index++);
  }

  int ICollection.Count => this.__len__(DefaultContext.Default);

  bool ICollection.IsSynchronized => false;

  object ICollection.SyncRoot => (object) this;

  bool IDictionary<object, object>.ContainsKey(object key)
  {
    return this.has_key(DefaultContext.Default, key);
  }

  ICollection<object> IDictionary<object, object>.Keys
  {
    get => this._dt.GetMemberDictionary(DefaultContext.Default, false).Keys;
  }

  bool IDictionary<object, object>.Remove(object key)
  {
    throw new InvalidOperationException("dictproxy is read-only");
  }

  bool IDictionary<object, object>.TryGetValue(object key, out object value)
  {
    return this.TryGetValue(DefaultContext.Default, key, out value);
  }

  ICollection<object> IDictionary<object, object>.Values
  {
    get => this._dt.GetMemberDictionary(DefaultContext.Default, false).Values;
  }

  void ICollection<KeyValuePair<object, object>>.Add(KeyValuePair<object, object> item)
  {
    this[item.Key] = item.Value;
  }

  bool ICollection<KeyValuePair<object, object>>.Contains(KeyValuePair<object, object> item)
  {
    return this.has_key(DefaultContext.Default, item.Key);
  }

  void ICollection<KeyValuePair<object, object>>.CopyTo(
    KeyValuePair<object, object>[] array,
    int arrayIndex)
  {
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) this)
      array.SetValue((object) keyValuePair, arrayIndex++);
  }

  int ICollection<KeyValuePair<object, object>>.Count => this.__len__(DefaultContext.Default);

  bool ICollection<KeyValuePair<object, object>>.IsReadOnly => true;

  bool ICollection<KeyValuePair<object, object>>.Remove(KeyValuePair<object, object> item)
  {
    return ((IDictionary<object, object>) this).Remove(item.Key);
  }

  IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
  {
    return this._dt.GetMemberDictionary(DefaultContext.Default, false).GetEnumerator();
  }

  private object GetIndex(CodeContext context, object index)
  {
    PythonTypeSlot slot;
    if (!(index is string name) || !this._dt.TryLookupSlot(context, name, out slot))
      throw PythonOps.KeyError(index.ToString());
    return slot is PythonTypeUserDescriptorSlot userDescriptorSlot ? userDescriptorSlot.Value : (object) slot;
  }

  private bool TryGetValue(CodeContext context, object key, out object value)
  {
    PythonTypeSlot slot;
    if (key is string name && this._dt.TryLookupSlot(context, name, out slot))
    {
      if (slot is PythonTypeUserDescriptorSlot userDescriptorSlot)
      {
        value = userDescriptorSlot.Value;
        return true;
      }
      value = (object) slot;
      return true;
    }
    value = (object) null;
    return false;
  }

  internal PythonType Type => this._dt;
}
