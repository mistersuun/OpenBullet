// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ScopeStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Compiler.Ast;

public abstract class ScopeStatement : Statement
{
  private bool _importStar;
  private bool _unqualifiedExec;
  private bool _nestedFreeVariables;
  private bool _locals;
  private bool _hasLateboundVarSets;
  private bool _containsExceptionHandling;
  private bool _forceCompile;
  private FunctionCode _funcCode;
  private Dictionary<string, PythonVariable> _variables;
  private ScopeStatement.ClosureInfo[] _closureVariables;
  private List<PythonVariable> _freeVars;
  private List<string> _globalVars;
  private List<string> _cellVars;
  private Dictionary<string, PythonReference> _references;
  internal Dictionary<PythonVariable, System.Linq.Expressions.Expression> _variableMapping = new Dictionary<PythonVariable, System.Linq.Expressions.Expression>();
  private ParameterExpression _localParentTuple;
  private readonly ScopeStatement.DelayedFunctionCode _funcCodeExpr = new ScopeStatement.DelayedFunctionCode();
  internal static ParameterExpression LocalCodeContextVariable = System.Linq.Expressions.Expression.Parameter(typeof (CodeContext), "$localContext");
  private static ParameterExpression _catchException = System.Linq.Expressions.Expression.Parameter(typeof (Exception), "$updException");
  internal const string NameForExec = "module: <exec>";

  internal bool ContainsImportStar
  {
    get => this._importStar;
    set => this._importStar = value;
  }

  internal bool ContainsExceptionHandling
  {
    get => this._containsExceptionHandling;
    set => this._containsExceptionHandling = value;
  }

  internal bool ContainsUnqualifiedExec
  {
    get => this._unqualifiedExec;
    set => this._unqualifiedExec = value;
  }

  internal virtual bool IsGeneratorMethod => false;

  internal ParameterExpression LocalParentTuple => this._localParentTuple;

  internal virtual System.Linq.Expressions.Expression LocalContext
  {
    get => (System.Linq.Expressions.Expression) ScopeStatement.LocalCodeContextVariable;
  }

  internal bool IsClosure => this.FreeVariables != null && this.FreeVariables.Count > 0;

  internal bool ContainsNestedFreeVariables
  {
    get => this._nestedFreeVariables;
    set => this._nestedFreeVariables = value;
  }

  internal bool NeedsLocalsDictionary
  {
    get => this._locals;
    set => this._locals = value;
  }

  public virtual string Name => "<unknown>";

  internal virtual string Filename => this.GlobalParent.SourceUnit.Path ?? "<string>";

  internal virtual bool HasLateBoundVariableSets
  {
    get => this._hasLateboundVarSets;
    set => this._hasLateboundVarSets = value;
  }

  internal Dictionary<string, PythonVariable> Variables => this._variables;

  internal virtual bool IsGlobal => false;

  internal bool NeedsLocalContext => this.NeedsLocalsDictionary || this.ContainsNestedFreeVariables;

  internal virtual string[] ParameterNames => ArrayUtils.EmptyStrings;

  internal virtual int ArgCount => 0;

  internal virtual FunctionAttributes Flags => FunctionAttributes.None;

  internal abstract LightLambdaExpression GetLambda();

  internal FunctionCode GetOrMakeFunctionCode()
  {
    if (this._funcCode == null)
      Interlocked.CompareExchange<FunctionCode>(ref this._funcCode, new FunctionCode(this.GlobalParent.PyContext, this.OriginalDelegate, this, this.ScopeDocumentation, new bool?(), true), (FunctionCode) null);
    return this._funcCode;
  }

  internal virtual string ScopeDocumentation => (string) null;

  internal virtual Delegate OriginalDelegate => (Delegate) null;

  internal virtual IList<string> GetVarNames()
  {
    List<string> res = new List<string>();
    this.AppendVariables(res);
    return (IList<string>) res;
  }

  internal void AddFreeVariable(PythonVariable variable, bool accessedInScope)
  {
    if (this._freeVars == null)
      this._freeVars = new List<PythonVariable>();
    if (this._freeVars.Contains(variable))
      return;
    this._freeVars.Add(variable);
  }

