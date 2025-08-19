// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.OverloadResolver
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public abstract class OverloadResolver
{
  private string _methodName;
  private NarrowingLevel _minLevel;
  private NarrowingLevel _maxLevel;
  private IList<string> _argNames;
  private Dictionary<int, CandidateSet> _candidateSets;
  private List<MethodCandidate> _paramsCandidates;
  private ActualArguments _actualArguments;
  private int _maxAccessedCollapsedArg;
  private List<ParameterExpression> _temps;

  protected OverloadResolver(ActionBinder binder)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    this.Binder = binder;
    this._maxAccessedCollapsedArg = -1;
  }

  public ActionBinder Binder { get; }

  internal List<ParameterExpression> Temps => this._temps;

  internal ParameterExpression GetTemporary(Type type, string name)
  {
    if (this._temps == null)
      this._temps = new List<ParameterExpression>();
    ParameterExpression temporary = Expression.Variable(type, name);
    this._temps.Add(temporary);
    return temporary;
  }

  public BindingTarget ResolveOverload(
    string methodName,
    IList<MethodBase> methods,
    NarrowingLevel minLevel,
    NarrowingLevel maxLevel)
  {
    return this.ResolveOverload(methodName, (IList<OverloadInfo>) ArrayUtils.ToArray<MethodBase, ReflectionOverloadInfo>((ICollection<MethodBase>) methods, (Func<MethodBase, ReflectionOverloadInfo>) (m => new ReflectionOverloadInfo(m))), minLevel, maxLevel);
  }

  public BindingTarget ResolveOverload(
    string methodName,
    IList<OverloadInfo> methods,
    NarrowingLevel minLevel,
    NarrowingLevel maxLevel)
  {
    ContractUtils.RequiresNotNullItems<OverloadInfo>(methods, nameof (methods));
    ContractUtils.Requires(minLevel <= maxLevel);
    if (this._candidateSets != null)
      throw new InvalidOperationException("Overload resolver cannot be reused.");
    this._methodName = methodName;
    this._minLevel = minLevel;
    this._maxLevel = maxLevel;
    IList<DynamicMetaObject> namedArgs;
    this.GetNamedArguments(out namedArgs, out this._argNames);
    this.BuildCandidateSets((IEnumerable<OverloadInfo>) methods);
    int preSplatLimit;
    int postSplatLimit;
    this.GetSplatLimits(out preSplatLimit, out postSplatLimit);
    this._actualArguments = this.CreateActualArguments(namedArgs, this._argNames, preSplatLimit, postSplatLimit);
    if (this._actualArguments == null)
      return new BindingTarget(methodName, BindingResult.InvalidArguments);
    CandidateSet candidateSet = this.GetCandidateSet();
    return candidateSet != null && !candidateSet.IsParamsDictionaryOnly() ? this.MakeBindingTarget(candidateSet) : new BindingTarget(methodName, this._actualArguments.VisibleCount, this.GetExpectedArgCounts());
  }

  protected internal virtual bool AllowMemberInitialization(OverloadInfo method)
  {
    return this.AllowKeywordArgumentSetting(method.ReflectionInfo);
  }

  [Obsolete("Use OverloadInfo.AllowMemberInitialization instead")]
  protected internal virtual bool AllowKeywordArgumentSetting(MethodBase method)
  {
    return CompilerHelpers.IsConstructor(method);
  }

  protected internal virtual Expression GetByRefArrayExpression(Expression argumentArrayExpression)
  {
    return argumentArrayExpression;
  }

  protected virtual bool BindToUnexpandedParams(MethodCandidate candidate) => true;

  protected internal virtual BitArray MapSpecialParameters(ParameterMapping mapping)
  {
    if (!mapping.Overload.IsStatic)
    {
      Type declaringType = mapping.Overload.DeclaringType;
      mapping.AddParameter(new ParameterWrapper((ParameterInfo) null, declaringType, (string) null, ParameterBindingFlags.ProhibitNull));
      mapping.AddInstanceBuilder(new InstanceBuilder(mapping.ArgIndex));
    }
    return (BitArray) null;
  }

  private void BuildCandidateSets(IEnumerable<OverloadInfo> methods)
  {
    this._candidateSets = new Dictionary<int, CandidateSet>();
    foreach (OverloadInfo method in methods)
    {
      if (!OverloadResolver.IsUnsupported(method))
        this.AddBasicMethodTargets(method);
    }
    if (this._paramsCandidates == null)
      return;
    foreach (MethodCandidate paramsCandidate in this._paramsCandidates)
    {
      foreach (int key in this._candidateSets.Keys)
      {
        MethodCandidate target = paramsCandidate.MakeParamsExtended(key, this._argNames);
        if (target != null)
          this.AddTarget(target);
      }
    }
  }

  private CandidateSet GetCandidateSet()
  {
    CandidateSet candidateSet;
    if (this._actualArguments.CollapsedCount == 0 && this._candidateSets.TryGetValue(this._actualArguments.Count, out candidateSet))
      return candidateSet;
    if (this._paramsCandidates != null)
    {
      candidateSet = this.BuildExpandedTargetSet(this._actualArguments.Count);
      if (candidateSet.Candidates.Count > 0)
        return candidateSet;
    }
    return (CandidateSet) null;
  }

  private CandidateSet BuildExpandedTargetSet(int count)
  {
    CandidateSet candidateSet = new CandidateSet(count);
    if (this._paramsCandidates != null)
    {
      foreach (MethodCandidate paramsCandidate in this._paramsCandidates)
      {
        MethodCandidate target = paramsCandidate.MakeParamsExtended(count, this._argNames);
        if (target != null)
          candidateSet.Add(target);
      }
    }
    return candidateSet;
  }

  private void AddTarget(MethodCandidate target)
  {
    int parameterCount = target.ParameterCount;
    CandidateSet candidateSet;
    if (!this._candidateSets.TryGetValue(parameterCount, out candidateSet))
    {
      candidateSet = new CandidateSet(parameterCount);
      this._candidateSets[parameterCount] = candidateSet;
    }
    candidateSet.Add(target);
  }

  private void AddSimpleTarget(MethodCandidate target)
  {
    if (target.HasParamsArray || target.HasParamsDictionary)
    {
      if (this.BindToUnexpandedParams(target))
        this.AddTarget(target);
      if (this._paramsCandidates == null)
        this._paramsCandidates = new List<MethodCandidate>();
      this._paramsCandidates.Add(target);
    }
    else
      this.AddTarget(target);
  }

  private void AddBasicMethodTargets(OverloadInfo method)
  {
    ParameterMapping parameterMapping = new ParameterMapping(this, method, this._argNames);
    parameterMapping.MapParameters(false);
    foreach (MethodCandidate defaultCandidate in parameterMapping.CreateDefaultCandidates())
      this.AddSimpleTarget(defaultCandidate);
    MethodCandidate reducedCandidate = parameterMapping.CreateByRefReducedCandidate();
    if (reducedCandidate != null)
      this.AddSimpleTarget(reducedCandidate);
    this.AddSimpleTarget(parameterMapping.CreateCandidate());
  }

  private static bool IsUnsupported(OverloadInfo method)
  {
    return (method.CallingConvention & CallingConventions.VarArgs) != 0;
  }

  public ActualArguments GetActualArguments()
  {
    return this._actualArguments != null ? this._actualArguments : throw new InvalidOperationException("Actual arguments have not been built yet.");
  }

  protected virtual void GetNamedArguments(
    out IList<DynamicMetaObject> namedArgs,
    out IList<string> argNames)
  {
    argNames = (IList<string>) ArrayUtils.EmptyStrings;
    namedArgs = (IList<DynamicMetaObject>) DynamicMetaObject.EmptyMetaObjects;
  }

  protected abstract ActualArguments CreateActualArguments(
    IList<DynamicMetaObject> namedArgs,
    IList<string> argNames,
    int preSplatLimit,
    int postSplatLimit);

  internal BindingTarget MakeBindingTarget(CandidateSet targetSet)
  {
    List<CallFailure> failures1 = (List<CallFailure>) null;
    List<CallFailure> failures2 = (List<CallFailure>) null;
    List<ApplicableCandidate> applicableCandidateList1 = this.EnsureMatchingNamedArgs(targetSet.Candidates, ref failures2);
    if (applicableCandidateList1.Count == 0)
      return this.MakeFailedBindingTarget(failures2.ToArray());
    for (NarrowingLevel minLevel = this._minLevel; minLevel <= this._maxLevel; ++minLevel)
    {
      failures1?.Clear();
      List<ApplicableCandidate> candidates = this.SelectCandidatesWithConvertibleArgs(applicableCandidateList1, minLevel, ref failures1);
      if (candidates.Count != 0)
      {
        if (candidates.Count == 1)
          return this.MakeSuccessfulBindingTarget(candidates[0], applicableCandidateList1, minLevel, targetSet);
        List<ApplicableCandidate> applicableCandidateList2 = this.SelectCandidatesWithConvertibleCollapsedArgs(candidates, minLevel, ref failures1);
        if (applicableCandidateList2.Count != 0)
        {
          if (applicableCandidateList2.Count == 1)
            return this.MakeSuccessfulBindingTarget(applicableCandidateList2[0], applicableCandidateList1, minLevel, targetSet);
          ApplicableCandidate result = this.SelectBestCandidate(applicableCandidateList2, minLevel);
          return result != null ? this.MakeSuccessfulBindingTarget(result, applicableCandidateList1, minLevel, targetSet) : this.MakeAmbiguousBindingTarget(applicableCandidateList2);
        }
      }
    }
    if (failures1 == null)
      return new BindingTarget(this._methodName, BindingResult.NoCallableMethod);
    if (failures2 != null)
      failures1.AddRange((IEnumerable<CallFailure>) failures2);
    return this.MakeFailedBindingTarget(failures1.ToArray());
  }

  private List<ApplicableCandidate> EnsureMatchingNamedArgs(
    List<MethodCandidate> candidates,
    ref List<CallFailure> failures)
  {
    List<ApplicableCandidate> applicableCandidateList = new List<ApplicableCandidate>();
    foreach (MethodCandidate candidate in candidates)
    {
      if (!candidate.HasParamsDictionary)
      {
        ArgumentBinding binding;
        CallFailure failure;
        if (this._actualArguments.TryBindNamedArguments(candidate, out binding, out failure))
          applicableCandidateList.Add(new ApplicableCandidate(candidate, binding));
        else
          OverloadResolver.AddFailure(ref failures, failure);
      }
    }
    return applicableCandidateList;
  }

  private List<ApplicableCandidate> SelectCandidatesWithConvertibleArgs(
    List<ApplicableCandidate> candidates,
    NarrowingLevel level,
    ref List<CallFailure> failures)
  {
    bool flag = false;
    List<ApplicableCandidate> applicableCandidateList = new List<ApplicableCandidate>();
    foreach (ApplicableCandidate candidate in candidates)
    {
      if (candidate.Method.Overload.ContainsGenericParameters)
      {
        flag = true;
      }
      else
      {
        CallFailure failure;
        if (this.TryConvertArguments(candidate.Method, candidate.ArgumentBinding, level, out failure))
          applicableCandidateList.Add(candidate);
        else
          OverloadResolver.AddFailure(ref failures, failure);
      }
    }
    if (flag)
    {
      foreach (ApplicableCandidate candidate in candidates)
      {
        if (candidate.Method.Overload.IsGenericMethodDefinition)
        {
          MethodCandidate methodCandidate = TypeInferer.InferGenericMethod(candidate, this._actualArguments);
          if (methodCandidate != null)
          {
            CallFailure failure;
            if (this.TryConvertArguments(methodCandidate, candidate.ArgumentBinding, level, out failure))
              applicableCandidateList.Add(new ApplicableCandidate(methodCandidate, candidate.ArgumentBinding));
            else
              OverloadResolver.AddFailure(ref failures, failure);
          }
          else
            OverloadResolver.AddFailure(ref failures, new CallFailure(candidate.Method, CallFailureReason.TypeInference));
        }
      }
    }
    return applicableCandidateList;
  }

  private List<ApplicableCandidate> SelectCandidatesWithConvertibleCollapsedArgs(
    List<ApplicableCandidate> candidates,
    NarrowingLevel level,
    ref List<CallFailure> failures)
  {
    if (this._actualArguments.CollapsedCount == 0)
      return candidates;
    List<ApplicableCandidate> applicableCandidateList = new List<ApplicableCandidate>();
    foreach (ApplicableCandidate candidate in candidates)
    {
      CallFailure failure;
      if (this.TryConvertCollapsedArguments(candidate.Method, level, out failure))
        applicableCandidateList.Add(candidate);
      else
        OverloadResolver.AddFailure(ref failures, failure);
    }
    return applicableCandidateList;
  }

  private static void AddFailure(ref List<CallFailure> failures, CallFailure failure)
  {
    if (failures == null)
      failures = new List<CallFailure>(1);
    failures.Add(failure);
  }

  private bool TryConvertArguments(
    MethodCandidate candidate,
    ArgumentBinding namesBinding,
    NarrowingLevel narrowingLevel,
    out CallFailure failure)
  {
    BitArray bitArray = new BitArray(this._actualArguments.Count);
    bool flag = true;
    for (int index = 0; index < this._actualArguments.Count; ++index)
      flag &= bitArray[index] = this.CanConvertFrom(this._actualArguments[index].GetLimitType(), this._actualArguments[index], candidate.GetParameter(index, namesBinding), narrowingLevel);
    if (!flag)
    {
      ConversionResult[] results = new ConversionResult[this._actualArguments.Count];
      for (int index = 0; index < this._actualArguments.Count; ++index)
        results[index] = new ConversionResult(this._actualArguments[index].Value, this._actualArguments[index].GetLimitType(), candidate.GetParameter(index, namesBinding).Type, !bitArray[index]);
      failure = new CallFailure(candidate, results);
    }
    else
      failure = (CallFailure) null;
    return flag;
  }

  private bool TryConvertCollapsedArguments(
    MethodCandidate candidate,
    NarrowingLevel narrowingLevel,
    out CallFailure failure)
  {
    ParameterWrapper parameter = candidate.GetParameter(this._actualArguments.SplatIndex - 1);
    for (int collapsedArgIndex = 0; collapsedArgIndex < this._actualArguments.CollapsedCount; ++collapsedArgIndex)
    {
      object collapsedArgumentValue = this.GetCollapsedArgumentValue(collapsedArgIndex);
      Type type = CompilerHelpers.GetType(collapsedArgumentValue);
      if (!this.CanConvertFrom(type, (DynamicMetaObject) null, parameter, narrowingLevel))
      {
        failure = new CallFailure(candidate, new ConversionResult[1]
        {
          new ConversionResult(collapsedArgumentValue, type, parameter.Type, false)
        });
        return false;
      }
    }
    failure = (CallFailure) null;
    return true;
  }

  private RestrictedArguments GetRestrictedArgs(
    ApplicableCandidate selectedCandidate,
    IList<ApplicableCandidate> candidates,
    int targetSetSize)
  {
    int count = this._actualArguments.Count;
    DynamicMetaObject[] objects = new DynamicMetaObject[count];
    Type[] types = new Type[count];
    bool hasUntypedRestrictions = false;
    for (int index = 0; index < count; ++index)
    {
      DynamicMetaObject actualArgument = this._actualArguments[index];
      if (targetSetSize > 0 && OverloadResolver.IsOverloadedOnParameter(index, count, candidates) || !selectedCandidate.GetParameter(index).Type.IsAssignableFrom(actualArgument.Expression.Type))
      {
        objects[index] = this.RestrictArgument(actualArgument, selectedCandidate.GetParameter(index));
        types[index] = actualArgument.GetLimitType();
      }
      else
        objects[index] = actualArgument;
      BindingRestrictions restrictions;
      if (selectedCandidate.Method.Restrictions != null && selectedCandidate.Method.Restrictions.TryGetValue(actualArgument, out restrictions))
      {
        hasUntypedRestrictions = true;
        objects[index] = new DynamicMetaObject(objects[index].Expression, objects[index].Restrictions.Merge(restrictions));
      }
    }
    return new RestrictedArguments(objects, types, hasUntypedRestrictions);
  }

  private DynamicMetaObject RestrictArgument(DynamicMetaObject arg, ParameterWrapper parameter)
  {
    return parameter.Type == typeof (object) ? new DynamicMetaObject(arg.Expression, BindingRestrictionsHelpers.GetRuntimeTypeRestriction(arg.Expression, arg.GetLimitType())) : arg.Restrict(arg.GetLimitType());
  }

  private static bool IsOverloadedOnParameter(
    int argIndex,
    int argCount,
    IList<ApplicableCandidate> overloads)
  {
    Type type1 = (Type) null;
    foreach (ApplicableCandidate overload in (IEnumerable<ApplicableCandidate>) overloads)
    {
      int parameterCount = overload.Method.ParameterCount;
      if (parameterCount != 0)
      {
        ParameterWrapper parameter1 = overload.Method.GetParameter(parameterCount - 1);
        Type type2;
        if (argIndex < parameterCount)
        {
          ParameterWrapper parameter2 = overload.GetParameter(argIndex);
          if (parameter2.IsParamsArray)
          {
            if (parameterCount == argCount)
              return true;
            type2 = parameter1.Type.GetElementType();
          }
          else
          {
            if (parameter2.Type.ContainsGenericParameters())
              return true;
            type2 = parameter2.Type;
          }
        }
        else if (parameter1.IsParamsArray)
          type2 = parameter1.Type.GetElementType();
        else
          continue;
        if (type1 == (Type) null)
          type1 = type2;
        else if (type1 != type2)
          return true;
      }
    }
    return false;
  }

  private bool IsBest(
    ApplicableCandidate candidate,
    List<ApplicableCandidate> candidates,
    NarrowingLevel level)
  {
    foreach (ApplicableCandidate candidate1 in candidates)
    {
      if (candidate != candidate1 && this.GetPreferredCandidate(candidate, candidate1, level) != Candidate.One)
        return false;
    }
    return true;
  }

  internal Candidate GetPreferredCandidate(
    ApplicableCandidate one,
    ApplicableCandidate two,
    NarrowingLevel level)
  {
    Candidate preferredParameters = this.GetPreferredParameters(one, two, level);
    return preferredParameters.Chosen() ? preferredParameters : this.CompareEquivalentCandidates(one, two);
  }

  protected internal virtual Candidate CompareEquivalentCandidates(
    ApplicableCandidate one,
    ApplicableCandidate two)
  {
    Candidate candidate = this.CompareEquivalentParameters(one.Method, two.Method);
    return candidate.Chosen() ? candidate : Candidate.Equivalent;
  }

  internal Candidate CompareEquivalentParameters(MethodCandidate one, MethodCandidate two)
  {
    if (two.Overload.IsPrivate && !one.Overload.IsPrivate)
      return Candidate.One;
    if (one.Overload.IsPrivate && !two.Overload.IsPrivate)
      return Candidate.Two;
    if (one.Overload.IsGenericMethod)
      return !two.Overload.IsGenericMethod ? Candidate.Two : Candidate.Equivalent;
    if (two.Overload.IsGenericMethod)
      return Candidate.One;
    switch (OverloadResolver.Compare(one.ReturnBuilder.CountOutParams, two.ReturnBuilder.CountOutParams))
    {
      case -1:
        return Candidate.One;
      case 1:
        return Candidate.Two;
      default:
        int maxPriority1;
        for (int ceiling = int.MaxValue; ceiling >= 0; ceiling = maxPriority1 - 1)
        {
          maxPriority1 = OverloadResolver.FindMaxPriority(one.ArgBuilders, ceiling);
          int maxPriority2 = OverloadResolver.FindMaxPriority(two.ArgBuilders, ceiling);
          if (maxPriority1 < maxPriority2)
            return Candidate.One;
          if (maxPriority2 < maxPriority1)
            return Candidate.Two;
        }
        if (one.Overload.Name != two.Overload.Name)
        {
          if (one.Overload.Name == this._methodName)
            return Candidate.One;
          if (two.Overload.Name == this._methodName)
            return Candidate.Two;
        }
        if (one.Overload.IsExtension == two.Overload.IsExtension)
          return Candidate.Equivalent;
        return !one.Overload.IsExtension ? Candidate.One : Candidate.Two;
    }
  }

  private static int Compare(int x, int y)
  {
    if (x < y)
      return -1;
    return x > y ? 1 : 0;
  }

  private static int FindMaxPriority(IList<ArgBuilder> abs, int ceiling)
  {
    int val1 = 0;
    foreach (ArgBuilder ab in (IEnumerable<ArgBuilder>) abs)
    {
      if (ab.Priority <= ceiling)
        val1 = Math.Max(val1, ab.Priority);
    }
    return val1;
  }

  private Candidate GetPreferredParameters(
    ApplicableCandidate one,
    ApplicableCandidate two,
    NarrowingLevel level)
  {
    ActualArguments actualArguments = this.GetActualArguments();
    Candidate preferredParameters = Candidate.Equivalent;
    for (int index = 0; index < actualArguments.Count; ++index)
    {
      Candidate preferredParameter = this.GetPreferredParameter(one.GetParameter(index), two.GetParameter(index), actualArguments[index], level);
      switch (preferredParameters)
      {
        case Candidate.Two:
          if (preferredParameter == Candidate.One)
            return Candidate.Ambiguous;
          break;
        case Candidate.Equivalent:
          preferredParameters = preferredParameter;
          break;
        case Candidate.One:
          if (preferredParameter == Candidate.Two)
            return Candidate.Ambiguous;
          break;
        case Candidate.Ambiguous:
          if (preferredParameter != Candidate.Equivalent)
          {
            preferredParameters = preferredParameter;
            break;
          }
          break;
        default:
          throw new InvalidOperationException();
      }
    }
    return preferredParameters;
  }

  private Candidate GetPreferredParameter(
    ParameterWrapper candidateOne,
    ParameterWrapper candidateTwo,
    DynamicMetaObject arg,
    NarrowingLevel level)
  {
    if (this.ParametersEquivalent(candidateOne, candidateTwo))
      return Candidate.Equivalent;
    Candidate candidate1 = this.SelectBestConversionFor(arg, candidateOne, candidateTwo, level);
    if (candidate1.Chosen())
      return candidate1;
    if (this.CanConvertFrom(candidateTwo, candidateOne))
      return this.CanConvertFrom(candidateOne, candidateTwo) ? Candidate.Ambiguous : Candidate.Two;
    if (this.CanConvertFrom(candidateOne, candidateTwo))
      return Candidate.One;
    Type type1 = candidateOne.Type;
    Type type2 = candidateTwo.Type;
    Candidate candidate2 = this.PreferConvert(type1, type2);
    if (candidate2.Chosen())
      return candidate2;
    Candidate candidate3 = this.PreferConvert(type2, type1).TheOther();
    if (candidate3.Chosen())
      return candidate3;
    Type limitType = arg.GetLimitType();
    NarrowingLevel level1 = NarrowingLevel.None;
    while (level1 < level && !this.CanConvertFrom(limitType, arg, candidateOne, level1) && level1 != NarrowingLevel.All)
      ++level1;
    NarrowingLevel level2 = NarrowingLevel.None;
    while (level2 < level && !this.CanConvertFrom(limitType, arg, candidateTwo, level2) && level2 != NarrowingLevel.All)
      ++level2;
    if (level1 < level2)
      return Candidate.One;
    return level1 > level2 ? Candidate.Two : Candidate.Ambiguous;
  }

  private ApplicableCandidate SelectBestCandidate(
    List<ApplicableCandidate> candidates,
    NarrowingLevel level)
  {
    foreach (ApplicableCandidate candidate in candidates)
    {
      if (this.IsBest(candidate, candidates, level))
        return candidate;
    }
    return (ApplicableCandidate) null;
  }

  private BindingTarget MakeSuccessfulBindingTarget(
    ApplicableCandidate result,
    List<ApplicableCandidate> potentialCandidates,
    NarrowingLevel level,
    CandidateSet targetSet)
  {
    return new BindingTarget(this._methodName, this._actualArguments.VisibleCount, result.Method, level, this.GetRestrictedArgs(result, (IList<ApplicableCandidate>) potentialCandidates, targetSet.Arity));
  }

  private BindingTarget MakeFailedBindingTarget(CallFailure[] failures)
  {
    return new BindingTarget(this._methodName, this._actualArguments.VisibleCount, failures);
  }

  private BindingTarget MakeAmbiguousBindingTarget(List<ApplicableCandidate> result)
  {
    MethodCandidate[] ambiguousMatches = new MethodCandidate[result.Count];
    for (int index = 0; index < result.Count; ++index)
      ambiguousMatches[index] = result[index].Method;
    return new BindingTarget(this._methodName, this._actualArguments.VisibleCount, ambiguousMatches);
  }

  public virtual bool ParametersEquivalent(ParameterWrapper parameter1, ParameterWrapper parameter2)
  {
    return parameter1.Type == parameter2.Type && parameter1.ProhibitNull == parameter2.ProhibitNull;
  }

  public virtual bool CanConvertFrom(ParameterWrapper parameter1, ParameterWrapper parameter2)
  {
    return this.CanConvertFrom(parameter1.Type, (DynamicMetaObject) null, parameter2, NarrowingLevel.None);
  }

  public virtual bool CanConvertFrom(
    Type fromType,
    DynamicMetaObject fromArgument,
    ParameterWrapper toParameter,
    NarrowingLevel level)
  {
    Type type = toParameter.Type;
    if (fromType == typeof (DynamicNull))
    {
      if (toParameter.ProhibitNull)
        return false;
      if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Nullable<>) || !type.IsValueType())
        return true;
    }
    return fromType == type || this.Binder.CanConvertFrom(fromType, type, toParameter.ProhibitNull, level);
  }

  public virtual Candidate SelectBestConversionFor(
    DynamicMetaObject arg,
    ParameterWrapper candidateOne,
    ParameterWrapper candidateTwo,
    NarrowingLevel level)
  {
    return Candidate.Equivalent;
  }

  public virtual Candidate PreferConvert(Type t1, Type t2) => this.Binder.PreferConvert(t1, t2);

  public virtual Expression Convert(
    DynamicMetaObject metaObject,
    Type restrictedType,
    ParameterInfo info,
    Type toType)
  {
    return this.Binder.ConvertExpression(metaObject.Expression, toType, ConversionResultKind.ExplicitCast, (OverloadResolverFactory) null);
  }

  public virtual Expression GetDynamicConversion(Expression value, Type type)
  {
    return (Expression) Expression.Convert(value, type);
  }

  private int[] GetExpectedArgCounts()
  {
    if (this._candidateSets.Count == 0 && this._paramsCandidates == null)
      return new int[0];
    int num = int.MaxValue;
    if (this._paramsCandidates != null)
    {
      foreach (MethodCandidate paramsCandidate in this._paramsCandidates)
      {
        if (paramsCandidate.HasParamsArray)
          num = Math.Min(num, paramsCandidate.GetVisibleParameterCount() - 1);
      }
    }
    List<int> intList = new List<int>();
    if (this._candidateSets.Count > 0)
    {
      BitArray bitArray = new BitArray(Math.Min(this._candidateSets.Keys.Max(), num) + 1);
      foreach (CandidateSet candidateSet in this._candidateSets.Values)
      {
        foreach (MethodCandidate candidate in candidateSet.Candidates)
        {
          if (!candidate.HasParamsArray)
          {
            int visibleParameterCount = candidate.GetVisibleParameterCount();
            if (visibleParameterCount < bitArray.Length)
              bitArray[visibleParameterCount] = true;
          }
        }
      }
      for (int index = 0; index < bitArray.Length; ++index)
      {
        if (bitArray[index] || index == num)
          intList.Add(index);
      }
    }
    else if (num < int.MaxValue)
      intList.Add(num);
    if (num < int.MaxValue)
      intList.Add(int.MaxValue);
    return intList.ToArray();
  }

  public virtual ErrorInfo MakeInvalidParametersError(BindingTarget target)
  {
    switch (target.Result)
    {
      case BindingResult.AmbiguousMatch:
        return this.MakeAmbiguousCallError(target);
      case BindingResult.IncorrectArgumentCount:
        return OverloadResolver.MakeIncorrectArgumentCountError(target);
      case BindingResult.CallFailure:
        return this.MakeCallFailureError(target);
      case BindingResult.InvalidArguments:
        return this.MakeInvalidArgumentsError();
      case BindingResult.NoCallableMethod:
        return this.MakeNoCallableMethodError();
      default:
        throw new InvalidOperationException();
    }
  }

  private static ErrorInfo MakeIncorrectArgumentCountError(BindingTarget target)
  {
    int val1_1 = int.MaxValue;
    int val1_2 = int.MinValue;
    foreach (int val2 in (IEnumerable<int>) target.ExpectedArgumentCount)
    {
      val1_1 = Math.Min(val1_1, val2);
      val1_2 = Math.Max(val1_2, val2);
    }
    return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("TypeErrorForIncorrectArgumentCount", new Type[7]
    {
      typeof (string),
      typeof (int),
      typeof (int),
      typeof (int),
      typeof (int),
      typeof (bool),
      typeof (bool)
    }), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) target.Name, typeof (string)), Microsoft.Scripting.Ast.Utils.Constant((object) val1_1), Microsoft.Scripting.Ast.Utils.Constant((object) val1_2), Microsoft.Scripting.Ast.Utils.Constant((object) 0), Microsoft.Scripting.Ast.Utils.Constant((object) target.ActualArgumentCount), Microsoft.Scripting.Ast.Utils.Constant((object) false), Microsoft.Scripting.Ast.Utils.Constant((object) false)));
  }

  private ErrorInfo MakeAmbiguousCallError(BindingTarget target)
  {
    StringBuilder stringBuilder = new StringBuilder("Multiple targets could match: ");
    string str1 = "";
    foreach (MethodCandidate ambiguousMatch in target.AmbiguousMatches)
    {
      Type[] parameterTypes = ambiguousMatch.GetParameterTypes();
      string str2 = "";
      stringBuilder.Append(str1);
      stringBuilder.Append(target.Name);
      stringBuilder.Append('(');
      foreach (Type t in parameterTypes)
      {
        stringBuilder.Append(str2);
        stringBuilder.Append(this.Binder.GetTypeName(t));
        str2 = ", ";
      }
      stringBuilder.Append(')');
      str1 = ", ";
    }
    return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("SimpleTypeError"), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) stringBuilder.ToString(), typeof (string))));
  }

  private ErrorInfo MakeCallFailureError(BindingTarget target)
  {
    foreach (CallFailure callFailure in (IEnumerable<CallFailure>) target.CallFailures)
    {
      switch (callFailure.Reason)
      {
        case CallFailureReason.ConversionFailure:
          using (IEnumerator<ConversionResult> enumerator = callFailure.ConversionResults.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ConversionResult current = enumerator.Current;
              if (current.Failed)
                return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("SimpleTypeError"), Microsoft.Scripting.Ast.Utils.Constant((object) $"expected {this.Binder.GetTypeName(current.To)}, got {current.GetArgumentTypeName(this.Binder)}")));
            }
            continue;
          }
        case CallFailureReason.UnassignableKeyword:
          return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("TypeErrorForExtraKeywordArgument"), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) target.Name, typeof (string)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) callFailure.KeywordArguments[0], typeof (string))));
        case CallFailureReason.DuplicateKeyword:
          return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("TypeErrorForDuplicateKeywordArgument"), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) target.Name, typeof (string)), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) callFailure.KeywordArguments[0], typeof (string))));
        case CallFailureReason.TypeInference:
          return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("TypeErrorForNonInferrableMethod"), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) target.Name, typeof (string))));
        default:
          throw new InvalidOperationException();
      }
    }
    throw new InvalidOperationException();
  }

  private ErrorInfo MakeInvalidArgumentsError()
  {
    return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("SimpleTypeError"), Microsoft.Scripting.Ast.Utils.Constant((object) "Invalid arguments.")));
  }

  private ErrorInfo MakeNoCallableMethodError()
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (InvalidOperationException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) "No callable method.")));
  }

  private void GetSplatLimits(out int preSplatLimit, out int postSplatLimit)
  {
    if (this._paramsCandidates != null)
    {
      int val1_1 = -1;
      int val1_2 = -1;
      foreach (MethodCandidate paramsCandidate in this._paramsCandidates)
      {
        val1_1 = Math.Max(val1_1, paramsCandidate.ParamsArrayIndex);
        val1_2 = Math.Max(val1_2, paramsCandidate.ParameterCount - paramsCandidate.ParamsArrayIndex - 1);
      }
      int num = this._candidateSets.Keys.Max();
      if (val1_1 + val1_2 < num)
        val1_1 = num - val1_2;
      preSplatLimit = val1_1 + 1;
      postSplatLimit = val1_2;
    }
    else
    {
      postSplatLimit = int.MaxValue;
      preSplatLimit = int.MaxValue;
    }
  }

  internal Expression GetSplattedItemExpression(Expression indexExpression)
  {
    return (Expression) Expression.Call(this.GetSplattedExpression(), typeof (IList).GetMethod("get_Item"), indexExpression);
  }

  protected abstract Expression GetSplattedExpression();

  protected abstract object GetSplattedItem(int index);

  internal object GetCollapsedArgumentValue(int collapsedArgIndex)
  {
    object splattedItem = this.GetSplattedItem(this._actualArguments.ToSplattedItemIndex(collapsedArgIndex));
    this._maxAccessedCollapsedArg = Math.Max(this._maxAccessedCollapsedArg, collapsedArgIndex);
    return splattedItem;
  }

  public int MaxAccessedCollapsedArg => this._maxAccessedCollapsedArg;

  internal Type[] GetAccessedCollapsedArgTypes()
  {
    Type[] collapsedArgTypes = new Type[this._maxAccessedCollapsedArg + 1];
    for (int collapsedArgIndex = 0; collapsedArgIndex < collapsedArgTypes.Length; ++collapsedArgIndex)
    {
      object splattedItem = this.GetSplattedItem(this._actualArguments.ToSplattedItemIndex(collapsedArgIndex));
      collapsedArgTypes[collapsedArgIndex] = splattedItem?.GetType();
    }
    return collapsedArgTypes;
  }

  public Expression GetCollapsedArgsCondition()
  {
    if (this._maxAccessedCollapsedArg < 0)
      return (Expression) null;
    Type[] collapsedArgTypes = this.GetAccessedCollapsedArgTypes();
    return (Expression) Expression.Call((Expression) null, typeof (CompilerHelpers).GetMethod("TypesEqual"), this.GetSplattedExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) this._actualArguments.ToSplattedItemIndex(0)), (Expression) Expression.Constant((object) collapsedArgTypes));
  }

  public virtual Type GetGenericInferenceType(DynamicMetaObject dynamicObject)
  {
    return dynamicObject.LimitType;
  }

  public override string ToString()
  {
    string str = string.Empty;
    foreach (CandidateSet candidateSet in this._candidateSets.Values)
      str = str + (object) candidateSet + Environment.NewLine;
    return str;
  }
}
