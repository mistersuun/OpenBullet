// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.DefaultArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class DefaultArgBuilder(ParameterInfo info) : ArgBuilder(info)
{
  public override int Priority => 2;

  public override int ConsumedArgumentCount => 0;

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    object obj = this.ParameterInfo.GetDefaultValue();
    if (obj is Missing)
      obj = CompilerHelpers.GetMissingValue(this.ParameterInfo.ParameterType);
    if (this.ParameterInfo.ParameterType.IsByRef)
      return (Expression) Microsoft.Scripting.Ast.Utils.Constant(obj, this.ParameterInfo.ParameterType.GetElementType());
    DynamicMetaObject metaObject = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant(obj), BindingRestrictions.Empty, obj);
    return resolver.Convert(metaObject, CompilerHelpers.GetType(obj), this.ParameterInfo, this.ParameterInfo.ParameterType);
  }
}
