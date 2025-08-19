// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonAst
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
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public sealed class PythonAst : ScopeStatement
{
  private Statement _body;
  private CompilationMode _mode;
  private readonly bool _isModule;
  private readonly bool _printExpressions;
  private ModuleOptions _languageFeatures;
  private readonly CompilerContext _compilerContext;
  private readonly SymbolDocumentInfo _document;
  private readonly string _name;
  internal int[] _lineLocations;
  private PythonVariable _docVariable;
  private PythonVariable _nameVariable;
  private PythonVariable _fileVariable;
  private ModuleContext _modContext;
  private readonly bool _onDiskProxy;
  internal System.Linq.Expressions.Expression _arrayExpression;
  private CompilationMode.ConstantInfo _contextInfo;
  private Dictionary<PythonVariable, System.Linq.Expressions.Expression> _globalVariables = new Dictionary<PythonVariable, System.Linq.Expressions.Expression>();
  internal readonly Profiler _profiler;
  internal const string GlobalContextName = "$globalContext";
  internal static ParameterExpression _functionCode = System.Linq.Expressions.Expression.Variable(typeof (FunctionCode), "$functionCode");
  internal static readonly ParameterExpression _globalArray = System.Linq.Expressions.Expression.Parameter(typeof (PythonGlobal[]), "$globalArray");
  internal static readonly ParameterExpression _globalContext = System.Linq.Expressions.Expression.Parameter(typeof (CodeContext), "$globalContext");
  internal static readonly ReadOnlyCollection<ParameterExpression> _arrayFuncParams = new ReadOnlyCollectionBuilder<ParameterExpression>((IEnumerable<ParameterExpression>) new ParameterExpression[2]
  {
    PythonAst._globalContext,
    PythonAst._functionCode
  }).ToReadOnlyCollection();

  public PythonAst(
    Statement body,
    bool isModule,
    ModuleOptions languageFeatures,
    bool printExpressions)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    this._body = body;
    this._isModule = isModule;
    this._printExpressions = printExpressions;
    this._languageFeatures = languageFeatures;
  }

  public PythonAst(
    Statement body,
    bool isModule,
    ModuleOptions languageFeatures,
    bool printExpressions,
    CompilerContext context,
    int[] lineLocations)
    : this(isModule, languageFeatures, printExpressions, context)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    this._body = body;
    this._lineLocations = lineLocations;
  }

  public PythonAst(
    bool isModule,
    ModuleOptions languageFeatures,
    bool printExpressions,
    CompilerContext context)
  {
    this._isModule = isModule;
    this._printExpressions = printExpressions;
    this._languageFeatures = languageFeatures;
    this._mode = ((PythonCompilerOptions) context.Options).CompilationMode ?? PythonAst.GetCompilationMode(context);
    this._compilerContext = context;
    this.FuncCodeExpr = (System.Linq.Expressions.Expression) PythonAst._functionCode;
    PythonCompilerOptions options = context.Options as PythonCompilerOptions;
    string fileName = !context.SourceUnit.HasPath || (options.Module & ModuleOptions.ExecOrEvalCode) != ModuleOptions.None ? "<module>" : context.SourceUnit.Path;
    this._name = fileName;
    if (((PythonContext) context.SourceUnit.LanguageContext).PythonOptions.EnableProfiler && this._mode != CompilationMode.ToDisk)
      this._profiler = Profiler.GetProfiler(this.PyContext);
    this._document = context.SourceUnit.Document ?? System.Linq.Expressions.Expression.SymbolDocument(fileName, this.PyContext.LanguageGuid, this.PyContext.VendorGuid);
  }

  internal PythonAst(CompilerContext context)
    : this((Statement) new EmptyStatement(), true, ModuleOptions.None, false, context, (int[]) null)
  {
    this._onDiskProxy = true;
  }

  public void ParsingFinished(int[] lineLocations, Statement body, ModuleOptions languageFeatures)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    this._body = this._body == null ? body : throw new InvalidOperationException("cannot set body twice");
    this._lineLocations = lineLocations;
    this._languageFeatures = languageFeatures;
  }

  public void Bind() => PythonNameBinder.BindAst(this, this._compilerContext);

  public override string Name => "<module>";

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._body != null)
      this._body.Walk(walker);
    walker.PostWalk(this);
  }

  internal override bool ExposesLocalVariable(PythonVariable variable) => true;

  internal override void FinishBind(PythonNameBinder binder)
  {
    this._contextInfo = this.CompilationMode.GetContext();
    PythonGlobal[] data = new PythonGlobal[this.Variables == null ? 0 : this.Variables.Count];
    Dictionary<string, PythonGlobal> globals = new Dictionary<string, PythonGlobal>();
    ModuleContext moduleContext = this._modContext = new ModuleContext(new PythonDictionary((DictionaryStorage) new GlobalDictionaryStorage(globals, data)), this.PyContext);
    if (this._mode == CompilationMode.ToDisk)
    {
      this._arrayExpression = (System.Linq.Expressions.Expression) PythonAst._globalArray;
    }
    else
    {
      ConstantExpression constantExpression = new ConstantExpression((object) data);
      constantExpression.Parent = (ScopeStatement) this;
      this._arrayExpression = (System.Linq.Expressions.Expression) constantExpression;
    }
    if (this.Variables != null)
    {
      int num = 0;
      foreach (PythonVariable pythonVariable in this.Variables.Values)
      {
        PythonGlobal global = new PythonGlobal(moduleContext.GlobalContext, pythonVariable.Name);
        this._globalVariables[pythonVariable] = this.CompilationMode.GetGlobal(this.GetGlobalContext(), globals.Count, pythonVariable, global);
        data[num++] = globals[pythonVariable.Name] = global;
      }
    }
    this.CompilationMode.PublishContext(moduleContext.GlobalContext, this._contextInfo);
  }

  internal override System.Linq.Expressions.Expression LocalContext => this.GetGlobalContext();

  internal override PythonVariable BindReference(PythonNameBinder binder, PythonReference reference)
  {
    return this.EnsureVariable(reference.Name);
  }

  internal override bool TryBindOuter(
    ScopeStatement from,
    PythonReference reference,
    out PythonVariable variable)
  {
    from.AddReferencedGlobal(reference.Name);
    if (from.HasLateBoundVariableSets)
    {
      variable = (PythonVariable) null;
      return false;
    }
    variable = this.EnsureGlobalVariable(reference.Name);
    return true;
  }

  internal override bool IsGlobal => true;

  internal PythonVariable DocVariable
  {
    get => this._docVariable;
    set => this._docVariable = value;
  }

  internal PythonVariable NameVariable
  {
    get => this._nameVariable;
    set => this._nameVariable = value;
  }

  internal PythonVariable FileVariable
  {
    get => this._fileVariable;
    set => this._fileVariable = value;
  }

  internal CompilerContext CompilerContext => this._compilerContext;

  internal System.Linq.Expressions.Expression GlobalArrayInstance => this._arrayExpression;

  internal SymbolDocumentInfo Document => this._document;

  internal Dictionary<PythonVariable, System.Linq.Expressions.Expression> ModuleVariables
  {
    get => this._globalVariables;
  }

  internal ModuleContext ModuleContext => this._modContext;

  internal PythonVariable EnsureGlobalVariable(string name)
  {
    PythonVariable variable;
    if (!this.TryGetVariable(name, out variable))
      variable = this.CreateVariable(name, VariableKind.Global);
    return variable;
  }

  public override Type Type => this.CompilationMode.DelegateType;

  public override System.Linq.Expressions.Expression Reduce() => (System.Linq.Expressions.Expression) this.GetLambda();

  internal override LightLambdaExpression GetLambda()
  {
    return this.CompilationMode.ReduceAst(this, ((PythonCompilerOptions) this._compilerContext.Options).ModuleName ?? "<unnamed>");
  }

  public bool TrueDivision => (this._languageFeatures & ModuleOptions.TrueDivision) != 0;

  public bool AllowWithStatement => (this._languageFeatures & ModuleOptions.WithStatement) != 0;

  public bool AbsoluteImports => (this._languageFeatures & ModuleOptions.AbsoluteImports) != 0;

  internal PythonDivisionOptions DivisionOptions => this.PyContext.PythonOptions.DivisionOptions;

  public Statement Body => this._body;

  public bool Module => this._isModule;

  internal ScriptCode ToScriptCode() => this.CompilationMode.MakeScriptCode(this);

  internal System.Linq.Expressions.Expression ReduceWorker()
  {
    if (this._body is ReturnStatement body1 && (this._languageFeatures == ModuleOptions.None || this._languageFeatures == (ModuleOptions.ExecOrEvalCode | ModuleOptions.Interpret) || this._languageFeatures == (ModuleOptions.ExecOrEvalCode | ModuleOptions.Interpret | ModuleOptions.LightThrow)))
    {
      ReturnStatement body = (ReturnStatement) this._body;
      System.Linq.Expressions.Expression expression = (this._languageFeatures & ModuleOptions.LightThrow) == ModuleOptions.None ? body1.Expression.Reduce() : LightExceptions.Rewrite(body1.Expression.Reduce());
      SourceLocation location1 = this.IndexToLocation(body.Expression.StartIndex);
      SourceLocation location2 = this.IndexToLocation(body.Expression.EndIndex);
      return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.DebugInfo(this._document, location1.Line, location1.Column, location2.Line, location2.Column), Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (object)));
    }
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> block = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>();
    this.AddInitialiation(block);
    if (this._isModule)
      block.Add(Node.AssignValue(this.GetVariableExpression(this._docVariable), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this.GetDocumentation(this._body))));
    if (!(this._body is SuiteStatement) && this._body.CanThrow)
      block.Add(Node.UpdateLineNumber(this._body.Start.Line));
    block.Add((System.Linq.Expressions.Expression) this._body);
    System.Linq.Expressions.Expression expression1 = this.AddProfiling(this.AddModulePublishing(this.WrapScopeStatements((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) block.ToReadOnlyCollection()), this.Body.CanThrow)));
    if ((((PythonCompilerOptions) this._compilerContext.Options).Module & ModuleOptions.LightThrow) != ModuleOptions.None)
      expression1 = LightExceptions.Rewrite(expression1);
    System.Linq.Expressions.Expression expression2 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Label(FunctionDefinition._returnLabel, Microsoft.Scripting.Ast.Utils.Convert(expression1, typeof (object)));
    if (expression2.Type == typeof (void))
      expression2 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(expression2, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) null));
    return expression2;
  }

  private void AddInitialiation(ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> block)
  {
    if (this._isModule)
    {
      block.Add(Node.AssignValue(this.GetVariableExpression(this._fileVariable), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this.ModuleFileName)));
      block.Add(Node.AssignValue(this.GetVariableExpression(this._nameVariable), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this.ModuleName)));
    }
    if (this._languageFeatures == ModuleOptions.None && !this._isModule)
      return;
    block.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ModuleStarted, this.LocalContext, Microsoft.Scripting.Ast.Utils.Constant((object) this._languageFeatures)));
  }

  internal override bool PrintExpressions => this._printExpressions;

  private System.Linq.Expressions.Expression AddModulePublishing(System.Linq.Expressions.Expression body)
  {
    if (this._isModule)
    {
      PythonCompilerOptions options = this._compilerContext.Options as PythonCompilerOptions;
      string moduleName = this.ModuleName;
      if ((options.Module & ModuleOptions.Initialize) != ModuleOptions.None)
      {
        ParameterExpression left = System.Linq.Expressions.Expression.Variable(typeof (object), "$originalModule");
        body = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left
        }, (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Try((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.PublishModule, this.LocalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) moduleName))), body).Catch(typeof (Exception), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.RemoveModule, this.LocalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) moduleName), (System.Linq.Expressions.Expression) left), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Rethrow(body.Type)));
      }
    }
    return body;
  }

  private string ModuleFileName => this._name;

  private string ModuleName
  {
    get
    {
      return (this._compilerContext.Options as PythonCompilerOptions).ModuleName ?? (!this._compilerContext.SourceUnit.HasPath || this._compilerContext.SourceUnit.Path.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 ? "<module>" : Path.GetFileNameWithoutExtension(this._compilerContext.SourceUnit.Path));
    }
  }

  internal override FunctionAttributes Flags
  {
    get
    {
      int module = (int) ((PythonCompilerOptions) this._compilerContext.Options).Module;
      FunctionAttributes flags = FunctionAttributes.None;
      if ((module & 1) != 0)
        flags |= FunctionAttributes.FutureDivision;
      return flags;
    }
  }

  internal SourceUnit SourceUnit
  {
    get => this._compilerContext == null ? (SourceUnit) null : this._compilerContext.SourceUnit;
  }

  internal string[] GetNames()
  {
    string[] names = new string[this.Variables.Count];
    int num = 0;
    foreach (PythonVariable pythonVariable in this.Variables.Values)
      names[num++] = pythonVariable.Name;
    return names;
  }

  private static CompilationMode GetCompilationMode(CompilerContext context)
  {
    PythonCompilerOptions options = (PythonCompilerOptions) context.Options;
    if ((options.Module & ModuleOptions.ExecOrEvalCode) != ModuleOptions.None)
      return CompilationMode.Lookup;
    PythonContext languageContext = (PythonContext) context.SourceUnit.LanguageContext;
    return !languageContext.PythonOptions.Optimize && !options.Optimized || languageContext.PythonOptions.LightweightScopes ? CompilationMode.Collectable : CompilationMode.Uncollectable;
  }

  internal CompilationMode CompilationMode => this._mode;

  private System.Linq.Expressions.Expression GetGlobalContext()
  {
    return this._contextInfo != null ? this._contextInfo.Expression : (System.Linq.Expressions.Expression) PythonAst._globalContext;
  }

  internal void PrepareScope(
    ReadOnlyCollectionBuilder<ParameterExpression> locals,
    List<System.Linq.Expressions.Expression> init)
  {
    this.CompilationMode.PrepareScope(this, locals, init);
  }

  internal System.Linq.Expressions.Expression Constant(object value)
  {
    return (System.Linq.Expressions.Expression) new PythonConstantExpression(this.CompilationMode, value);
  }

  internal System.Linq.Expressions.Expression Convert(
    Type type,
    ConversionResultKind resultKind,
    System.Linq.Expressions.Expression target)
  {
    return resultKind == ConversionResultKind.ExplicitCast ? (System.Linq.Expressions.Expression) new DynamicConvertExpression(this.PyContext.Convert(type, resultKind), this.CompilationMode, target) : this.CompilationMode.Dynamic((DynamicMetaObjectBinder) this.PyContext.Convert(type, resultKind), type, target);
  }

  internal System.Linq.Expressions.Expression Operation(
    Type resultType,
    PythonOperationKind operation,
    System.Linq.Expressions.Expression arg0)
  {
    return resultType == typeof (object) ? (System.Linq.Expressions.Expression) new PythonDynamicExpression1((CallSiteBinder) Binders.UnaryOperationBinder(this.PyContext, operation), this.CompilationMode, arg0) : this.CompilationMode.Dynamic(Binders.UnaryOperationBinder(this.PyContext, operation), resultType, arg0);
  }

  internal System.Linq.Expressions.Expression Operation(
    Type resultType,
    PythonOperationKind operation,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1)
  {
    return resultType == typeof (object) ? (System.Linq.Expressions.Expression) new PythonDynamicExpression2((CallSiteBinder) Binders.BinaryOperationBinder(this.PyContext, operation), this._mode, arg0, arg1) : this.CompilationMode.Dynamic(Binders.BinaryOperationBinder(this.PyContext, operation), resultType, arg0, arg1);
  }

  internal System.Linq.Expressions.Expression Set(string name, System.Linq.Expressions.Expression target, System.Linq.Expressions.Expression value)
  {
    return (System.Linq.Expressions.Expression) new PythonDynamicExpression2((CallSiteBinder) this.PyContext.SetMember(name), this.CompilationMode, target, value);
  }

  internal System.Linq.Expressions.Expression Get(string name, System.Linq.Expressions.Expression target)
  {
    return (System.Linq.Expressions.Expression) new DynamicGetMemberExpression(this.PyContext.GetMember(name), this._mode, target, this.LocalContext);
  }

  internal System.Linq.Expressions.Expression Delete(
    Type resultType,
    string name,
    System.Linq.Expressions.Expression target)
  {
    return this.CompilationMode.Dynamic((DynamicMetaObjectBinder) this.PyContext.DeleteMember(name), resultType, target);
  }

  internal System.Linq.Expressions.Expression GetIndex(System.Linq.Expressions.Expression[] expressions)
  {
    return (System.Linq.Expressions.Expression) new PythonDynamicExpressionN((CallSiteBinder) this.PyContext.GetIndex(expressions.Length), this.CompilationMode, (IList<System.Linq.Expressions.Expression>) expressions);
  }

  internal System.Linq.Expressions.Expression GetSlice(System.Linq.Expressions.Expression[] expressions)
  {
    return (System.Linq.Expressions.Expression) new PythonDynamicExpressionN((CallSiteBinder) this.PyContext.GetSlice, this.CompilationMode, (IList<System.Linq.Expressions.Expression>) expressions);
  }

  internal System.Linq.Expressions.Expression SetIndex(System.Linq.Expressions.Expression[] expressions)
  {
    return (System.Linq.Expressions.Expression) new PythonDynamicExpressionN((CallSiteBinder) this.PyContext.SetIndex(expressions.Length - 1), this.CompilationMode, (IList<System.Linq.Expressions.Expression>) expressions);
  }

  internal System.Linq.Expressions.Expression SetSlice(System.Linq.Expressions.Expression[] expressions)
  {
    return (System.Linq.Expressions.Expression) new PythonDynamicExpressionN((CallSiteBinder) this.PyContext.SetSliceBinder, this.CompilationMode, (IList<System.Linq.Expressions.Expression>) expressions);
  }

  internal System.Linq.Expressions.Expression DeleteIndex(System.Linq.Expressions.Expression[] expressions)
  {
    return this.CompilationMode.Dynamic((DynamicMetaObjectBinder) this.PyContext.DeleteIndex(expressions.Length), typeof (void), expressions);
  }

  internal System.Linq.Expressions.Expression DeleteSlice(System.Linq.Expressions.Expression[] expressions)
  {
    return (System.Linq.Expressions.Expression) new PythonDynamicExpressionN((CallSiteBinder) this.PyContext.DeleteSlice, this.CompilationMode, (IList<System.Linq.Expressions.Expression>) expressions);
  }

  internal PythonAst MakeLookupCode()
  {
    PythonAst ast = (PythonAst) this.MemberwiseClone();
    ast._mode = CompilationMode.Lookup;
    ast._contextInfo = (CompilationMode.ConstantInfo) null;
    Dictionary<PythonVariable, System.Linq.Expressions.Expression> dictionary = new Dictionary<PythonVariable, System.Linq.Expressions.Expression>();
    foreach (KeyValuePair<PythonVariable, System.Linq.Expressions.Expression> globalVariable in this._globalVariables)
      dictionary[globalVariable.Key] = CompilationMode.Lookup.GetGlobal((System.Linq.Expressions.Expression) PythonAst._globalContext, -1, globalVariable.Key, (PythonGlobal) null);
    ast._globalVariables = dictionary;
    ast._body = (Statement) new RewrittenBodyStatement(this._body, new PythonAst.LookupVisitor(ast, this.GetGlobalContext()).Visit((System.Linq.Expressions.Expression) this._body));
    return ast;
  }

  internal override string ProfilerName
  {
    get
    {
      if (this._mode == CompilationMode.Lookup)
        return "module: <exec>";
      return this._name.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ? "module " + this._name : "module " + Path.GetFileNameWithoutExtension(this._name);
    }
  }

  internal new bool EmitDebugSymbols => this.PyContext.EmitDebugSymbols(this.SourceUnit);

  internal bool OnDiskProxy => this._onDiskProxy;

  internal class LookupVisitor : ExpressionVisitor
  {
    private readonly System.Linq.Expressions.Expression _globalContext;
    private ScopeStatement _curScope;

    public LookupVisitor(PythonAst ast, System.Linq.Expressions.Expression globalContext)
    {
      this._globalContext = globalContext;
      this._curScope = (ScopeStatement) ast;
    }

    protected override System.Linq.Expressions.Expression VisitMember(System.Linq.Expressions.MemberExpression node)
    {
      return node == this._globalContext ? (System.Linq.Expressions.Expression) PythonAst._globalContext : base.VisitMember(node);
    }

    protected override System.Linq.Expressions.Expression VisitExtension(System.Linq.Expressions.Expression node)
    {
      if (node == this._globalContext)
        return (System.Linq.Expressions.Expression) PythonAst._globalContext;
      switch (node)
      {
        case ScopeStatement scope:
          return base.VisitExtension((System.Linq.Expressions.Expression) this.VisitScope(scope));
        case LambdaExpression lambdaExpression:
          return base.VisitExtension((System.Linq.Expressions.Expression) new LambdaExpression((FunctionDefinition) this.VisitScope((ScopeStatement) lambdaExpression.Function)));
        case GeneratorExpression generatorExpression:
          return base.VisitExtension((System.Linq.Expressions.Expression) new GeneratorExpression((FunctionDefinition) this.VisitScope((ScopeStatement) generatorExpression.Function), generatorExpression.Iterable));
        case PythonGlobalVariableExpression variableExpression1:
          return (System.Linq.Expressions.Expression) new LookupGlobalVariable(this._curScope == null ? (System.Linq.Expressions.Expression) PythonAst._globalContext : this._curScope.LocalContext, variableExpression1.Variable.Name, variableExpression1.Variable.Kind == VariableKind.Local);
        case PythonSetGlobalVariableExpression variableExpression2:
          return variableExpression2.Value == PythonGlobalVariableExpression.Uninitialized ? new LookupGlobalVariable(this._curScope == null ? (System.Linq.Expressions.Expression) PythonAst._globalContext : this._curScope.LocalContext, variableExpression2.Global.Variable.Name, variableExpression2.Global.Variable.Kind == VariableKind.Local).Delete() : new LookupGlobalVariable(this._curScope == null ? (System.Linq.Expressions.Expression) PythonAst._globalContext : this._curScope.LocalContext, variableExpression2.Global.Variable.Name, variableExpression2.Global.Variable.Kind == VariableKind.Local).Assign(this.Visit(variableExpression2.Value));
        case PythonRawGlobalValueExpression globalValueExpression:
          return (System.Linq.Expressions.Expression) new LookupGlobalVariable(this._curScope == null ? (System.Linq.Expressions.Expression) PythonAst._globalContext : this._curScope.LocalContext, globalValueExpression.Global.Variable.Name, globalValueExpression.Global.Variable.Kind == VariableKind.Local);
        default:
          return base.VisitExtension(node);
      }
    }

    private ScopeStatement VisitScope(ScopeStatement scope)
    {
      ScopeStatement scopeStatement = scope.CopyForRewrite();
      ScopeStatement curScope = this._curScope;
      try
      {
        this._curScope = scopeStatement;
        scopeStatement.Parent = curScope;
        scopeStatement.RewriteBody((ExpressionVisitor) this);
      }
      finally
      {
        this._curScope = curScope;
      }
      return scopeStatement;
    }
  }
}
