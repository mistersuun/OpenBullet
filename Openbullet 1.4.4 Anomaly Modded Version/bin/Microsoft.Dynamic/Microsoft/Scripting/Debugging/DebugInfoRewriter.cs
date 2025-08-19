// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugInfoRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Debugging.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal class DebugInfoRewriter : DynamicExpressionVisitor
{
  private readonly DebugContext _debugContext;
  private readonly bool _transformToGenerator;
  private readonly Expression _thread;
  private readonly Expression _frame;
  private readonly Expression _debugMarker;
  private readonly Expression _traceLocations;
  private readonly Dictionary<ParameterExpression, ParameterExpression> _replacedLocals;
  private readonly Dictionary<ParameterExpression, VariableInfo> _localsToVarInfos;
  private readonly Stack<BlockExpression> _currentLocals;
  private readonly Dictionary<int, DebugSourceSpan> _markerLocationMap;
  private readonly Dictionary<int, IList<VariableInfo>> _variableScopeMap;
  private readonly Dictionary<BlockExpression, IList<VariableInfo>> _variableScopeMapCache;
  private readonly Dictionary<DebugSourceFile, ParameterExpression> _sourceFilesToVariablesMap;
  private readonly Expression _globalDebugMode;
  private readonly LabelTarget _generatorLabelTarget;
  private readonly ConstantExpression _debugYieldValue;
  private readonly Expression _pushFrame;
  private readonly DebugLambdaInfo _lambdaInfo;
  private int _locationCookie;
  private bool _hasUnconditionalFunctionCalls;
  private bool _insideConditionalBlock;

  internal DebugInfoRewriter(
    DebugContext debugContext,
    bool transformToGenerator,
    Expression traceLocations,
    Expression thread,
    Expression frame,
    Expression pushFrame,
    Expression debugMarker,
    Expression globalDebugMode,
    Dictionary<DebugSourceFile, ParameterExpression> sourceFilesToVariablesMap,
    LabelTarget generatorLabel,
    Dictionary<ParameterExpression, ParameterExpression> replacedLocals,
    Dictionary<ParameterExpression, VariableInfo> localsToVarInfos,
    DebugLambdaInfo lambdaInfo)
  {
    this._debugContext = debugContext;
    this._transformToGenerator = transformToGenerator;
    this._traceLocations = traceLocations;
    this._thread = thread;
    this._frame = frame;
    this._pushFrame = pushFrame;
    if (this._transformToGenerator)
    {
      this._debugYieldValue = Expression.Constant(DebugContext.DebugYieldValue);
      this._markerLocationMap = new Dictionary<int, DebugSourceSpan>();
      this._variableScopeMap = new Dictionary<int, IList<VariableInfo>>();
      this._currentLocals = new Stack<BlockExpression>();
      this._variableScopeMapCache = new Dictionary<BlockExpression, IList<VariableInfo>>();
    }
    this._debugMarker = debugMarker;
    this._globalDebugMode = globalDebugMode;
    this._sourceFilesToVariablesMap = sourceFilesToVariablesMap;
    this._generatorLabelTarget = generatorLabel;
    this._replacedLocals = replacedLocals;
    this._localsToVarInfos = localsToVarInfos;
    this._lambdaInfo = lambdaInfo;
  }

  internal DebugSourceSpan[] DebugMarkerLocationMap
  {
    get
    {
      DebugSourceSpan[] markerLocationMap = new DebugSourceSpan[this._locationCookie];
      for (int key = 0; key < markerLocationMap.Length; ++key)
      {
        DebugSourceSpan debugSourceSpan;
        if (this._markerLocationMap.TryGetValue(key, out debugSourceSpan))
          markerLocationMap[key] = debugSourceSpan;
      }
      return markerLocationMap;
    }
  }

  internal bool HasUnconditionalFunctionCalls => this._hasUnconditionalFunctionCalls;

  internal IList<VariableInfo>[] VariableScopeMap
  {
    get
    {
      IList<VariableInfo>[] variableScopeMap = new IList<VariableInfo>[this._locationCookie];
      for (int key = 0; key < variableScopeMap.Length; ++key)
      {
        IList<VariableInfo> variableInfoList;
        if (this._variableScopeMap.TryGetValue(key, out variableInfoList))
          variableScopeMap[key] = variableInfoList;
      }
      return variableScopeMap;
    }
  }

  protected virtual Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

  protected virtual Expression VisitBlock(BlockExpression node)
  {
    if (this._transformToGenerator)
      this._currentLocals.Push(node);
    try
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitBlock(Expression.Block(node.Type, (IEnumerable<Expression>) node.Expressions)));
    }
    finally
    {
      if (this._transformToGenerator)
        this._currentLocals.Pop();
    }
  }

  protected virtual Expression VisitTry(TryExpression node)
  {
    Expression body = ((ExpressionVisitor) this).Visit(node.Body);
    ReadOnlyCollection<CatchBlock> handlers = ExpressionVisitor.Visit<CatchBlock>(node.Handlers, new Func<CatchBlock, CatchBlock>(((ExpressionVisitor) this).VisitCatchBlock));
    Expression @finally = ((ExpressionVisitor) this).Visit(node.Finally);
    this._insideConditionalBlock = true;
    Expression fault;
    try
    {
      fault = ((ExpressionVisitor) this).Visit(node.Fault);
    }
    finally
    {
      this._insideConditionalBlock = false;
    }
    node = Expression.MakeTry(node.Type, body, @finally, fault, (IEnumerable<CatchBlock>) handlers);
    List<CatchBlock> catchBlockList = (List<CatchBlock>) null;
    Expression expression1 = (Expression) null;
    if (node.Handlers != null && node.Handlers.Count > 0)
    {
      catchBlockList = new List<CatchBlock>();
      foreach (CatchBlock handler in node.Handlers)
      {
        ParameterExpression variable = handler.Variable ?? Expression.Parameter(handler.Test, (string) null);
        Expression expression2;
        Expression expression3;
        if (this._transformToGenerator)
        {
          expression2 = (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("GetCurrentSequencePointForGeneratorFrame"), this._frame);
          expression3 = (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("GetThread"), this._frame);
        }
        else
        {
          expression2 = this._debugMarker;
          expression3 = this._thread;
        }
        Expression expression4 = (Expression) Expression.Block((Expression) Utils.If((Expression) Expression.TypeIs((Expression) variable, typeof (ForceToGeneratorLoopException)), (Expression) Expression.Throw((Expression) variable)), (Expression) Utils.If((Expression) Expression.Equal(this._globalDebugMode, Utils.Constant((object) 3)), this._pushFrame ?? (Expression) Expression.Empty(), (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("OnTraceEvent"), expression3, expression2, (Expression) variable)));
        catchBlockList.Add(Expression.MakeCatchBlock(handler.Test, variable, (Expression) Expression.Block(expression4, handler.Body), handler.Filter));
      }
    }
    if (!this._transformToGenerator && node.Finally != null)
      expression1 = (Expression) Utils.If((Expression) Expression.Not((Expression) Expression.Call(typeof (RuntimeOps).GetMethod("IsCurrentLeafFrameRemappingToGenerator"), this._thread)), node.Finally);
    if (catchBlockList != null || expression1 != null)
      node = Expression.MakeTry(node.Type, node.Body, expression1 ?? node.Finally, node.Fault, catchBlockList != null ? (IEnumerable<CatchBlock>) catchBlockList : (IEnumerable<CatchBlock>) node.Handlers);
    return (Expression) node;
  }

  protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
  {
    ParameterExpression variable = ((ExpressionVisitor) this).VisitAndConvert<ParameterExpression>(node.Variable, nameof (VisitCatchBlock));
    this._insideConditionalBlock = true;
    Expression filter;
    Expression body;
    try
    {
      filter = ((ExpressionVisitor) this).Visit(node.Filter);
      body = ((ExpressionVisitor) this).Visit(node.Body);
    }
    finally
    {
      this._insideConditionalBlock = false;
    }
    return variable == node.Variable && body == node.Body && filter == node.Filter ? node : Expression.MakeCatchBlock(node.Test, variable, body, filter);
  }

  protected virtual Expression VisitConditional(ConditionalExpression node)
  {
    Expression test = ((ExpressionVisitor) this).Visit(node.Test);
    this._insideConditionalBlock = true;
    Expression ifTrue;
    Expression ifFalse;
    try
    {
      ifTrue = ((ExpressionVisitor) this).Visit(node.IfTrue);
      ifFalse = ((ExpressionVisitor) this).Visit(node.IfFalse);
    }
    finally
    {
      this._insideConditionalBlock = false;
    }
    return test == node.Test && ifTrue == node.IfTrue && ifFalse == node.IfFalse ? (Expression) node : (Expression) Expression.Condition(test, ifTrue, ifFalse, node.Type);
  }

  protected virtual SwitchCase VisitSwitchCase(SwitchCase node)
  {
    this._insideConditionalBlock = true;
    try
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitSwitchCase(node));
    }
    finally
    {
      this._insideConditionalBlock = false;
    }
  }

  protected virtual Expression VisitDynamic(DynamicExpression node)
  {
    return this.VisitCall(base.VisitDynamic(node));
  }

  protected virtual Expression VisitMethodCall(MethodCallExpression node)
  {
    // ISSUE: explicit non-virtual call
    return this.VisitCall(__nonvirtual (((ExpressionVisitor) this).VisitMethodCall(node)));
  }

  protected virtual Expression VisitInvocation(InvocationExpression node)
  {
    // ISSUE: explicit non-virtual call
    return this.VisitCall(__nonvirtual (((ExpressionVisitor) this).VisitInvocation(node)));
  }

  protected virtual Expression VisitNew(NewExpression node)
  {
    // ISSUE: explicit non-virtual call
    return this.VisitCall(__nonvirtual (((ExpressionVisitor) this).VisitNew(node)));
  }

  internal Expression VisitCall(Expression node)
  {
    if (this._lambdaInfo.OptimizeForLeafFrames && (this._lambdaInfo.CompilerSupport == null || this._lambdaInfo.CompilerSupport.IsCallToDebuggableLambda(node)))
    {
      if (!this._insideConditionalBlock)
        this._hasUnconditionalFunctionCalls = true;
      if (!this._transformToGenerator && this._pushFrame != null)
        return (Expression) Expression.Block(this._pushFrame, node);
    }
    return node;
  }

  protected virtual Expression VisitParameter(ParameterExpression node)
  {
    if (this._replacedLocals == null)
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitParameter(node));
    }
    ParameterExpression parameterExpression;
    // ISSUE: explicit non-virtual call
    return this._replacedLocals.TryGetValue(node, out parameterExpression) ? (Expression) parameterExpression : __nonvirtual (((ExpressionVisitor) this).VisitParameter(node));
  }

  protected virtual Expression VisitDebugInfo(DebugInfoExpression node)
  {
    if (node.IsClear)
      return (Expression) Expression.Empty();
    if (node.Document == null)
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ErrorStrings.DebugInfoWithoutSymbolDocumentInfo, (object) this._locationCookie));
    DebugSourceFile debugSourceFile = this._debugContext.GetDebugSourceFile(string.IsNullOrEmpty(node.Document.FileName) ? "<compile>" : node.Document.FileName);
    int num = this._locationCookie++;
    Expression expression1;
    if (!this._transformToGenerator)
    {
      Expression expression2 = num != 0 ? (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("OnTraceEvent"), this._thread, Utils.Constant((object) num), (Expression) Expression.Convert((Expression) Expression.Constant((object) null), typeof (Exception))) : (Expression) Expression.Empty();
      expression1 = (Expression) Expression.Block((Expression) Expression.Assign(this._debugMarker, Utils.Constant((object) num)), (Expression) Expression.IfThen((Expression) Expression.GreaterThan((Expression) Expression.Property((Expression) this._sourceFilesToVariablesMap[debugSourceFile], "Mode"), (Expression) Expression.Constant((object) 1)), (Expression) Expression.IfThen((Expression) Expression.OrElse((Expression) Expression.Equal((Expression) Expression.Property((Expression) this._sourceFilesToVariablesMap[debugSourceFile], "Mode"), (Expression) Expression.Constant((object) 3)), (Expression) Expression.ArrayIndex(this._traceLocations, Utils.Constant((object) num))), (Expression) Expression.Block(this._pushFrame ?? (Expression) Expression.Empty(), expression2))));
    }
    else
    {
      expression1 = (Expression) Expression.Block((Expression) Utils.YieldReturn(this._generatorLabelTarget, (Expression) this._debugYieldValue, num));
      if (this._currentLocals.Count > 0)
      {
        BlockExpression key = this._currentLocals.Peek();
        IList<VariableInfo> variableInfoList;
        if (!this._variableScopeMapCache.TryGetValue(key, out variableInfoList))
        {
          variableInfoList = (IList<VariableInfo>) new List<VariableInfo>();
          BlockExpression[] array = this._currentLocals.ToArray();
          for (int index = array.Length - 1; index >= 0; --index)
          {
            foreach (ParameterExpression variable in array[index].Variables)
              variableInfoList.Add(this._localsToVarInfos[variable]);
          }
          this._variableScopeMapCache.Add(key, variableInfoList);
        }
        this._variableScopeMap.Add(num, variableInfoList);
      }
      DebugSourceSpan debugSourceSpan = new DebugSourceSpan(debugSourceFile, node.StartLine, node.StartColumn, node.EndLine, node.EndColumn);
      this._markerLocationMap.Add(num, debugSourceSpan);
    }
    return expression1;
  }
}