  internal bool ShouldInterpret
  {
    get
    {
      if (this._forceCompile)
        return false;
      if (this.GlobalParent.CompilationMode == CompilationMode.Lookup)
        return true;
      CompilerContext compilerContext = this.GlobalParent.CompilerContext;
      return ((PythonContext) compilerContext.SourceUnit.LanguageContext).ShouldInterpret((PythonCompilerOptions) compilerContext.Options, compilerContext.SourceUnit);
    }
    set => this._forceCompile = !value;
  }

  internal string AddReferencedGlobal(string name)
  {
    if (this._globalVars == null)
      this._globalVars = new List<string>();
    if (!this._globalVars.Contains(name))
      this._globalVars.Add(name);
    return name;
  }

  internal void AddCellVariable(PythonVariable variable)
  {
    if (this._cellVars == null)
      this._cellVars = new List<string>();
    if (this._cellVars.Contains(variable.Name))
      return;
    this._cellVars.Add(variable.Name);
  }

  internal List<string> AppendVariables(List<string> res)
  {
    if (this.Variables != null)
    {
      foreach (KeyValuePair<string, PythonVariable> variable in this.Variables)
      {
        if (variable.Value.Kind == VariableKind.Local && (this.CellVariables == null || !this.CellVariables.Contains(variable.Key)))
          res.Add(variable.Key);
      }
    }
    return res;
  }

  internal IList<PythonVariable> FreeVariables => (IList<PythonVariable>) this._freeVars;

  internal IList<string> GlobalVariables => (IList<string>) this._globalVars;

  internal IList<string> CellVariables => (IList<string>) this._cellVars;

  internal Type GetClosureTupleType()
  {
    if (this.TupleCells <= 0)
      return (Type) null;
    Type[] typeArray = new Type[this.TupleCells];
    for (int index = 0; index < this.TupleCells; ++index)
      typeArray[index] = typeof (ClosureCell);
    return MutableTuple.MakeTupleType(typeArray);
  }

  internal virtual int TupleCells
  {
    get => this._closureVariables == null ? 0 : this._closureVariables.Length;
  }

  internal abstract bool ExposesLocalVariable(PythonVariable variable);

  internal virtual System.Linq.Expressions.Expression GetParentClosureTuple()
  {
    throw new NotSupportedException();
  }

  private bool TryGetAnyVariable(string name, out PythonVariable variable)
  {
    if (this._variables != null)
      return this._variables.TryGetValue(name, out variable);
    variable = (PythonVariable) null;
    return false;
  }

  internal bool TryGetVariable(string name, out PythonVariable variable)
  {
    if (this.TryGetAnyVariable(name, out variable))
      return true;
    variable = (PythonVariable) null;
    return false;
  }

  internal virtual bool TryBindOuter(
    ScopeStatement from,
    PythonReference reference,
    out PythonVariable variable)
  {
    variable = (PythonVariable) null;
    return false;
  }

  internal abstract PythonVariable BindReference(PythonNameBinder binder, PythonReference reference);

