// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LambdaParameterRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal sealed class LambdaParameterRewriter : ExpressionVisitor
{
  private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

  internal LambdaParameterRewriter(
    Dictionary<ParameterExpression, ParameterExpression> map)
  {
    this._map = map;
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    ParameterExpression parameterExpression;
    return this._map.TryGetValue(node, out parameterExpression) ? (Expression) parameterExpression : (Expression) node;
  }
}
