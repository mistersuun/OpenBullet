// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ObjectDictionaryExpando
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

public sealed class ObjectDictionaryExpando : IDynamicMetaObjectProvider
{
  private readonly IDictionary<object, object> _data;

  public ObjectDictionaryExpando(IDictionary<object, object> dictionary) => this._data = dictionary;

  public IDictionary<object, object> Dictionary => this._data;

  private static object TryGetMember(object adapter, string name)
  {
    object obj;
    return ((ObjectDictionaryExpando) adapter)._data.TryGetValue((object) name, out obj) ? obj : StringDictionaryExpando._getFailed;
  }

  private static void TrySetMember(object adapter, string name, object value)
  {
    ((ObjectDictionaryExpando) adapter)._data[(object) name] = value;
  }

  private static bool TryDeleteMember(object adapter, string name)
  {
    return ((ObjectDictionaryExpando) adapter)._data.Remove((object) name);
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new DictionaryExpandoMetaObject(parameter, (object) this, (IEnumerable) this._data.Keys, new Func<object, string, object>(ObjectDictionaryExpando.TryGetMember), new Action<object, string, object>(ObjectDictionaryExpando.TrySetMember), new Func<object, string, bool>(ObjectDictionaryExpando.TryDeleteMember));
  }
}
