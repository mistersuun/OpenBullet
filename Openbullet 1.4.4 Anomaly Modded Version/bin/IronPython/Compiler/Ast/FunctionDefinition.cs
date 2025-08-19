// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.FunctionDefinition
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Compiler.Ast;

public class FunctionDefinition : ScopeStatement, IInstructionProvider
{
  protected Statement _body;
  private readonly string _name;
  private readonly IronPython.Compiler.Ast.Parameter[] _parameters;
  private IList<Expression> _decorators;
  private bool _generator;
  private bool _isLambda;
  private bool _canSetSysExcInfo;
  private bool _containsTryFinally;
  private PythonVariable _variable;
  internal PythonVariable _nameVariable;
  private LightLambdaExpression _dlrBody;
  internal bool _hasReturn;
  private int _headerIndex;
  private static int _lambdaId;
  internal static readonly ParameterExpression _functionParam = System.Linq.Expressions.Expression.Parameter(typeof (PythonFunction), "$function");
  private static readonly System.Linq.Expressions.Expression _GetClosureTupleFromFunctionCall = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) null, typeof (PythonOps).GetMethod("GetClosureTupleFromFunction"), (System.Linq.Expressions.Expression) FunctionDefinition._functionParam);
  private static readonly System.Linq.Expressions.Expression _parentContext = (System.Linq.Expressions.Expression) new GetParentContextFromFunctionExpression();
  internal static readonly LabelTarget _returnLabel = System.Linq.Expressions.Expression.Label(typeof (object), "return");
  internal static readonly FunctionDefinition.ArbitraryGlobalsVisitor ArbitraryGlobalsVisitorInstance = new FunctionDefinition.ArbitraryGlobalsVisitor();

  public FunctionDefinition(string name, IronPython.Compiler.Ast.Parameter[] parameters)
    : this(name, parameters, (Statement) null)
  {
  }

  public FunctionDefinition(string name, IronPython.Compiler.Ast.Parameter[] parameters, Statement body)
  {
    ContractUtils.RequiresNotNullItems<IronPython.Compiler.Ast.Parameter>((IList<IronPython.Compiler.Ast.Parameter>) parameters, nameof (parameters));
    if (name == null)
    {
      this._name = $"<lambda${(object) Interlocked.Increment(ref FunctionDefinition._lambdaId)}>";
      this._isLambda = true;
    }
    else
      this._name = name;
    this._parameters = parameters;
    this._body = body;
  }

  [Obsolete("sourceUnit is now ignored.  FunctionDefinitions should belong to a PythonAst which has a SourceUnit")]
  public FunctionDefinition(string name, IronPython.Compiler.Ast.Parameter[] parameters, SourceUnit sourceUnit)
    : this(name, parameters, (Statement) null)
  {
  }

  [Obsolete("sourceUnit is now ignored.  FunctionDefinitions should belong to a PythonAst which has a SourceUnit")]
  public FunctionDefinition(
    string name,
    IronPython.Compiler.Ast.Parameter[] parameters,
    Statement body,
    SourceUnit sourceUnit)
    : this(name, parameters, body)
  {
  }

  internal override System.Linq.Expressions.Expression LocalContext
  {
    get => this.NeedsLocalContext ? base.LocalContext : this.GlobalParent.LocalContext;
  }

  public bool IsLambda => this._isLambda;

  public IList<IronPython.Compiler.Ast.Parameter> Parameters => (IList<IronPython.Compiler.Ast.Parameter>) this._parameters;

  internal override string[] ParameterNames
  {
    get
    {
      return ArrayUtils.ConvertAll<IronPython.Compiler.Ast.Parameter, string>(this._parameters, (Func<IronPython.Compiler.Ast.Parameter, string>) (val => val.Name));
    }
  }

  internal override int ArgCount => this._parameters.Length;

  public Statement Body
  {
    get => this._body;
    set => this._body = value;
  }

  public SourceLocation Header => this.GlobalParent.IndexToLocation(this._headerIndex);

  public int HeaderIndex
  {
    get => this._headerIndex;
    set => this._headerIndex = value;
  }

  public override string Name => this._name;

  public IList<Expression> Decorators
  {
    get => this._decorators;
    internal set => this._decorators = value;
  }

  internal override bool IsGeneratorMethod => this.IsGenerator;

  public bool IsGenerator
  {
    get => this._generator;
    set => this._generator = value;
  }

  internal bool CanSetSysExcInfo
  {
    set => this._canSetSysExcInfo = value;
  }

  internal bool ContainsTryFinally
  {
    get => this._containsTryFinally;
    set => this._containsTryFinally = value;
  }

  internal PythonVariable PythonVariable
  {
    get => this._variable;
    set => this._variable = value;
  }

  internal override bool ExposesLocalVariable(PythonVariable variable)
  {
    return this.NeedsLocalsDictionary;
  }

  internal override FunctionAttributes Flags
  {
    get
    {
      FunctionAttributes flags = FunctionAttributes.None;
      if (this._parameters != null)
      {
        int index;
        for (index = 0; index < this._parameters.Length; ++index)
        {
          IronPython.Compiler.Ast.Parameter parameter = this._parameters[index];
          if (parameter.IsDictionary || parameter.IsList)
            break;
        }
        if (index < this._parameters.Length && this._parameters[index].IsList)
        {
          ++index;
          flags |= FunctionAttributes.ArgumentList;
        }
        if (index < this._parameters.Length && this._parameters[index].IsDictionary)
        {
          int num = index + 1;
          flags |= FunctionAttributes.KeywordDictionary;
        }
      }
      if (this._canSetSysExcInfo)
        flags |= FunctionAttributes.CanSetSysExcInfo;
      if (this.ContainsTryFinally)
        flags |= FunctionAttributes.ContainsTryFinally;
      if (this.IsGenerator)
        flags |= FunctionAttributes.Generator;
      return flags;
    }
  }

  internal override bool TryBindOuter(
    ScopeStatement from,
    PythonReference reference,
    out PythonVariable variable)
  {
    this.ContainsNestedFreeVariables = true;
    if (!this.TryGetVariable(reference.Name, out variable))
      return false;
    variable.AccessedInNestedScope = true;
    if (variable.Kind == VariableKind.Local || variable.Kind == VariableKind.Parameter)
    {
      from.AddFreeVariable(variable, true);
      for (ScopeStatement parent = from.Parent; parent != this; parent = parent.Parent)
        parent.AddFreeVariable(variable, false);
      this.AddCellVariable(variable);
    }
    else
      from.AddReferencedGlobal(reference.Name);
    return true;
  }

  internal override PythonVariable BindReference(PythonNameBinder binder, PythonReference reference)
  {
    PythonVariable variable;
    if (this.TryGetVariable(reference.Name, out variable))
    {
      if (variable.Kind == VariableKind.Global)
        this.AddReferencedGlobal(reference.Name);
      return variable;
    }
    for (ScopeStatement parent = this.Parent; parent != null; parent = parent.Parent)
    {
      if (parent.TryBindOuter((ScopeStatement) this, reference, out variable))
        return variable;
    }
    return (PythonVariable) null;
  }

  internal override void Bind(PythonNameBinder binder)
  {
    base.Bind(binder);
    this.Verify(binder);
    if (!((PythonContext) binder.Context.SourceUnit.LanguageContext).PythonOptions.FullFrames)
      return;
    this.NeedsLocalsDictionary = true;
  }

  internal override void FinishBind(PythonNameBinder binder)
  {
    foreach (IronPython.Compiler.Ast.Parameter parameter in this._parameters)
      this._variableMapping[parameter.PythonVariable] = parameter.FinishBind(this.NeedsLocalsDictionary);
    base.FinishBind(binder);
  }

  private void Verify(PythonNameBinder binder)
  {
    if (this.ContainsImportStar)
      binder.ReportSyntaxWarning("import * only allowed at module level", (Node) this);
    if (this.ContainsImportStar && this.IsClosure)
      binder.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "import * is not allowed in function '{0}' because it is a nested function", (object) this.Name), (Node) this);
    if (this.ContainsImportStar && this.Parent is FunctionDefinition)
      binder.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "import * is not allowed in function '{0}' because it is a nested function", (object) this.Name), (Node) this);
    if (this.ContainsImportStar && this.ContainsNestedFreeVariables)
      binder.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "import * is not allowed in function '{0}' because it contains a nested function with free variables", (object) this.Name), (Node) this);
    if (this.ContainsUnqualifiedExec && this.ContainsNestedFreeVariables)
      binder.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "unqualified exec is not allowed in function '{0}' because it contains a nested function with free variables", (object) this.Name), (Node) this);
    if (!this.ContainsUnqualifiedExec || !this.IsClosure)
      return;
    binder.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "unqualified exec is not allowed in function '{0}' because it is a nested function", (object) this.Name), (Node) this);
  }

  internal override System.Linq.Expressions.Expression GetParentClosureTuple()
  {
    return FunctionDefinition._GetClosureTupleFromFunctionCall;
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    System.Linq.Expressions.Expression expression = this.MakeFunctionExpression();
    return this.GlobalParent.AddDebugInfoAndVoid(Node.AssignValue(this.Parent.GetVariableExpression(this._variable), expression), new SourceSpan(this.GlobalParent.IndexToLocation(this.StartIndex), this.GlobalParent.IndexToLocation(this.HeaderIndex)));
  }

  internal System.Linq.Expressions.Expression MakeFunctionExpression()
  {
    List<System.Linq.Expressions.Expression> initializers = new List<System.Linq.Expressions.Expression>(0);
    foreach (IronPython.Compiler.Ast.Parameter parameter in this._parameters)
    {
      if (parameter.DefaultValue != null)
        initializers.Add(Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) parameter.DefaultValue, typeof (object)));
    }
    this.FuncCodeExpr = this.GlobalParent.Constant((object) this.GetOrMakeFunctionCode());
    System.Linq.Expressions.Expression ret;
    if (this.EmitDebugFunction())
    {
      LightLambdaExpression functionLambda = this.CreateFunctionLambda();
      ret = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeFunctionDebug, this.Parent.LocalContext, this.FuncCodeExpr, ((IPythonGlobalExpression) this.GetVariableExpression(this._nameVariable)).RawValue(), initializers.Count == 0 ? (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, typeof (object[])) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), (IEnumerable<System.Linq.Expressions.Expression>) initializers), this.IsGenerator ? (System.Linq.Expressions.Expression) new PythonGeneratorExpression(functionLambda, this.GlobalParent.PyContext.Options.CompilationThreshold) : (System.Linq.Expressions.Expression) functionLambda);
    }
    else
      ret = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeFunction, this.Parent.LocalContext, this.FuncCodeExpr, ((IPythonGlobalExpression) this.GetVariableExpression(this._nameVariable)).RawValue(), initializers.Count == 0 ? (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, typeof (object[])) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), (IEnumerable<System.Linq.Expressions.Expression>) initializers));
    return this.AddDecorators(ret, this._decorators);
  }

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    if (this._decorators != null)
    {
      compiler.Compile(this.Reduce());
    }
    else
    {
      this.FuncCodeExpr = this.GlobalParent.Constant((object) this.GetOrMakeFunctionCode());
      System.Linq.Expressions.Expression variableExpression = this.Parent.GetVariableExpression(this._variable);
      FunctionDefinition.CompileAssignment(compiler, variableExpression, new Action<LightCompiler>(this.CreateFunctionInstructions));
    }
  }

  private void CreateFunctionInstructions(LightCompiler compiler)
  {
    CodeContext context = (CodeContext) null;
    compiler.Compile(this.Parent.LocalContext);
    PythonGlobalVariableExpression variableExpression = this.GetVariableExpression(this._nameVariable) as PythonGlobalVariableExpression;
    PythonGlobal name = (PythonGlobal) null;
    if (variableExpression == null)
      compiler.Compile(((IPythonGlobalExpression) this.GetVariableExpression(this._nameVariable)).RawValue());
    else
      name = variableExpression.Global;
    int defaultCount = 0;
    for (int index = this._parameters.Length - 1; index >= 0; --index)
    {
      IronPython.Compiler.Ast.Parameter parameter = this._parameters[index];
      if (parameter.DefaultValue != null)
      {
        compiler.Compile(Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) parameter.DefaultValue, typeof (object)));
        ++defaultCount;
      }
    }
    compiler.Instructions.Emit((Instruction) new FunctionDefinition.FunctionDefinitionInstruction(context, this, defaultCount, name));
  }

  private static void CompileAssignment(
    LightCompiler compiler,
    System.Linq.Expressions.Expression variable,
    Action<LightCompiler> compileValue)
  {
    InstructionList instructions = compiler.Instructions;
    if (variable is ClosureExpression closureExpression)
      compiler.Compile(closureExpression.ClosureCell);
    if (variable is LookupGlobalVariable lookupGlobalVariable)
    {
      compiler.Compile(lookupGlobalVariable.CodeContext);
      instructions.EmitLoad((object) lookupGlobalVariable.Name);
    }
    compileValue(compiler);
    if (closureExpression != null)
      instructions.EmitStoreField(ClosureExpression._cellField);
    else if (lookupGlobalVariable != null)
    {
      MethodInfo method = typeof (PythonOps).GetMethod(lookupGlobalVariable.IsLocal ? "SetLocal" : "SetGlobal");
      instructions.Emit((Instruction) CallInstruction.Create(method));
    }
    else
    {
      switch (variable)
      {
        case ParameterExpression var:
          instructions.EmitStoreLocal(compiler.Locals.GetLocalIndex(var));
          break;
        case PythonGlobalVariableExpression variableExpression:
          instructions.Emit((Instruction) new PythonSetGlobalInstruction(variableExpression.Global));
          instructions.EmitPop();
          break;
      }
    }
  }

  private LightLambdaExpression EnsureFunctionLambda()
  {
    if (this._dlrBody == null)
      this._dlrBody = this.CreateFunctionLambda();
    return this._dlrBody;
  }

  internal override Delegate OriginalDelegate
  {
    get
    {
      Delegate originalTarget;
      FunctionDefinition.GetDelegateType(this._parameters, this._parameters.Length > 15, out originalTarget);
      return originalTarget;
    }
  }

  internal override string ScopeDocumentation => this.GetDocumentation(this._body);

  private LightLambdaExpression CreateFunctionLambda()
  {
    bool flag = this._parameters.Length > 15;
    Type delegateType = FunctionDefinition.GetDelegateType(this._parameters, flag, out Delegate _);
    ParameterExpression left1 = (ParameterExpression) null;
    ReadOnlyCollectionBuilder<ParameterExpression> locals = new ReadOnlyCollectionBuilder<ParameterExpression>();
    if (this.NeedsLocalContext)
    {
      left1 = ScopeStatement.LocalCodeContextVariable;
      locals.Add(left1);
    }
    ParameterExpression[] parameters = this.CreateParameters(flag, locals);
    List<System.Linq.Expressions.Expression> init = new List<System.Linq.Expressions.Expression>();
    foreach (IronPython.Compiler.Ast.Parameter parameter in this._parameters)
    {
      if (this.GetVariableExpression(parameter.PythonVariable) is IPythonVariableExpression variableExpression)
      {
        System.Linq.Expressions.Expression expression = variableExpression.Create();
        if (expression != null)
          init.Add(expression);
      }
    }
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ClearDebugInfo(this.GlobalParent.Document));
    locals.Add(PythonAst._globalContext);
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalContext, (System.Linq.Expressions.Expression) new GetGlobalContextExpression(FunctionDefinition._parentContext)));
    this.GlobalParent.PrepareScope(locals, init);
    this.CreateFunctionVariables(locals, init);
    this.InitializeParameters(init, flag, (System.Linq.Expressions.Expression[]) parameters);
    List<System.Linq.Expressions.Expression> expressionList = new List<System.Linq.Expressions.Expression>();
    SourceLocation location = this.GlobalParent.IndexToLocation(this.StartIndex);
    expressionList.Add(this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Empty(), new SourceSpan(new SourceLocation(0, location.Line, location.Column), new SourceLocation(0, location.Line, int.MaxValue))));
    if (this.IsGenerator)
    {
      System.Linq.Expressions.Expression checkThrowExpression = YieldExpression.CreateCheckThrowExpression(SourceSpan.None);
      expressionList.Add(checkThrowExpression);
    }
    ParameterExpression left2 = (ParameterExpression) null;
    if (!this.IsGenerator && this._canSetSysExcInfo)
    {
      left2 = System.Linq.Expressions.Expression.Parameter(typeof (Exception), "$ex");
      locals.Add(left2);
    }
    if (this._body.CanThrow && !(this._body is SuiteStatement) && this._body.StartIndex != -1)
      expressionList.Add(Node.UpdateLineNumber(this.GlobalParent.IndexToLocation(this._body.StartIndex).Line));
    expressionList.Add((System.Linq.Expressions.Expression) this.Body);
    System.Linq.Expressions.Expression expression1 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) expressionList);
    if (left2 != null)
      expression1 = (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Try((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left2, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.SaveCurrentException)), expression1).Finally((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.RestoreCurrentException, (System.Linq.Expressions.Expression) left2));
    if (this._body.CanThrow && this.GlobalParent.PyContext.PythonOptions.Frames)
    {
      expression1 = Node.AddFrame(this.LocalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) FunctionDefinition._functionParam, typeof (PythonFunction).GetProperty("__code__")), expression1);
      locals.Add(Node.FunctionStackVariable);
    }
    System.Linq.Expressions.Expression expression2 = this.AddReturnTarget((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(this.WrapScopeStatements(this.AddProfiling(expression1), this._body.CanThrow), (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Empty()));
    if (left1 != null)
    {
      MethodCallExpression localContext = this.CreateLocalContext(FunctionDefinition._parentContext);
      init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left1, (System.Linq.Expressions.Expression) localContext));
    }
    init.Add(expression2);
    System.Linq.Expressions.Expression expression3 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) init);
    System.Linq.Expressions.Expression body = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) locals.ToReadOnlyCollection(), expression3);
    return Microsoft.Scripting.Ast.Utils.LightLambda(typeof (object), delegateType, FunctionDefinition.AddDefaultReturn(body, typeof (object)), $"{this.Name}${(object) Interlocked.Increment(ref FunctionDefinition._lambdaId)}", (IList<ParameterExpression>) parameters);
  }

  internal override LightLambdaExpression GetLambda() => this.EnsureFunctionLambda();

  internal FunctionCode FunctionCode => this.GetOrMakeFunctionCode();

  private static System.Linq.Expressions.Expression AddDefaultReturn(
    System.Linq.Expressions.Expression body,
    Type returnType)
  {
    if (body.Type == typeof (void) && returnType != typeof (void))
      body = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(body, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Default(returnType));
    return body;
  }

  private ParameterExpression[] CreateParameters(
    bool needsWrapperMethod,
    ReadOnlyCollectionBuilder<ParameterExpression> locals)
  {
    ParameterExpression[] parameters;
    if (needsWrapperMethod)
    {
      parameters = new ParameterExpression[2]
      {
        FunctionDefinition._functionParam,
        System.Linq.Expressions.Expression.Parameter(typeof (object[]), "allArgs")
      };
      foreach (IronPython.Compiler.Ast.Parameter parameter in this._parameters)
        locals.Add(parameter.ParameterExpression);
    }
    else
    {
      parameters = new ParameterExpression[this._parameters.Length + 1];
      for (int index = 1; index < parameters.Length; ++index)
        parameters[index] = this._parameters[index - 1].ParameterExpression;
      parameters[0] = FunctionDefinition._functionParam;
    }
    return parameters;
  }

  internal void CreateFunctionVariables(
    ReadOnlyCollectionBuilder<ParameterExpression> locals,
    List<System.Linq.Expressions.Expression> init)
  {
    this.CreateVariables(locals, init);
  }

  internal System.Linq.Expressions.Expression AddReturnTarget(System.Linq.Expressions.Expression expression)
  {
    return this._hasReturn ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Label(FunctionDefinition._returnLabel, Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (object))) : expression;
  }

  internal override string ProfilerName
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder("def ");
      stringBuilder.Append(this.Name);
      stringBuilder.Append('(');
      bool flag = false;
      foreach (IronPython.Compiler.Ast.Parameter parameter in this._parameters)
      {
        if (flag)
          stringBuilder.Append(", ");
        else
          flag = true;
        stringBuilder.Append(parameter.Name);
      }
      stringBuilder.Append(')');
      return stringBuilder.ToString();
    }
  }

  private bool EmitDebugFunction()
  {
    return this.EmitDebugSymbols && !this.GlobalParent.PyContext.EnableTracing;
  }

  internal override IList<string> GetVarNames()
  {
    List<string> res = new List<string>();
    foreach (IronPython.Compiler.Ast.Parameter parameter in this._parameters)
      res.Add(parameter.Name);
    this.AppendVariables(res);
    return (IList<string>) res;
  }

  private void InitializeParameters(
    List<System.Linq.Expressions.Expression> init,
    bool needsWrapperMethod,
    System.Linq.Expressions.Expression[] parameters)
  {
    for (int index = 0; index < this._parameters.Length; ++index)
    {
      IronPython.Compiler.Ast.Parameter parameter = this._parameters[index];
      if (needsWrapperMethod)
        init.Add(Node.AssignValue(this.GetVariableExpression(parameter.PythonVariable), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayIndex(parameters[1], (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) index))));
      parameter.Init(init);
    }
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._parameters != null)
      {
        foreach (Node parameter in this._parameters)
          parameter.Walk(walker);
      }
      if (this._decorators != null)
      {
        foreach (Node decorator in (IEnumerable<Expression>) this._decorators)
          decorator.Walk(walker);
      }
      if (this._body != null)
        this._body.Walk(walker);
    }
    walker.PostWalk(this);
  }

  private static Type GetDelegateType(
    IronPython.Compiler.Ast.Parameter[] parameters,
    bool wrapper,
    out Delegate originalTarget)
  {
    return PythonCallTargets.GetPythonTargetType(wrapper, parameters.Length, out originalTarget);
  }

  internal override bool CanThrow => false;

  internal override void RewriteBody(ExpressionVisitor visitor)
  {
    this._dlrBody = (LightLambdaExpression) null;
    this.FuncCodeExpr = this.GlobalParent.Constant((object) this.GetOrMakeFunctionCode());
    this.Body = (Statement) new RewrittenBodyStatement(this.Body, visitor.Visit((System.Linq.Expressions.Expression) this.Body));
  }

  private class FunctionDefinitionInstruction : Instruction
  {
    private readonly FunctionDefinition _def;
    private readonly int _defaultCount;
    private readonly CodeContext _context;
    private readonly PythonGlobal _name;

    public FunctionDefinitionInstruction(
      CodeContext context,
      FunctionDefinition definition,
      int defaultCount,
      PythonGlobal name)
    {
      this._context = context;
      this._defaultCount = defaultCount;
      this._def = definition;
      this._name = name;
    }

    public override int Run(InterpretedFrame frame)
    {
      object[] defaults;
      if (this._defaultCount > 0)
      {
        defaults = new object[this._defaultCount];
        for (int index = 0; index < this._defaultCount; ++index)
          defaults[index] = frame.Pop();
      }
      else
        defaults = ArrayUtils.EmptyObjects;
      object modName = this._name == null ? frame.Pop() : this._name.RawValue;
      CodeContext context = (CodeContext) frame.Pop();
      frame.Push(PythonOps.MakeFunction(context, this._def.FunctionCode, modName, defaults));
      return 1;
    }

    public override int ConsumedStack
    {
      get => this._defaultCount + (this._context == null ? 1 : 0) + (this._name == null ? 1 : 0);
    }

    public override int ProducedStack => 1;
  }

  internal class ArbitraryGlobalsVisitor : ExpressionVisitor
  {
    protected override System.Linq.Expressions.Expression VisitExtension(System.Linq.Expressions.Expression node)
    {
      switch (node)
      {
        case PythonGlobalVariableExpression variableExpression1:
          return (System.Linq.Expressions.Expression) new LookupGlobalVariable((System.Linq.Expressions.Expression) PythonAst._globalContext, variableExpression1.Variable.Name, variableExpression1.Variable.Kind == VariableKind.Local);
        case PythonSetGlobalVariableExpression variableExpression2:
          return variableExpression2.Value == PythonGlobalVariableExpression.Uninitialized ? new LookupGlobalVariable((System.Linq.Expressions.Expression) PythonAst._globalContext, variableExpression2.Global.Variable.Name, variableExpression2.Global.Variable.Kind == VariableKind.Local).Delete() : new LookupGlobalVariable((System.Linq.Expressions.Expression) PythonAst._globalContext, variableExpression2.Global.Variable.Name, variableExpression2.Global.Variable.Kind == VariableKind.Local).Assign(this.Visit(variableExpression2.Value));
        case PythonRawGlobalValueExpression globalValueExpression:
          return (System.Linq.Expressions.Expression) new LookupGlobalVariable((System.Linq.Expressions.Expression) PythonAst._globalContext, globalValueExpression.Global.Variable.Name, globalValueExpression.Global.Variable.Kind == VariableKind.Local);
        default:
          return base.VisitExtension(node);
      }
    }
  }
}
