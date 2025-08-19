// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightLambdaExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightLambdaExpression : Expression
{
  internal LightLambdaExpression(
    Type retType,
    Expression body,
    string name,
    IList<ParameterExpression> args)
  {
    this.Body = body;
    this.Name = name;
    this.Parameters = args;
    this.ReturnType = retType;
  }

  public Expression Body { get; }

  public string Name { get; }

  public IList<ParameterExpression> Parameters { get; }

  internal virtual LambdaExpression ReduceToLambdaWorker() => throw new InvalidOperationException();

  public Delegate Compile() => this.Compile(-1);

  public Delegate Compile(int compilationThreshold)
  {
    return new LightCompiler(compilationThreshold).CompileTop(this).CreateDelegate();
  }

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override bool CanReduce => true;

  public override Expression Reduce() => (Expression) this.ReduceToLambdaWorker();

  public Type ReturnType { get; }
}
