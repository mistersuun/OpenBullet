// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ScopeDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class ScopeDictionaryStorage : DictionaryStorage
{
  private readonly Scope _scope;
  private readonly PythonContext _context;

  public ScopeDictionaryStorage(PythonContext context, Scope scope)
  {
    this._scope = scope;
    this._context = context;
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    if (key is string name)
      PythonOps.ScopeSetMember(this._context.SharedContext, this._scope, name, value);
    else
      ((PythonScopeExtension) this._context.EnsureScopeExtension(this._scope)).EnsureObjectKeys().Add(key, value);
  }

  public override bool Contains(object key) => this.TryGetValue(key, out object _);

  public override bool Remove(ref DictionaryStorage storage, object key) => this.Remove(key);

  private bool Remove(object key)
  {
    if (key is string name)
      return this.Contains(key) && PythonOps.ScopeDeleteMember(this._context.SharedContext, this.Scope, name);
    PythonScopeExtension pythonScopeExtension = (PythonScopeExtension) this._context.EnsureScopeExtension(this._scope);
    return pythonScopeExtension.ObjectKeys != null && pythonScopeExtension.ObjectKeys.Remove(key);
  }

  public override bool TryGetValue(object key, out object value)
  {
    if (key is string name)
      return PythonOps.ScopeTryGetMember(this._context.SharedContext, this._scope, name, out value);
    PythonScopeExtension pythonScopeExtension = (PythonScopeExtension) this._context.EnsureScopeExtension(this._scope);
    if (pythonScopeExtension.ObjectKeys != null && pythonScopeExtension.ObjectKeys.TryGetValue(key, out value))
      return true;
    value = (object) null;
    return false;
  }

  public override int Count => this.GetItems().Count;

  public override void Clear(ref DictionaryStorage storage)
  {
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
      this.Remove(keyValuePair.Key);
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>();
    foreach (object memberName in (IEnumerable<object>) PythonOps.ScopeGetMemberNames(this._context.SharedContext, this._scope))
    {
      object obj;
      if (this.TryGetValue(memberName, out obj))
        items.Add(new KeyValuePair<object, object>(memberName, obj));
    }
    return items;
  }

  internal Scope Scope => this._scope;
}
