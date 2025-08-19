// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.GeneratorExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public sealed class GeneratorExpression : Expression
{
  private readonly LabelTarget _target;
  private readonly Expression _body;
  private Expression _reduced;
  private readonly string _name;
  private readonly bool _rewriteAssignments;

  internal GeneratorExpression(
    string name,
    Type type,
    LabelTarget label,
    Expression body,
    bool rewriteAssignments)
  {
    this._target = label;
    this._body = body;
    this.Type = type;
    this._name = name;
    this._rewriteAssignments = rewriteAssignments;
  }

  public override bool CanReduce => true;

  public sealed override Type Type { get; }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public string Name => this._name;

  public LabelTarget Target => this._target;

  public Expression Body => this._body;

  public bool RewriteAssignments => this._rewriteAssignments;

  public override Expression Reduce()
  {
    if (this._reduced == null)
      this._reduced = new GeneratorRewriter(this).Reduce();
    return this._reduced;
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression body = visitor.Visit(this._body);
    return body == this._body ? (Expression) this : (Expression) Utils.Generator(this._name, this._target, body, this.Type);
  }

  internal bool IsEnumerable => Utils.IsEnumerableType(this.Type);
}
