// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.FastGetBase
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal abstract class FastGetBase
{
  internal Func<CallSite, object, CodeContext, object> _func;
  internal int _hitCount;

  public abstract bool IsValid(PythonType type);

  internal virtual bool ShouldCache => true;

  internal bool ShouldUseNonOptimizedSite => this._hitCount < 100;

  protected static object Update(CallSite site, object self, CodeContext context)
  {
    return ((CallSite<Func<CallSite, object, CodeContext, object>>) site).Update(site, self, context);
  }
}
