// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.NameExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;

#nullable disable
namespace IronPython.Compiler.Ast;

public class NameExpression : Expression
{
  private readonly string _name;
  private PythonReference _reference;
  private bool _assigned;

  public NameExpression(string name) => this._name = name;

  public string Name => this._name;

  internal PythonReference Reference
  {
    get => this._reference;
    set => this._reference = value;
  }

  internal bool Assigned
  {
    get => this._assigned;
    set => this._assigned = value;
  }

  public override string ToString() => $"{base.ToString()}:{this._name}";

  public override System.Linq.Expressions.Expression Reduce()
  {
    System.Linq.Expressions.Expression expression = this._reference.PythonVariable != null ? this.Parent.GetVariableExpression(this._reference.PythonVariable) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.LookupName, this.Parent.LocalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this._name));
    if (!this._assigned && !(expression is IPythonGlobalExpression))
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.CheckUninitialized, expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this._name));
    return expression;
  }

  internal override System.Linq.Expressions.Expression TransformSet(
    SourceSpan span,
    System.Linq.Expressions.Expression right,
    PythonOperationKind op)
  {
    if (op != PythonOperationKind.None)
      right = this.GlobalParent.Operation(typeof (object), op, (System.Linq.Expressions.Expression) this, right);
    SourceSpan location = span.IsValid ? new SourceSpan(this.Span.Start, span.End) : SourceSpan.None;
    return this.GlobalParent.AddDebugInfoAndVoid(this._reference.PythonVariable == null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) null, AstMethods.SetName, this.Parent.LocalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this._name), Utils.Convert(right, typeof (object))) : Node.AssignValue(this.Parent.GetVariableExpression(this._reference.PythonVariable), Node.ConvertIfNeeded(right, typeof (object))), location);
  }

  internal override string CheckAssign() => (string) null;

  internal override string CheckDelete() => (string) null;

  internal override System.Linq.Expressions.Expression TransformDelete()
  {
    if (this._reference.PythonVariable == null)
      return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.RemoveName, this.Parent.LocalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this._name));
    System.Linq.Expressions.Expression variableExpression = this.Parent.GetVariableExpression(this._reference.PythonVariable);
    System.Linq.Expressions.Expression expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.KeepAlive, variableExpression), Node.Delete(variableExpression));
    if (!this._assigned)
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) this, expression, (System.Linq.Expressions.Expression) Utils.Empty());
    return expression;
  }

  public override void Walk(PythonWalker walker)
  {
    walker.Walk(this);
    walker.PostWalk(this);
  }

  internal override bool CanThrow => !this.Assigned;
}
