// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.TypeNameKey
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal struct TypeNameKey(string assemblyName, string typeName) : IEquatable<TypeNameKey>
{
  internal readonly string AssemblyName = assemblyName;
  internal readonly string TypeName = typeName;

  public override int GetHashCode()
  {
    string assemblyName = this.AssemblyName;
    int hashCode1 = assemblyName != null ? assemblyName.GetHashCode() : 0;
    string typeName = this.TypeName;
    int hashCode2 = typeName != null ? typeName.GetHashCode() : 0;
    return hashCode1 ^ hashCode2;
  }

  public override bool Equals(object obj) => obj is TypeNameKey other && this.Equals(other);

  public bool Equals(TypeNameKey other)
  {
    return this.AssemblyName == other.AssemblyName && this.TypeName == other.TypeName;
  }
}
