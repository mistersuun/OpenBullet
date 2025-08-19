// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightExpression`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightExpression<T> : LightLambdaExpression
{
  internal LightExpression(
    Type retType,
    Expression body,
    string name,
    IList<ParameterExpression> args)
    : base(retType, body, name, args)
  {
  }

  public Expression<T> ReduceToLambda()
  {
    return Expression.Lambda<T>(this.Body, this.Name, (IEnumerable<ParameterExpression>) this.Parameters);
  }

  public override Type Type => typeof (T);

  public T Compile() => this.Compile(-1);

  public T Compile(int compilationThreshold)
  {
    return (T) new LightCompiler(compilationThreshold).CompileTop((LightLambdaExpression) this).CreateDelegate();
  }

  internal override LambdaExpression ReduceToLambdaWorker()
  {
    return (LambdaExpression) this.ReduceToLambda();
  }
}
