// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.DynamicUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class DynamicUtils
{
  public static Expression[] GetExpressions(DynamicMetaObject[] objects)
  {
    ContractUtils.RequiresNotNull((object) objects, nameof (objects));
    Expression[] expressions = new Expression[objects.Length];
    for (int index = 0; index < objects.Length; ++index)
    {
      DynamicMetaObject dynamicMetaObject = objects[index];
      expressions[index] = dynamicMetaObject?.Expression;
    }
    return expressions;
  }

  public static DynamicMetaObject ObjectToMetaObject(
    object argValue,
    Expression parameterExpression)
  {
    return argValue is IDynamicMetaObjectProvider metaObjectProvider ? metaObjectProvider.GetMetaObject(parameterExpression) : new DynamicMetaObject(parameterExpression, BindingRestrictions.Empty, argValue);
  }

  public static T LightBind<T>(
    this DynamicMetaObjectBinder binder,
    object[] args,
    int compilationThreshold)
    where T : class
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    return DynamicUtils.GenericInterpretedBinder<T>.Instance.Bind(binder, compilationThreshold < 0 ? 32 /*0x20*/ : compilationThreshold, args);
  }

  private class GenericInterpretedBinder<T> where T : class
  {
    public static DynamicUtils.GenericInterpretedBinder<T> Instance = new DynamicUtils.GenericInterpretedBinder<T>();
    private readonly ReadOnlyCollection<ParameterExpression> _parameters;
    private readonly Expression _updateExpression;

    private GenericInterpretedBinder()
    {
      ParameterInfo[] parameters = typeof (T).GetMethod("Invoke").GetParameters();
      ReadOnlyCollectionBuilder<ParameterExpression> collectionBuilder1 = new ReadOnlyCollectionBuilder<ParameterExpression>(parameters.Length);
      ReadOnlyCollectionBuilder<Expression> collectionBuilder2 = new ReadOnlyCollectionBuilder<Expression>(parameters.Length);
      for (int index = 0; index < parameters.Length; ++index)
      {
        ParameterExpression parameterExpression = Expression.Parameter(parameters[index].ParameterType);
        if (index == 0)
          collectionBuilder2.Add((Expression) Expression.Convert((Expression) parameterExpression, typeof (CallSite<T>)));
        else
          collectionBuilder2.Add((Expression) parameterExpression);
        collectionBuilder1.Add(parameterExpression);
      }
      this._parameters = collectionBuilder1.ToReadOnlyCollection();
      this._updateExpression = (Expression) Expression.Block((Expression) Expression.Label(CallSiteBinder.UpdateLabel), (Expression) Expression.Invoke((Expression) Expression.Property(collectionBuilder2[0], typeof (CallSite<T>).GetDeclaredProperty("Update")), (IEnumerable<Expression>) collectionBuilder2.ToReadOnlyCollection()));
    }

    public T Bind(DynamicMetaObjectBinder binder, int compilationThreshold, object[] args)
    {
      if (CachedBindingInfo<T>.LastInterpretedFailure != null && CachedBindingInfo<T>.LastInterpretedFailure.Binder == binder)
      {
        T compiledTarget = CachedBindingInfo<T>.LastInterpretedFailure.CompiledTarget;
        CachedBindingInfo<T>.LastInterpretedFailure = (CachedBindingInfo<T>) null;
        return compiledTarget;
      }
      CachedBindingInfo<T> bindingInfo = new CachedBindingInfo<T>(binder, compilationThreshold);
      DynamicMetaObject target = DynamicMetaObject.Create(args[0], (Expression) this._parameters[1]);
      DynamicMetaObject[] args1 = new DynamicMetaObject[args.Length - 1];
      for (int index = 0; index < args1.Length; ++index)
        args1[index] = DynamicMetaObject.Create(args[index + 1], (Expression) this._parameters[index + 2]);
      return this.CreateDelegate(binder.Bind(target, args1), bindingInfo);
    }

    private T CreateDelegate(DynamicMetaObject binding, CachedBindingInfo<T> bindingInfo)
    {
      return this.Compile(binding, bindingInfo).LightCompile<T>(int.MaxValue);
    }

    private Expression<T> Compile(DynamicMetaObject obj, CachedBindingInfo<T> bindingInfo)
    {
      Expression<T> expression = Expression.Lambda<T>((Expression) Expression.Condition((Expression) new DynamicUtils.GenericInterpretedBinder<T>.InterpretedRuleHitCheckExpression(obj.Restrictions.ToExpression(), (CachedBindingInfo) bindingInfo), Microsoft.Scripting.Ast.Utils.Convert(obj.Expression, this._updateExpression.Type), this._updateExpression), "CallSite.Target", true, (IEnumerable<ParameterExpression>) this._parameters);
      bindingInfo.Target = expression;
      return expression;
    }

    private class InterpretedRuleHitCheckExpression : Expression, IInstructionProvider
    {
      private readonly Expression _test;
      private readonly CachedBindingInfo _bindingInfo;
      private static readonly MethodInfo InterpretedCallSiteTest = typeof (ScriptingRuntimeHelpers).GetMethod(nameof (InterpretedCallSiteTest));

      public InterpretedRuleHitCheckExpression(Expression test, CachedBindingInfo bindingInfo)
      {
        this._test = test;
        this._bindingInfo = bindingInfo;
      }

      public override Expression Reduce() => this._test;

      protected override Expression VisitChildren(ExpressionVisitor visitor)
      {
        Expression test = visitor.Visit(this._test);
        return test != this._test ? (Expression) new DynamicUtils.GenericInterpretedBinder<T>.InterpretedRuleHitCheckExpression(test, this._bindingInfo) : (Expression) this;
      }

      public override bool CanReduce => true;

      public override ExpressionType NodeType => ExpressionType.Extension;

      public override Type Type => typeof (bool);

      public void AddInstructions(LightCompiler compiler)
      {
        compiler.Compile(this._test);
        compiler.Instructions.EmitLoad((object) this._bindingInfo);
        compiler.EmitCall(DynamicUtils.GenericInterpretedBinder<T>.InterpretedRuleHitCheckExpression.InterpretedCallSiteTest);
      }
    }
  }
}
