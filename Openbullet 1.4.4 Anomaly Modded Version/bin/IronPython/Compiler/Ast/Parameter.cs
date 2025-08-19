// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.Parameter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class Parameter : Node
{
  private readonly string _name;
  protected readonly ParameterKind _kind;
  protected Expression _defaultValue;
  private PythonVariable _variable;
  private ParameterExpression _parameter;

  public Parameter(string name)
    : this(name, ParameterKind.Normal)
  {
  }

  public Parameter(string name, ParameterKind kind)
  {
    this._name = name;
    this._kind = kind;
  }

  public string Name => this._name;

  public Expression DefaultValue
  {
    get => this._defaultValue;
    set => this._defaultValue = value;
  }

  public bool IsList => this._kind == ParameterKind.List;

  public bool IsDictionary => this._kind == ParameterKind.Dictionary;

  internal ParameterKind Kind => this._kind;

  internal PythonVariable PythonVariable
  {
    get => this._variable;
    set => this._variable = value;
  }

  internal System.Linq.Expressions.Expression FinishBind(bool needsLocalsDictionary)
  {
    if (!(this._variable.AccessedInNestedScope | needsLocalsDictionary))
      return (System.Linq.Expressions.Expression) (this._parameter = System.Linq.Expressions.Expression.Parameter(typeof (object), this.Name));
    this._parameter = System.Linq.Expressions.Expression.Parameter(typeof (object), this.Name);
    return (System.Linq.Expressions.Expression) new ClosureExpression(this._variable, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Parameter(typeof (ClosureCell), this.Name), this._parameter);
  }

  internal ParameterExpression ParameterExpression => this._parameter;

  internal virtual void Init(List<System.Linq.Expressions.Expression> init)
  {
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._defaultValue != null)
      this._defaultValue.Walk(walker);
    walker.PostWalk(this);
  }
}
