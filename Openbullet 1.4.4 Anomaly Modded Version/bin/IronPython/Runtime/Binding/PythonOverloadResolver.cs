// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonOverloadResolver
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal sealed class PythonOverloadResolver : DefaultOverloadResolver
{
  private readonly Expression _context;

  public Expression ContextExpression => this._context;

  public PythonOverloadResolver(
    PythonBinder binder,
    DynamicMetaObject instance,
    IList<DynamicMetaObject> args,
    CallSignature signature,
    Expression codeContext)
    : base((ActionBinder) binder, instance, args, signature)
  {
    this._context = codeContext;
  }

  public PythonOverloadResolver(
    PythonBinder binder,
    IList<DynamicMetaObject> args,
    CallSignature signature,
    Expression codeContext)
    : this(binder, args, signature, CallTypes.None, codeContext)
  {
  }

  public PythonOverloadResolver(
    PythonBinder binder,
    IList<DynamicMetaObject> args,
    CallSignature signature,
    CallTypes callType,
    Expression codeContext)
    : base((ActionBinder) binder, args, signature, callType)
  {
    this._context = codeContext;
  }

  private PythonBinder Binder => (PythonBinder) base.Binder;

  public override bool CanConvertFrom(
    Type fromType,
    DynamicMetaObject fromArg,
    ParameterWrapper toParameter,
    NarrowingLevel level)
  {
    if (fromType == typeof (List) || fromType.IsSubclassOf(typeof (List)))
    {
      if (toParameter.Type.IsGenericType() && toParameter.Type.GetGenericTypeDefinition() == typeof (IList<>) && (toParameter.ParameterInfo.IsDefined(typeof (BytesConversionAttribute), false) || toParameter.ParameterInfo.IsDefined(typeof (BytesConversionNoStringAttribute), false)))
        return false;
    }
    else if (fromType == typeof (string))
    {
      if (toParameter.Type == typeof (IList<byte>) && !this.Binder.Context.PythonOptions.Python30 && toParameter.ParameterInfo.IsDefined(typeof (BytesConversionAttribute), false))
        return true;
    }
    else if ((fromType == typeof (Bytes) || fromType == typeof (PythonBuffer) || fromType == typeof (ByteArray)) && toParameter.Type == typeof (string) && !this.Binder.Context.PythonOptions.Python30 && toParameter.ParameterInfo.IsDefined(typeof (BytesConversionAttribute), false))
      return true;
    return base.CanConvertFrom(fromType, fromArg, toParameter, level);
  }

  protected override BitArray MapSpecialParameters(ParameterMapping mapping)
  {
    IList<ParameterInfo> parameters = mapping.Overload.Parameters;
    BitArray bitArray = base.MapSpecialParameters(mapping);
    if (parameters.Count > 0)
    {
      bool flag1 = false;
      for (int index = 0; index < parameters.Count; ++index)
      {
        bool flag2 = false;
        if (parameters[index].ParameterType.IsSubclassOf(typeof (SiteLocalStorage)))
        {
          mapping.AddBuilder((ArgBuilder) new SiteLocalStorageBuilder(parameters[index]));
          flag2 = true;
        }
        else if (parameters[index].ParameterType == typeof (CodeContext) && !flag1)
        {
          mapping.AddBuilder((ArgBuilder) new ContextArgBuilder(parameters[index]));
          flag2 = true;
        }
        else
          flag1 = true;
        if (flag2)
          (bitArray = bitArray ?? new BitArray(parameters.Count))[index] = true;
      }
    }
    return bitArray;
  }

  protected override Expression GetByRefArrayExpression(Expression argumentArrayExpression)
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeTuple"), argumentArrayExpression);
  }

  protected override bool AllowMemberInitialization(OverloadInfo method)
  {
    return method.IsInstanceFactory && !method.DeclaringType.IsDefined(typeof (PythonTypeAttribute), true);
  }

  public override Expression Convert(
    DynamicMetaObject metaObject,
    Type restrictedType,
    ParameterInfo info,
    Type toType)
  {
    return this.Binder.ConvertExpression(metaObject.Expression, toType, ConversionResultKind.ExplicitCast, (OverloadResolverFactory) new PythonOverloadResolverFactory(this.Binder, this._context));
  }

  public override Expression GetDynamicConversion(Expression value, Type type)
  {
    return (Expression) Expression.Dynamic((CallSiteBinder) this.Binder.Context.Convert(type, ConversionResultKind.ExplicitCast), type, value);
  }

  public override Type GetGenericInferenceType(DynamicMetaObject dynamicObject)
  {
    Type finalSystemType = PythonTypeOps.GetFinalSystemType(dynamicObject.LimitType);
    return finalSystemType == typeof (ExtensibleString) || finalSystemType == typeof (ExtensibleComplex) || finalSystemType.IsGenericType() && finalSystemType.GetGenericTypeDefinition() == typeof (Extensible<>) ? typeof (object) : finalSystemType;
  }
}
