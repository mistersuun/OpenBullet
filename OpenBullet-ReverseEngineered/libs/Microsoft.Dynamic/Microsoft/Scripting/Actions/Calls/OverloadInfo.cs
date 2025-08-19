// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.OverloadInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

[DebuggerDisplay("{(object)ReflectionInfo ?? Name}")]
public abstract class OverloadInfo
{
  public abstract string Name { get; }

  public abstract IList<ParameterInfo> Parameters { get; }

  public virtual int ParameterCount => this.Parameters.Count;

  public abstract ParameterInfo ReturnParameter { get; }

  public virtual bool ProhibitsNull(int parameterIndex)
  {
    return this.Parameters[parameterIndex].ProhibitsNull();
  }

  public virtual bool ProhibitsNullItems(int parameterIndex)
  {
    return this.Parameters[parameterIndex].ProhibitsNullItems();
  }

  public virtual bool IsParamArray(int parameterIndex)
  {
    return this.Parameters[parameterIndex].IsParamArray();
  }

  public virtual bool IsParamDictionary(int parameterIndex)
  {
    return this.Parameters[parameterIndex].IsParamDictionary();
  }

  public abstract Type DeclaringType { get; }

  public abstract Type ReturnType { get; }

  public abstract MethodAttributes Attributes { get; }

  public abstract bool IsConstructor { get; }

  public abstract bool IsExtension { get; }

  public abstract bool IsVariadic { get; }

  public abstract bool IsGenericMethodDefinition { get; }

  public abstract bool IsGenericMethod { get; }

  public abstract bool ContainsGenericParameters { get; }

  public abstract IList<Type> GenericArguments { get; }

  public abstract OverloadInfo MakeGenericMethod(Type[] genericArguments);

  public virtual CallingConventions CallingConvention => CallingConventions.Standard;

  public virtual MethodBase ReflectionInfo => (MethodBase) null;

  public virtual bool IsInstanceFactory => this.IsConstructor;

  public bool IsPrivate
  {
    get => (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
  }

  public bool IsPublic
  {
    get => (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
  }

  public bool IsAssembly
  {
    get => (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
  }

  public bool IsFamily
  {
    get => (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
  }

  public bool IsFamilyOrAssembly
  {
    get => (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;
  }

  public bool IsFamilyAndAssembly
  {
    get => (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
  }

  public bool IsProtected => this.IsFamily || this.IsFamilyOrAssembly;

  public bool IsStatic => this.IsConstructor || (this.Attributes & MethodAttributes.Static) != 0;

  public bool IsVirtual => (this.Attributes & MethodAttributes.Virtual) != 0;

  public bool IsSpecialName => (this.Attributes & MethodAttributes.SpecialName) != 0;

  public bool IsFinal => (this.Attributes & MethodAttributes.Final) != 0;
}
