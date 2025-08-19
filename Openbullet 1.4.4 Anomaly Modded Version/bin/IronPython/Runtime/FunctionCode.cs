// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FunctionCode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

[PythonType("code")]
[DebuggerDisplay("{co_name}, FileName = {co_filename}")]
public class FunctionCode : IExpressionSerializable, ICodeFormattable
{
  [PythonHidden(new PlatformID[] {})]
  internal Delegate Target;
  [PythonHidden(new PlatformID[] {})]
  internal Delegate LightThrowTarget;
  internal Delegate _normalDelegate;
  private ScopeStatement _lambda;
  internal readonly string _initialDoc;
  private readonly int _localCount;
  private readonly int _argCount;
  private bool _compilingLight;
  private int _exceptionCount;
  private LambdaExpression _tracingLambda;
  internal Delegate _tracingDelegate;
  private static FunctionCode.CodeList _CodeCreateAndUpdateDelegateLock = new FunctionCode.CodeList();

  internal FunctionCode(
    PythonContext context,
    Delegate code,
    ScopeStatement scope,
    string documentation,
    int localCount)
  {
    this._normalDelegate = code;
    this._lambda = scope;
    this._argCount = this.CalculateArgumentCount();
    this._initialDoc = documentation;
    lock (FunctionCode._CodeCreateAndUpdateDelegateLock)
      this.SetTarget(this.AddRecursionCheck(context, code));
    this.RegisterFunctionCode(context);
  }

  internal FunctionCode(
    PythonContext context,
    Delegate initialDelegate,
    ScopeStatement scope,
    string documentation,
    bool? tracing,
    bool register)
  {
    this._lambda = scope;
    this.Target = this.LightThrowTarget = initialDelegate;
    this._initialDoc = documentation;
    this._localCount = scope.Variables == null ? 0 : scope.Variables.Count;
    this._argCount = this.CalculateArgumentCount();
    if (tracing.HasValue)
    {
      if (tracing.Value)
        this._tracingDelegate = initialDelegate;
      else
        this._normalDelegate = initialDelegate;
    }
    if (!register)
      return;
    this.RegisterFunctionCode(context);
  }

  private static PythonTuple SymbolListToTuple(IList<string> vars)
  {
    if (vars == null || vars.Count == 0)
      return PythonTuple.EMPTY;
    object[] objArray = new object[vars.Count];
    for (int index = 0; index < vars.Count; ++index)
      objArray[index] = (object) vars[index];
    return PythonTuple.MakeTuple(objArray);
  }

  private static PythonTuple StringArrayToTuple(string[] closureVars)
  {
    return closureVars != null && closureVars.Length != 0 ? PythonTuple.MakeTuple((object[]) closureVars) : PythonTuple.EMPTY;
  }

  private void RegisterFunctionCode(PythonContext context)
  {
    if (this._lambda == null)
      return;
    WeakReference code = new WeakReference((object) this);
    lock (FunctionCode._CodeCreateAndUpdateDelegateLock)
    {
      FunctionCode.CodeList allCodes;
      do
      {
        allCodes = context._allCodes;
      }
      while (Interlocked.CompareExchange<FunctionCode.CodeList>(ref context._allCodes, new FunctionCode.CodeList(code, allCodes), allCodes) != allCodes);
      if (context._codeCount++ != context._nextCodeCleanup)
        return;
      FunctionCode.CleanFunctionCodes(context, false);
    }
  }

  internal static void CleanFunctionCodes(PythonContext context, bool synchronous)
  {
    if (synchronous)
      FunctionCode.CodeCleanup((object) context);
    else
      ThreadPool.QueueUserWorkItem(new WaitCallback(FunctionCode.CodeCleanup), (object) context);
  }

  internal void SetTarget(Delegate target) => this.Target = this.LightThrowTarget = target;

