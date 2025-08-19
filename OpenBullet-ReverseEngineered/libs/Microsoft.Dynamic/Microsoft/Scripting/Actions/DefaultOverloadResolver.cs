// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.DefaultOverloadResolver
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

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
namespace Microsoft.Scripting.Actions;

public class DefaultOverloadResolver : OverloadResolver
{
  private readonly IList<DynamicMetaObject> _args;
  private readonly CallSignature _signature;
  private readonly CallTypes _callType;
  private DynamicMetaObject _invalidSplattee;
  private static readonly DefaultOverloadResolverFactory _factory = new DefaultOverloadResolverFactory(DefaultBinder.Instance);

  public DefaultOverloadResolver(
    ActionBinder binder,
    DynamicMetaObject instance,
    IList<DynamicMetaObject> args,
    CallSignature signature)
    : this(binder, (IList<DynamicMetaObject>) ArrayUtils.Insert<DynamicMetaObject>(instance, args), signature, CallTypes.ImplicitInstance)
  {
  }

  public DefaultOverloadResolver(
    ActionBinder binder,
    IList<DynamicMetaObject> args,
    CallSignature signature)
    : this(binder, args, signature, CallTypes.None)
  {
  }

  public DefaultOverloadResolver(
    ActionBinder binder,
    IList<DynamicMetaObject> args,
    CallSignature signature,
    CallTypes callType)
    : base(binder)
  {
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>(args, nameof (args));
    this._args = args;
    this._signature = signature;
    this._callType = callType;
  }

  public static OverloadResolverFactory Factory
  {
    get => (OverloadResolverFactory) DefaultOverloadResolver._factory;
  }

  public CallSignature Signature => this._signature;

  public IList<DynamicMetaObject> Arguments => this._args;

  public CallTypes CallType => this._callType;

  protected internal override BitArray MapSpecialParameters(ParameterMapping mapping)
  {
    if (!mapping.Overload.IsStatic)
    {
      Type declaringType = mapping.Overload.DeclaringType;
      ParameterBindingFlags flags = (ParameterBindingFlags) (1 | (this._callType == CallTypes.ImplicitInstance ? 16 /*0x10*/ : 0));
      mapping.AddParameter(new ParameterWrapper((ParameterInfo) null, declaringType, (string) null, flags));
      mapping.AddInstanceBuilder(new InstanceBuilder(mapping.ArgIndex));
    }
    return (BitArray) null;
  }

  protected internal override Candidate CompareEquivalentCandidates(
    ApplicableCandidate one,
    ApplicableCandidate two)
  {
    Candidate candidate = base.CompareEquivalentCandidates(one, two);
    if (candidate.Chosen())
      return candidate;
    if (one.Method.Overload.IsStatic && !two.Method.Overload.IsStatic)
      return this._callType != CallTypes.ImplicitInstance ? Candidate.One : Candidate.Two;
    if (one.Method.Overload.IsStatic || !two.Method.Overload.IsStatic)
      return Candidate.Equivalent;
    return this._callType != CallTypes.ImplicitInstance ? Candidate.Two : Candidate.One;
  }

  private DynamicMetaObject GetArgument(int i)
  {
    return this._args[(this.CallType == CallTypes.ImplicitInstance ? 1 : 0) + i];
  }

  protected override void GetNamedArguments(
    out IList<DynamicMetaObject> namedArgs,
    out IList<string> argNames)
  {
    if (this._signature.HasNamedArgument() || this._signature.HasDictionaryArgument())
    {
      List<DynamicMetaObject> splattedArgs = new List<DynamicMetaObject>();
      List<string> splattedNames = new List<string>();
      for (int index = 0; index < this._signature.ArgumentCount; ++index)
      {
        if (this._signature.GetArgumentKind(index) == ArgumentType.Named)
        {
          splattedArgs.Add(this.GetArgument(index));
          splattedNames.Add(this._signature.GetArgumentName(index));
        }
      }
      if (this._signature.HasDictionaryArgument())
      {
        if (splattedArgs == null)
        {
          splattedArgs = new List<DynamicMetaObject>();
          splattedNames = new List<string>();
        }
        this.SplatDictionaryArgument((IList<string>) splattedNames, (IList<DynamicMetaObject>) splattedArgs);
      }
      splattedNames.TrimExcess();
      splattedArgs.TrimExcess();
      argNames = (IList<string>) splattedNames;
      namedArgs = (IList<DynamicMetaObject>) splattedArgs;
    }
    else
    {
      argNames = (IList<string>) ArrayUtils.EmptyStrings;
      namedArgs = (IList<DynamicMetaObject>) DynamicMetaObject.EmptyMetaObjects;
    }
  }

