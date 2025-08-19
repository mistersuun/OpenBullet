// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DelegateSignatureInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal sealed class DelegateSignatureInfo
{
  private readonly Type _returnType;
  private readonly Type[] _parameterTypes;

  internal DelegateSignatureInfo(MethodInfo invoke)
  {
    ParameterInfo[] parameters = invoke.GetParameters();
    Type[] typeArray = new Type[parameters.Length];
    for (int index = 0; index < parameters.Length; ++index)
      typeArray[index] = parameters[index].ParameterType;
    this._parameterTypes = typeArray;
    this._returnType = invoke.ReturnType;
  }

  internal Type ReturnType => this._returnType;

  internal Type[] ParameterTypes => this._parameterTypes;

  public override bool Equals(object obj)
  {
    if (!(obj is DelegateSignatureInfo delegateSignatureInfo) || delegateSignatureInfo._parameterTypes.Length != this._parameterTypes.Length || delegateSignatureInfo._returnType != this._returnType)
      return false;
    for (int index = 0; index < this._parameterTypes.Length; ++index)
    {
      if (delegateSignatureInfo._parameterTypes[index] != this._parameterTypes[index])
        return false;
    }
    return true;
  }

  public override int GetHashCode()
  {
    int num = 5331;
    for (int index = 0; index < this._parameterTypes.Length; ++index)
      num ^= this._parameterTypes[index].GetHashCode();
    return num ^ this._returnType.GetHashCode();
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append((object) this._returnType);
    stringBuilder.Append("(");
    for (int index = 0; index < this._parameterTypes.Length; ++index)
    {
      if (index != 0)
        stringBuilder.Append(", ");
      stringBuilder.Append(this._parameterTypes[index].Name);
    }
    stringBuilder.Append(")");
    return stringBuilder.ToString();
  }
}
