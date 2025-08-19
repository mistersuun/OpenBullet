// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.CompatibilityInvokeBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.ComInterop;
using System.Dynamic;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class CompatibilityInvokeBinder : InvokeBinder, IPythonSite
{
  private readonly PythonContext _context;

  public CompatibilityInvokeBinder(PythonContext context, CallInfo callInfo)
    : base(callInfo)
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackInvoke(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    DynamicMetaObject errorSuggestion)
  {
    if (target.Value is IDynamicMetaObjectProvider && errorSuggestion == null)
      return target.BindCreateInstance((CreateInstanceBinder) this._context.Create(this, this.CallInfo), args);
    DynamicMetaObject result;
    return ComBinder.TryBindInvoke((InvokeBinder) this, target, BindingHelpers.GetComArguments(args), out result) ? result : this.InvokeFallback(target, args, BindingHelpers.CallInfoToSignature(this.CallInfo), errorSuggestion);
  }

  internal DynamicMetaObject InvokeFallback(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    CallSignature sig,
    DynamicMetaObject errorSuggestion)
  {
    return PythonProtocol.Call((DynamicMetaObjectBinder) this, target, args) ?? this.Context.Binder.Create(sig, target, args, Utils.Constant((object) this._context.SharedContext)) ?? this.Context.Binder.Call(sig, errorSuggestion, (OverloadResolverFactory) new PythonOverloadResolverFactory(this.Context.Binder, Utils.Constant((object) this._context.SharedContext)), target, args);
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is CompatibilityInvokeBinder compatibilityInvokeBinder && compatibilityInvokeBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;
}
