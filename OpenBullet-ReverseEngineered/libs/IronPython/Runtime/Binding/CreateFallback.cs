// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.CreateFallback
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Dynamic;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class CreateFallback : CreateInstanceBinder, IPythonSite
{
  private readonly CompatibilityInvokeBinder _fallback;

  public CreateFallback(CompatibilityInvokeBinder realFallback, CallInfo callInfo)
    : base(callInfo)
  {
    this._fallback = realFallback;
  }

  public override DynamicMetaObject FallbackCreateInstance(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    DynamicMetaObject errorSuggestion)
  {
    return this._fallback.InvokeFallback(target, args, BindingHelpers.GetCallSignature((DynamicMetaObjectBinder) this), errorSuggestion);
  }

  public PythonContext Context => this._fallback.Context;
}
