// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ImportStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ImportStatement : Statement
{
  private readonly ModuleName[] _names;
  private readonly string[] _asNames;
  private readonly bool _forceAbsolute;
  private PythonVariable[] _variables;

  public ImportStatement(ModuleName[] names, string[] asNames, bool forceAbsolute)
  {
    this._names = names;
    this._asNames = asNames;
    this._forceAbsolute = forceAbsolute;
  }

  internal PythonVariable[] Variables
  {
    get => this._variables;
    set => this._variables = value;
  }

  public IList<DottedName> Names => (IList<DottedName>) this._names;

  public IList<string> AsNames => (IList<string>) this._asNames;

  public override System.Linq.Expressions.Expression Reduce()
  {
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>();
    for (int index = 0; index < this._names.Length; ++index)
      collectionBuilder.Add(this.GlobalParent.AddDebugInfoAndVoid(Node.AssignValue(this.Parent.GetVariableExpression(this._variables[index]), LightExceptions.CheckAndThrow((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(this._asNames[index] == null ? AstMethods.ImportTop : AstMethods.ImportBottom, this.Parent.LocalContext, Utils.Constant((object) this._names[index].MakeString()), Utils.Constant((object) (this._forceAbsolute ? 0 : -1))))), this._names[index].Span));
    collectionBuilder.Add((System.Linq.Expressions.Expression) Utils.Empty());
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder.ToReadOnlyCollection()), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    walker.Walk(this);
    walker.PostWalk(this);
  }
}
