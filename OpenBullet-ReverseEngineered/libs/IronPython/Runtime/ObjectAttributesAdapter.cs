// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ObjectAttributesAdapter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class ObjectAttributesAdapter : DictionaryStorage
{
  private readonly object _backing;
  private readonly CodeContext _context;

  public ObjectAttributesAdapter(CodeContext context, object backing)
  {
    this._backing = backing;
    this._context = context;
  }

  internal object Backing => this._backing;

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    this._context.LanguageContext.SetIndex(this._backing, key, value);
  }

  public override bool Contains(object key) => this.TryGetValue(key, out object _);

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    try
    {
      this._context.LanguageContext.DelIndex(this._backing, key);
      return true;
    }
    catch (KeyNotFoundException ex)
    {
      return false;
    }
  }

  public override bool TryGetValue(object key, out object value)
  {
    try
    {
      value = PythonOps.GetIndex(this._context, this._backing, key);
      return true;
    }
    catch (KeyNotFoundException ex)
    {
    }
    value = (object) null;
    return false;
  }

  public override int Count => PythonOps.Length(this._backing);

  public override void Clear(ref DictionaryStorage storage)
  {
    PythonOps.Invoke(this._context, this._backing, "clear");
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>();
    foreach (object key in (IEnumerable<object>) this.Keys)
    {
      object obj;
      this.TryGetValue(key, out obj);
      items.Add(new KeyValuePair<object, object>(key, obj));
    }
    return items;
  }

  private ICollection<object> Keys
  {
    get
    {
      return (ICollection<object>) Converter.Convert(PythonOps.Invoke(this._context, this._backing, "keys"), typeof (ICollection<object>));
    }
  }
}
