// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.TypeInferer
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public static class TypeInferer
{
  private static TypeInferer.ArgumentInputs EnsureInputs(
    Dictionary<Type, TypeInferer.ArgumentInputs> dict,
    Type type)
  {
    TypeInferer.ArgumentInputs argumentInputs;
    if (!dict.TryGetValue(type, out argumentInputs))
      dict[type] = argumentInputs = new TypeInferer.ArgumentInputs(type);
    return argumentInputs;
  }

  internal static MethodCandidate InferGenericMethod(
    ApplicableCandidate candidate,
    ActualArguments actualArgs)
  {
    OverloadInfo overload = candidate.Method.Overload;
    List<DynamicMetaObject> allArguments = TypeInferer.GetAllArguments(candidate, actualArgs);
    if (allArguments == null)
      return (MethodCandidate) null;
    Dictionary<Type, List<Type>> dependencyMapping = TypeInferer.GetDependencyMapping(overload);
    Type[] genericArguments = TypeInferer.GetSortedGenericArguments(overload, dependencyMapping);
    Dictionary<Type, TypeInferer.ArgumentInputs> argumentToInputMapping = TypeInferer.GetArgumentToInputMapping(candidate.Method, (IList<DynamicMetaObject>) allArguments);
    Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
    Dictionary<DynamicMetaObject, BindingRestrictions> restrictions = new Dictionary<DynamicMetaObject, BindingRestrictions>();
    bool flag = false;
    foreach (Type key in genericArguments)
    {
      TypeInferer.ArgumentInputs argumentInputs;
      if (argumentToInputMapping.TryGetValue(key, out argumentInputs) && argumentInputs.GetBestType(candidate.Method.Resolver, dictionary, restrictions) == (Type) null)
      {
        flag = true;
        break;
      }
    }
    if (flag)
      return (MethodCandidate) null;
    Type[] forInferedMethod = TypeInferer.GetGenericArgumentsForInferedMethod(overload, dictionary);
    if (forInferedMethod == null)
      return (MethodCandidate) null;
    OverloadInfo overloadInfo = overload.MakeGenericMethod(forInferedMethod);
    List<ParameterWrapper> newWrappers = TypeInferer.CreateNewWrappers(candidate.Method, overloadInfo, overload);
    List<ArgBuilder> newArgBuilders = TypeInferer.CreateNewArgBuilders(candidate.Method, overloadInfo);
    if (newArgBuilders == null)
      return (MethodCandidate) null;
    if (restrictions.Count == 0)
      restrictions = (Dictionary<DynamicMetaObject, BindingRestrictions>) null;
    return candidate.Method.ReplaceMethod(overloadInfo, newWrappers, (IList<ArgBuilder>) newArgBuilders, restrictions);
  }

  private static Type[] GetGenericArgumentsForInferedMethod(
    OverloadInfo target,
    Dictionary<Type, Type> constraints)
  {
    Type[] forInferedMethod = ArrayUtils.MakeArray<Type>((ICollection<Type>) target.GenericArguments);
    for (int index = 0; index < forInferedMethod.Length; ++index)
    {
      Type type;
      if (!constraints.TryGetValue(forInferedMethod[index], out type))
        return (Type[]) null;
      forInferedMethod[index] = type;
    }
    return forInferedMethod;
  }

  private static List<ArgBuilder> CreateNewArgBuilders(
    MethodCandidate candidate,
    OverloadInfo newOverload)
  {
    List<ArgBuilder> newArgBuilders = new List<ArgBuilder>();
    foreach (ArgBuilder argBuilder1 in (IEnumerable<ArgBuilder>) candidate.ArgBuilders)
    {
      ParameterInfo parameterInfo = argBuilder1.ParameterInfo;
      if (parameterInfo != null && (parameterInfo.ParameterType.IsGenericParameter || parameterInfo.ParameterType.ContainsGenericParameters()))
      {
        ArgBuilder argBuilder2 = argBuilder1.Clone(newOverload.Parameters[parameterInfo.Position]);
        if (argBuilder2 == null)
          return (List<ArgBuilder>) null;
        newArgBuilders.Add(argBuilder2);
      }
      else
        newArgBuilders.Add(argBuilder1);
    }
    return newArgBuilders;
  }

  private static List<ParameterWrapper> CreateNewWrappers(
    MethodCandidate candidate,
    OverloadInfo newOverload,
    OverloadInfo oldOverload)
  {
    List<ParameterWrapper> newWrappers = new List<ParameterWrapper>();
    for (int parameterIndex = 0; parameterIndex < candidate.ParameterCount; ++parameterIndex)
    {
      ParameterWrapper parameter1 = candidate.GetParameter(parameterIndex);
      ParameterInfo info = (ParameterInfo) null;
      Type type = parameter1.Type;
      if (parameter1.ParameterInfo != null)
      {
        info = newOverload.Parameters[parameter1.ParameterInfo.Position];
        ParameterInfo parameter2 = oldOverload.Parameters[parameter1.ParameterInfo.Position];
        if (parameter2.ParameterType == parameter1.Type)
          type = info.ParameterType;
        else if (info.ParameterType.IsByRef)
        {
          type = info.ParameterType.GetElementType();
          if (parameter2.ParameterType.GetElementType() != parameter1.Type)
            type = typeof (StrongBox<>).MakeGenericType(type);
        }
        else
          type = info.ParameterType.GetElementType();
      }
      newWrappers.Add(new ParameterWrapper(info, type, parameter1.Name, parameter1.Flags));
    }
    return newWrappers;
  }

  private static List<DynamicMetaObject> GetAllArguments(
    ApplicableCandidate candidate,
    ActualArguments actualArgs)
  {
    List<DynamicMetaObject> allArguments = new List<DynamicMetaObject>();
    for (int argumentIndex = 0; argumentIndex < actualArgs.Count; ++argumentIndex)
    {
      int parameter = candidate.ArgumentBinding.ArgumentToParameter(argumentIndex);
      if (parameter < actualArgs.Arguments.Count)
        allArguments.Add(actualArgs.Arguments[parameter]);
      else
        allArguments.Add(actualArgs.NamedArguments[parameter - actualArgs.Arguments.Count]);
    }
    return allArguments;
  }

  private static Type[] GetSortedGenericArguments(
    OverloadInfo info,
    Dictionary<Type, List<Type>> dependencies)
  {
    Type[] array = ArrayUtils.MakeArray<Type>((ICollection<Type>) info.GenericArguments);
    Array.Sort<Type>(array, (Comparison<Type>) ((x, y) =>
    {
      if ((object) x == (object) y)
        return 0;
      if (TypeInferer.IsDependentConstraint(dependencies, x, y))
        return 1;
      if (TypeInferer.IsDependentConstraint(dependencies, y, x))
        return -1;
      int hashCode1 = x.GetHashCode();
      int hashCode2 = y.GetHashCode();
      if (hashCode1 != hashCode2)
        return hashCode1 - hashCode2;
      return IdDispenser.GetId((object) x) - IdDispenser.GetId((object) y) <= 0L ? -1 : 1;
    }));
    return array;
  }

  private static bool IsDependentConstraint(
    Dictionary<Type, List<Type>> dependencies,
    Type x,
    Type y)
  {
    List<Type> typeList;
    if (dependencies.TryGetValue(x, out typeList))
    {
      foreach (Type x1 in typeList)
      {
        if (x1 == y || TypeInferer.IsDependentConstraint(dependencies, x1, y))
          return true;
      }
    }
    return false;
  }

  private static Dictionary<Type, List<Type>> GetDependencyMapping(OverloadInfo info)
  {
    Dictionary<Type, List<Type>> dependencies = new Dictionary<Type, List<Type>>();
    foreach (Type genericArgument in (IEnumerable<Type>) info.GenericArguments)
    {
      foreach (Type parameterConstraint in genericArgument.GetGenericParameterConstraints())
      {
        if (parameterConstraint.IsGenericParameter)
          TypeInferer.AddDependency(dependencies, genericArgument, parameterConstraint);
        else if (parameterConstraint.ContainsGenericParameters())
          TypeInferer.AddNestedDependencies(dependencies, genericArgument, parameterConstraint);
      }
    }
    return dependencies;
  }

  private static void AddNestedDependencies(
    Dictionary<Type, List<Type>> dependencies,
    Type genArg,
    Type t)
  {
    foreach (Type genericArgument in t.GetGenericArguments())
    {
      if (genericArgument.IsGenericParameter)
        TypeInferer.AddDependency(dependencies, genArg, genericArgument);
      else if (genericArgument.ContainsGenericParameters())
        TypeInferer.AddNestedDependencies(dependencies, genArg, genericArgument);
    }
  }

  private static void AddDependency(Dictionary<Type, List<Type>> dependencies, Type genArg, Type t)
  {
    List<Type> typeList;
    if (!dependencies.TryGetValue(genArg, out typeList))
      dependencies[genArg] = typeList = new List<Type>();
    typeList.Add(t);
  }

  private static Dictionary<Type, TypeInferer.ArgumentInputs> GetArgumentToInputMapping(
    MethodCandidate candidate,
    IList<DynamicMetaObject> args)
  {
    Dictionary<Type, TypeInferer.ArgumentInputs> inputs = new Dictionary<Type, TypeInferer.ArgumentInputs>();
    for (int index = 0; index < candidate.ParameterCount; ++index)
    {
      ParameterWrapper parameter = candidate.GetParameter(index);
      if (parameter.IsParamsArray)
        TypeInferer.AddOneInput(inputs, args[index], parameter.Type.GetElementType());
      else if (parameter.IsByRef)
        TypeInferer.AddOneInput(inputs, args[index], parameter.ParameterInfo.ParameterType);
      else
        TypeInferer.AddOneInput(inputs, args[index], parameter.Type);
    }
    return inputs;
  }

  private static void AddOneInput(
    Dictionary<Type, TypeInferer.ArgumentInputs> inputs,
    DynamicMetaObject arg,
    Type paramType)
  {
    if (!paramType.ContainsGenericParameters())
      return;
    List<Type> containedGenArgs = new List<Type>();
    TypeInferer.CollectGenericParameters(paramType, containedGenArgs);
    foreach (Type type in containedGenArgs)
      TypeInferer.EnsureInputs(inputs, type).AddInput(arg, paramType);
  }

  private static void CollectGenericParameters(Type type, List<Type> containedGenArgs)
  {
    if (type.IsGenericParameter)
    {
      if (containedGenArgs.Contains(type))
        return;
      containedGenArgs.Add(type);
    }
    else
    {
      if (!type.ContainsGenericParameters())
        return;
      if (type.IsArray || type.IsByRef)
      {
        TypeInferer.CollectGenericParameters(type.GetElementType(), containedGenArgs);
      }
      else
      {
        foreach (Type genericArgument in type.GetGenericArguments())
          TypeInferer.CollectGenericParameters(genericArgument, containedGenArgs);
      }
    }
  }

  internal static Type GetInferedType(
    OverloadResolver resolver,
    Type genericParameter,
    Type parameterType,
    DynamicMetaObject input,
    Dictionary<Type, Type> binding,
    Dictionary<DynamicMetaObject, BindingRestrictions> restrictions)
  {
    if (parameterType.IsSubclassOf(typeof (Delegate)) && input is IInferableInvokable inferableInvokable)
    {
      InferenceResult inferredType = inferableInvokable.GetInferredType(parameterType, genericParameter);
      if (inferredType != null)
      {
        if (inferredType.Restrictions != BindingRestrictions.Empty)
          restrictions[input] = inferredType.Restrictions;
        binding[genericParameter] = inferredType.Type;
        return TypeInferer.ConstraintsViolated(inferredType.Type, genericParameter, binding) ? (Type) null : inferredType.Type;
      }
    }
    return TypeInferer.GetInferedType(genericParameter, parameterType, resolver.GetGenericInferenceType(input), input.LimitType, binding);
  }

  public static Type GetInferedType(
    Type genericParameter,
    Type parameterType,
    Type inputType,
    Type argType,
    Dictionary<Type, Type> binding)
  {
    if (parameterType.IsGenericParameter)
    {
      if (inputType != (Type) null)
      {
        binding[genericParameter] = inputType;
        if (TypeInferer.ConstraintsViolated(inputType, genericParameter, binding))
          return (Type) null;
      }
      return inputType;
    }
    if (parameterType.IsInterface())
      return TypeInferer.GetInferedTypeForInterface(genericParameter, parameterType, inputType, binding);
    if (parameterType.IsArray)
      return binding[genericParameter] = TypeInferer.MatchGenericParameter(genericParameter, argType, parameterType, binding);
    if (parameterType.IsByRef)
    {
      if (CompilerHelpers.IsStrongBox(argType))
        argType = argType.GetGenericArguments()[0];
      return binding[genericParameter] = TypeInferer.MatchGenericParameter(genericParameter, argType, parameterType.GetElementType(), binding);
    }
    Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();
    for (; argType != typeof (object); argType = argType.GetBaseType())
    {
      if (argType.IsGenericType() && argType.GetGenericTypeDefinition() == genericTypeDefinition)
        return binding[genericParameter] = TypeInferer.MatchGenericParameter(genericParameter, argType, parameterType, binding);
    }
    return (Type) null;
  }

  private static Type GetInferedTypeForInterface(
    Type genericParameter,
    Type interfaceType,
    Type inputType,
    Dictionary<Type, Type> binding)
  {
    Type match = (Type) null;
    Type genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
    foreach (Type type in inputType.GetInterfaces())
    {
      if (type.IsGenericType() && type.GetGenericTypeDefinition() == genericTypeDefinition && !TypeInferer.MatchGenericParameter(genericParameter, type, interfaceType, binding, ref match))
        return (Type) null;
    }
    binding[genericParameter] = match;
    return match;
  }

  private static bool ConstraintsViolated(
    Type inputType,
    Type genericMethodParameterType,
    Dictionary<Type, Type> binding)
  {
    return ReflectionUtils.ConstraintsViolated(genericMethodParameterType, inputType, binding, false);
  }

  private static Type MatchGenericParameter(
    Type genericParameter,
    Type closedType,
    Type openType,
    Dictionary<Type, Type> binding)
  {
    Type match = (Type) null;
    return !TypeInferer.MatchGenericParameter(genericParameter, closedType, openType, binding, ref match) ? (Type) null : match;
  }

  private static bool MatchGenericParameter(
    Type genericParameter,
    Type closedType,
    Type openType,
    Dictionary<Type, Type> binding,
    ref Type match)
  {
    Type m = match;
    int num = ReflectionUtils.BindGenericParameters(openType, closedType, (Func<Type, Type, bool>) ((parameter, type) =>
    {
      if (parameter == genericParameter)
      {
        if (m != (Type) null)
          return m == type;
        if (TypeInferer.ConstraintsViolated(type, genericParameter, binding))
          return false;
        m = type;
      }
      return true;
    })) ? 1 : 0;
    match = m;
    return num != 0;
  }

  private class ArgumentInputs
  {
    private readonly List<Type> _parameterTypes = new List<Type>();
    private readonly List<DynamicMetaObject> _inputs = new List<DynamicMetaObject>();
    private readonly Type _genericParam;

    public ArgumentInputs(Type genericParam) => this._genericParam = genericParam;

    public void AddInput(DynamicMetaObject value, Type parameterType)
    {
      this._parameterTypes.Add(parameterType);
      this._inputs.Add(value);
    }

    public Type GetBestType(
      OverloadResolver resolver,
      Dictionary<Type, Type> binding,
      Dictionary<DynamicMetaObject, BindingRestrictions> restrictions)
    {
      Type c = (Type) null;
      for (int index = 0; index < this._parameterTypes.Count; ++index)
      {
        Type inferedType = TypeInferer.GetInferedType(resolver, this._genericParam, this._parameterTypes[index], this._inputs[index], binding, restrictions);
        if (inferedType == (Type) null)
          return (Type) null;
        if (c == (Type) null || c.IsAssignableFrom(inferedType))
        {
          c = inferedType;
        }
        else
        {
          if (!inferedType.IsAssignableFrom(c))
            return (Type) null;
          c = inferedType;
        }
      }
      return c;
    }
  }
}
