// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.TypeGet
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

internal class TypeGet : TypeGetBase
{
  private int _version;

  public TypeGet(
    PythonGetMemberBinder binder,
    FastGetDelegate[] delegates,
    int version,
    bool isMeta,
    bool canOptimize)
    : base(binder, delegates)
  {
    this._version = version;
    if (canOptimize)
    {
      if (isMeta)
        this._func = new Func<CallSite, object, CodeContext, object>(this.MetaOnlyTargetOptimizing);
      else
        this._func = new Func<CallSite, object, CodeContext, object>(this.TargetOptimizing);
    }
    else if (isMeta)
      this._func = new Func<CallSite, object, CodeContext, object>(this.MetaOnlyTarget);
    else
      this._func = new Func<CallSite, object, CodeContext, object>(this.Target);
  }

  public object Target(CallSite site, object self, CodeContext context)
  {
    return self is PythonType pythonType && pythonType.Version == this._version ? this.RunDelegatesNoOptimize(self, context) : FastGetBase.Update(site, self, context);
  }

  public object MetaOnlyTarget(CallSite site, object self, CodeContext context)
  {
    return self is PythonType o && PythonOps.CheckTypeVersion((object) o, this._version) ? this.RunDelegatesNoOptimize(self, context) : FastGetBase.Update(site, self, context);
  }

  public object TargetOptimizing(CallSite site, object self, CodeContext context)
  {
    return self is PythonType pythonType && pythonType.Version == this._version && this.ShouldUseNonOptimizedSite ? this.RunDelegates(self, context) : FastGetBase.Update(site, self, context);
  }

  public object MetaOnlyTargetOptimizing(CallSite site, object self, CodeContext context)
  {
    return self is PythonType o && PythonOps.CheckTypeVersion((object) o, this._version) && this.ShouldUseNonOptimizedSite ? this.RunDelegates(self, context) : FastGetBase.Update(site, self, context);
  }

  public override bool IsValid(PythonType type)
  {
    return this._func == new Func<CallSite, object, CodeContext, object>(this.MetaOnlyTarget) || this._func == new Func<CallSite, object, CodeContext, object>(this.MetaOnlyTargetOptimizing) ? PythonOps.CheckTypeVersion((object) type, this._version) : type.Version == this._version;
  }
}
