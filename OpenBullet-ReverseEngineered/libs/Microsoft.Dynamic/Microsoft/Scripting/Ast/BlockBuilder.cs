// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.BlockBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public sealed class BlockBuilder : ExpressionCollectionBuilder<Expression>
{
  public Expression ToExpression()
  {
    switch (this.Count)
    {
      case 0:
        return (Expression) null;
      case 1:
        return this.Expression0;
      case 2:
        return (Expression) Expression.Block(this.Expression0, this.Expression1);
      case 3:
        return (Expression) Expression.Block(this.Expression0, this.Expression1, this.Expression2);
      case 4:
        return (Expression) Expression.Block(this.Expression0, this.Expression1, this.Expression2, this.Expression3);
      default:
        return (Expression) Expression.Block((IEnumerable<Expression>) this.Expressions);
    }
  }

  public static implicit operator Expression(BlockBuilder block) => block.ToExpression();
}
