// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.FieldBuilderExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.Generation;

public class FieldBuilderExpression : Expression
{
  private readonly FieldBuilder _builder;

  public FieldBuilderExpression(FieldBuilder builder) => this._builder = builder;

  public override bool CanReduce => true;

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => this._builder.FieldType;

  public override Expression Reduce()
  {
    return (Expression) Expression.Field((Expression) null, this.GetFieldInfo());
  }

  private FieldInfo GetFieldInfo()
  {
    return this._builder.DeclaringType.Module.ResolveField(this._builder.GetToken().Token);
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor) => (Expression) this;
}