  internal void LightThrowCompile(CodeContext context)
  {
    if (++this._exceptionCount <= 20 || this._compilingLight || (object) this.Target != (object) this.LightThrowTarget)
      return;
    this._compilingLight = true;
    if (this.IsOnDiskCode)
      return;
    ThreadPool.QueueUserWorkItem((WaitCallback) (x =>
    {
      try
      {
        PythonContext languageContext = context.LanguageContext;
        bool enableTracing;
        lock (languageContext._codeUpdateLock)
          enableTracing = context.LanguageContext.EnableTracing;
        Delegate @delegate = !enableTracing ? ((LambdaExpression) LightExceptions.Rewrite(this.GetGeneratorOrNormalLambda().Reduce())).Compile() : ((LambdaExpression) LightExceptions.Rewrite(this.GetGeneratorOrNormalLambdaTracing(languageContext).Reduce())).Compile();
        lock (languageContext._codeUpdateLock)
        {
          if (context.LanguageContext.EnableTracing != enableTracing)
            return;
          this.LightThrowTarget = @delegate;
        }
      }
      catch (InvalidOperationException ex)
      {
        if (!string.IsNullOrEmpty(ex.Message))
          throw ex;
      }
    }));
  }

  private bool IsOnDiskCode
  {
    get
    {
      if (this._lambda is SerializedScopeStatement)
        return true;
      return this._lambda is PythonAst && ((PythonAst) this._lambda).OnDiskProxy;
    }
  }

  private static IEnumerable<FunctionCode> GetAllCode(PythonContext context)
  {
    lock (FunctionCode._CodeCreateAndUpdateDelegateLock)
    {
      FunctionCode.CodeList curCodeList = Interlocked.Exchange<FunctionCode.CodeList>(ref context._allCodes, FunctionCode._CodeCreateAndUpdateDelegateLock);
      FunctionCode.CodeList initialCode = curCodeList;
      try
      {
        for (; curCodeList != null; curCodeList = curCodeList.Next)
        {
          FunctionCode target = (FunctionCode) curCodeList.Code.Target;
          if (target != null)
            yield return target;
        }
      }
      finally
      {
        Interlocked.Exchange<FunctionCode.CodeList>(ref context._allCodes, initialCode);
      }
      curCodeList = (FunctionCode.CodeList) null;
      initialCode = (FunctionCode.CodeList) null;
    }
  }

  internal static void UpdateAllCode(PythonContext context)
  {
    foreach (FunctionCode functionCode in FunctionCode.GetAllCode(context))
      functionCode.UpdateDelegate(context, false);
  }

  private static void CodeCleanup(object state)
  {
    PythonContext context = (PythonContext) state;
    lock (context._codeCleanupLock)
    {
      int num1 = 0;
      int num2 = 0;
      FunctionCode.CodeList codeList = (FunctionCode.CodeList) null;
      FunctionCode.CodeList comparand = FunctionCode.GetRootCodeNoUpdating(context);
      while (comparand != null)
      {
        if (!comparand.Code.IsAlive)
        {
          if (codeList == null)
          {
            if (Interlocked.CompareExchange<FunctionCode.CodeList>(ref context._allCodes, comparand.Next, comparand) != comparand)
            {
              comparand = FunctionCode.GetRootCodeNoUpdating(context);
            }
            else
            {
              comparand = comparand.Next;
              ++num1;
            }
          }
          else
          {
            ++num1;
            comparand = codeList.Next = comparand.Next;
          }
        }
        else
        {
          ++num2;
          codeList = comparand;
          comparand = comparand.Next;
        }
      }
      lock (FunctionCode._CodeCreateAndUpdateDelegateLock)
      {
        if (context._codeCount == 0)
        {
          context._nextCodeCleanup = 200;
        }
        else
        {
          double num3 = (double) num1 / (double) context._codeCount / 0.5;
          int num4 = Interlocked.Add(ref context._codeCount, -num1);
          int num5 = num3 != 0.0 ? num4 + (int) ((double) context._nextCodeCleanup / num3) : -1;
          if (num5 > 0)
            context._nextCodeCleanup = num5;
          else
            context._nextCodeCleanup += 500;
        }
      }
    }
  }

