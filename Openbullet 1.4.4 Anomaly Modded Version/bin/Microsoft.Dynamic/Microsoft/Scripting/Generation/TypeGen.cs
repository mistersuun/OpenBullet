// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.TypeGen
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.Generation;

public sealed class TypeGen
{
  private ILGen _initGen;
  private const MethodAttributes MethodAttributesToEraseInOveride = MethodAttributes.ReservedMask | MethodAttributes.Abstract;

  public ILGen TypeInitializer
  {
    get
    {
      if (this._initGen == null)
        this._initGen = new ILGen(this.TypeBuilder.DefineTypeInitializer().GetILGenerator());
      return this._initGen;
    }
  }

  internal AssemblyGen AssemblyGen { get; }

  public TypeBuilder TypeBuilder { get; }

  public TypeGen(AssemblyGen myAssembly, TypeBuilder myType)
  {
    this.AssemblyGen = myAssembly;
    this.TypeBuilder = myType;
  }

  public override string ToString() => this.TypeBuilder.ToString();

  public Type FinishType()
  {
    this._initGen?.Emit(OpCodes.Ret);
    return this.TypeBuilder.CreateType();
  }

  public FieldBuilder AddStaticField(Type fieldType, string name)
  {
    return this.TypeBuilder.DefineField(name, fieldType, FieldAttributes.Public | FieldAttributes.Static);
  }

  public FieldBuilder AddStaticField(Type fieldType, FieldAttributes attributes, string name)
  {
    return this.TypeBuilder.DefineField(name, fieldType, attributes | FieldAttributes.Static);
  }

  public ILGen DefineExplicitInterfaceImplementation(MethodInfo baseMethod)
  {
    ContractUtils.RequiresNotNull((object) baseMethod, nameof (baseMethod));
    MethodAttributes attributes = baseMethod.Attributes & ~(MethodAttributes.Public | MethodAttributes.Abstract) | MethodAttributes.Final | MethodAttributes.VtableLayoutMask;
    Type[] parameterTypes = ((ICollection<ParameterInfo>) baseMethod.GetParameters()).Map<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType));
    MethodBuilder methodInfoBody = this.TypeBuilder.DefineMethod($"{baseMethod.DeclaringType.Name}.{baseMethod.Name}", attributes, baseMethod.ReturnType, parameterTypes);
    this.TypeBuilder.DefineMethodOverride((MethodInfo) methodInfoBody, baseMethod);
    return new ILGen(methodInfoBody.GetILGenerator());
  }

  public ILGen DefineMethodOverride(MethodInfo baseMethod)
  {
    MethodAttributes attributes = baseMethod.Attributes & ~(MethodAttributes.ReservedMask | MethodAttributes.Abstract);
    Type[] parameterTypes = ((ICollection<ParameterInfo>) baseMethod.GetParameters()).Map<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType));
    MethodBuilder methodInfoBody = this.TypeBuilder.DefineMethod(baseMethod.Name, attributes, baseMethod.ReturnType, parameterTypes);
    this.TypeBuilder.DefineMethodOverride((MethodInfo) methodInfoBody, baseMethod);
    return new ILGen(methodInfoBody.GetILGenerator());
  }
}
