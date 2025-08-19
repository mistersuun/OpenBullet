// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ComprehensionIf
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ComprehensionIf : ComprehensionIterator
{
  private readonly Expression _test;

  public ComprehensionIf(Expression test) => this._test = test;

  public Expression Test => this._test;

  internal override System.Linq.Expressions.Expression Transform(System.Linq.Expressions.Expression body)
  {
    return this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) Utils.If(this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, (System.Linq.Expressions.Expression) this._test), body), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._test != null)
      this._test.Walk(walker);
    walker.PostWalk(this);
  }
}
