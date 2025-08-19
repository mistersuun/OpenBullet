// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.CompatConversionBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Dynamic;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class CompatConversionBinder : ConvertBinder
{
  private readonly PythonConversionBinder _binder;

  public CompatConversionBinder(PythonConversionBinder binder, Type toType, bool isExplicit)
    : base(toType, isExplicit)
  {
    this._binder = binder;
  }

  public override DynamicMetaObject FallbackConvert(
    DynamicMetaObject target,
    DynamicMetaObject errorSuggestion)
  {
    return this._binder.FallbackConvert(this.ReturnType, target, errorSuggestion);
  }
}
