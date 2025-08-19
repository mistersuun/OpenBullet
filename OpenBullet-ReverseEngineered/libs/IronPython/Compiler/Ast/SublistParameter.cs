// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SublistParameter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Compiler.Ast;

public class SublistParameter : IronPython.Compiler.Ast.Parameter
{
  private readonly TupleExpression _tuple;

  public SublistParameter(int position, TupleExpression tuple)
    : base("." + (object) position, ParameterKind.Normal)
  {
    this._tuple = tuple;
  }

  public TupleExpression Tuple => this._tuple;

  internal override void Init(List<System.Linq.Expressions.Expression> init)
  {
    init.Add(this._tuple.TransformSet(this.Span, (System.Linq.Expressions.Expression) this.ParameterExpression, PythonOperationKind.None));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._tuple != null)
        this._tuple.Walk(walker);
      if (this._defaultValue != null)
        this._defaultValue.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
