// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ReturnReferenceArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class ReturnReferenceArgBuilder(ParameterInfo info, int index) : SimpleArgBuilder(info, info.ParameterType.GetElementType(), index, false, false)
{
  private ParameterExpression _tmp;

  protected override SimpleArgBuilder Copy(int newIndex)
  {
    return (SimpleArgBuilder) new ReturnReferenceArgBuilder(this.ParameterInfo, newIndex);
  }

  public override ArgBuilder Clone(ParameterInfo newType)
  {
    return (ArgBuilder) new ReturnReferenceArgBuilder(newType, this.Index);
  }

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    if (this._tmp == null)
      this._tmp = resolver.GetTemporary(this.Type, "outParam");
    return (Expression) Expression.Block((Expression) Expression.Assign((Expression) this._tmp, base.ToExpression(resolver, args, hasBeenUsed)), (Expression) this._tmp);
  }

  internal override Expression ToReturnExpression(OverloadResolver resolver)
  {
    return (Expression) this._tmp;
  }

  internal override Expression ByRefArgument => (Expression) this._tmp;

  public override int Priority => 5;
}
