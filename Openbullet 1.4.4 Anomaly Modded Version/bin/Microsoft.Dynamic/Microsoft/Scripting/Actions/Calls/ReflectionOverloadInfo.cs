// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ReflectionOverloadInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public class ReflectionOverloadInfo : OverloadInfo
{
  private readonly MethodBase _method;
  private ReadOnlyCollection<ParameterInfo> _parameters;
  private ReadOnlyCollection<Type> _genericArguments;
  private ReflectionOverloadInfo._Flags _flags;

  public ReflectionOverloadInfo(MethodBase method) => this._method = method;

  public override MethodBase ReflectionInfo => this._method;

  public override string Name => this._method.Name;

  public override IList<ParameterInfo> Parameters
  {
    get
    {
      return (IList<ParameterInfo>) this._parameters ?? (IList<ParameterInfo>) (this._parameters = new ReadOnlyCollection<ParameterInfo>((IList<ParameterInfo>) this._method.GetParameters()));
    }
  }

  public override ParameterInfo ReturnParameter
  {
    get
    {
      MethodInfo method = this._method as MethodInfo;
      return !(method != (MethodInfo) null) ? (ParameterInfo) null : method.ReturnParameter;
    }
  }

  public override IList<Type> GenericArguments
  {
    get
    {
      return (IList<Type>) this._genericArguments ?? (IList<Type>) (this._genericArguments = new ReadOnlyCollection<Type>((IList<Type>) this._method.GetGenericArguments()));
    }
  }

  public override Type DeclaringType => this._method.DeclaringType;

  public override Type ReturnType => this._method.GetReturnType();

  public override CallingConventions CallingConvention => this._method.CallingConvention;

  public override MethodAttributes Attributes => this._method.Attributes;

  public override bool IsInstanceFactory => CompilerHelpers.IsConstructor(this._method);

  public override bool IsConstructor => this._method.IsConstructor;

  public override bool IsExtension
  {
    get
    {
      if ((this._flags & ReflectionOverloadInfo._Flags.KnownExtension) == ReflectionOverloadInfo._Flags.None)
        this._flags |= (ReflectionOverloadInfo._Flags) (32 /*0x20*/ | (this._method.IsExtension() ? 16 /*0x10*/ : 0));
      return (this._flags & ReflectionOverloadInfo._Flags.IsExtension) != 0;
    }
  }

  public override bool IsVariadic
  {
    get
    {
      if ((this._flags & ReflectionOverloadInfo._Flags.KnownVariadic) == ReflectionOverloadInfo._Flags.None)
        this._flags |= (ReflectionOverloadInfo._Flags) (2 | (this.IsVariadicInternal() ? 1 : 0));
      return (this._flags & ReflectionOverloadInfo._Flags.IsVariadic) != 0;
    }
  }

  private bool IsVariadicInternal()
  {
    IList<ParameterInfo> parameters = this.Parameters;
    for (int index = parameters.Count - 1; index >= 0; --index)
    {
      if (parameters[index].IsParamArray() || parameters[index].IsParamDictionary())
        return true;
    }
    return false;
  }

  public override bool IsGenericMethod => this._method.IsGenericMethod;

  public override bool IsGenericMethodDefinition => this._method.IsGenericMethodDefinition;

  public override bool ContainsGenericParameters
  {
    get
    {
      if ((this._flags & ReflectionOverloadInfo._Flags.KnownContainsGenericParameters) == ReflectionOverloadInfo._Flags.None)
        this._flags |= (ReflectionOverloadInfo._Flags) (8 | (this._method.ContainsGenericParameters ? 4 : 0));
      return (this._flags & ReflectionOverloadInfo._Flags.ContainsGenericParameters) != 0;
    }
  }

  public override OverloadInfo MakeGenericMethod(Type[] genericArguments)
  {
    return (OverloadInfo) new ReflectionOverloadInfo((MethodBase) ((MethodInfo) this._method).MakeGenericMethod(genericArguments));
  }

  public static OverloadInfo[] CreateArray(MethodBase[] methods)
  {
    return (OverloadInfo[]) ArrayUtils.ConvertAll<MethodBase, ReflectionOverloadInfo>(methods, (Func<MethodBase, ReflectionOverloadInfo>) (m => new ReflectionOverloadInfo(m)));
  }

  [Flags]
  private enum _Flags
  {
    None = 0,
    IsVariadic = 1,
    KnownVariadic = 2,
    ContainsGenericParameters = 4,
    KnownContainsGenericParameters = 8,
    IsExtension = 16, // 0x00000010
    KnownExtension = 32, // 0x00000020
  }
}
