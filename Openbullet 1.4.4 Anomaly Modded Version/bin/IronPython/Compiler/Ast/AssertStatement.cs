// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.AssertStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;

#nullable disable
namespace IronPython.Compiler.Ast;

public class AssertStatement : Statement
{
  public AssertStatement(Expression test, Expression message)
  {
    this.Test = test;
    this.Message = message;
  }

  public Expression Test { get; }

  public Expression Message { get; }

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.Optimize ? (System.Linq.Expressions.Expression) Utils.Empty() : this.GlobalParent.AddDebugInfoAndVoid(Utils.Unless(this.TransformAndDynamicConvert(this.Test, typeof (bool)), this.Message == null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.RaiseAssertionErrorNoMessage, this.Parent.LocalContext) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.RaiseAssertionError, this.Parent.LocalContext, Node.TransformOrConstantNull(this.Message, typeof (object)))), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      this.Test?.Walk(walker);
      this.Message?.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
