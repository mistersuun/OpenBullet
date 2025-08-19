// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedIndexer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("indexer#")]
public sealed class ReflectedIndexer : ReflectedGetterSetter
{
  private readonly object _instance;
  private readonly PropertyInfo _info;

  public ReflectedIndexer(PropertyInfo info, NameType nt, bool privateBinding)
    : base(new MethodInfo[1]
    {
      info.GetGetMethod(privateBinding)
    }, new MethodInfo[1]
    {
      info.GetSetMethod(privateBinding)
    }, nt)
  {
    this._info = info;
  }

  public ReflectedIndexer(ReflectedIndexer from, object instance)
    : base((ReflectedGetterSetter) from)
  {
    this._instance = instance;
    this._info = from._info;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = (object) new ReflectedIndexer(this, instance);
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override Type DeclaringType => this._info.DeclaringType;

  public override PythonType PropertyType
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      return DynamicHelpers.GetPythonTypeFromType(this._info.PropertyType);
    }
  }

  public override string __name__ => this._info.Name;

  public bool SetValue(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    object[] keys,
    object value)
  {
    return this.CallSetter(context, storage, this._instance, keys, value);
  }

  public object GetValue(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    object[] keys)
  {
    return this.CallGetter(context, storage, this._instance, keys);
  }

  public new object __get__(CodeContext context, object instance, object owner)
  {
    object obj;
    this.TryGetValue(context, instance, owner as PythonType, out obj);
    return obj;
  }

  public object this[
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    params object[] key]
  {
    get => this.GetValue(DefaultContext.Default, storage, key);
    set
    {
      if (!this.SetValue(DefaultContext.Default, storage, key, value))
        throw PythonOps.AttributeErrorForReadonlyAttribute(this.DeclaringType.Name, this.__name__);
    }
  }
}
