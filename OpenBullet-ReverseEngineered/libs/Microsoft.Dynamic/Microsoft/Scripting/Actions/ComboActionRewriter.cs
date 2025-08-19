// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ComboActionRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ComboActionRewriter : DynamicExpressionVisitor
{
  protected virtual Expression VisitDynamic(DynamicExpression node)
  {
    if (!(node.Binder is DynamicMetaObjectBinder binder1))
      return (Expression) node;
    ReadOnlyCollection<Expression> arguments = node.Arguments;
    bool flag = false;
    List<Expression> expressionList = new List<Expression>();
    List<BinderMappingInfo> binders = new List<BinderMappingInfo>();
    List<ParameterMappingInfo> mappingInfo1 = new List<ParameterMappingInfo>();
    int num1 = 0;
    for (int index = 0; index < arguments.Count; ++index)
    {
      Expression node1 = arguments[index];
      if (!flag)
      {
        Expression rewritten = ((ExpressionVisitor) this).Visit(node1);
        if (rewritten is ComboActionRewriter.ComboDynamicSiteExpression dynamicSiteExpression)
        {
          int num2 = num1;
          foreach (BinderMappingInfo binder2 in dynamicSiteExpression.Binders)
          {
            List<ParameterMappingInfo> mappingInfo2 = new List<ParameterMappingInfo>();
            foreach (ParameterMappingInfo parameterMappingInfo in (IEnumerable<ParameterMappingInfo>) binder2.MappingInfo)
            {
              if (parameterMappingInfo.IsParameter)
              {
                mappingInfo2.Add(ParameterMappingInfo.Parameter(expressionList.Count));
                expressionList.Add(dynamicSiteExpression.Inputs[parameterMappingInfo.ParameterIndex]);
              }
              else if (parameterMappingInfo.IsAction)
              {
                mappingInfo2.Add(ParameterMappingInfo.Action(parameterMappingInfo.ActionIndex + num2));
                ++num1;
              }
              else
                mappingInfo2.Add(parameterMappingInfo);
            }
            binders.Add(new BinderMappingInfo(binder2.Binder, (IList<ParameterMappingInfo>) mappingInfo2));
          }
          mappingInfo1.Add(ParameterMappingInfo.Action(num1++));
        }
        else if (rewritten is ConstantExpression e)
          mappingInfo1.Add(ParameterMappingInfo.Fixed(e));
        else if (this.IsSideEffectFree(rewritten))
        {
          mappingInfo1.Add(ParameterMappingInfo.Parameter(expressionList.Count));
          expressionList.Add(rewritten);
        }
        else
        {
          flag = true;
          mappingInfo1.Add(ParameterMappingInfo.Parameter(expressionList.Count));
          expressionList.Add(node1);
        }
      }
      else
      {
        mappingInfo1.Add(ParameterMappingInfo.Parameter(expressionList.Count));
        expressionList.Add(node1);
      }
    }
    binders.Add(new BinderMappingInfo(binder1, (IList<ParameterMappingInfo>) mappingInfo1));
    return (Expression) new ComboActionRewriter.ComboDynamicSiteExpression(node.Type, binders, expressionList.ToArray());
  }

  private bool IsSideEffectFree(Expression rewritten)
  {
    if (rewritten is ParameterExpression)
      return true;
    if (rewritten.NodeType == ExpressionType.TypeIs)
      return this.IsSideEffectFree(((UnaryExpression) rewritten).Operand);
    switch (rewritten)
    {
      case BinaryExpression binaryExpression when binaryExpression.Method == (MethodInfo) null && this.IsSideEffectFree(binaryExpression.Left) && this.IsSideEffectFree(binaryExpression.Right):
        return true;
      case MethodCallExpression methodCallExpression when methodCallExpression.Method != (MethodInfo) null:
        return methodCallExpression.Method.IsDefined(typeof (NoSideEffectsAttribute), false);
      case ConditionalExpression conditionalExpression:
        return this.IsSideEffectFree(conditionalExpression.Test) && this.IsSideEffectFree(conditionalExpression.IfTrue) && this.IsSideEffectFree(conditionalExpression.IfFalse);
      case MemberExpression memberExpression:
        FieldInfo member = memberExpression.Member as FieldInfo;
        return false;
      default:
        return false;
    }
  }

  private class ComboDynamicSiteExpression : Expression
  {
    public ComboDynamicSiteExpression(
      Type type,
      List<BinderMappingInfo> binders,
      Expression[] inputs)
    {
      this.Binders = binders;
      this.Inputs = inputs;
      this.Type = type;
    }

    public override bool CanReduce => true;

    public sealed override Type Type { get; }

    public sealed override ExpressionType NodeType => ExpressionType.Extension;

    public Expression[] Inputs { get; }

    public List<BinderMappingInfo> Binders { get; }

    public override Expression Reduce()
    {
      return (Expression) Expression.Dynamic((CallSiteBinder) new ComboBinder((ICollection<BinderMappingInfo>) this.Binders), this.Type, this.Inputs);
    }
  }
}
