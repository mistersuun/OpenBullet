// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.LambdaWalker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal sealed class LambdaWalker : ExpressionVisitor
{
  private readonly List<ParameterExpression> _locals;
  private readonly Dictionary<ParameterExpression, object> _strongBoxedLocals;

  internal LambdaWalker()
  {
    this._locals = new List<ParameterExpression>();
    this._strongBoxedLocals = new Dictionary<ParameterExpression, object>();
  }

  internal List<ParameterExpression> Locals => this._locals;

  internal Dictionary<ParameterExpression, object> StrongBoxedLocals => this._strongBoxedLocals;

  protected override Expression VisitBlock(BlockExpression node)
  {
    foreach (ParameterExpression variable in node.Variables)
      this._locals.Add(variable);
    return base.VisitBlock(node);
  }

  protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
  {
    foreach (ParameterExpression variable in node.Variables)
      this._strongBoxedLocals.Add(variable, (object) null);
    return base.VisitRuntimeVariables(node);
  }

  protected override Expression VisitLambda<T>(Expression<T> node) => (Expression) node;
}
