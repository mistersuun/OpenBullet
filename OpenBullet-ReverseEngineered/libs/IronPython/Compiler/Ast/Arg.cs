// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.Arg
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class Arg : Node
{
  private readonly string _name;
  private readonly Expression _expression;

  public Arg(Expression expression)
    : this((string) null, expression)
  {
  }

  public Arg(string name, Expression expression)
  {
    this._name = name;
    this._expression = expression;
  }

  public string Name => this._name;

  public Expression Expression => this._expression;

  public override string ToString() => $"{base.ToString()}:{this._name}";

  internal Argument GetArgumentInfo()
  {
    if (this._name == null)
      return Argument.Simple;
    if (this._name == "*")
      return new Argument(ArgumentType.List);
    return this._name == "**" ? new Argument(ArgumentType.Dictionary) : new Argument(this._name);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }
}
