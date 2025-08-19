// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ExtensionMethodInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Utils;

public struct ExtensionMethodInfo : IEquatable<ExtensionMethodInfo>
{
  private readonly Type _extendedType;
  private readonly MethodInfo _method;

  internal ExtensionMethodInfo(Type extendedType, MethodInfo method)
  {
    this._extendedType = extendedType;
    this._method = method;
  }

  public Type ExtendedType => this._extendedType;

  public MethodInfo Method => this._method;

  public override bool Equals(object obj) => obj is ExtensionMethodInfo other && this.Equals(other);

  public bool Equals(ExtensionMethodInfo other) => this._method.Equals((object) other._method);

  public static bool operator ==(ExtensionMethodInfo self, ExtensionMethodInfo other)
  {
    return self.Equals(other);
  }

  public static bool operator !=(ExtensionMethodInfo self, ExtensionMethodInfo other)
  {
    return !self.Equals(other);
  }

  public override int GetHashCode() => this._method.GetHashCode();

  public bool IsExtensionOf(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (type.IsEquivalentTo(this.ExtendedType))
      return true;
    return this._extendedType.ContainsGenericParameters && ReflectionUtils.BindGenericParameters(this._extendedType, type, true) != null;
  }
}
