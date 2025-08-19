// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightThrowExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal class LightThrowExpression : Expression
{
  private readonly Expression _exception;
  private static MethodInfo _throw = RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<Exception, object>(LightExceptions.Throw));

  public LightThrowExpression(Expression exception) => this._exception = exception;

  public override Expression Reduce()
  {
    return (Expression) Expression.Call(LightThrowExpression._throw, this._exception);
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression exception = visitor.Visit(this._exception);
    return exception != this._exception ? (Expression) new LightThrowExpression(exception) : (Expression) this;
  }

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => typeof (object);
}
