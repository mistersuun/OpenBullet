// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebuggableLambdaBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal class DebuggableLambdaBuilder
{
  private readonly DebugContext _debugContext;
  private Dictionary<DebugSourceFile, ParameterExpression> _sourceFilesMap;
  private readonly DebugLambdaInfo _lambdaInfo;
  private string _alias;
  private readonly Expression _debugContextExpression;
  private readonly Expression _globalDebugMode;
  private LabelTarget _generatorLabelTarget;
  private DebugSourceSpan[] _debugMarkerLocationMap;
  private IList<VariableInfo>[] _variableScopeMap;
  private List<ParameterExpression> _lambdaVars;
  private List<ParameterExpression> _lambdaParams;
  private List<ParameterExpression> _generatorVars;
  private List<ParameterExpression> _generatorParams;
  private ParameterExpression _retVal;
  private Expression _pushFrame;
  private Expression _conditionalPushFrame;
  private bool _noPushFrameOptimization;
  private readonly List<ParameterExpression> _pendingLocals;
  private readonly List<ParameterExpression> _verifiedLocals;
  private readonly Dictionary<string, object> _verifiedLocalNames;
  private readonly List<VariableInfo> _variableInfos;
  private readonly Dictionary<ParameterExpression, VariableInfo> _pendingToVariableInfosMap;
  private readonly Dictionary<ParameterExpression, ParameterExpression> _pendingToVerifiedLocalsMap;
  private Expression _functionInfo;
  private int _lambdaId;
  private static readonly ParameterExpression _frame = Expression.Variable(typeof (DebugFrame), "$frame");
  private static readonly ParameterExpression _thread = Expression.Variable(typeof (DebugThread), "$thread");
  private static readonly ParameterExpression _debugMarker = Expression.Variable(typeof (int), "$debugMarker");
  private static readonly ParameterExpression _framePushed = Expression.Variable(typeof (bool), "$framePushed");
  private static readonly ParameterExpression _funcInfo = Expression.Parameter(typeof (FunctionInfo), "$funcInfo");
  private static readonly ParameterExpression _traceLocations = Expression.Parameter(typeof (bool[]), "$traceLocations");
  private static readonly ParameterExpression _retValAsObject = Expression.Variable(typeof (object), "$retVal");
  private static readonly ParameterExpression _retValFromGeneratorLoop = Expression.Variable(typeof (object), "$retValFromGen");
  private static readonly ParameterExpression _frameExitException = Expression.Parameter(typeof (bool), "$frameExitException");

  internal DebuggableLambdaBuilder(DebugContext debugContext, DebugLambdaInfo lambdaInfo)
  {
    this._debugContext = debugContext;
    this._lambdaInfo = lambdaInfo;
    this._alias = this._lambdaInfo.LambdaAlias;
    this._debugContextExpression = Microsoft.Scripting.Ast.Utils.Constant((object) debugContext);
    this._verifiedLocals = new List<ParameterExpression>();
    this._verifiedLocalNames = new Dictionary<string, object>();
    this._pendingLocals = new List<ParameterExpression>();
    this._variableInfos = new List<VariableInfo>();
    this._pendingToVariableInfosMap = new Dictionary<ParameterExpression, VariableInfo>();
    this._pendingToVerifiedLocalsMap = new Dictionary<ParameterExpression, ParameterExpression>();
    this._globalDebugMode = (Expression) Expression.Property(this._debugContextExpression, "Mode");
  }

  internal LambdaExpression Transform(LambdaExpression lambda)
  {
    if (this._alias == null)
    {
      this._alias = lambda.Name;
      if (this._alias == null)
        this._alias = "$lambda" + (object) ++this._lambdaId;
    }
    this._lambdaVars = new List<ParameterExpression>();
    this._lambdaParams = new List<ParameterExpression>();
    this._generatorVars = new List<ParameterExpression>();
    this._generatorParams = new List<ParameterExpression>();
    return lambda.Body is GeneratorExpression ? this.TransformGenerator(lambda) : this.TransformLambda(lambda);
  }

  private LambdaExpression TransformLambda(LambdaExpression lambda)
  {
    Expression body1 = lambda.Body;
    this._lambdaVars.AddRange((IEnumerable<ParameterExpression>) new ParameterExpression[6]
    {
      DebuggableLambdaBuilder._thread,
      DebuggableLambdaBuilder._framePushed,
      DebuggableLambdaBuilder._funcInfo,
      DebuggableLambdaBuilder._traceLocations,
      DebuggableLambdaBuilder._debugMarker,
      DebuggableLambdaBuilder._frameExitException
    });
    this._generatorParams.Add(DebuggableLambdaBuilder._frame);
    Type returnType = lambda.Type.GetMethod("Invoke").ReturnType;
    if (returnType == typeof (object))
      this._retVal = DebuggableLambdaBuilder._retValAsObject;
    else if (returnType != typeof (void))
      this._retVal = Expression.Variable(returnType, "$retVal");
    if (this._retVal != null)
    {
      this._lambdaVars.Add(this._retVal);
      this._generatorVars.Add(this._retVal);
    }
    this._lambdaVars.Add(DebuggableLambdaBuilder._retValFromGeneratorLoop);
    Dictionary<ParameterExpression, object> parameters = new Dictionary<ParameterExpression, object>();
    foreach (ParameterExpression parameter in lambda.Parameters)
      parameters.Add(parameter, (object) null);
    this._pendingLocals.AddRange((IEnumerable<ParameterExpression>) lambda.Parameters);
    LambdaWalker lambdaWalker = new LambdaWalker();
    Expression body2 = lambdaWalker.Visit(body1);
    this._pendingLocals.AddRange((IEnumerable<ParameterExpression>) lambdaWalker.Locals);
    this.LayOutVariables(lambdaWalker.StrongBoxedLocals, parameters);
    Expression generatorBody = this.TransformToGeneratorBody(body2);
    this._lambdaVars.AddRange((IEnumerable<ParameterExpression>) this._sourceFilesMap.Values);
    this.CreatePushFrameExpression();
    Expression debuggableBody = this.TransformToDebuggableBody(body2);
    this.CreateFunctionInfo(this.CreateGeneratorFactoryLambda(generatorBody));
    return this.CreateOuterLambda(lambda.Type, debuggableBody);
  }

  private LambdaExpression TransformGenerator(LambdaExpression lambda)
  {
    GeneratorExpression body1 = (GeneratorExpression) lambda.Body;
    Expression body2 = body1.Body;
    this._generatorLabelTarget = body1.Target;
    this._generatorParams.Add(DebuggableLambdaBuilder._frame);
    Dictionary<ParameterExpression, object> parameters = new Dictionary<ParameterExpression, object>();
    foreach (ParameterExpression parameter in lambda.Parameters)
      parameters.Add(parameter, (object) null);
    this._pendingLocals.AddRange((IEnumerable<ParameterExpression>) lambda.Parameters);
    LambdaWalker lambdaWalker = new LambdaWalker();
    lambdaWalker.Visit(body2);
    this._pendingLocals.AddRange((IEnumerable<ParameterExpression>) lambdaWalker.Locals);
    this.LayoutVariablesForGenerator(parameters);
    this.CreateFunctionInfo(this.CreateGeneratorFactoryLambda(this.TransformToGeneratorBody(body2)));
    return this.CreateOuterGeneratorFactory(lambda.Type);
  }

  private void LayOutVariables(
    Dictionary<ParameterExpression, object> strongBoxedLocals,
    Dictionary<ParameterExpression, object> parameters)
  {
    IList<ParameterExpression> hiddenVariables = this._lambdaInfo.HiddenVariables;
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < this._pendingLocals.Count; ++index)
    {
      ParameterExpression pendingLocal = this._pendingLocals[index];
      ParameterExpression parameterExpression = pendingLocal;
      if (!this._pendingToVariableInfosMap.ContainsKey(pendingLocal))
      {
        string str;
        if (this._lambdaInfo.VariableAliases == null || !this._lambdaInfo.VariableAliases.TryGetValue(pendingLocal, out str))
          str = pendingLocal.Name;
        bool parameter = parameters.ContainsKey(pendingLocal);
        bool hidden = hiddenVariables != null && hiddenVariables.Contains(pendingLocal);
        bool strongBoxed = strongBoxedLocals.ContainsKey(pendingLocal);
        if (str == null)
        {
          str = "local";
          hidden = true;
        }
        bool flag = this._verifiedLocalNames.ContainsKey(str);
        if (flag)
        {
          int num3 = 1;
          for (; flag; flag = this._verifiedLocalNames.ContainsKey(str))
            str += (string) (object) num3++;
          parameterExpression = Expression.Parameter(parameterExpression.Type, str);
        }
        this._verifiedLocals.Add(parameterExpression);
        this._verifiedLocalNames.Add(str, (object) null);
        if (pendingLocal != parameterExpression)
          this._pendingToVerifiedLocalsMap.Add(pendingLocal, parameterExpression);
        int localIndex = strongBoxed ? num2++ : num1++;
        VariableInfo variableInfo = new VariableInfo(str, pendingLocal.Type, parameter, hidden, strongBoxed, localIndex, this._variableInfos.Count);
        this._variableInfos.Add(variableInfo);
        this._pendingToVariableInfosMap.Add(pendingLocal, variableInfo);
        if (parameter)
        {
          this._lambdaParams.Add(pendingLocal);
          this._generatorParams.Add(pendingLocal);
        }
        else
        {
          this._lambdaVars.Add(parameterExpression);
          this._generatorVars.Add(pendingLocal);
        }
      }
    }
  }

  private void LayoutVariablesForGenerator(Dictionary<ParameterExpression, object> parameters)
  {
    IList<ParameterExpression> hiddenVariables = this._lambdaInfo.HiddenVariables;
    int num1 = 0;
    for (int index = 0; index < this._pendingLocals.Count; ++index)
    {
      ParameterExpression pendingLocal = this._pendingLocals[index];
      ParameterExpression parameterExpression = pendingLocal;
      string str;
      if (this._lambdaInfo.VariableAliases == null || !this._lambdaInfo.VariableAliases.TryGetValue(pendingLocal, out str))
        str = pendingLocal.Name;
      bool parameter = parameters.ContainsKey(pendingLocal);
      bool hidden = hiddenVariables != null && hiddenVariables.Contains(pendingLocal);
      if (str == null)
      {
        str = "local";
        hidden = true;
      }
      bool flag = this._verifiedLocalNames.ContainsKey(str);
      if (flag)
      {
        int num2 = 1;
        for (; flag; flag = this._verifiedLocalNames.ContainsKey(str))
          str += (string) (object) num2++;
        parameterExpression = Expression.Parameter(parameterExpression.Type, str);
      }
      this._verifiedLocals.Add(parameterExpression);
      this._verifiedLocalNames.Add(str, (object) null);
      if (pendingLocal != parameterExpression)
        this._pendingToVerifiedLocalsMap.Add(pendingLocal, parameterExpression);
      VariableInfo variableInfo = new VariableInfo(str, pendingLocal.Type, parameter, hidden, true, num1++, this._variableInfos.Count);
      this._variableInfos.Add(variableInfo);
      this._pendingToVariableInfosMap.Add(pendingLocal, variableInfo);
      if (parameter)
      {
        this._lambdaParams.Add(pendingLocal);
        this._generatorParams.Add(pendingLocal);
      }
      else
        this._generatorVars.Add(pendingLocal);
    }
  }

  private void CreatePushFrameExpression()
  {
    this._pushFrame = (Expression) Expression.Block((Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._framePushed, (Expression) Expression.Constant((object) true)), (Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._thread, (Expression) Microsoft.Scripting.Ast.Utils.SimpleCallHelper(typeof (RuntimeOps).GetMethod("GetCurrentThread"), this._debugContextExpression)), this._debugContext.ThreadFactory.CreatePushFrameExpression(DebuggableLambdaBuilder._funcInfo, DebuggableLambdaBuilder._debugMarker, (IList<ParameterExpression>) this._verifiedLocals, (IList<VariableInfo>) this._variableInfos, (Expression) DebuggableLambdaBuilder._thread));
    this._conditionalPushFrame = (Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.Equal((Expression) DebuggableLambdaBuilder._framePushed, (Expression) Expression.Constant((object) false)), this._pushFrame);
  }

  private void CreateFunctionInfo(LambdaExpression generatorFactoryLambda)
  {
    if (this._lambdaInfo.CompilerSupport != null && this._lambdaInfo.CompilerSupport.DoesExpressionNeedReduction((Expression) generatorFactoryLambda))
      this._functionInfo = this._lambdaInfo.CompilerSupport.QueueExpressionForReduction((Expression) Expression.Call(typeof (RuntimeOps).GetMethod(nameof (CreateFunctionInfo)), (Expression) generatorFactoryLambda, Microsoft.Scripting.Ast.Utils.Constant((object) this._alias), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) this._debugMarkerLocationMap, typeof (object)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) this._variableScopeMap, typeof (object)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) this._variableInfos, typeof (object)), (Expression) Expression.Constant(this._lambdaInfo.CustomPayload, typeof (object))));
    else
      this._functionInfo = (Expression) Expression.Constant((object) DebugContext.CreateFunctionInfo(generatorFactoryLambda.Compile(), this._alias, this._debugMarkerLocationMap, this._variableScopeMap, (IList<VariableInfo>) this._variableInfos, this._lambdaInfo.CustomPayload), typeof (FunctionInfo));
  }

  private Expression TransformToDebuggableBody(Expression body)
  {
    return ((ExpressionVisitor) new DebugInfoRewriter(this._debugContext, false, (Expression) DebuggableLambdaBuilder._traceLocations, (Expression) DebuggableLambdaBuilder._thread, (Expression) DebuggableLambdaBuilder._frame, this._noPushFrameOptimization ? (Expression) null : this._conditionalPushFrame, (Expression) DebuggableLambdaBuilder._debugMarker, this._globalDebugMode, this._sourceFilesMap, (LabelTarget) null, this._pendingToVerifiedLocalsMap, (Dictionary<ParameterExpression, VariableInfo>) null, this._lambdaInfo)).Visit(body);
  }

  private Expression TransformToGeneratorBody(Expression body)
  {
    if (this._generatorLabelTarget == null)
      this._generatorLabelTarget = Expression.Label(typeof (object));
    DebugInfoRewriter debugInfoRewriter = new DebugInfoRewriter(this._debugContext, true, (Expression) DebuggableLambdaBuilder._traceLocations, (Expression) DebuggableLambdaBuilder._thread, (Expression) DebuggableLambdaBuilder._frame, (Expression) null, (Expression) null, this._globalDebugMode, (Dictionary<DebugSourceFile, ParameterExpression>) null, this._generatorLabelTarget, (Dictionary<ParameterExpression, ParameterExpression>) null, this._pendingToVariableInfosMap, this._lambdaInfo);
    Expression generatorBody = ((ExpressionVisitor) debugInfoRewriter).Visit(body);
    this._debugMarkerLocationMap = debugInfoRewriter.DebugMarkerLocationMap;
    this._variableScopeMap = debugInfoRewriter.VariableScopeMap;
    this._sourceFilesMap = new Dictionary<DebugSourceFile, ParameterExpression>();
    foreach (DebugSourceSpan debugMarkerLocation in this._debugMarkerLocationMap)
    {
      if (!this._sourceFilesMap.ContainsKey(debugMarkerLocation.SourceFile))
        this._sourceFilesMap.Add(debugMarkerLocation.SourceFile, Expression.Parameter(typeof (DebugSourceFile)));
    }
    this._noPushFrameOptimization = !this._lambdaInfo.OptimizeForLeafFrames || debugInfoRewriter.HasUnconditionalFunctionCalls;
    return generatorBody;
  }

  private LambdaExpression CreateOuterLambda(Type lambdaType, Expression debuggableBody)
  {
    List<Expression> expressionList1 = new List<Expression>();
    List<Expression> expressionList2 = new List<Expression>();
    List<Expression> expressionList3 = new List<Expression>();
    Type returnType = lambdaType.GetMethod("Invoke").ReturnType;
    LabelTarget target = Expression.Label(returnType);
    expressionList2.Add((Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._funcInfo, (Expression) Expression.Convert(this._functionInfo, typeof (FunctionInfo))));
    expressionList2.Add((Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._traceLocations, (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("GetTraceLocations"), (Expression) DebuggableLambdaBuilder._funcInfo)));
    foreach (KeyValuePair<DebugSourceFile, ParameterExpression> sourceFiles in this._sourceFilesMap)
      expressionList2.Add((Expression) Expression.Assign((Expression) sourceFiles.Value, (Expression) Expression.Constant((object) sourceFiles.Key, typeof (DebugSourceFile))));
    if (this._noPushFrameOptimization)
      expressionList2.Add(this._pushFrame);
    expressionList2.Add((Expression) Expression.Call(typeof (RuntimeOps).GetMethod("OnFrameEnterTraceEvent"), (Expression) DebuggableLambdaBuilder._thread));
    IfStatementBuilder body1 = Microsoft.Scripting.Ast.Utils.If((Expression) Expression.Equal(this._debugMarkerLocationMap.Length != 0 ? (Expression) Expression.Property((Expression) this._sourceFilesMap[this._debugMarkerLocationMap[0].SourceFile], "Mode") : this._globalDebugMode, Microsoft.Scripting.Ast.Utils.Constant((object) 3)), (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("OnFrameExitTraceEvent"), (Expression) DebuggableLambdaBuilder._thread, (Expression) DebuggableLambdaBuilder._debugMarker, this._retVal != null ? (Expression) Expression.Convert((Expression) this._retVal, typeof (object)) : (Expression) Expression.Constant((object) null)));
    expressionList2.Add((Expression) Expression.Block(this._retVal != null ? (Expression) Expression.Assign((Expression) this._retVal, debuggableBody) : debuggableBody, (Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._frameExitException, (Expression) Expression.Constant((object) true)), (Expression) body1));
    expressionList2.Add(this._retVal != null ? (Expression) Expression.Return(target, (Expression) this._retVal) : (Expression) Expression.Empty());
    Expression[] collection = new Expression[1]
    {
      (Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.AndAlso((Expression) Expression.Equal((Expression) Expression.Call(typeof (RuntimeOps).GetMethod("PopFrame"), (Expression) DebuggableLambdaBuilder._thread), (Expression) Expression.Constant((object) true)), (Expression) Expression.Equal(this._globalDebugMode, Microsoft.Scripting.Ast.Utils.Constant((object) 3))), (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("OnThreadExitEvent"), (Expression) DebuggableLambdaBuilder._thread))
    };
    if (this._noPushFrameOptimization)
      expressionList3.AddRange((IEnumerable<Expression>) collection);
    else
      expressionList3.Add((Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.Equal((Expression) DebuggableLambdaBuilder._framePushed, (Expression) Expression.Constant((object) true)), collection));
    List<Expression> expressionList4 = expressionList1;
    BlockExpression body2 = Expression.Block(ArrayUtils.Append<Expression>(expressionList2.ToArray(), (Expression) Expression.Default(returnType)));
    CatchBlock[] catchBlockArray1 = new CatchBlock[1];
    ParameterExpression variable;
    ParameterExpression parameterExpression = variable = Expression.Variable(typeof (Exception), "$caughtException");
    BlockExpression body3 = Expression.Block((Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) parameterExpression, typeof (ForceToGeneratorLoopException))), (Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.NotEqual(this._globalDebugMode, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), this._noPushFrameOptimization ? (Expression) Expression.Empty() : this._conditionalPushFrame, (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("OnTraceEventUnwind"), (Expression) DebuggableLambdaBuilder._thread, (Expression) DebuggableLambdaBuilder._debugMarker, (Expression) parameterExpression)), (Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.Not((Expression) DebuggableLambdaBuilder._frameExitException), (Expression) body1)), (Expression) Expression.Rethrow(), (Expression) Expression.Default(returnType));
    catchBlockArray1[0] = Expression.Catch(variable, (Expression) body3);
    TryExpression body4 = Expression.TryCatch((Expression) body2, catchBlockArray1);
    BlockExpression @finally = Expression.Block((IEnumerable<Expression>) expressionList3);
    CatchBlock[] catchBlockArray2 = new CatchBlock[1];
    Type type = typeof (ForceToGeneratorLoopException);
    BlockExpression blockExpression;
    if (!(returnType != typeof (void)))
      blockExpression = Expression.Block((Expression) Expression.Call(typeof (RuntimeOps).GetMethod("GeneratorLoopProc"), (Expression) DebuggableLambdaBuilder._thread), (Expression) Expression.Return(target));
    else
      blockExpression = Expression.Block((Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._retValFromGeneratorLoop, (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("GeneratorLoopProc"), (Expression) DebuggableLambdaBuilder._thread)), Microsoft.Scripting.Ast.Utils.If((Expression) Expression.NotEqual((Expression) DebuggableLambdaBuilder._retValFromGeneratorLoop, (Expression) Expression.Constant((object) null)), (Expression) Expression.Assign((Expression) this._retVal, (Expression) Expression.Convert((Expression) DebuggableLambdaBuilder._retValFromGeneratorLoop, returnType)), (Expression) Expression.Return(target, (Expression) Expression.Convert((Expression) DebuggableLambdaBuilder._retValFromGeneratorLoop, returnType))).Else((Expression) Expression.Assign((Expression) this._retVal, (Expression) Expression.Default(returnType)), (Expression) Expression.Return(target, (Expression) Expression.Default(returnType))));
    DefaultExpression defaultExpression = Expression.Default(returnType);
    TryExpression body5 = Expression.TryFinally((Expression) Expression.Block((Expression) blockExpression, (Expression) defaultExpression), (Expression) Expression.Assign((Expression) DebuggableLambdaBuilder._debugMarker, (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("GetCurrentSequencePointForLeafGeneratorFrame"), (Expression) DebuggableLambdaBuilder._thread)));
    catchBlockArray2[0] = Expression.Catch(type, (Expression) body5);
    TryExpression tryExpression = Expression.TryCatchFinally((Expression) body4, (Expression) @finally, catchBlockArray2);
    expressionList4.Add((Expression) tryExpression);
    Expression defaultValue = (Expression) Expression.Block((IEnumerable<Expression>) expressionList1);
    if (defaultValue.Type == typeof (void) && returnType != typeof (void))
      defaultValue = (Expression) Expression.Block(defaultValue, (Expression) Expression.Default(returnType));
    return Expression.Lambda(lambdaType, (Expression) Expression.Block((IEnumerable<ParameterExpression>) this._lambdaVars, (Expression) Expression.Label(target, defaultValue)), this._alias, (IEnumerable<ParameterExpression>) this._lambdaParams);
  }

  private LambdaExpression CreateGeneratorFactoryLambda(Expression generatorBody)
  {
    Expression expression = (Expression) Expression.Block((Expression) Expression.Call(typeof (RuntimeOps).GetMethod("ReplaceLiftedLocals"), (Expression) DebuggableLambdaBuilder._frame, (Expression) Expression.RuntimeVariables((IEnumerable<ParameterExpression>) this._pendingLocals)), generatorBody);
    if (this._retVal != null)
      expression = (Expression) Expression.Block((Expression) Expression.Assign((Expression) this._retVal, expression), (Expression) Microsoft.Scripting.Ast.Utils.YieldReturn(this._generatorLabelTarget, (Expression) Expression.Convert((Expression) this._retVal, typeof (object))));
    List<Type> typeList = new List<Type>();
    for (int index = 0; index < this._variableInfos.Count; ++index)
    {
      VariableInfo variableInfo = this._variableInfos[index];
      if (variableInfo.IsParameter)
        typeList.Add(variableInfo.VariableType);
    }
    if (expression.Type != typeof (void))
      expression = Microsoft.Scripting.Ast.Utils.Void(expression);
    return Microsoft.Scripting.Ast.Utils.GeneratorLambda(InvokeTargets.GetGeneratorFactoryTarget(typeList.ToArray()), this._generatorLabelTarget, (Expression) Expression.Block((IEnumerable<ParameterExpression>) this._generatorVars, expression), this._alias, (IEnumerable<ParameterExpression>) this._generatorParams);
  }

  private LambdaExpression CreateOuterGeneratorFactory(Type lambdaType)
  {
    LabelTarget target = Expression.Label(lambdaType.GetMethod("Invoke").ReturnType);
    Expression expression = (Expression) Expression.Return(target, (Expression) Expression.Call(typeof (RuntimeOps), "CreateDebugGenerator", new Type[1]
    {
      this._generatorLabelTarget.Type
    }, (Expression) Expression.Call(typeof (RuntimeOps).GetMethod("CreateFrameForGenerator"), this._debugContextExpression, this._functionInfo)));
    LabelExpression labelExpression = !(target.Type == typeof (void)) ? Expression.Label(target, Microsoft.Scripting.Ast.Utils.Convert(expression, target.Type)) : Expression.Label(target, Microsoft.Scripting.Ast.Utils.Void(expression));
    return Expression.Lambda(lambdaType, (Expression) Expression.Block((IEnumerable<ParameterExpression>) this._lambdaVars, (Expression) labelExpression), this._alias, (IEnumerable<ParameterExpression>) this._lambdaParams);
  }
}
