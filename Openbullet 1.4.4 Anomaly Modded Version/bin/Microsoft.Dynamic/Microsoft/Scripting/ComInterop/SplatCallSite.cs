// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.SplatCallSite
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class SplatCallSite
{
  internal readonly object _callable;
  internal CallSite<Func<CallSite, object, object[], object>> _site;

  internal SplatCallSite(object callable) => this._callable = callable;

  internal object Invoke(object[] args)
  {
    if (this._callable is Delegate callable)
      return callable.DynamicInvoke(args);
    if (this._site == null)
      this._site = CallSite<Func<CallSite, object, object[], object>>.Create((CallSiteBinder) SplatInvokeBinder.Instance);
    return this._site.Target((CallSite) this._site, this._callable, args);
  }
}