  protected override ActualArguments CreateActualArguments(
    IList<DynamicMetaObject> namedArgs,
    IList<string> argNames,
    int preSplatLimit,
    int postSplatLimit)
  {
    List<DynamicMetaObject> args = new List<DynamicMetaObject>();
    if (this.CallType == CallTypes.ImplicitInstance)
      args.Add(this._args[0]);
    for (int index1 = 0; index1 < this._signature.ArgumentCount; ++index1)
    {
      DynamicMetaObject dynamicMetaObject = this.GetArgument(index1);
      switch (this._signature.GetArgumentKind(index1))
      {
        case ArgumentType.Simple:
        case ArgumentType.Instance:
          args.Add(dynamicMetaObject);
          continue;
        case ArgumentType.Named:
        case ArgumentType.Dictionary:
          continue;
        case ArgumentType.List:
          if (!(dynamicMetaObject.Value is IList<object> objectList))
          {
            this._invalidSplattee = dynamicMetaObject;
            return (ActualArguments) null;
          }
          for (int index2 = 0; index2 < objectList.Count; ++index2)
            args.Add(DynamicMetaObject.Create(objectList[index2], (Expression) Expression.Call((Expression) Expression.Convert(dynamicMetaObject.Expression, typeof (IList<object>)), typeof (IList<object>).GetMethod("get_Item"), Microsoft.Scripting.Ast.Utils.Constant((object) index2))));
          continue;
        default:
          throw new NotImplementedException();
      }
    }
    args.TrimExcess();
    return new ActualArguments((IList<DynamicMetaObject>) args, namedArgs, argNames, this._callType == CallTypes.ImplicitInstance ? 1 : 0, 0, -1, -1);
  }

  private void SplatDictionaryArgument(
    IList<string> splattedNames,
    IList<DynamicMetaObject> splattedArgs)
  {
    DynamicMetaObject dynamicMetaObject = this.GetArgument(this._signature.ArgumentCount - 1);
    IDictionaryEnumerator enumerator = ((IDictionary) dynamicMetaObject.Value).GetEnumerator();
    while (enumerator.MoveNext())
    {
      DictionaryEntry entry = enumerator.Entry;
      if (entry.Key is string key)
      {
        splattedNames.Add(key);
        splattedArgs.Add(DynamicMetaObject.Create(entry.Value, (Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (IDictionary)), typeof (IDictionary).GetMethod("get_Item"), Microsoft.Scripting.Ast.Utils.Constant((object) key))));
      }
    }
  }

  protected override Expression GetSplattedExpression() => throw Assert.Unreachable;

  protected override object GetSplattedItem(int index) => throw Assert.Unreachable;

  public override ErrorInfo MakeInvalidParametersError(BindingTarget target)
  {
    return target.Result == BindingResult.InvalidArguments && this._invalidSplattee != null ? this.MakeInvalidSplatteeError(target) : base.MakeInvalidParametersError(target);
  }

  private ErrorInfo MakeInvalidSplatteeError(BindingTarget target)
  {
    return ErrorInfo.FromException((Expression) Expression.Call(typeof (BinderOps).GetMethod("InvalidSplatteeError"), Microsoft.Scripting.Ast.Utils.Constant((object) target.Name), Microsoft.Scripting.Ast.Utils.Constant((object) this.Binder.GetTypeName(this._invalidSplattee.GetLimitType()))));
  }
}
