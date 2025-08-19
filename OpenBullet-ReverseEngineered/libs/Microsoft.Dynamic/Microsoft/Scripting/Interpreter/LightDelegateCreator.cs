// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LightDelegateCreator
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LightDelegateCreator
{
  private readonly Expression _lambda;
  private Type _compiledDelegateType;
  private Delegate _compiled;
  private readonly object _compileLock = new object();

  internal LightDelegateCreator(Microsoft.Scripting.Interpreter.Interpreter interpreter, LambdaExpression lambda)
  {
    this.Interpreter = interpreter;
    this._lambda = (Expression) lambda;
  }

  internal LightDelegateCreator(Microsoft.Scripting.Interpreter.Interpreter interpreter, LightLambdaExpression lambda)
  {
    this.Interpreter = interpreter;
    this._lambda = (Expression) lambda;
  }

  internal Microsoft.Scripting.Interpreter.Interpreter Interpreter { get; }

  private bool HasClosure => this.Interpreter != null && this.Interpreter.ClosureSize > 0;

  internal bool HasCompiled => this._compiled != null;

  internal bool SameDelegateType => this._compiledDelegateType == this.DelegateType;

  internal Delegate CreateDelegate() => this.CreateDelegate((StrongBox<object>[]) null);

  internal Delegate CreateDelegate(StrongBox<object>[] closure)
  {
    if ((object) this._compiled != null && this.SameDelegateType)
      return this.CreateCompiledDelegate(closure);
    if (this.Interpreter != null)
      return new LightLambda(this, closure, this.Interpreter._compilationThreshold).MakeDelegate(this.DelegateType);
    this.Compile((object) null);
    return this.CreateCompiledDelegate(closure);
  }

  private Type DelegateType
  {
    get => this._lambda is LambdaExpression lambda ? lambda.Type : this._lambda.Type;
  }

  internal Delegate CreateCompiledDelegate(StrongBox<object>[] closure)
  {
    return this.HasClosure ? ((Func<StrongBox<object>[], Delegate>) this._compiled)(closure) : this._compiled;
  }

  internal void Compile(object state)
  {
    if ((object) this._compiled != null)
      return;
    lock (this._compileLock)
    {
      if ((object) this._compiled != null)
        return;
      if (!(this._lambda is LambdaExpression lambdaExpression))
        lambdaExpression = (LambdaExpression) this._lambda.Reduce();
      LambdaExpression lambda = lambdaExpression;
      if (this.Interpreter != null)
      {
        this._compiledDelegateType = LightDelegateCreator.GetFuncOrAction(lambda);
        lambda = Expression.Lambda(this._compiledDelegateType, lambda.Body, lambda.Name, (IEnumerable<ParameterExpression>) lambda.Parameters);
      }
      if (this.HasClosure)
        this._compiled = (Delegate) LightLambdaClosureVisitor.BindLambda(lambda, this.Interpreter.ClosureVariables);
      else
        this._compiled = lambda.Compile();
    }
  }

  private static Type GetFuncOrAction(LambdaExpression lambda)
  {
    bool flag = lambda.ReturnType == typeof (void);
    if (flag && lambda.Parameters.Count == 2 && lambda.Parameters[0].IsByRef && lambda.Parameters[1].IsByRef)
      return typeof (ActionRef<,>).MakeGenericType(lambda.Parameters.Map<ParameterExpression, Type>((Func<ParameterExpression, Type>) (p => p.Type)));
    Type[] typeArray = lambda.Parameters.Map<ParameterExpression, Type>((Func<ParameterExpression, Type>) (p => !p.IsByRef ? p.Type : p.Type.MakeByRefType()));
    if (flag)
    {
      Type actionType;
      if (Expression.TryGetActionType(typeArray, out actionType))
        return actionType;
    }
    else
    {
      Type funcType;
      if (Expression.TryGetFuncType(((IList<Type>) typeArray).AddLast<Type>(lambda.ReturnType), out funcType))
        return funcType;
    }
    return lambda.Type;
  }
}
