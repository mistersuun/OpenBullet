// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.SystemInstanceCreator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

internal class SystemInstanceCreator(PythonType type) : InstanceCreator(type)
{
  private CallSite<Func<CallSite, CodeContext, BuiltinFunction, object[], object>> _ctorSite;
  private CallSite<Func<CallSite, CodeContext, BuiltinFunction, object>> _ctorSite0;
  private CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>> _ctorSite1;
  private CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object>> _ctorSite2;
  private CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object, object>> _ctorSite3;

  internal override object CreateInstance(CodeContext context)
  {
    if (this._ctorSite0 == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object>>>(ref this._ctorSite0, CallSite<Func<CallSite, CodeContext, BuiltinFunction, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeNone), (CallSite<Func<CallSite, CodeContext, BuiltinFunction, object>>) null);
    return this._ctorSite0.Target((CallSite) this._ctorSite0, context, this.Type.Ctor);
  }

  internal override object CreateInstance(CodeContext context, object arg0)
  {
    if (this._ctorSite1 == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>>>(ref this._ctorSite1, CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>>.Create((CallSiteBinder) context.LanguageContext.Invoke(new CallSignature(1))), (CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>>) null);
    return this._ctorSite1.Target((CallSite) this._ctorSite1, context, this.Type.Ctor, arg0);
  }

  internal override object CreateInstance(CodeContext context, object arg0, object arg1)
  {
    if (this._ctorSite2 == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object>>>(ref this._ctorSite2, CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object>>.Create((CallSiteBinder) context.LanguageContext.Invoke(new CallSignature(2))), (CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object>>) null);
    return this._ctorSite2.Target((CallSite) this._ctorSite2, context, this.Type.Ctor, arg0, arg1);
  }

  internal override object CreateInstance(
    CodeContext context,
    object arg0,
    object arg1,
    object arg2)
  {
    if (this._ctorSite3 == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object, object>>>(ref this._ctorSite3, CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object, object>>.Create((CallSiteBinder) context.LanguageContext.Invoke(new CallSignature(3))), (CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, object, object>>) null);
    return this._ctorSite3.Target((CallSite) this._ctorSite3, context, this.Type.Ctor, arg0, arg1, arg2);
  }

  internal override object CreateInstance(CodeContext context, params object[] args)
  {
    if (this._ctorSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object[], object>>>(ref this._ctorSite, CallSite<Func<CallSite, CodeContext, BuiltinFunction, object[], object>>.Create((CallSiteBinder) context.LanguageContext.Invoke(new CallSignature(new Argument[1]
      {
        new Argument(ArgumentType.List)
      }))), (CallSite<Func<CallSite, CodeContext, BuiltinFunction, object[], object>>) null);
    return this._ctorSite.Target((CallSite) this._ctorSite, context, this.Type.Ctor, args);
  }

  internal override object CreateInstance(CodeContext context, object[] args, string[] names)
  {
    return PythonOps.CallWithKeywordArgs(context, (object) this.Type.Ctor, args, names);
  }
}
