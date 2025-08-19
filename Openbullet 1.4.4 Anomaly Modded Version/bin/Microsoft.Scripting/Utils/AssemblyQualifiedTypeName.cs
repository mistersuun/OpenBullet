// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.AssemblyQualifiedTypeName
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Utils;

[Serializable]
internal readonly struct AssemblyQualifiedTypeName : IEquatable<AssemblyQualifiedTypeName>
{
  public readonly string TypeName;
  public readonly AssemblyName AssemblyName;

  public AssemblyQualifiedTypeName(string typeName, AssemblyName assemblyName)
  {
    ContractUtils.RequiresNotNull((object) typeName, nameof (typeName));
    ContractUtils.RequiresNotNull((object) assemblyName, nameof (assemblyName));
    this.TypeName = typeName;
    this.AssemblyName = assemblyName;
  }

  public AssemblyQualifiedTypeName(Type type)
  {
    this.TypeName = type.FullName;
    this.AssemblyName = type.Assembly.GetName();
  }

  public AssemblyQualifiedTypeName(string assemblyQualifiedTypeName)
  {
    ContractUtils.RequiresNotNull((object) assemblyQualifiedTypeName, nameof (assemblyQualifiedTypeName));
    int length = assemblyQualifiedTypeName.IndexOf(",");
    if (length != -1)
    {
      this.TypeName = assemblyQualifiedTypeName.Substring(0, length).Trim();
      string assemblyName = assemblyQualifiedTypeName.Substring(length + 1).Trim();
      if (this.TypeName.Length > 0)
      {
        if (assemblyName.Length > 0)
        {
          try
          {
            this.AssemblyName = new AssemblyName(assemblyName);
            return;
          }
          catch (Exception ex)
          {
            throw new ArgumentException($"Invalid assembly qualified name '{assemblyQualifiedTypeName}': {ex.Message}", ex);
          }
        }
      }
    }
    throw new ArgumentException($"Invalid assembly qualified name '{assemblyQualifiedTypeName}'");
  }

  internal static AssemblyQualifiedTypeName ParseArgument(string str, string argumentName)
  {
    try
    {
      return new AssemblyQualifiedTypeName(str);
    }
    catch (ArgumentException ex)
    {
      throw new ArgumentException(ex.Message, argumentName, ex.InnerException);
    }
  }

  public bool Equals(AssemblyQualifiedTypeName other)
  {
    return this.TypeName == other.TypeName && this.AssemblyName.FullName == other.AssemblyName.FullName;
  }

  public override bool Equals(object obj)
  {
    return obj is AssemblyQualifiedTypeName other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return this.TypeName.GetHashCode() ^ this.AssemblyName.FullName.GetHashCode();
  }

  public override string ToString() => $"{this.TypeName}, {this.AssemblyName.FullName}";

  public static bool operator ==(AssemblyQualifiedTypeName name, AssemblyQualifiedTypeName other)
  {
    return name.Equals(other);
  }

  public static bool operator !=(AssemblyQualifiedTypeName name, AssemblyQualifiedTypeName other)
  {
    return !name.Equals(other);
  }
}