  private static FunctionCode.CodeList GetRootCodeNoUpdating(PythonContext context)
  {
    FunctionCode.CodeList allCodes = context._allCodes;
    if (allCodes == FunctionCode._CodeCreateAndUpdateDelegateLock)
    {
      lock (FunctionCode._CodeCreateAndUpdateDelegateLock)
        allCodes = context._allCodes;
    }
    return allCodes;
  }

  public SourceSpan Span
  {
    [PythonHidden(new PlatformID[] {})] get => this._lambda.Span;
  }

  internal string[] ArgNames => this._lambda.ParameterNames;

  internal FunctionAttributes Flags => this._lambda.Flags;

  internal bool IsModule => this._lambda is PythonAst;

  public PythonTuple co_varnames => FunctionCode.SymbolListToTuple(this._lambda.GetVarNames());

  public int co_argcount => this._argCount;

  private int CalculateArgumentCount()
  {
    int argCount = this._lambda.ArgCount;
    int flags = (int) this.Flags;
    if ((flags & 4) != 0)
      --argCount;
    if ((flags & 8) != 0)
      --argCount;
    return argCount;
  }

  public PythonTuple co_cellvars
  {
    get
    {
      return FunctionCode.SymbolListToTuple(this._lambda.CellVariables != null ? (IList<string>) ArrayUtils.ToArray<string>((ICollection<string>) this._lambda.CellVariables) : (IList<string>) (string[]) null);
    }
  }

  public object co_code => (object) string.Empty;

  public PythonTuple co_consts
  {
    get
    {
      if (this._initialDoc == null)
        return PythonTuple.MakeTuple(new object[1]);
      return PythonTuple.MakeTuple((object) this._initialDoc, null);
    }
  }

  public string co_filename => this._lambda.Filename;

  public int co_firstlineno => this.Span.Start.Line;

  public int co_flags => (int) this.Flags;

  public PythonTuple co_freevars
  {
    get
    {
      return FunctionCode.SymbolListToTuple(this._lambda.FreeVariables != null ? CollectionUtils.ConvertAll<PythonVariable, string>(this._lambda.FreeVariables, (Func<PythonVariable, string>) (x => x.Name)) : (IList<string>) null);
    }
  }

  public object co_lnotab => throw PythonOps.NotImplementedError("");

  public string co_name => this._lambda.Name;

  public PythonTuple co_names => FunctionCode.SymbolListToTuple(this._lambda.GlobalVariables);

  public object co_nlocals => (object) this._localCount;

  public object co_stacksize => throw PythonOps.NotImplementedError("");

