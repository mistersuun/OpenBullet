// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightExceptionRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal class LightExceptionRewriter : DynamicExpressionVisitor
{
  private LabelTarget _currentHandler;
  private ParameterExpression _rethrow;
  private static readonly ParameterExpression _lastValue = Expression.Parameter(typeof (object), "$lastValue");
  private static readonly ReadOnlyCollection<ParameterExpression> _lastValueParamArray = new ReadOnlyCollectionBuilder<ParameterExpression>(1)
  {
    LightExceptionRewriter._lastValue
  }.ToReadOnlyCollection();
  private static readonly Expression _isLightExExpr = (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<Exception, bool>(LightExceptions.IsLightException)), (Expression) LightExceptionRewriter._lastValue);
  private static readonly Expression _lastException = (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<object, Exception>(LightExceptions.GetLightException)), (Expression) LightExceptionRewriter._lastValue);
  private readonly LabelTarget _returnLabel = Expression.Label(typeof (object), LightExceptionRewriter.GetEhLabelName("ehUnwind"));

  internal Expression Rewrite(Expression expr)
  {
    if (expr is LambdaExpression lambdaExpression)
      return (Expression) Expression.Lambda(this.Rewrite(lambdaExpression.Body), lambdaExpression.Name, lambdaExpression.TailCall, (IEnumerable<ParameterExpression>) lambdaExpression.Parameters);
    expr = ((ExpressionVisitor) this).Visit(expr);
    if (expr.Type == typeof (void))
      expr = (Expression) Expression.Block(expr, Utils.Constant((object) null));
    return (Expression) new LightExceptionRewriter.LightExceptionRewrittenCode(this._returnLabel, expr);
  }

  protected virtual Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

  protected virtual Expression VisitExtension(Expression node)
  {
    if (node is ILightExceptionAwareExpression exceptionAwareExpression)
    {
      Expression node1 = exceptionAwareExpression.ReduceForLightExceptions();
      if (node1 != node)
        return this.CheckExpression(((ExpressionVisitor) this).Visit(node1), node.Type);
    }
    // ISSUE: explicit non-virtual call
    return __nonvirtual (((ExpressionVisitor) this).VisitExtension(node));
  }

  protected virtual Expression VisitDynamic(DynamicExpression node)
  {
    if (node.Binder is ILightExceptionBinder binder)
    {
      CallSiteBinder lightExceptionBinder = binder.GetLightExceptionBinder();
      if (lightExceptionBinder != node.Binder)
        return this.CheckExpression((Expression) Expression.Dynamic(lightExceptionBinder, node.Type, (IEnumerable<Expression>) ((ExpressionVisitor) this).Visit(node.Arguments)), node.Type);
    }
    return base.VisitDynamic(node);
  }

  protected virtual Expression VisitTry(TryExpression node)
  {
    if (node.Fault != null)
      throw new NotSupportedException();
    if (node.Handlers != null && node.Handlers.Count > 0)
      return this.RewriteTryCatch(node);
    Expression tryBody = ((ExpressionVisitor) this).Visit(node.Body);
    return tryBody != node.Body ? this.RewriteTryFinally(tryBody, node.Finally) : (Expression) node;
  }

  private Expression RewriteTryFinally(Expression tryBody, Expression finallyBody)
  {
    return (Expression) Expression.TryFinally(tryBody, finallyBody);
  }

  private static string GetEhLabelName(string baseName) => baseName;

  protected virtual Expression VisitUnary(UnaryExpression node)
  {
    if (node.NodeType != ExpressionType.Throw)
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitUnary(node));
    }
    Expression node1 = node.Operand ?? (Expression) this._rethrow;
    return (Expression) Expression.Block((Expression) Expression.Assign((Expression) LightExceptionRewriter._lastValue, LightExceptions.Throw(((ExpressionVisitor) this).Visit(node1))), this.PropagateException(node.Type));
  }

  private Expression RewriteTryBody(TryExpression node, LabelTarget ehLabel)
  {
    LabelTarget currentHandler = this._currentHandler;
    this._currentHandler = ehLabel;
    try
    {
      return ((ExpressionVisitor) this).Visit(node.Body);
    }
    finally
    {
      this._currentHandler = currentHandler;
    }
  }

  private CatchBlock[] VisitHandlers(TryExpression node, bool realCatch)
  {
    CatchBlock[] catchBlockArray = new CatchBlock[node.Handlers.Count];
    for (int index = 0; index < node.Handlers.Count; ++index)
    {
      CatchBlock handler = node.Handlers[index];
      ParameterExpression rethrow = this._rethrow;
      try
      {
        if (handler.Variable == null)
        {
          ParameterExpression parameterExpression = this._rethrow = Expression.Parameter(handler.Test, "$exception");
          catchBlockArray[index] = Expression.Catch(parameterExpression, this.TrackCatch(((ExpressionVisitor) this).Visit(handler.Body), (Expression) parameterExpression, realCatch), ((ExpressionVisitor) this).Visit(handler.Filter));
        }
        else
        {
          ParameterExpression parameterExpression = this._rethrow = Expression.Parameter(typeof (Exception), "$exception");
          catchBlockArray[index] = Expression.Catch(handler.Variable, (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
          {
            parameterExpression
          }, (Expression) Expression.Assign((Expression) parameterExpression, (Expression) handler.Variable), this.TrackCatch(((ExpressionVisitor) this).Visit(handler.Body), (Expression) parameterExpression, realCatch)), ((ExpressionVisitor) this).Visit(handler.Filter));
        }
      }
      finally
      {
        this._rethrow = rethrow;
      }
    }
    return catchBlockArray;
  }

  private Expression TrackCatch(Expression expression, Expression exception, bool realCatch)
  {
    return expression;
  }

  private Expression RewriteTryCatch(TryExpression node)
  {
    LabelTarget labelTarget = Expression.Label(typeof (void), LightExceptionRewriter.GetEhLabelName("lightEh"));
    LabelTarget target = Expression.Label(node.Body.Type, LightExceptionRewriter.GetEhLabelName("tryDone"));
    Expression tryBody = (Expression) Expression.Block((Expression) Expression.Goto(target, (Expression) Expression.TryCatch(this.RewriteTryBody(node, labelTarget), this.VisitHandlers(node, true))), (Expression) Expression.Label(target, (Expression) Expression.Block((Expression) Expression.Label(labelTarget), Utils.Convert(this.LightCatch(this.VisitHandlers(node, false)), node.Body.Type))));
    if (node.Finally != null)
      tryBody = this.RewriteTryFinally(tryBody, node.Finally);
    return tryBody;
  }

  private Expression LightCatch(CatchBlock[] handlers)
  {
    Expression ifFalse = this.PropagateException(typeof (object));
    for (int index = handlers.Length - 1; index >= 0; --index)
    {
      CatchBlock handler = handlers[index];
      Expression test = (Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) handler.Variable, (Expression) Expression.TypeAs(LightExceptionRewriter._lastException, handler.Test)), (Expression) Expression.Constant((object) null));
      if (handlers[index].Filter != null)
        throw new NotSupportedException("filters for light exceptions");
      ifFalse = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        handler.Variable
      }, (Expression) Expression.Condition(test, Utils.Convert(handlers[index].Body, typeof (object)), ifFalse));
    }
    return ifFalse;
  }

  private Expression CheckExpression(Expression expr, Type retType)
  {
    if (expr.Type == typeof (object))
      expr = (Expression) new LightExceptionRewriter.LightExceptionCheckExpression(expr, retType, this._currentHandler ?? this._returnLabel, this._currentHandler == null ? LightExceptionRewriter._lastValue : (ParameterExpression) null);
    return expr;
  }

  private Expression PropagateException(Type retType)
  {
    return this._currentHandler == null ? (Expression) Expression.Goto(this._returnLabel, (Expression) LightExceptionRewriter._lastValue, retType) : (Expression) Expression.Goto(this._currentHandler, retType);
  }

  private class LightExceptionRewrittenCode : Expression, IInstructionProvider
  {
    private readonly LabelTarget _returnLabel;
    private readonly Expression _body;

    public LightExceptionRewrittenCode(LabelTarget target, Expression body)
    {
      this._returnLabel = target;
      this._body = body;
    }

    public override Expression Reduce()
    {
      return (Expression) Expression.Block(typeof (object), (IEnumerable<ParameterExpression>) LightExceptionRewriter._lastValueParamArray, (IEnumerable<Expression>) new ReadOnlyCollectionBuilder<Expression>()
      {
        (Expression) Expression.Label(this._returnLabel, this._body)
      });
    }

    public override bool CanReduce => true;

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => typeof (object);

    public void AddInstructions(LightCompiler compiler)
    {
      compiler.PushLabelBlock(LabelScopeKind.Block);
      LocalDefinition definition = compiler.Locals.DefineLocal(LightExceptionRewriter._lastValue, compiler.Instructions.Count);
      Microsoft.Scripting.Interpreter.LabelInfo labelInfo = compiler.DefineLabel(this._returnLabel);
      compiler.Compile(this._body);
      compiler.Instructions.MarkLabel(labelInfo.GetLabel(compiler));
      compiler.Locals.UndefineLocal(definition, compiler.Instructions.Count);
      compiler.PopLabelBlock(LabelScopeKind.Block);
    }
  }

  private class LightExceptionCheckExpression : Expression, IInstructionProvider
  {
    private readonly Type _retType;
    private readonly LabelTarget _target;
    private readonly Expression _lastValue;
    private readonly Expression _expr;

    public LightExceptionCheckExpression(
      Expression expr,
      Type retType,
      LabelTarget currentHandler,
      ParameterExpression lastValue)
    {
      this._expr = expr;
      this._retType = retType;
      this._target = currentHandler;
      this._lastValue = (Expression) lastValue;
    }

    public override Expression Reduce()
    {
      return (Expression) Expression.Condition((Expression) Expression.Block((Expression) Expression.Assign((Expression) LightExceptionRewriter._lastValue, this._expr), (Expression) LightExceptionRewriter.IsLightExceptionExpression.Instance), (Expression) Expression.Goto(this._target, this._lastValue, this._retType), Utils.Convert((Expression) LightExceptionRewriter._lastValue, this._retType));
    }

    public override bool CanReduce => true;

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => this._retType;

    public void AddInstructions(LightCompiler compiler)
    {
      BranchLabel branchLabel = compiler.Instructions.MakeLabel();
      BranchLabel label = compiler.Instructions.MakeLabel();
      compiler.Compile(this._expr);
      compiler.CompileSetVariable(LightExceptionRewriter._lastValue, false);
      compiler.Instructions.Emit((Instruction) LightExceptionRewriter.IsLightExceptionInstruction.Instance);
      compiler.Instructions.EmitBranchFalse(branchLabel);
      if (this._lastValue != null)
        compiler.CompileParameterExpression(this._lastValue);
      compiler.Instructions.EmitGoto(compiler.GetBranchLabel(this._target), this._retType != typeof (void), this._lastValue != null && this._lastValue.Type != typeof (void));
      compiler.Instructions.EmitBranch(label, false, true);
      compiler.Instructions.MarkLabel(branchLabel);
      compiler.CompileParameterExpression((Expression) LightExceptionRewriter._lastValue);
      compiler.Instructions.MarkLabel(label);
    }
  }

  private class IsLightExceptionExpression : Expression, IInstructionProvider
  {
    public static LightExceptionRewriter.IsLightExceptionExpression Instance = new LightExceptionRewriter.IsLightExceptionExpression();

    private IsLightExceptionExpression()
    {
    }

    public override Expression Reduce() => LightExceptionRewriter._isLightExExpr;

    public override Type Type => typeof (bool);

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override bool CanReduce => true;

    public void AddInstructions(LightCompiler compiler)
    {
      compiler.Compile((Expression) LightExceptionRewriter._lastValue);
      compiler.Instructions.Emit((Instruction) LightExceptionRewriter.IsLightExceptionInstruction.Instance);
    }
  }

  private class IsLightExceptionInstruction : Instruction
  {
    public static LightExceptionRewriter.IsLightExceptionInstruction Instance = new LightExceptionRewriter.IsLightExceptionInstruction();

    private IsLightExceptionInstruction()
    {
    }

    public override int ConsumedStack => 1;

    public override int ProducedStack => 1;

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(ScriptingRuntimeHelpers.BooleanToObject(LightExceptions.IsLightException(frame.Pop())));
      return 1;
    }
  }
}
