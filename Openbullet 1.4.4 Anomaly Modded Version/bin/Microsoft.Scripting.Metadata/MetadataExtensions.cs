// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataExtensions
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public static class MetadataExtensions
{
  public static bool IsNested(this TypeAttributes attrs)
  {
    return (uint) (attrs & TypeAttributes.VisibilityMask) > 1U;
  }

  public static bool IsForwarder(this TypeAttributes attrs)
  {
    return (attrs & (TypeAttributes) 2097152 /*0x200000*/) != 0;
  }

  public static AssemblyName GetAssemblyName(this AssemblyRef assemblyRef)
  {
    return MetadataExtensions.CreateAssemblyName(assemblyRef.Name, assemblyRef.Culture, assemblyRef.Version, assemblyRef.NameFlags, assemblyRef.GetPublicKeyOrToken());
  }

  public static AssemblyName GetAssemblyName(this AssemblyDef assemblyDef)
  {
    return MetadataExtensions.CreateAssemblyName(assemblyDef.Name, assemblyDef.Culture, assemblyDef.Version, assemblyDef.NameFlags, assemblyDef.GetPublicKey());
  }

  private static AssemblyName CreateAssemblyName(
    MetadataName name,
    MetadataName culture,
    Version version,
    AssemblyNameFlags flags,
    byte[] publicKeyOrToken)
  {
    AssemblyName assemblyName = new AssemblyName();
    assemblyName.Name = name.ToString();
    if (!culture.IsEmpty)
      assemblyName.CultureInfo = new CultureInfo(culture.ToString());
    assemblyName.Version = version;
    assemblyName.Flags = flags;
    if (publicKeyOrToken.Length != 0)
    {
      if ((assemblyName.Flags & AssemblyNameFlags.PublicKey) != AssemblyNameFlags.None)
        assemblyName.SetPublicKey(publicKeyOrToken);
      else
        assemblyName.SetPublicKeyToken(publicKeyOrToken);
    }
    return assemblyName;
  }

  public static MetadataTables GetMetadataTables(this Module module)
  {
    return MetadataTables.OpenFile(module.FullyQualifiedName);
  }
}
