// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaPythonFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaPythonFunction(
  Expression expression,
  BindingRestrictions restrictions,
  PythonFunction value) : 
  MetaPythonObject(expression, BindingRestrictions.Empty, (object) value),
  IPythonInvokable,
  IPythonOperable,
  IPythonConvertible,
  IInferableInvokable,
  IConvertibleMetaObject,
  IPythonGetable
{
  public DynamicMetaObject Invoke(
    PythonInvokeBinder pythonInvoke,
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return new MetaPythonFunction.FunctionBinderHelper((DynamicMetaObjectBinder) pythonInvoke, this, codeContext, args).MakeMetaObject();
  }

  public DynamicMetaObject GetMember(PythonGetMemberBinder member, DynamicMetaObject codeContext)
  {
    return this.BindGetMemberWorker((DynamicMetaObjectBinder) member, member.Name, codeContext);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder action,
    DynamicMetaObject[] args)
  {
    ParameterExpression left = Expression.Parameter(typeof (object));
    DynamicMetaObject dynamicMetaObject = action.FallbackInvokeMember((DynamicMetaObject) this, args);
    return action.FallbackInvokeMember((DynamicMetaObject) this, args, new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) left, (Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionGetMember"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonFunction)), (Expression) Expression.Constant((object) action.Name))), (Expression) Expression.Constant((object) OperationFailed.Value)), action.FallbackInvoke(new DynamicMetaObject((Expression) left, BindingRestrictions.Empty), args, (DynamicMetaObject) null).Expression, Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (object)))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)).Merge(dynamicMetaObject.Restrictions)));
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder call, DynamicMetaObject[] args)
  {
    return new MetaPythonFunction.FunctionBinderHelper((DynamicMetaObjectBinder) call, this, (Expression) null, args).MakeMetaObject();
  }

  public override DynamicMetaObject BindConvert(ConvertBinder conversion)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) conversion, conversion.Type, conversion.Explicit ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast);
  }

  public DynamicMetaObject BindConvert(PythonConversionBinder binder)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) binder, binder.Type, binder.ResultKind);
  }

  public DynamicMetaObject ConvertWorker(
    DynamicMetaObjectBinder binder,
    Type type,
    ConversionResultKind kind)
  {
    return type.IsSubclassOf(typeof (Delegate)) ? MetaPythonObject.MakeDelegateTarget(binder, type, this.Restrict(typeof (PythonFunction))) : this.FallbackConvert(binder);
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    foreach (object key in (IEnumerable<object>) this.Value.__dict__.Keys)
    {
      if (key is string)
        yield return (string) key;
    }
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    return this.BindGetMemberWorker((DynamicMetaObjectBinder) binder, binder.Name, PythonContext.GetCodeContextMO((DynamicMetaObjectBinder) binder));
  }

  private DynamicMetaObject BindGetMemberWorker(
    DynamicMetaObjectBinder binder,
    string name,
    DynamicMetaObject codeContext)
  {
    ParameterExpression parameterExpression = Expression.Parameter(typeof (object));
    DynamicMetaObject member = MetaPythonFunction.FallbackGetMember(binder, (DynamicMetaObject) this, codeContext);
    return MetaPythonFunction.FallbackGetMember(binder, (DynamicMetaObject) this, codeContext, new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionGetMember"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonFunction)), (Expression) Expression.Constant((object) name))), (Expression) Expression.Constant((object) OperationFailed.Value)), (Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(member.Expression, typeof (object)))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)).Merge(member.Restrictions)));
  }

  private static DynamicMetaObject FallbackGetMember(
    DynamicMetaObjectBinder binder,
    DynamicMetaObject self,
    DynamicMetaObject codeContext)
  {
    return MetaPythonFunction.FallbackGetMember(binder, self, codeContext, (DynamicMetaObject) null);
  }

  private static DynamicMetaObject FallbackGetMember(
    DynamicMetaObjectBinder binder,
    DynamicMetaObject self,
    DynamicMetaObject codeContext,
    DynamicMetaObject errorSuggestion)
  {
    return binder is PythonGetMemberBinder pythonGetMemberBinder ? pythonGetMemberBinder.Fallback(self, codeContext, errorSuggestion) : ((GetMemberBinder) binder).FallbackGetMember(self, errorSuggestion);
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
  {
    return binder.FallbackSetMember((DynamicMetaObject) this, value, new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionSetMember"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonFunction)), (Expression) Expression.Constant((object) binder.Name), Microsoft.Scripting.Ast.Utils.Convert(value.Expression, typeof (object))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction))));
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
  {
    switch (binder.Name)
    {
      case "func_dict":
      case "__dict__":
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionDeleteDict")), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)));
      case "__doc__":
      case "func_doc":
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionDeleteDoc"), (Expression) Expression.Convert(this.Expression, typeof (PythonFunction))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)));
      case "func_defaults":
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionDeleteDefaults"), (Expression) Expression.Convert(this.Expression, typeof (PythonFunction))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)));
      default:
        DynamicMetaObject dynamicMetaObject = binder.FallbackDeleteMember((DynamicMetaObject) this);
        return binder.FallbackDeleteMember((DynamicMetaObject) this, new DynamicMetaObject((Expression) Expression.Condition((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonFunctionDeleteMember"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonFunction)), (Expression) Expression.Constant((object) binder.Name)), (Expression) Expression.Default(typeof (void)), Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (void))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)).Merge(dynamicMetaObject.Restrictions)));
    }
  }

  private static DynamicMetaObject MakeCallSignatureRule(DynamicMetaObject self)
  {
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetFunctionSignature"), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, typeof (PythonFunction))), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, typeof (PythonFunction)));
  }

  private static DynamicMetaObject MakeIsCallableRule(DynamicMetaObject self)
  {
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) true), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, typeof (PythonFunction)));
  }

  public PythonFunction Value => (PythonFunction) base.Value;

  DynamicMetaObject IPythonOperable.BindOperation(
    PythonOperationBinder action,
    DynamicMetaObject[] args)
  {
    switch (action.Operation)
    {
      case PythonOperationKind.CallSignatures:
        return MetaPythonFunction.MakeCallSignatureRule((DynamicMetaObject) this);
      case PythonOperationKind.IsCallable:
        return MetaPythonFunction.MakeIsCallableRule((DynamicMetaObject) this);
      default:
        return (DynamicMetaObject) null;
    }
  }

  InferenceResult IInferableInvokable.GetInferredType(Type delegateType, Type parameterType)
  {
    if (!delegateType.IsSubclassOf(typeof (Delegate)))
      throw new InvalidOperationException();
    return delegateType.GetMethod("Invoke").GetParameters().Length == this.Value.NormalArgumentCount ? new InferenceResult(typeof (object), this.Restrictions.Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (PythonFunction)).Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal((Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionGetCompatibility"), (Expression) Expression.Convert(this.Expression, typeof (PythonFunction))), (Expression) Expression.Constant((object) this.Value.FunctionCompatibility)))))) : (InferenceResult) null;
  }

  bool IConvertibleMetaObject.CanConvertTo(Type type, bool @explicit)
  {
    return type.IsSubclassOf(typeof (Delegate));
  }

  private class FunctionBinderHelper
  {
    private readonly MetaPythonFunction _func;
    private readonly DynamicMetaObject[] _args;
    private readonly DynamicMetaObject[] _originalArgs;
    private readonly DynamicMetaObjectBinder _call;
    private readonly Expression _codeContext;
    private List<ParameterExpression> _temps;
    private ParameterExpression _dict;
    private ParameterExpression _params;
    private ParameterExpression _paramsLen;
    private List<Expression> _init;
    private Expression _error;
    private bool _extractedParams;
    private bool _extractedDefault;
    private bool _needCodeTest;
    private Expression _deferTest;
    private Expression _userProvidedParams;
    private Expression _paramlessCheck;

    public FunctionBinderHelper(
      DynamicMetaObjectBinder call,
      MetaPythonFunction function,
      Expression codeContext,
      DynamicMetaObject[] args)
    {
      this._call = call;
      this._func = function;
      this._args = args;
      this._originalArgs = args;
      this._temps = new List<ParameterExpression>();
      this._codeContext = codeContext;
      int indexToRemove = this.Signature.IndexOf(ArgumentType.Instance);
      if (indexToRemove <= -1)
        return;
      this._args = ArrayUtils.RemoveAt<DynamicMetaObject>(this._args, indexToRemove);
    }

    public DynamicMetaObject MakeMetaObject()
    {
      Expression[] argumentsForRule = this.GetArgumentsForRule();
      BindingRestrictions restrictions = this._func.Restrictions.Merge(this.GetRestrictions().Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) this._args)));
      DynamicMetaObject res;
      if (argumentsForRule != null)
      {
        Expression expression = this.AddInitialization(this.MakeFunctionInvoke(argumentsForRule));
        if (this._temps.Count > 0)
          expression = (Expression) Expression.Block((IEnumerable<ParameterExpression>) this._temps, expression);
        res = new DynamicMetaObject(expression, restrictions);
      }
      else if (this._error != null)
      {
        res = new DynamicMetaObject(this._error, restrictions);
      }
      else
      {
        DynamicMetaObjectBinder call = this._call;
        Type type = typeof (PythonOps);
        CallSignature signature = this.Signature;
        string name = signature.HasKeywordArgument() ? "BadKeywordArgumentError" : "FunctionBadArgumentError";
        // ISSUE: explicit non-virtual call
        MethodInfo method = __nonvirtual (type.GetMethod(name));
        Expression expression1 = Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction));
        signature = this.Signature;
        Expression expression2 = Microsoft.Scripting.Ast.Utils.Constant((object) signature.GetProvidedPositionalArgumentCount());
        MethodCallExpression exceptionValue = Expression.Call(method, expression1, expression2);
        Type retType = typeof (object);
        res = new DynamicMetaObject(call.Throw((Expression) exceptionValue, retType), restrictions);
      }
      DynamicMetaObject[] dynamicMetaObjectArray = ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this._func, this._originalArgs);
      if (this._codeContext != null)
        dynamicMetaObjectArray = ArrayUtils.Insert<DynamicMetaObject>(new DynamicMetaObject(this._codeContext, BindingRestrictions.Empty), dynamicMetaObjectArray);
      return BindingHelpers.AddDynamicTestAndDefer(this._call, res, dynamicMetaObjectArray, new ValidationInfo(this._deferTest), res.Expression.Type);
    }

    private CallSignature Signature => BindingHelpers.GetCallSignature(this._call);

    private BindingRestrictions GetRestrictions()
    {
      return !this.Signature.HasKeywordArgument() ? this.GetSimpleRestriction() : this.GetComplexRestriction();
    }

    private BindingRestrictions GetSimpleRestriction()
    {
      this._deferTest = (Expression) Expression.Equal((Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionGetCompatibility"), (Expression) Expression.Convert(this._func.Expression, typeof (PythonFunction))), Microsoft.Scripting.Ast.Utils.Constant((object) this._func.Value.FunctionCompatibility));
      return BindingRestrictionsHelpers.GetRuntimeTypeRestriction(this._func.Expression, typeof (PythonFunction));
    }

    private BindingRestrictions GetComplexRestriction()
    {
      if (this._extractedDefault)
        return BindingRestrictions.GetInstanceRestriction(this._func.Expression, (object) this._func.Value);
      return this._needCodeTest ? this.GetSimpleRestriction().Merge(BindingRestrictions.GetInstanceRestriction((Expression) Expression.Property((Expression) this.GetFunctionParam(), "__code__"), (object) this._func.Value.__code__)) : this.GetSimpleRestriction();
    }

    private Expression[] GetArgumentsForRule()
    {
      Expression[] exprArgs = new Expression[this._func.Value.NormalArgumentCount + this._func.Value.ExtraArguments];
      List<Expression> paramsArgs = (List<Expression>) null;
      Dictionary<string, Expression> namedArgs = (Dictionary<string, Expression>) null;
      int num = this.Signature.IndexOf(ArgumentType.Instance);
      for (int index1 = 0; index1 < this._args.Length; ++index1)
      {
        int index2 = num == -1 || index1 < num ? index1 : index1 + 1;
        switch (this.Signature.GetArgumentKind(index1))
        {
          case ArgumentType.Named:
            this._needCodeTest = true;
            bool flag = false;
            for (int index3 = 0; index3 < this._func.Value.NormalArgumentCount; ++index3)
            {
              if (this._func.Value.ArgNames[index3] == this.Signature.GetArgumentName(index1))
              {
                if (exprArgs[index3] != null)
                {
                  if (this._error == null)
                    this._error = this._call.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("MultipleKeywordArgumentError"), (Expression) this.GetFunctionParam(), (Expression) Expression.Constant((object) this._func.Value.ArgNames[index3])), typeof (object));
                  return (Expression[]) null;
                }
                exprArgs[index3] = this._args[index2].Expression;
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              if (namedArgs == null)
                namedArgs = new Dictionary<string, Expression>();
              namedArgs[this.Signature.GetArgumentName(index1)] = this._args[index2].Expression;
              break;
            }
            break;
          case ArgumentType.List:
            this._userProvidedParams = this._args[index2].Expression;
            break;
          case ArgumentType.Dictionary:
            this._args[index2] = this.MakeDictionaryCopy(this._args[index2]);
            break;
          default:
            if (index1 < this._func.Value.NormalArgumentCount)
            {
              exprArgs[index1] = this._args[index2].Expression;
              break;
            }
            if (paramsArgs == null)
              paramsArgs = new List<Expression>();
            paramsArgs.Add(this._args[index2].Expression);
            break;
        }
      }
      if (this.FinishArguments(exprArgs, paramsArgs, namedArgs))
        return this.GetArgumentsForTargetType(exprArgs);
      if (namedArgs != null && this._func.Value.ExpandDictPosition == -1)
        this.MakeUnexpectedKeywordError(namedArgs);
      return (Expression[]) null;
    }

    private bool FinishArguments(
      Expression[] exprArgs,
      List<Expression> paramsArgs,
      Dictionary<string, Expression> namedArgs)
    {
      int num = this._func.Value.NormalArgumentCount - this._func.Value.Defaults.Length;
      for (int index = 0; index < this._func.Value.NormalArgumentCount; ++index)
      {
        if (exprArgs[index] != null)
        {
          if (this._userProvidedParams != null && index >= this.Signature.GetProvidedPositionalArgumentCount())
            exprArgs[index] = this.ValidateNotDuplicate(exprArgs[index], this._func.Value.ArgNames[index], index);
        }
        else if (index < num)
        {
          exprArgs[index] = this.ExtractNonDefaultValue(this._func.Value.ArgNames[index]);
          if (exprArgs[index] == null)
            return false;
        }
        else
          exprArgs[index] = this.ExtractDefaultValue(index, index - num);
      }
      if (!this.TryFinishList(exprArgs, paramsArgs) || !this.TryFinishDictionary(exprArgs, namedArgs))
        return false;
      this.AddCheckForNoExtraParameters(exprArgs);
      return true;
    }

    private bool TryFinishList(Expression[] exprArgs, List<Expression> paramsArgs)
    {
      if (this._func.Value.ExpandListPosition != -1)
      {
        if (this._userProvidedParams != null)
        {
          if (this._params == null && paramsArgs == null)
          {
            exprArgs[this._func.Value.ExpandListPosition] = (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetOrCopyParamsTuple"), (Expression) this.GetFunctionParam(), Microsoft.Scripting.Ast.Utils.Convert(this._userProvidedParams, typeof (object)));
          }
          else
          {
            this.EnsureParams();
            exprArgs[this._func.Value.ExpandListPosition] = (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeTupleFromSequence"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this._params, typeof (object)));
            if (paramsArgs != null)
              this.MakeParamsAddition(paramsArgs);
          }
        }
        else
          exprArgs[this._func.Value.ExpandListPosition] = MetaPythonFunction.FunctionBinderHelper.MakeParamsTuple(paramsArgs);
      }
      else if (paramsArgs != null)
        return false;
      return true;
    }

    private void MakeParamsAddition(List<Expression> paramsArgs)
    {
      this._extractedParams = true;
      List<Expression> expressionList = new List<Expression>(paramsArgs.Count + 1);
      expressionList.Add((Expression) this._params);
      expressionList.AddRange((IEnumerable<Expression>) paramsArgs);
      this.EnsureInit();
      this._init.Add(Microsoft.Scripting.Ast.Utils.ComplexCallHelper(typeof (PythonOps).GetMethod("AddParamsArguments"), expressionList.ToArray()));
    }

    private bool TryFinishDictionary(
      Expression[] exprArgs,
      Dictionary<string, Expression> namedArgs)
    {
      if (this._func.Value.ExpandDictPosition != -1)
      {
        if (this._dict != null)
        {
          exprArgs[this._func.Value.ExpandDictPosition] = (Expression) this._dict;
          if (namedArgs != null)
          {
            foreach (KeyValuePair<string, Expression> namedArg in namedArgs)
              this.MakeDictionaryAddition(namedArg);
          }
        }
        else
          exprArgs[this._func.Value.ExpandDictPosition] = this.MakeDictionary(namedArgs);
      }
      else if (namedArgs != null)
        return false;
      return true;
    }

    private void MakeDictionaryAddition(KeyValuePair<string, Expression> kvp)
    {
      this._init.Add((Expression) Expression.Call(typeof (PythonOps).GetMethod("AddDictionaryArgument"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Constant((object) kvp.Key), Microsoft.Scripting.Ast.Utils.Convert(kvp.Value, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert((Expression) this._dict, typeof (PythonDictionary))));
    }

    private void AddCheckForNoExtraParameters(Expression[] exprArgs)
    {
      List<Expression> expressionList = new List<Expression>(3);
      if (this._func.Value.ExpandListPosition == -1)
      {
        if (this._params != null)
          expressionList.Add((Expression) Expression.Call(typeof (PythonOps).GetMethod("CheckParamsZero"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), (Expression) this._params));
        else if (this._userProvidedParams != null)
          expressionList.Add((Expression) Expression.Call(typeof (PythonOps).GetMethod("CheckUserParamsZero"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Convert(this._userProvidedParams, typeof (object))));
      }
      if (this._func.Value.ExpandDictPosition == -1 && this._dict != null)
        expressionList.Add((Expression) Expression.Call(typeof (PythonOps).GetMethod("CheckDictionaryZero"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Convert((Expression) this._dict, typeof (IDictionary))));
      if (expressionList.Count == 0)
        return;
      if (exprArgs.Length != 0)
      {
        Expression exprArg = exprArgs[exprArgs.Length - 1];
        ParameterExpression left;
        this._temps.Add(left = Expression.Variable(exprArg.Type, "$temp"));
        expressionList.Insert(0, (Expression) Expression.Assign((Expression) left, exprArg));
        expressionList.Add((Expression) left);
        exprArgs[exprArgs.Length - 1] = (Expression) Expression.Block(expressionList.ToArray());
      }
      else
        this._paramlessCheck = (Expression) Expression.Block(expressionList.ToArray());
    }

    private Expression ValidateNotDuplicate(Expression value, string name, int position)
    {
      this.EnsureParams();
      return (Expression) Expression.Block((Expression) Expression.Call(typeof (PythonOps).GetMethod("VerifyUnduplicatedByPosition"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) name, typeof (string)), Microsoft.Scripting.Ast.Utils.Constant((object) position), (Expression) this._paramsLen), value);
    }

    private Expression ExtractNonDefaultValue(string name)
    {
      return this._userProvidedParams != null ? (this._dict != null ? this.ExtractFromListOrDictionary(name) : this.ExtractNextParamsArg()) : (this._dict != null ? this.ExtractDictionaryArgument(name) : (Expression) null);
    }

    private Expression ExtractDictionaryArgument(string name)
    {
      this._needCodeTest = true;
      return (Expression) Expression.Call(typeof (PythonOps).GetMethod(nameof (ExtractDictionaryArgument)), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) name, typeof (string)), Microsoft.Scripting.Ast.Utils.Constant((object) this.Signature.ArgumentCount), Microsoft.Scripting.Ast.Utils.Convert((Expression) this._dict, typeof (PythonDictionary)));
    }

    private Expression ExtractDefaultValue(int index, int dfltIndex)
    {
      if (this._dict == null && this._userProvidedParams == null)
        return (Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionGetDefaultValue"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Constant((object) dfltIndex));
      if (this._userProvidedParams != null)
        this.EnsureParams();
      this._extractedDefault = true;
      return (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetFunctionParameterValue"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Constant((object) dfltIndex), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) this._func.Value.ArgNames[index], typeof (string)), MetaPythonFunction.FunctionBinderHelper.VariableOrNull(this._params, typeof (List)), MetaPythonFunction.FunctionBinderHelper.VariableOrNull(this._dict, typeof (PythonDictionary)));
    }

    private Expression ExtractFromListOrDictionary(string name)
    {
      this.EnsureParams();
      this._needCodeTest = true;
      return (Expression) Expression.Call(typeof (PythonOps).GetMethod("ExtractAnyArgument"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) name, typeof (string)), (Expression) this._paramsLen, (Expression) this._params, Microsoft.Scripting.Ast.Utils.Convert((Expression) this._dict, typeof (IDictionary)));
    }

    private void EnsureParams()
    {
      if (this._extractedParams)
        return;
      this.MakeParamsCopy(this._userProvidedParams);
      this._extractedParams = true;
    }

    private Expression ExtractNextParamsArg()
    {
      if (!this._extractedParams)
      {
        this.MakeParamsCopy(this._userProvidedParams);
        this._extractedParams = true;
      }
      return (Expression) Expression.Call(typeof (PythonOps).GetMethod("ExtractParamsArgument"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Constant((object) this.Signature.ArgumentCount), (Expression) this._params);
    }

    private static Expression VariableOrNull(ParameterExpression var, Type type)
    {
      return var != null ? Microsoft.Scripting.Ast.Utils.Convert((Expression) var, type) : (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, type);
    }

    private Expression[] GetArgumentsForTargetType(Expression[] exprArgs)
    {
      if (this._func.Value.func_code.Target.GetType() == typeof (Func<PythonFunction, object[], object>))
        exprArgs = new Expression[1]
        {
          (Expression) Microsoft.Scripting.Ast.Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) exprArgs)
        };
      return exprArgs;
    }

    private UnaryExpression GetFunctionParam()
    {
      return Expression.Convert(this._func.Expression, typeof (PythonFunction));
    }

    private DynamicMetaObject MakeDictionaryCopy(DynamicMetaObject userDict)
    {
      userDict = userDict.Restrict(userDict.GetLimitType());
      this._temps.Add(this._dict = Expression.Variable(typeof (PythonDictionary), "$dict"));
      this.EnsureInit();
      this._init.Add((Expression) Expression.Assign((Expression) this._dict, (Expression) Expression.Call(typeof (PythonOps).GetMethod(!typeof (PythonDictionary).IsAssignableFrom(userDict.GetLimitType()) ? (!typeof (IDictionary).IsAssignableFrom(userDict.GetLimitType()) ? "CopyAndVerifyUserMapping" : "CopyAndVerifyDictionary") : "CopyAndVerifyPythonDictionary"), (Expression) this.GetFunctionParam(), Microsoft.Scripting.Ast.Utils.Convert(userDict.Expression, userDict.GetLimitType()))));
      return userDict;
    }

    private void MakeParamsCopy(Expression userList)
    {
      this._temps.Add(this._params = Expression.Variable(typeof (List), "$list"));
      this._temps.Add(this._paramsLen = Expression.Variable(typeof (int), "$paramsLen"));
      this.EnsureInit();
      this._init.Add((Expression) Expression.Assign((Expression) this._params, (Expression) Expression.Call(typeof (PythonOps).GetMethod("CopyAndVerifyParamsList"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), Microsoft.Scripting.Ast.Utils.Convert(userList, typeof (object)))));
      this._init.Add((Expression) Expression.Assign((Expression) this._paramsLen, (Expression) Expression.Add((Expression) Expression.Call((Expression) this._params, typeof (List).GetMethod("__len__")), Microsoft.Scripting.Ast.Utils.Constant((object) this.Signature.GetProvidedPositionalArgumentCount()))));
    }

    private Expression MakeDictionary(Dictionary<string, Expression> namedArgs)
    {
      this._temps.Add(this._dict = Expression.Variable(typeof (PythonDictionary), "$dict"));
      Expression expression1;
      if (namedArgs != null)
      {
        Expression[] expressionArray1 = new Expression[namedArgs.Count * 2];
        int num1 = 0;
        foreach (KeyValuePair<string, Expression> namedArg in namedArgs)
        {
          Expression[] expressionArray2 = expressionArray1;
          int index1 = num1;
          int num2 = index1 + 1;
          Expression expression2 = Microsoft.Scripting.Ast.Utils.Convert(namedArg.Value, typeof (object));
          expressionArray2[index1] = expression2;
          Expression[] expressionArray3 = expressionArray1;
          int index2 = num2;
          num1 = index2 + 1;
          ConstantExpression constantExpression = Microsoft.Scripting.Ast.Utils.Constant((object) namedArg.Key, typeof (object));
          expressionArray3[index2] = (Expression) constantExpression;
        }
        expression1 = (Expression) Expression.Assign((Expression) this._dict, (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeHomogeneousDictFromItems"), (Expression) Expression.NewArrayInit(typeof (object), expressionArray1)));
      }
      else
        expression1 = (Expression) Expression.Assign((Expression) this._dict, (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeDict"), Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
      return expression1;
    }

    private static Expression MakeParamsTuple(List<Expression> extraArgs)
    {
      return extraArgs != null ? Microsoft.Scripting.Ast.Utils.ComplexCallHelper(typeof (PythonOps).GetMethod("MakeTuple"), extraArgs.ToArray()) : (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeTuple"), (Expression) Expression.NewArrayInit(typeof (object[])));
    }

    private Expression MakeFunctionInvoke(Expression[] invokeArgs)
    {
      Type type = this._func.Value.func_code.Target.GetType();
      MethodInfo method = type.GetMethod("Invoke");
      invokeArgs = ArrayUtils.Insert<Expression>((Expression) this.GetFunctionParam(), invokeArgs);
      Expression expression = (Expression) Microsoft.Scripting.Ast.Utils.SimpleCallHelper((Expression) Expression.Convert((Expression) Expression.Call(this._call.SupportsLightThrow() ? typeof (PythonOps).GetMethod("FunctionGetLightThrowTarget") : typeof (PythonOps).GetMethod("FunctionGetTarget"), (Expression) this.GetFunctionParam()), type), method, invokeArgs);
      if (this._paramlessCheck != null)
        expression = (Expression) Expression.Block(this._paramlessCheck, expression);
      return expression;
    }

    private Expression AddInitialization(Expression body)
    {
      if (this._init == null)
        return body;
      return (Expression) Expression.Block((IEnumerable<Expression>) new List<Expression>((IEnumerable<Expression>) this._init)
      {
        body
      });
    }

    private void MakeUnexpectedKeywordError(Dictionary<string, Expression> namedArgs)
    {
      string str = (string) null;
      using (Dictionary<string, Expression>.KeyCollection.Enumerator enumerator = namedArgs.Keys.GetEnumerator())
      {
        if (enumerator.MoveNext())
          str = enumerator.Current;
      }
      this._error = this._call.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("UnexpectedKeywordArgumentError"), Microsoft.Scripting.Ast.Utils.Convert((Expression) this.GetFunctionParam(), typeof (PythonFunction)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) str, typeof (string))), typeof (PythonOps));
    }

    private void EnsureInit()
    {
      if (this._init != null)
        return;
      this._init = new List<Expression>();
    }
  }
}
