// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.StringDictionaryExpando
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class StringDictionaryExpando : IDynamicMetaObjectProvider
{
  private readonly IDictionary<string, object> _data;
  internal static readonly object _getFailed = new object();

  public StringDictionaryExpando(IDictionary<string, object> data) => this._data = data;

  public IDictionary<string, object> Dictionary => this._data;

  private static object TryGetMember(object adapter, string name)
  {
    object obj;
    return ((StringDictionaryExpando) adapter)._data.TryGetValue(name, out obj) ? obj : StringDictionaryExpando._getFailed;
  }

  private static void TrySetMember(object adapter, string name, object value)
  {
    ((StringDictionaryExpando) adapter)._data[name] = value;
  }

  private static bool TryDeleteMember(object adapter, string name)
  {
    return ((StringDictionaryExpando) adapter)._data.Remove(name);
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new DictionaryExpandoMetaObject(parameter, (object) this, (IEnumerable) this._data.Keys, new Func<object, string, object>(StringDictionaryExpando.TryGetMember), new Action<object, string, object>(StringDictionaryExpando.TrySetMember), new Func<object, string, bool>(StringDictionaryExpando.TryDeleteMember));
  }
}
