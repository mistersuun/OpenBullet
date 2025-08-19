// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonNameBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class PythonNameBinder : PythonWalker
{
  private PythonAst _globalScope;
  internal ScopeStatement _currentScope;
  private List<ScopeStatement> _scopes = new List<ScopeStatement>();
  private List<ILoopStatement> _loops = new List<ILoopStatement>();
  private List<int> _finallyCount = new List<int>();
  private DefineBinder _define;
  private DeleteBinder _delete;
  private ParameterBinder _parameter;
  private readonly CompilerContext _context;

  public CompilerContext Context => this._context;

  private PythonNameBinder(CompilerContext context)
  {
    this._define = new DefineBinder(this);
    this._delete = new DeleteBinder(this);
    this._parameter = new ParameterBinder(this);
    this._context = context;
  }

  internal static void BindAst(PythonAst ast, CompilerContext context)
  {
    new PythonNameBinder(context).Bind(ast);
  }

  private void Bind(PythonAst unboundAst)
  {
    this._currentScope = (ScopeStatement) (this._globalScope = unboundAst);
    this._finallyCount.Add(0);
    unboundAst.Walk((PythonWalker) this);
    foreach (ScopeStatement scope in this._scopes)
      scope.Bind(this);
    unboundAst.Bind(this);
    for (int index = this._scopes.Count - 1; index >= 0; --index)
      this._scopes[index].FinishBind(this);
    unboundAst.FinishBind(this);
    foreach (ScopeStatement scope in this._scopes)
      FlowChecker.Check(scope);
  }

  private void PushScope(ScopeStatement node)
  {
    node.Parent = this._currentScope;
    this._currentScope = node;
    this._finallyCount.Add(0);
  }

  private void PopScope()
  {
    this._scopes.Add(this._currentScope);
    this._currentScope = this._currentScope.Parent;
    this._finallyCount.RemoveAt(this._finallyCount.Count - 1);
  }

  internal PythonReference Reference(string name) => this._currentScope.Reference(name);

  internal PythonVariable DefineName(string name) => this._currentScope.EnsureVariable(name);

  internal PythonVariable DefineParameter(string name) => this._currentScope.DefineParameter(name);

  internal PythonVariable DefineDeleted(string name)
  {
    PythonVariable pythonVariable = this._currentScope.EnsureVariable(name);
    pythonVariable.Deleted = true;
    return pythonVariable;
  }

  internal void ReportSyntaxWarning(string message, Node node)
  {
    this._context.Errors.Add(this._context.SourceUnit, message, node.Span, -1, Severity.Warning);
  }

  internal void ReportSyntaxError(string message, Node node)
  {
    this._context.Errors.Add(this._context.SourceUnit, message, node.Span, -1, Severity.FatalError);
  }

  public override bool Walk(AssignmentStatement node)
  {
    node.Parent = this._currentScope;
    foreach (Node node1 in (IEnumerable<Expression>) node.Left)
      node1.Walk((PythonWalker) this._define);
    return true;
  }

  public override bool Walk(AugmentedAssignStatement node)
  {
    node.Parent = this._currentScope;
    node.Left.Walk((PythonWalker) this._define);
    return true;
  }

  public override void PostWalk(CallExpression node)
  {
    if (!node.NeedsLocalsDictionary())
      return;
    this._currentScope.NeedsLocalsDictionary = true;
  }

  public override bool Walk(ClassDefinition node)
  {
    node.PythonVariable = this.DefineName(node.Name);
    foreach (Node node1 in (IEnumerable<Expression>) node.Bases)
      node1.Walk((PythonWalker) this);
    if (node.Decorators != null)
    {
      foreach (Node decorator in (IEnumerable<Expression>) node.Decorators)
        decorator.Walk((PythonWalker) this);
    }
    this.PushScope((ScopeStatement) node);
    node.ModuleNameVariable = this._globalScope.EnsureGlobalVariable("__name__");
    if (node.Body.Documentation != null)
      node.DocVariable = this.DefineName("__doc__");
    node.ModVariable = this.DefineName("__module__");
    node.Body.Walk((PythonWalker) this);
    return false;
  }

  public override void PostWalk(ClassDefinition node) => this.PopScope();

  public override bool Walk(DelStatement node)
  {
    node.Parent = this._currentScope;
    foreach (Node expression in (IEnumerable<Expression>) node.Expressions)
      expression.Walk((PythonWalker) this._delete);
    return true;
  }

  public override bool Walk(ExecStatement node)
  {
    node.Parent = this._currentScope;
    if (node.Locals == null && node.Globals == null)
      this._currentScope.ContainsUnqualifiedExec = true;
    return true;
  }

  public override void PostWalk(ExecStatement node)
  {
    if (node.NeedsLocalsDictionary())
      this._currentScope.NeedsLocalsDictionary = true;
    if (node.Locals != null)
      return;
    this._currentScope.HasLateBoundVariableSets = true;
  }

  public override bool Walk(SetComprehension node)
  {
    node.Parent = this._currentScope;
    this.PushScope((ScopeStatement) node.Scope);
    return base.Walk(node);
  }

  public override void PostWalk(SetComprehension node)
  {
    base.PostWalk(node);
    this.PopScope();
    if (!node.Scope.NeedsLocalsDictionary)
      return;
    this._currentScope.NeedsLocalsDictionary = true;
  }

  public override bool Walk(DictionaryComprehension node)
  {
    node.Parent = this._currentScope;
    this.PushScope((ScopeStatement) node.Scope);
    return base.Walk(node);
  }

  public override void PostWalk(DictionaryComprehension node)
  {
    base.PostWalk(node);
    this.PopScope();
    if (!node.Scope.NeedsLocalsDictionary)
      return;
    this._currentScope.NeedsLocalsDictionary = true;
  }

  public override void PostWalk(ConditionalExpression node)
  {
    node.Parent = this._currentScope;
    base.PostWalk(node);
  }

  public override bool Walk(AndExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(Arg node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(AssertStatement node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(BackQuoteExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(BinaryExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(CallExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ComprehensionIf node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ConditionalExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ConstantExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(DictionaryExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(DottedName node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(EmptyStatement node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ErrorExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ExpressionStatement node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(GeneratorExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(IfStatement node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(IfStatementTest node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(IndexExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(LambdaExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ListComprehension node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ListExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(MemberExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ModuleName node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(OrExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(Parameter node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(ParenthesisExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(PrintStatement node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(RelativeModuleName node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(SetExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(SliceExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(SublistParameter node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(SuiteStatement node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(TryStatementHandler node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(TupleExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(UnaryExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(YieldExpression node)
  {
    node.Parent = this._currentScope;
    return base.Walk(node);
  }

  public override bool Walk(RaiseStatement node)
  {
    node.Parent = this._currentScope;
    node.InFinally = this._finallyCount[this._finallyCount.Count - 1] != 0;
    return base.Walk(node);
  }

  public override bool Walk(ForStatement node)
  {
    node.Parent = this._currentScope;
    if (this._currentScope is FunctionDefinition)
      this._currentScope.ShouldInterpret = false;
    node.Left.Walk((PythonWalker) this._define);
    if (node.Left != null)
      node.Left.Walk((PythonWalker) this);
    if (node.List != null)
      node.List.Walk((PythonWalker) this);
    this.PushLoop((ILoopStatement) node);
    if (node.Body != null)
      node.Body.Walk((PythonWalker) this);
    this.PopLoop();
    if (node.Else != null)
      node.Else.Walk((PythonWalker) this);
    return false;
  }

  private void PushLoop(ILoopStatement node)
  {
    node.BreakLabel = System.Linq.Expressions.Expression.Label("break");
    node.ContinueLabel = System.Linq.Expressions.Expression.Label("continue");
    this._loops.Add(node);
  }

  private void PopLoop() => this._loops.RemoveAt(this._loops.Count - 1);

  public override bool Walk(WhileStatement node)
  {
    node.Parent = this._currentScope;
    if (node.Test != null)
      node.Test.Walk((PythonWalker) this);
    this.PushLoop((ILoopStatement) node);
    if (node.Body != null)
      node.Body.Walk((PythonWalker) this);
    this.PopLoop();
    if (node.ElseStatement != null)
      node.ElseStatement.Walk((PythonWalker) this);
    return false;
  }

  public override bool Walk(BreakStatement node)
  {
    node.Parent = this._currentScope;
    node.LoopStatement = this._loops[this._loops.Count - 1];
    return base.Walk(node);
  }

  public override bool Walk(ContinueStatement node)
  {
    node.Parent = this._currentScope;
    node.LoopStatement = this._loops[this._loops.Count - 1];
    return base.Walk(node);
  }

  public override bool Walk(ReturnStatement node)
  {
    node.Parent = this._currentScope;
    if (this._currentScope is FunctionDefinition currentScope)
      currentScope._hasReturn = true;
    return base.Walk(node);
  }

  public override bool Walk(WithStatement node)
  {
    node.Parent = this._currentScope;
    this._currentScope.ContainsExceptionHandling = true;
    if (node.Variable != null)
    {
      string message = node.Variable.CheckAssign();
      if (message != null)
        this.ReportSyntaxError(message, (Node) node);
      node.Variable.Walk((PythonWalker) this._define);
    }
    return true;
  }

  public override bool Walk(FromImportStatement node)
  {
    node.Parent = this._currentScope;
    if (node.Names != FromImportStatement.Star)
    {
      PythonVariable[] pythonVariableArray = new PythonVariable[node.Names.Count];
      node.Root.Parent = this._currentScope;
      for (int index = 0; index < node.Names.Count; ++index)
      {
        string name = node.AsNames[index] != null ? node.AsNames[index] : node.Names[index];
        pythonVariableArray[index] = this.DefineName(name);
      }
      node.Variables = pythonVariableArray;
    }
    else
    {
      this._currentScope.ContainsImportStar = true;
      this._currentScope.NeedsLocalsDictionary = true;
      this._currentScope.HasLateBoundVariableSets = true;
    }
    return true;
  }

  public override bool Walk(FunctionDefinition node)
  {
    node._nameVariable = this._globalScope.EnsureGlobalVariable("__name__");
    if (!node.IsLambda)
      node.PythonVariable = this.DefineName(node.Name);
    foreach (Parameter parameter in (IEnumerable<Parameter>) node.Parameters)
    {
      if (parameter.DefaultValue != null)
        parameter.DefaultValue.Walk((PythonWalker) this);
    }
    if (node.Decorators != null)
    {
      foreach (Node decorator in (IEnumerable<Expression>) node.Decorators)
        decorator.Walk((PythonWalker) this);
    }
    this.PushScope((ScopeStatement) node);
    foreach (Node parameter in (IEnumerable<Parameter>) node.Parameters)
      parameter.Walk((PythonWalker) this._parameter);
    node.Body.Walk((PythonWalker) this);
    return false;
  }

  public override void PostWalk(FunctionDefinition node) => this.PopScope();

  public override bool Walk(GlobalStatement node)
  {
    node.Parent = this._currentScope;
    foreach (string name in (IEnumerable<string>) node.Names)
    {
      bool flag = false;
      PythonVariable variable1;
      if (this._currentScope.TryGetVariable(name, out variable1))
      {
        switch (variable1.Kind)
        {
          case VariableKind.Local:
          case VariableKind.Global:
            flag = true;
            this.ReportSyntaxWarning($"name '{name}' is assigned to before global declaration", (Node) node);
            break;
          case VariableKind.Parameter:
            this.ReportSyntaxError($"name '{name}' is local and global", (Node) node);
            break;
        }
      }
      if (this._currentScope.IsReferenced(name) && !flag)
        this.ReportSyntaxWarning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "name '{0}' is used prior to global declaration", (object) name), (Node) node);
      PythonVariable variable2 = this._globalScope.EnsureGlobalVariable(name);
      variable2.Kind = VariableKind.Global;
      if (variable1 == null)
        this._currentScope.AddGlobalVariable(variable2);
    }
    return true;
  }

  public override bool Walk(NameExpression node)
  {
    node.Parent = this._currentScope;
    node.Reference = this.Reference(node.Name);
    return true;
  }

  public override bool Walk(PythonAst node)
  {
    if (node.Module)
    {
      node.NameVariable = this.DefineName("__name__");
      node.FileVariable = this.DefineName("__file__");
      node.DocVariable = this.DefineName("__doc__");
      this.DefineName("__path__");
      this.DefineName("__builtins__");
      this.DefineName("__package__");
    }
    return true;
  }

  public override void PostWalk(PythonAst node)
  {
    this._currentScope = this._currentScope.Parent;
    this._finallyCount.RemoveAt(this._finallyCount.Count - 1);
  }

  public override bool Walk(ImportStatement node)
  {
    node.Parent = this._currentScope;
    PythonVariable[] pythonVariableArray = new PythonVariable[node.Names.Count];
    for (int index = 0; index < node.Names.Count; ++index)
    {
      string name = node.AsNames[index] != null ? node.AsNames[index] : node.Names[index].Names[0];
      pythonVariableArray[index] = this.DefineName(name);
      node.Names[index].Parent = this._currentScope;
    }
    node.Variables = pythonVariableArray;
    return true;
  }

  public override bool Walk(TryStatement node)
  {
    node.Parent = this._currentScope;
    this._currentScope.ContainsExceptionHandling = true;
    node.Body.Walk((PythonWalker) this);
    if (node.Handlers != null)
    {
      foreach (TryStatementHandler handler in (IEnumerable<TryStatementHandler>) node.Handlers)
      {
        if (handler.Target != null)
          handler.Target.Walk((PythonWalker) this._define);
        handler.Parent = this._currentScope;
        handler.Walk((PythonWalker) this);
      }
    }
    if (node.Else != null)
      node.Else.Walk((PythonWalker) this);
    if (node.Finally != null)
    {
      this._finallyCount[this._finallyCount.Count - 1]++;
      node.Finally.Walk((PythonWalker) this);
      this._finallyCount[this._finallyCount.Count - 1]--;
    }
    return false;
  }

  public override bool Walk(ComprehensionFor node)
  {
    node.Parent = this._currentScope;
    node.Left.Walk((PythonWalker) this._define);
    return true;
  }
}