  public string __repr__(CodeContext context)
  {
    return $"<code object {this.co_name} at {PythonOps.HexId((object) this)}, file {(!string.IsNullOrEmpty(this.co_filename) ? (object) $"\"{this.co_filename}\"" : (object) "???")}, line {(this.co_firstlineno != 0 ? this.co_firstlineno : -1)}>";
  }

  internal LightLambdaExpression Code => this._lambda.GetLambda();

  internal ScopeStatement PythonCode => this._lambda;

  internal object Call(CodeContext context)
  {
    if (this.co_freevars != PythonTuple.EMPTY)
      throw PythonOps.TypeError("cannot exec code object that contains free variables: {0}", (object) this.co_freevars.__repr__(context));
    if ((object) this.Target == null || RuntimeReflectionExtensions.GetMethodInfo(this.Target) != (MethodInfo) null && RuntimeReflectionExtensions.GetMethodInfo(this.Target).DeclaringType == typeof (PythonCallTargets))
      this.UpdateDelegate(context.LanguageContext, true);
    if (this.Target is Func<CodeContext, CodeContext> target1)
      return (object) target1(context);
    if (this.Target is LookupCompilationDelegate target2)
      return target2(context, this);
    if (this.Target is Func<FunctionCode, object> target3)
      return target3(this);
    PythonFunction pythonFunction = new PythonFunction(context, this, (object) null, ArrayUtils.EmptyObjects, (MutableTuple) new MutableTuple<object>());
    CallSite<Func<CallSite, CodeContext, PythonFunction, object>> functionCallSite = context.LanguageContext.FunctionCallSite;
    return functionCallSite.Target((CallSite) functionCallSite, context, pythonFunction);
  }

  internal static FunctionCode FromSourceUnit(
    SourceUnit sourceUnit,
    PythonCompilerOptions options,
    bool register)
  {
    ScriptCode scriptCode = PythonContext.CompilePythonCode(sourceUnit, (CompilerOptions) options, (ErrorSink) ThrowingErrorSink.Default);
    ScriptCodeParseResult? codeProperties = sourceUnit.CodeProperties;
    ScriptCodeParseResult scriptCodeParseResult = ScriptCodeParseResult.Empty;
    if (codeProperties.GetValueOrDefault() == scriptCodeParseResult & codeProperties.HasValue)
      throw new SyntaxErrorException("unexpected EOF while parsing", sourceUnit, new SourceSpan(new SourceLocation(0, 1, 1), new SourceLocation(0, 1, 1)), 0, Severity.Error);
    return ((RunnableScriptCode) scriptCode).GetFunctionCode(register);
  }

  private void ExpandArgsTuple(List<string> names, PythonTuple toExpand)
  {
    for (int index = 0; index < toExpand.__len__(); ++index)
    {
      if (toExpand[index] is PythonTuple)
        this.ExpandArgsTuple(names, toExpand[index] as PythonTuple);
      else
        names.Add(toExpand[index] as string);
    }
  }

  public override bool Equals(object obj) => base.Equals(obj);

  public override int GetHashCode() => base.GetHashCode();

  public int __cmp__(CodeContext context, [NotNull] FunctionCode other)
  {
    if (other == this)
      return 0;
    return IdDispenser.GetId((object) this) - IdDispenser.GetId((object) other) <= 0L ? -1 : 1;
  }

  [Python3Warning("code inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator >(FunctionCode self, FunctionCode other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("code inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator <(FunctionCode self, FunctionCode other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("code inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator >=(FunctionCode self, FunctionCode other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("code inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator <=(FunctionCode self, FunctionCode other)
  {
    return PythonOps.NotImplemented;
  }

  internal void LazyCompileFirstTarget(PythonFunction function)
  {
    lock (FunctionCode._CodeCreateAndUpdateDelegateLock)
      this.UpdateDelegate(function.Context.LanguageContext, true);
  }

  internal void UpdateDelegate(PythonContext context, bool forceCreation)
  {
    Delegate finalTarget;
    if (context.EnableTracing && this._lambda != null)
    {
      if (this._tracingLambda == null)
      {
        if (!forceCreation)
        {
          PythonCallTargets.GetPythonTargetType(this._lambda.ParameterNames.Length > 15, this._lambda.ParameterNames.Length, out this.Target);
          this.LightThrowTarget = this.Target;
          return;
        }
        this._tracingLambda = this.GetGeneratorOrNormalLambdaTracing(context);
      }
      if ((object) this._tracingDelegate == null)
        this._tracingDelegate = this.CompileLambda(this._tracingLambda, new EventHandler<LightLambdaCompileEventArgs>(new FunctionCode.TargetUpdaterForCompilation(context, this).SetCompiledTargetTracing));
      finalTarget = this._tracingDelegate;
    }
    else
    {
      if ((object) this._normalDelegate == null)
      {
        if (!forceCreation)
        {
          PythonCallTargets.GetPythonTargetType(this._lambda.ParameterNames.Length > 15, this._lambda.ParameterNames.Length, out this.Target);
          this.LightThrowTarget = this.Target;
          return;
        }
        this._normalDelegate = this.CompileLambda(this.GetGeneratorOrNormalLambda(), new EventHandler<LightLambdaCompileEventArgs>(new FunctionCode.TargetUpdaterForCompilation(context, this).SetCompiledTarget));
      }
      finalTarget = this._normalDelegate;
    }
    this.SetTarget(this.AddRecursionCheck(context, finalTarget));
  }

  internal void SetDebugTarget(PythonContext context, Delegate target)
  {
    this._normalDelegate = target;
    this.SetTarget(this.AddRecursionCheck(context, target));
  }

  private LambdaExpression GetGeneratorOrNormalLambdaTracing(PythonContext context)
  {
    DebugLambdaInfo debugInfo = new DebugLambdaInfo((IDebugCompilerSupport) null, (string) null, false, (IList<ParameterExpression>) null, (IDictionary<ParameterExpression, string>) null, (object) new PythonDebuggingPayload(this));
    return (this.Flags & FunctionAttributes.Generator) == FunctionAttributes.None ? context.DebugContext.TransformLambda((LambdaExpression) Node.RemoveFrame((Expression) this._lambda.GetLambda()), debugInfo) : Expression.Lambda(this.Code.Type, new GeneratorRewriter(this._lambda.Name, Node.RemoveFrame(this.Code.Body)).Reduce(this._lambda.ShouldInterpret, this._lambda.EmitDebugSymbols, context.Options.CompilationThreshold, this.Code.Parameters, (Func<Expression<Func<MutableTuple, object>>, Expression<Func<MutableTuple, object>>>) (x => (Expression<Func<MutableTuple, object>>) context.DebugContext.TransformLambda((LambdaExpression) x, debugInfo))), this.Code.Name, (IEnumerable<ParameterExpression>) this.Code.Parameters);
  }

  private LightLambdaExpression GetGeneratorOrNormalLambda()
  {
    return (this.Flags & FunctionAttributes.Generator) != FunctionAttributes.None ? this.Code.ToGenerator(this._lambda.ShouldInterpret, this._lambda.EmitDebugSymbols, this._lambda.GlobalParent.PyContext.Options.CompilationThreshold) : this.Code;
  }

  private Delegate CompileLambda(
    LightLambdaExpression code,
    EventHandler<LightLambdaCompileEventArgs> handler)
  {
    if (!this._lambda.ShouldInterpret)
      return code.Compile();
    Delegate @delegate = code.Compile(this._lambda.GlobalParent.PyContext.Options.CompilationThreshold);
    if (!(@delegate.Target is LightLambda target))
      return @delegate;
    target.Compile += handler;
    return @delegate;
  }

  private Delegate CompileLambda(
    LambdaExpression code,
    EventHandler<LightLambdaCompileEventArgs> handler)
  {
    if (!this._lambda.ShouldInterpret)
      return code.Compile();
    Delegate @delegate = code.LightCompile(this._lambda.GlobalParent.PyContext.Options.CompilationThreshold);
    if (!(@delegate.Target is LightLambda target))
      return @delegate;
    target.Compile += handler;
    return @delegate;
  }

  internal Delegate AddRecursionCheck(PythonContext context, Delegate finalTarget)
  {
    if (context.RecursionLimit != int.MaxValue)
    {
      switch (finalTarget)
      {
        case Func<CodeContext, CodeContext> _:
        case Func<FunctionCode, object> _:
        case LookupCompilationDelegate _:
          return finalTarget;
        default:
          switch (this._lambda.ParameterNames.Length)
          {
            case 0:
              finalTarget = (Delegate) new Func<PythonFunction, object>(new PythonFunctionRecursionCheck0((Func<PythonFunction, object>) finalTarget).CallTarget);
              break;
            case 1:
              finalTarget = (Delegate) new Func<PythonFunction, object, object>(new PythonFunctionRecursionCheck1((Func<PythonFunction, object, object>) finalTarget).CallTarget);
              break;
            case 2:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object>(new PythonFunctionRecursionCheck2((Func<PythonFunction, object, object, object>) finalTarget).CallTarget);
              break;
            case 3:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object>(new PythonFunctionRecursionCheck3((Func<PythonFunction, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 4:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object>(new PythonFunctionRecursionCheck4((Func<PythonFunction, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 5:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object>(new PythonFunctionRecursionCheck5((Func<PythonFunction, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 6:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck6((Func<PythonFunction, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 7:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck7((Func<PythonFunction, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 8:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck8((Func<PythonFunction, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 9:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck9((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 10:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck10((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 11:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck11((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 12:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck12((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 13:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck13((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 14:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck14((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            case 15:
              finalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(new PythonFunctionRecursionCheck15((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) finalTarget).CallTarget);
              break;
            default:
              finalTarget = (Delegate) new Func<PythonFunction, object[], object>(new PythonFunctionRecursionCheckN((Func<PythonFunction, object[], object>) finalTarget).CallTarget);
              break;
          }
          break;
      }
    }
    return finalTarget;
  }

  Expression IExpressionSerializable.CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeFunctionCode"), (Expression) PythonAst._globalContext, (Expression) Expression.Constant((object) this._lambda.Name), (Expression) Expression.Constant((object) this._initialDoc, typeof (string)), (Expression) Expression.NewArrayInit(typeof (string), (Expression[]) ArrayUtils.ConvertAll<string, ConstantExpression>(this._lambda.ParameterNames, (Func<string, ConstantExpression>) (x => Expression.Constant((object) x)))), (Expression) Expression.Constant((object) this.Flags), (Expression) Expression.Constant((object) this._lambda.IndexSpan.Start), (Expression) Expression.Constant((object) this._lambda.IndexSpan.End), (Expression) Expression.Constant((object) this._lambda.Filename), (Expression) this.GetGeneratorOrNormalLambda(), FunctionCode.TupleToStringArray(this.co_freevars), FunctionCode.TupleToStringArray(this.co_names), FunctionCode.TupleToStringArray(this.co_cellvars), FunctionCode.TupleToStringArray(this.co_varnames), (Expression) Expression.Constant((object) this._localCount));
  }

  private static Expression TupleToStringArray(PythonTuple tuple)
  {
    return tuple.Count <= 0 ? (Expression) Expression.Constant((object) null, typeof (string[])) : (Expression) Expression.NewArrayInit(typeof (string), (Expression[]) ArrayUtils.ConvertAll<object, ConstantExpression>(tuple._data, (Func<object, ConstantExpression>) (x => Expression.Constant(x))));
  }

  private class TargetUpdaterForCompilation
  {
    private readonly PythonContext _context;
    private readonly FunctionCode _code;

    public TargetUpdaterForCompilation(PythonContext context, FunctionCode code)
    {
      this._code = code;
      this._context = context;
    }

    public void SetCompiledTarget(object sender, LightLambdaCompileEventArgs e)
    {
      this._code.SetTarget(this._code.AddRecursionCheck(this._context, this._code._normalDelegate = e.Compiled));
    }

    public void SetCompiledTargetTracing(object sender, LightLambdaCompileEventArgs e)
    {
      this._code.SetTarget(this._code.AddRecursionCheck(this._context, this._code._tracingDelegate = e.Compiled));
    }
  }

  internal class CodeList
  {
    public readonly WeakReference Code;
    public FunctionCode.CodeList Next;

    public CodeList()
    {
    }

    public CodeList(WeakReference code, FunctionCode.CodeList next)
    {
      this.Code = code;
      this.Next = next;
    }
  }
}