  internal virtual void Bind(PythonNameBinder binder)
  {
    if (this._references == null)
      return;
    foreach (PythonReference reference in this._references.Values)
    {
      PythonVariable pythonVariable;
      reference.PythonVariable = pythonVariable = this.BindReference(binder, reference);
      if (pythonVariable != null && pythonVariable.Deleted && pythonVariable.Scope != this && !pythonVariable.Scope.IsGlobal)
        binder.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "can not delete variable '{0}' referenced in nested scope", (object) reference.Name), (Node) this);
    }
  }

  internal virtual void FinishBind(PythonNameBinder binder)
  {
    List<ScopeStatement.ClosureInfo> closureVariables1 = (List<ScopeStatement.ClosureInfo>) null;
    if (this.FreeVariables != null && this.FreeVariables.Count > 0)
    {
      this._localParentTuple = System.Linq.Expressions.Expression.Parameter(this.Parent.GetClosureTupleType(), "$tuple");
      foreach (PythonVariable freeVar in this._freeVars)
      {
        ScopeStatement.ClosureInfo[] closureVariables2 = this.Parent._closureVariables;
        for (int index = 0; index < closureVariables2.Length; ++index)
        {
          if (closureVariables2[index].Variable == freeVar)
          {
            this._variableMapping[freeVar] = (System.Linq.Expressions.Expression) new ClosureExpression(freeVar, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) this._localParentTuple, $"Item{index:D3}"), (ParameterExpression) null);
            break;
          }
        }
        if (closureVariables1 == null)
          closureVariables1 = new List<ScopeStatement.ClosureInfo>();
        closureVariables1.Add(new ScopeStatement.ClosureInfo(freeVar, !(this is ClassDefinition)));
      }
    }
    if (this.Variables != null)
    {
      foreach (PythonVariable pythonVariable in this.Variables.Values)
      {
        if (!ScopeStatement.HasClosureVariable(closureVariables1, pythonVariable) && !pythonVariable.IsGlobal && (pythonVariable.AccessedInNestedScope || this.ExposesLocalVariable(pythonVariable)))
        {
          if (closureVariables1 == null)
            closureVariables1 = new List<ScopeStatement.ClosureInfo>();
          closureVariables1.Add(new ScopeStatement.ClosureInfo(pythonVariable, true));
        }
        if (pythonVariable.Kind == VariableKind.Local)
          this._variableMapping[pythonVariable] = pythonVariable.AccessedInNestedScope || this.ExposesLocalVariable(pythonVariable) ? (System.Linq.Expressions.Expression) new ClosureExpression(pythonVariable, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Parameter(typeof (ClosureCell), pythonVariable.Name), (ParameterExpression) null) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Parameter(typeof (object), pythonVariable.Name);
      }
    }
    if (closureVariables1 != null)
      this._closureVariables = closureVariables1.ToArray();
    this._references = (Dictionary<string, PythonReference>) null;
  }

  private static bool HasClosureVariable(
    List<ScopeStatement.ClosureInfo> closureVariables,
    PythonVariable variable)
  {
    if (closureVariables == null)
      return false;
    for (int index = 0; index < closureVariables.Count; ++index)
    {
      if (closureVariables[index].Variable == variable)
        return true;
    }
    return false;
  }

  private void EnsureVariables()
  {
    if (this._variables != null)
      return;
    this._variables = new Dictionary<string, PythonVariable>((IEqualityComparer<string>) StringComparer.Ordinal);
  }

  internal void AddGlobalVariable(PythonVariable variable)
  {
    this.EnsureVariables();
    this._variables[variable.Name] = variable;
  }

  internal PythonReference Reference(string name)
  {
    if (this._references == null)
      this._references = new Dictionary<string, PythonReference>((IEqualityComparer<string>) StringComparer.Ordinal);
    PythonReference pythonReference;
    if (!this._references.TryGetValue(name, out pythonReference))
      this._references[name] = pythonReference = new PythonReference(name);
    return pythonReference;
  }

  internal bool IsReferenced(string name)
  {
    return this._references != null && this._references.TryGetValue(name, out PythonReference _);
  }

  internal PythonVariable CreateVariable(string name, VariableKind kind)
  {
    this.EnsureVariables();
    return this._variables[name] = new PythonVariable(name, kind, this);
  }

  internal PythonVariable EnsureVariable(string name)
  {
    PythonVariable variable;
    return !this.TryGetVariable(name, out variable) ? this.CreateVariable(name, VariableKind.Local) : variable;
  }

  internal PythonVariable DefineParameter(string name)
  {
    return this.CreateVariable(name, VariableKind.Parameter);
  }

  internal PythonContext PyContext
  {
    get => (PythonContext) this.GlobalParent.CompilerContext.SourceUnit.LanguageContext;
  }

  private SymbolDocumentInfo Document => this.GlobalParent.Document;

  internal System.Linq.Expressions.Expression AddDebugInfo(
    System.Linq.Expressions.Expression expression,
    SourceLocation start,
    SourceLocation end)
  {
    if (this.PyContext.PythonOptions.GCStress.HasValue)
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (GC).GetMethod("Collect", new Type[1]
      {
        typeof (int)
      }), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this.PyContext.PythonOptions.GCStress.Value)), expression);
    return Microsoft.Scripting.Ast.Utils.AddDebugInfo(expression, this.Document, start, end);
  }

  internal System.Linq.Expressions.Expression AddDebugInfo(
    System.Linq.Expressions.Expression expression,
    SourceSpan location)
  {
    return this.AddDebugInfo(expression, location.Start, location.End);
  }

  internal System.Linq.Expressions.Expression AddDebugInfoAndVoid(
    System.Linq.Expressions.Expression expression,
    SourceSpan location)
  {
    if (expression.Type != typeof (void))
      expression = Microsoft.Scripting.Ast.Utils.Void(expression);
    return this.AddDebugInfo(expression, location);
  }

  internal System.Linq.Expressions.Expression GetUpdateTrackbackExpression(
    ParameterExpression exception)
  {
    return !this._containsExceptionHandling ? this.UpdateStackTrace(exception) : this.GetSaveLineNumberExpression(exception, true);
  }

  private System.Linq.Expressions.Expression UpdateStackTrace(ParameterExpression exception)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.UpdateStackTrace, (System.Linq.Expressions.Expression) exception, this.LocalContext, (System.Linq.Expressions.Expression) this._funcCodeExpr, (System.Linq.Expressions.Expression) Node.LineNumberExpression);
  }

  internal System.Linq.Expressions.Expression GetSaveLineNumberExpression(
    ParameterExpression exception,
    bool preventAdditionalAdds)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.If((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Not((System.Linq.Expressions.Expression) Node.LineNumberUpdated), this.UpdateStackTrace(exception)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) Node.LineNumberUpdated, Microsoft.Scripting.Ast.Utils.Constant((object) preventAdditionalAdds)), (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Empty());
  }

  internal System.Linq.Expressions.Expression WrapScopeStatements(System.Linq.Expressions.Expression body, bool canThrow)
  {
    if (canThrow)
      body = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
      {
        Node.LineNumberExpression,
        Node.LineNumberUpdated
      }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.TryCatch(body, System.Linq.Expressions.Expression.Catch(ScopeStatement._catchException, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(this.GetUpdateTrackbackExpression(ScopeStatement._catchException), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Rethrow(body.Type)))));
    return body;
  }

  internal System.Linq.Expressions.Expression FuncCodeExpr
  {
    get => this._funcCodeExpr.Code;
    set => this._funcCodeExpr.Code = value;
  }

  internal MethodCallExpression CreateLocalContext(System.Linq.Expressions.Expression parentContext)
  {
    ScopeStatement.ClosureInfo[] input = this._closureVariables;
    if (this._closureVariables == null)
      input = new ScopeStatement.ClosureInfo[0];
    return System.Linq.Expressions.Expression.Call(AstMethods.CreateLocalContext, parentContext, MutableTuple.Create(ArrayUtils.ConvertAll<ScopeStatement.ClosureInfo, System.Linq.Expressions.Expression>(input, (Func<ScopeStatement.ClosureInfo, System.Linq.Expressions.Expression>) (x => this.GetClosureCell(x)))), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) ArrayUtils.ConvertAll<ScopeStatement.ClosureInfo, string>(input, (Func<ScopeStatement.ClosureInfo, string>) (x => !x.AccessedInScope ? (string) null : x.Variable.Name))));
  }

  private System.Linq.Expressions.Expression GetClosureCell(ScopeStatement.ClosureInfo variable)
  {
    return ((ClosureExpression) this.GetVariableExpression(variable.Variable)).ClosureCell;
  }

  internal virtual System.Linq.Expressions.Expression GetVariableExpression(PythonVariable variable)
  {
    return variable.IsGlobal ? this.GlobalParent.ModuleVariables[variable] : this._variableMapping[variable];
  }

  internal void CreateVariables(
    ReadOnlyCollectionBuilder<ParameterExpression> locals,
    List<System.Linq.Expressions.Expression> init)
  {
    if (this.Variables != null)
    {
      foreach (PythonVariable variable in this.Variables.Values)
      {
        if (variable.Kind != VariableKind.Global)
        {
          if (this.GetVariableExpression(variable) is ClosureExpression variableExpression)
          {
            init.Add(variableExpression.Create());
            locals.Add((ParameterExpression) variableExpression.ClosureCell);
          }
          else if (variable.Kind == VariableKind.Local)
          {
            locals.Add((ParameterExpression) this.GetVariableExpression(variable));
            if (variable.ReadBeforeInitialized)
              init.Add(Node.AssignValue(this.GetVariableExpression(variable), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (Uninitialized).GetField("Instance"))));
          }
        }
      }
    }
    if (!this.IsClosure)
      return;
    Type closureTupleType = this.Parent.GetClosureTupleType();
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) this.LocalParentTuple, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(this.GetParentClosureTuple(), closureTupleType)));
    locals.Add(this.LocalParentTuple);
  }

  internal System.Linq.Expressions.Expression AddDecorators(
    System.Linq.Expressions.Expression ret,
    IList<Expression> decorators)
  {
    if (decorators != null)
    {
      for (int index = decorators.Count - 1; index >= 0; --index)
        ret = this.Parent.Invoke(new CallSignature(1), this.Parent.LocalContext, (System.Linq.Expressions.Expression) decorators[index], ret);
    }
    return ret;
  }

  internal System.Linq.Expressions.Expression Invoke(
    CallSignature signature,
    params System.Linq.Expressions.Expression[] args)
  {
    PythonInvokeBinder binder = this.PyContext.Invoke(signature);
    switch (args.Length)
    {
      case 1:
        return this.GlobalParent.CompilationMode.Dynamic((DynamicMetaObjectBinder) binder, typeof (object), args[0]);
      case 2:
        return this.GlobalParent.CompilationMode.Dynamic((DynamicMetaObjectBinder) binder, typeof (object), args[0], args[1]);
      case 3:
        return this.GlobalParent.CompilationMode.Dynamic((DynamicMetaObjectBinder) binder, typeof (object), args[0], args[1], args[2]);
      case 4:
        return this.GlobalParent.CompilationMode.Dynamic((DynamicMetaObjectBinder) binder, typeof (object), args[0], args[1], args[2], args[3]);
      default:
        return this.GlobalParent.CompilationMode.Dynamic((DynamicMetaObjectBinder) binder, typeof (object), args);
    }
  }

  internal ScopeStatement CopyForRewrite() => (ScopeStatement) this.MemberwiseClone();

  internal virtual void RewriteBody(ExpressionVisitor visitor)
  {
    this._funcCode = (FunctionCode) null;
  }

  internal virtual bool PrintExpressions => false;

  internal virtual string ProfilerName => this.Name;

  internal System.Linq.Expressions.Expression AddProfiling(System.Linq.Expressions.Expression body)
  {
    if (this.GlobalParent._profiler == null)
      return body;
    ParameterExpression tick = System.Linq.Expressions.Expression.Variable(typeof (long), "$tick");
    return (System.Linq.Expressions.Expression) new ScopeStatement.DelayedProfiling(this, body, tick);
  }

  private class DelayedFunctionCode : System.Linq.Expressions.Expression
  {
    private System.Linq.Expressions.Expression _funcCode;

    public override bool CanReduce => true;

    public System.Linq.Expressions.Expression Code
    {
      get => this._funcCode;
      set => this._funcCode = value;
    }

    public override Type Type => typeof (FunctionCode);

    protected override System.Linq.Expressions.Expression VisitChildren(ExpressionVisitor visitor)
    {
      if (this._funcCode != null)
      {
        System.Linq.Expressions.Expression expression = visitor.Visit(this._funcCode);
        if (expression != this._funcCode)
          return (System.Linq.Expressions.Expression) new ScopeStatement.DelayedFunctionCode()
          {
            _funcCode = expression
          };
      }
      return (System.Linq.Expressions.Expression) this;
    }

    public override System.Linq.Expressions.Expression Reduce() => this._funcCode;

    public override ExpressionType NodeType => ExpressionType.Extension;
  }

  private struct ClosureInfo(PythonVariable variable, bool accessedInScope)
  {
    public PythonVariable Variable = variable;
    public bool AccessedInScope = accessedInScope;
  }

  private class DelayedProfiling : System.Linq.Expressions.Expression
  {
    private readonly ScopeStatement _ast;
    private readonly System.Linq.Expressions.Expression _body;
    private readonly ParameterExpression _tick;

    public DelayedProfiling(ScopeStatement ast, System.Linq.Expressions.Expression body, ParameterExpression tick)
    {
      this._ast = ast;
      this._body = body;
      this._tick = tick;
    }

    public override bool CanReduce => true;

    public override Type Type => this._body.Type;

    protected override System.Linq.Expressions.Expression VisitChildren(ExpressionVisitor visitor)
    {
      return visitor.Visit(this._body);
    }

    public override System.Linq.Expressions.Expression Reduce()
    {
      string profilerName = this._ast.ProfilerName;
      bool unique = profilerName == "module: <exec>";
      return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        this._tick
      }, this._ast.GlobalParent._profiler.AddProfiling(this._body, this._tick, profilerName, unique));
    }

    public override ExpressionType NodeType => ExpressionType.Extension;
  }
}
