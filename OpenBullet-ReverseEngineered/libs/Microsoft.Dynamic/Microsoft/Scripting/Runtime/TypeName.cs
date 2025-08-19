// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.TypeName
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal struct TypeName
{
  internal TypeName(Type type)
  {
    this.Namespace = type.Namespace;
    this.Name = type.Name;
  }

  internal TypeName(string nameSpace, string typeName)
  {
    this.Namespace = nameSpace;
    this.Name = typeName;
  }

  internal string Namespace { get; private set; }

  internal string Name { get; private set; }

  public override int GetHashCode()
  {
    int hashCode = 13631488 /*0xD00000*/;
    if (this.Namespace != null)
      hashCode ^= this.Namespace.GetHashCode();
    if (this.Name != null)
      hashCode ^= this.Name.GetHashCode();
    return hashCode;
  }

  public override bool Equals(object obj)
  {
    return obj is TypeName typeName && typeName.Namespace == this.Namespace && typeName.Name == this.Name;
  }

  public static bool operator ==(TypeName a, TypeName b)
  {
    return a.Namespace == b.Namespace && a.Name == b.Name;
  }

  public static bool operator !=(TypeName a, TypeName b) => !(a == b);
}
