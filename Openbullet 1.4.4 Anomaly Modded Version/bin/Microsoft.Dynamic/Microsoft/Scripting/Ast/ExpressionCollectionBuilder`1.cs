// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.ExpressionCollectionBuilder`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class ExpressionCollectionBuilder<TExpression> : 
  IEnumerable<TExpression>,
  IEnumerable,
  ICollection<TExpression>
{
  private int _count;
  private ReadOnlyCollectionBuilder<TExpression> _expressions;

  public TExpression Expression0 { get; private set; }

  public TExpression Expression1 { get; private set; }

  public TExpression Expression2 { get; private set; }

  public TExpression Expression3 { get; private set; }

  public int Count => this._count;

  public ReadOnlyCollectionBuilder<TExpression> Expressions => this._expressions;

  public void Add(IEnumerable<TExpression> expressions)
  {
    if (expressions == null)
      return;
    foreach (TExpression expression in expressions)
      this.Add(expression);
  }

  public void Add(TExpression item)
  {
    if ((object) item == null)
      return;
    switch (this._count)
    {
      case 0:
        this.Expression0 = item;
        break;
      case 1:
        this.Expression1 = item;
        break;
      case 2:
        this.Expression2 = item;
        break;
      case 3:
        this.Expression3 = item;
        break;
      case 4:
        this._expressions = new ReadOnlyCollectionBuilder<TExpression>()
        {
          this.Expression0,
          this.Expression1,
          this.Expression2,
          this.Expression3,
          item
        };
        break;
      default:
        this._expressions.Add(item);
        break;
    }
    ++this._count;
  }

  private IEnumerator<TExpression> GetItemEnumerator()
  {
    if (this._count > 0)
      yield return this.Expression0;
    if (this._count > 1)
      yield return this.Expression1;
    if (this._count > 2)
      yield return this.Expression2;
    if (this._count > 3)
      yield return this.Expression3;
  }

  public IEnumerator<TExpression> GetEnumerator()
  {
    return this._expressions != null ? this._expressions.GetEnumerator() : this.GetItemEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) CollectionUtils.ToCovariant<TExpression, object>((IEnumerable<TExpression>) this).GetEnumerator();
  }

  public void Clear()
  {
    TExpression expression = default (TExpression);
    this.Expression3 = expression;
    this.Expression0 = this.Expression1 = this.Expression2 = expression;
    this._expressions = (ReadOnlyCollectionBuilder<TExpression>) null;
    this._count = 0;
  }

  public bool Contains(TExpression item)
  {
    return this.Any<TExpression>((Func<TExpression, bool>) (e => e.Equals((object) item)));
  }

  public void CopyTo(TExpression[] array, int arrayIndex)
  {
    foreach (TExpression expression in this)
      array[arrayIndex++] = expression;
  }

  public bool IsReadOnly => false;

  public bool Remove(TExpression item) => throw new NotImplementedException();
}
