// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ParameterMapping
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class ParameterMapping
{
  private readonly OverloadResolver _resolver;
  private readonly IList<string> _argNames;
  private readonly List<ParameterWrapper> _parameters;
  private readonly List<ArgBuilder> _arguments;
  private List<int> _returnArgs;
  private InstanceBuilder _instanceBuilder;
  private ReturnBuilder _returnBuilder;
  private readonly List<ArgBuilder> _defaultArguments;
  private bool _hasByRef;
  private bool _hasDefaults;
  private ParameterWrapper _paramsDict;

  public OverloadInfo Overload { get; }

  public int ArgIndex { get; private set; }

  [Obsolete("Use Overload.ReflectionInfo instead")]
  public MethodBase Method => this.Overload.ReflectionInfo;

  [Obsolete("Use Overload.Parameters instead")]
  public ParameterInfo[] ParameterInfos
  {
    get
    {
      return ArrayUtils.MakeArray<ParameterInfo>((ICollection<ParameterInfo>) this.Overload.Parameters);
    }
  }

  internal ParameterMapping(OverloadResolver resolver, OverloadInfo method, IList<string> argNames)
  {
    this._resolver = resolver;
    this.Overload = method;
    this._argNames = argNames;
    this._parameters = new List<ParameterWrapper>();
    this._arguments = new List<ArgBuilder>(method.ParameterCount);
    this._defaultArguments = new List<ArgBuilder>();
  }

  internal void MapParameters(bool reduceByRef)
  {
    if (reduceByRef)
    {
      this._returnArgs = new List<int>();
      if (this.Overload.ReturnType != typeof (void))
        this._returnArgs.Add(-1);
    }
    BitArray specialParameters = this._resolver.MapSpecialParameters(this);
    if (this._instanceBuilder == null)
      this._instanceBuilder = new InstanceBuilder(-1);
    foreach (ParameterInfo parameter in (IEnumerable<ParameterInfo>) this.Overload.Parameters)
    {
      if (!this.IsSpecialParameter(specialParameters, parameter.Position))
      {
        if (reduceByRef)
          this.MapParameterReduceByRef(parameter);
        else
          this.MapParameter(parameter);
      }
    }
    this._returnBuilder = this.MakeReturnBuilder(specialParameters);
  }

  private bool IsSpecialParameter(BitArray specialParameters, int infoIndex)
  {
    return specialParameters != null && infoIndex < specialParameters.Length && specialParameters[infoIndex];
  }

  public void AddInstanceBuilder(InstanceBuilder builder)
  {
    ContractUtils.Requires(this._instanceBuilder == null);
    ContractUtils.Requires(builder.HasValue);
    this._instanceBuilder = builder;
    this.ArgIndex += builder.ConsumedArgumentCount;
  }

  public void AddBuilder(ArgBuilder builder)
  {
    ContractUtils.Requires(builder.ConsumedArgumentCount != -1);
    this._arguments.Add(builder);
    this.ArgIndex += builder.ConsumedArgumentCount;
  }

  public void AddParameter(ParameterWrapper parameter) => this._parameters.Add(parameter);

  public void MapParameter(ParameterInfo pi)
  {
    int kwArgIndex = this._argNames.IndexOf(pi.Name);
    int index = kwArgIndex != -1 ? 0 : this.ArgIndex++;
    if (!pi.IsMandatory())
    {
      if (kwArgIndex == -1 || !this._hasDefaults)
      {
        this._defaultArguments.Add((ArgBuilder) new DefaultArgBuilder(pi));
        this._hasDefaults = true;
      }
      else
        this._defaultArguments.Add((ArgBuilder) null);
    }
    else if (this._defaultArguments.Count > 0)
      this._defaultArguments.Add((ArgBuilder) null);
    ArgBuilder builder;
    if (pi.ParameterType.IsByRef)
    {
      this._hasByRef = true;
      Type elementType = pi.ParameterType.GetElementType();
      Type type = typeof (StrongBox<>).MakeGenericType(elementType);
      this._parameters.Add(new ParameterWrapper(pi, type, pi.Name, ParameterBindingFlags.ProhibitNull));
      builder = (ArgBuilder) new ReferenceArgBuilder(pi, elementType, type, index);
    }
    else if (pi.Position == 0 && this.Overload.IsExtension)
    {
      this._parameters.Add(new ParameterWrapper(pi, pi.ParameterType, pi.Name, ParameterBindingFlags.IsHidden));
      builder = (ArgBuilder) new SimpleArgBuilder(pi, pi.ParameterType, index, false, false);
    }
    else
      builder = (ArgBuilder) this.AddSimpleParameterMapping(pi, index);
    if (kwArgIndex == -1)
      this._arguments.Add(builder);
    else
      this._arguments.Add((ArgBuilder) new KeywordArgBuilder(builder, this._argNames.Count, kwArgIndex));
  }

  private void MapParameterReduceByRef(ParameterInfo pi)
  {
    int index = 0;
    int kwArgIndex = -1;
    if (!pi.IsOutParameter())
    {
      kwArgIndex = this._argNames.IndexOf(pi.Name);
      if (kwArgIndex == -1)
        index = this.ArgIndex++;
    }
    ArgBuilder builder;
    if (pi.IsOutParameter())
    {
      this._returnArgs.Add(this._arguments.Count);
      builder = (ArgBuilder) new OutArgBuilder(pi);
    }
    else if (pi.ParameterType.IsByRef)
    {
      if ((pi.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) != ParameterAttributes.In)
        this._returnArgs.Add(this._arguments.Count);
      this._parameters.Add(new ParameterWrapper(pi, pi.ParameterType.GetElementType(), pi.Name, ParameterBindingFlags.None));
      builder = (ArgBuilder) new ReturnReferenceArgBuilder(pi, index);
    }
    else
      builder = (ArgBuilder) this.AddSimpleParameterMapping(pi, index);
    if (kwArgIndex == -1)
      this._arguments.Add(builder);
    else
      this._arguments.Add((ArgBuilder) new KeywordArgBuilder(builder, this._argNames.Count, kwArgIndex));
  }

  private ParameterWrapper CreateParameterWrapper(ParameterInfo info)
  {
    bool flag1 = this.Overload.IsParamArray(info.Position);
    bool flag2 = !flag1 && this.Overload.IsParamDictionary(info.Position);
    bool flag3 = flag1 | flag2 && this.Overload.ProhibitsNullItems(info.Position);
    return new ParameterWrapper(info, info.ParameterType, info.Name, (ParameterBindingFlags) ((this.Overload.ProhibitsNull(info.Position) ? 1 : 0) | (flag3 ? 2 : 0) | (flag1 ? 4 : 0) | (flag2 ? 8 : 0)));
  }

  private SimpleArgBuilder AddSimpleParameterMapping(ParameterInfo info, int index)
  {
    ParameterWrapper parameterWrapper = this.CreateParameterWrapper(info);
    if (parameterWrapper.IsParamsDict)
      this._paramsDict = parameterWrapper;
    else
      this._parameters.Add(parameterWrapper);
    return new SimpleArgBuilder(info, info.ParameterType, index, parameterWrapper.IsParamsArray, parameterWrapper.IsParamsDict);
  }

  internal MethodCandidate CreateCandidate()
  {
    return new MethodCandidate(this._resolver, this.Overload, this._parameters, this._paramsDict, this._returnBuilder, this._instanceBuilder, (IList<ArgBuilder>) this._arguments, (Dictionary<DynamicMetaObject, BindingRestrictions>) null);
  }

  internal MethodCandidate CreateByRefReducedCandidate()
  {
    if (!this._hasByRef)
      return (MethodCandidate) null;
    ParameterMapping parameterMapping = new ParameterMapping(this._resolver, this.Overload, this._argNames);
    parameterMapping.MapParameters(true);
    return parameterMapping.CreateCandidate();
  }

  internal IEnumerable<MethodCandidate> CreateDefaultCandidates()
  {
    if (this._hasDefaults)
    {
      for (int defaultsUsed = 1; defaultsUsed < this._defaultArguments.Count + 1; ++defaultsUsed)
      {
        if (this._defaultArguments[this._defaultArguments.Count - defaultsUsed] != null)
          yield return this.CreateDefaultCandidate(defaultsUsed);
      }
    }
  }

  private MethodCandidate CreateDefaultCandidate(int defaultsUsed)
  {
    List<ArgBuilder> argBuilders = new List<ArgBuilder>((IEnumerable<ArgBuilder>) this._arguments);
    List<ParameterWrapper> range = this._parameters.GetRange(0, this._parameters.Count - defaultsUsed);
    for (int index1 = 0; index1 < defaultsUsed; ++index1)
    {
      int index2 = this._defaultArguments.Count - defaultsUsed + index1;
      int index3 = argBuilders.Count - defaultsUsed + index1;
      if (this._defaultArguments[index2] != null)
        argBuilders[index3] = this._defaultArguments[index2];
      else
        range.Add(this._parameters[this._parameters.Count - defaultsUsed + index1]);
    }
    int num = this.Overload.IsStatic ? 0 : 1;
    for (int index4 = 0; index4 < argBuilders.Count; ++index4)
    {
      if (argBuilders[index4] is SimpleArgBuilder simpleArgBuilder1)
      {
        List<ArgBuilder> argBuilderList = argBuilders;
        int index5 = index4;
        int newIndex = num++;
        SimpleArgBuilder simpleArgBuilder = simpleArgBuilder1.MakeCopy(newIndex);
        argBuilderList[index5] = (ArgBuilder) simpleArgBuilder;
      }
    }
    return new MethodCandidate(this._resolver, this.Overload, range, this._paramsDict, this._returnBuilder, this._instanceBuilder, (IList<ArgBuilder>) argBuilders, (Dictionary<DynamicMetaObject, BindingRestrictions>) null);
  }

  private ReturnBuilder MakeReturnBuilder(BitArray specialParameters)
  {
    ReturnBuilder builder = this._returnArgs != null ? (ReturnBuilder) new ByRefReturnBuilder((IList<int>) this._returnArgs) : new ReturnBuilder(this.Overload.ReturnType);
    if (this._argNames.Count > 0 && this._resolver.AllowMemberInitialization(this.Overload))
    {
      List<string> unusedArgNames = this.GetUnusedArgNames(specialParameters);
      List<MemberInfo> bindableMembers = ParameterMapping.GetBindableMembers(builder.ReturnType, unusedArgNames);
      if (unusedArgNames.Count == bindableMembers.Count)
      {
        List<int> intList = new List<int>();
        foreach (MemberInfo memberInfo in bindableMembers)
        {
          PropertyInfo propertyInfo = memberInfo as PropertyInfo;
          this._parameters.Add(new ParameterWrapper((ParameterInfo) null, propertyInfo != (PropertyInfo) null ? propertyInfo.PropertyType : ((FieldInfo) memberInfo).FieldType, memberInfo.Name, ParameterBindingFlags.None));
          intList.Add(this._argNames.IndexOf(memberInfo.Name));
        }
        return (ReturnBuilder) new KeywordConstructorReturnBuilder(builder, this._argNames.Count, intList.ToArray(), bindableMembers.ToArray(), this._resolver.Binder.PrivateBinding);
      }
    }
    return builder;
  }

  private static List<MemberInfo> GetBindableMembers(Type returnType, List<string> unusedNames)
  {
    List<MemberInfo> bindableMembers = new List<MemberInfo>();
    foreach (string unusedName in unusedNames)
    {
      Type type = returnType;
      MemberInfo[] array;
      for (array = type.GetInheritedMembers(unusedName).ToArray<MemberInfo>(); array.Length != 1 && type != (Type) null; type = type.GetBaseType())
      {
        array = type.GetDeclaredMembers(unusedName).WithBindingFlags(BindingFlags.Instance | BindingFlags.Public).ToArray<MemberInfo>();
        if (array.Length > 1)
          break;
      }
      if (array.Length == 1 && ((object) (array[0] as PropertyInfo) != null || (object) (array[0] as FieldInfo) != null))
        bindableMembers.Add(array[0]);
    }
    return bindableMembers;
  }

  private List<string> GetUnusedArgNames(BitArray specialParameters)
  {
    List<string> unusedArgNames = new List<string>();
    foreach (string argName in (IEnumerable<string>) this._argNames)
    {
      bool flag = false;
      foreach (ParameterInfo parameter in (IEnumerable<ParameterInfo>) this.Overload.Parameters)
      {
        if (!this.IsSpecialParameter(specialParameters, parameter.Position) && parameter.Name == argName)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        unusedArgNames.Add(argName);
    }
    return unusedArgNames;
  }
}
