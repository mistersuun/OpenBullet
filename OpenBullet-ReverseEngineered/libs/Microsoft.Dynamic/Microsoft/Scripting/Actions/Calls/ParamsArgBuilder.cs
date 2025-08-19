// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ParamsArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class ParamsArgBuilder : ArgBuilder
{
  private readonly int _start;
  private readonly int _expandedCount;
  private readonly Type _elementType;

  internal ParamsArgBuilder(ParameterInfo info, Type elementType, int start, int expandedCount)
    : base(info)
  {
    this._start = start;
    this._expandedCount = expandedCount;
    this._elementType = elementType;
  }

  public override int ConsumedArgumentCount => this._expandedCount;

  public override int Priority => 4;

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    ActualArguments actualArguments = resolver.GetActualArguments();
    int splatIndex = actualArguments.SplatIndex;
    int collapsedCount = actualArguments.CollapsedCount;
    int firstSplattedArg = actualArguments.FirstSplattedArg;
    Expression[] expressionArray1 = new Expression[2 + this._expandedCount + (collapsedCount > 0 ? 2 : 0)];
    ParameterExpression temporary1 = resolver.GetTemporary(this._elementType.MakeArrayType(), "a");
    int num1 = 0;
    Expression[] expressionArray2 = expressionArray1;
    int index1 = num1;
    int num2 = index1 + 1;
    BinaryExpression binaryExpression1 = Expression.Assign((Expression) temporary1, (Expression) Expression.NewArrayBounds(this._elementType, (Expression) Expression.Constant((object) (this._expandedCount + collapsedCount))));
    expressionArray2[index1] = (Expression) binaryExpression1;
    int num3 = 0;
    int start = this._start;
    while (true)
    {
      if (start == splatIndex)
      {
        ParameterExpression temporary2 = resolver.GetTemporary(typeof (int), "t");
        Expression[] expressionArray3 = expressionArray1;
        int index2 = num2;
        int num4 = index2 + 1;
        BinaryExpression binaryExpression2 = Expression.Assign((Expression) temporary2, Utils.Constant((object) 0));
        expressionArray3[index2] = (Expression) binaryExpression2;
        Expression[] expressionArray4 = expressionArray1;
        int index3 = num4;
        num2 = index3 + 1;
        LoopExpression loopExpression = Utils.Loop((Expression) Expression.LessThan((Expression) temporary2, (Expression) Expression.Constant((object) collapsedCount)), (Expression) Expression.Assign((Expression) temporary2, (Expression) Expression.Add((Expression) temporary2, Utils.Constant((object) 1))), (Expression) Expression.Assign((Expression) Expression.ArrayAccess((Expression) temporary1, (Expression) Expression.Add(Utils.Constant((object) num3), (Expression) temporary2)), resolver.Convert(new DynamicMetaObject(resolver.GetSplattedItemExpression((Expression) Expression.Add(Utils.Constant((object) (splatIndex - firstSplattedArg)), (Expression) temporary2)), BindingRestrictions.Empty), (Type) null, this.ParameterInfo, this._elementType)), (Expression) null);
        expressionArray4[index3] = (Expression) loopExpression;
        num3 += collapsedCount;
      }
      if (start < this._start + this._expandedCount)
      {
        hasBeenUsed[start] = true;
        expressionArray1[num2++] = (Expression) Expression.Assign((Expression) Expression.ArrayAccess((Expression) temporary1, Utils.Constant((object) num3++)), resolver.Convert(args.GetObject(start), args.GetType(start), this.ParameterInfo, this._elementType));
        ++start;
      }
      else
        break;
    }
    Expression[] expressionArray5 = expressionArray1;
    int index4 = num2;
    int num5 = index4 + 1;
    ParameterExpression parameterExpression = temporary1;
    expressionArray5[index4] = (Expression) parameterExpression;
    return (Expression) Expression.Block(expressionArray1);
  }

  public override Type Type => this._elementType.MakeArrayType();

  public override ArgBuilder Clone(ParameterInfo newType)
  {
    return (ArgBuilder) new ParamsArgBuilder(newType, newType.ParameterType.GetElementType(), this._start, this._expandedCount);
  }
}
