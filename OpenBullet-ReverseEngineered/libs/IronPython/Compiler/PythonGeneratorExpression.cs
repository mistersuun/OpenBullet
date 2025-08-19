// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonGeneratorExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal sealed class PythonGeneratorExpression : Expression
{
  private readonly LightLambdaExpression _lambda;
  private readonly int _compilationThreshold;

  public PythonGeneratorExpression(LightLambdaExpression lambda, int compilationThreshold)
  {
    this._lambda = lambda;
    this._compilationThreshold = compilationThreshold;
  }

  public override Expression Reduce()
  {
    return (Expression) this._lambda.ToGenerator(false, true, this._compilationThreshold);
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => this._lambda.Type;

  public override bool CanReduce => true;
}
