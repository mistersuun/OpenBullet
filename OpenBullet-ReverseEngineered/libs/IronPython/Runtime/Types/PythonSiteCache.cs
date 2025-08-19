// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonSiteCache
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

internal class PythonSiteCache
{
  private Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>> _tryGetMemSite;
  private Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>> _tryGetMemSiteShowCls;
  private CallSite<Func<CallSite, CodeContext, object, object>> _dirSite;
  private CallSite<Func<CallSite, CodeContext, object, string, object>> _getAttributeSite;
  private CallSite<Func<CallSite, CodeContext, object, object, string, object, object>> _setAttrSite;
  private CallSite<Func<CallSite, CodeContext, object, object>> _lenSite;

  internal CallSite<Func<CallSite, object, CodeContext, object>> GetTryGetMemberSite(
    CodeContext context,
    string name)
  {
    CallSite<Func<CallSite, object, CodeContext, object>> tryGetMemberSite;
    if (PythonOps.IsClsVisible(context))
    {
      if (this._tryGetMemSiteShowCls == null)
        Interlocked.CompareExchange<Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>>>(ref this._tryGetMemSiteShowCls, new Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>>((IEqualityComparer<string>) StringComparer.Ordinal), (Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>>) null);
      lock (this._tryGetMemSiteShowCls)
      {
        if (!this._tryGetMemSiteShowCls.TryGetValue(name, out tryGetMemberSite))
          this._tryGetMemSiteShowCls[name] = tryGetMemberSite = CallSite<Func<CallSite, object, CodeContext, object>>.Create((CallSiteBinder) context.LanguageContext.GetMember(name, true));
      }
    }
    else
    {
      if (this._tryGetMemSite == null)
        Interlocked.CompareExchange<Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>>>(ref this._tryGetMemSite, new Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>>((IEqualityComparer<string>) StringComparer.Ordinal), (Dictionary<string, CallSite<Func<CallSite, object, CodeContext, object>>>) null);
      lock (this._tryGetMemSite)
      {
        if (!this._tryGetMemSite.TryGetValue(name, out tryGetMemberSite))
          this._tryGetMemSite[name] = tryGetMemberSite = CallSite<Func<CallSite, object, CodeContext, object>>.Create((CallSiteBinder) context.LanguageContext.GetMember(name, true));
      }
    }
    return tryGetMemberSite;
  }

  internal CallSite<Func<CallSite, CodeContext, object, object>> GetDirSite(CodeContext context)
  {
    if (this._dirSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>>(ref this._dirSite, CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeNone), (CallSite<Func<CallSite, CodeContext, object, object>>) null);
    return this._dirSite;
  }

  internal CallSite<Func<CallSite, CodeContext, object, string, object>> GetGetAttributeSite(
    CodeContext context)
  {
    if (this._getAttributeSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, string, object>>>(ref this._getAttributeSite, CallSite<Func<CallSite, CodeContext, object, string, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeOne), (CallSite<Func<CallSite, CodeContext, object, string, object>>) null);
    return this._getAttributeSite;
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, string, object, object>> GetSetAttrSite(
    CodeContext context)
  {
    if (this._setAttrSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, string, object, object>>>(ref this._setAttrSite, CallSite<Func<CallSite, CodeContext, object, object, string, object, object>>.Create((CallSiteBinder) context.LanguageContext.Invoke(new CallSignature(4))), (CallSite<Func<CallSite, CodeContext, object, object, string, object, object>>) null);
    return this._setAttrSite;
  }

  internal CallSite<Func<CallSite, CodeContext, object, object>> GetLenSite(CodeContext context)
  {
    if (this._lenSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>>(ref this._lenSite, CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeNone), (CallSite<Func<CallSite, CodeContext, object, object>>) null);
    return this._lenSite;
  }
}
