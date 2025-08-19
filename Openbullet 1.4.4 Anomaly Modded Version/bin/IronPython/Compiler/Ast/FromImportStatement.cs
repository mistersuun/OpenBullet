// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.FromImportStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class FromImportStatement : Statement
{
  private static readonly string[] _star = new string[1]
  {
    "*"
  };
  private readonly ModuleName _root;
  private readonly string[] _names;
  private readonly string[] _asNames;
  private readonly bool _fromFuture;
  private readonly bool _forceAbsolute;
  private PythonVariable[] _variables;

  public static IList<string> Star => (IList<string>) FromImportStatement._star;

  public DottedName Root => (DottedName) this._root;

  public bool IsFromFuture => this._fromFuture;

  public IList<string> Names => (IList<string>) this._names;

  public IList<string> AsNames => (IList<string>) this._asNames;

  internal PythonVariable[] Variables
  {
    get => this._variables;
    set => this._variables = value;
  }

  public FromImportStatement(
    ModuleName root,
    string[] names,
    string[] asNames,
    bool fromFuture,
    bool forceAbsolute)
  {
    this._root = root;
    this._names = names;
    this._asNames = asNames;
    this._fromFuture = fromFuture;
    this._forceAbsolute = forceAbsolute;
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    if (this._names == FromImportStatement._star)
      return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ImportStar, this.Parent.LocalContext, Utils.Constant((object) this._root.MakeString()), Utils.Constant(this.GetLevel())), this.Span);
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>();
    ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Variable(typeof (object), "module");
    System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[this._names.Length];
    for (int index = 0; index < expressionArray.Length; ++index)
      expressionArray[index] = Utils.Constant((object) this._names[index]);
    collectionBuilder.Add(this.GlobalParent.AddDebugInfoAndVoid(Node.AssignValue((System.Linq.Expressions.Expression) parameterExpression, LightExceptions.CheckAndThrow((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ImportWithNames, this.Parent.LocalContext, Utils.Constant((object) this._root.MakeString()), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (string), expressionArray), Utils.Constant(this.GetLevel())))), this._root.Span));
    for (int index = 0; index < expressionArray.Length; ++index)
      collectionBuilder.Add(this.GlobalParent.AddDebugInfoAndVoid(Node.AssignValue(this.Parent.GetVariableExpression(this._variables[index]), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ImportFrom, this.Parent.LocalContext, (System.Linq.Expressions.Expression) parameterExpression, expressionArray[index])), this.Span));
    collectionBuilder.Add((System.Linq.Expressions.Expression) Utils.Empty());
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, collectionBuilder.ToArray()), this.Span);
  }

  private object GetLevel()
  {
    if (this._root is RelativeModuleName root)
      return (object) root.DotCount;
    return this._forceAbsolute ? (object) 0 : (object) -1;
  }

  public override void Walk(PythonWalker walker)
  {
    walker.Walk(this);
    walker.PostWalk(this);
  }
}
