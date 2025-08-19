// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyRef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct AssemblyRef
{
  private readonly MetadataRecord m_record;

  internal AssemblyRef(MetadataRecord record) => this.m_record = record;

  public byte[] GetHashValue()
  {
    MetadataRecord record = this.m_record;
    MetadataImport import = record.Import;
    AssemblyRefTable assemblyRefTable = import.AssemblyRefTable;
    record = this.m_record;
    int rid = record.Rid;
    return import.GetBlob(assemblyRefTable.GetHashValue(rid));
  }

  public Version Version
  {
    get
    {
      MetadataRecord record = this.m_record;
      AssemblyRefTable assemblyRefTable = record.Import.AssemblyRefTable;
      record = this.m_record;
      int rid = record.Rid;
      return assemblyRefTable.GetVersion(rid);
    }
  }

  public AssemblyNameFlags NameFlags
  {
    get
    {
      MetadataRecord record = this.m_record;
      AssemblyRefTable assemblyRefTable = record.Import.AssemblyRefTable;
      record = this.m_record;
      int rid = record.Rid;
      return assemblyRefTable.GetFlags(rid);
    }
  }

  public byte[] GetPublicKeyOrToken()
  {
    MetadataRecord record = this.m_record;
    MetadataImport import = record.Import;
    AssemblyRefTable assemblyRefTable = import.AssemblyRefTable;
    record = this.m_record;
    int rid = record.Rid;
    return import.GetBlob(assemblyRefTable.GetPublicKeyOrToken(rid));
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      AssemblyRefTable assemblyRefTable = record.Import.AssemblyRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) assemblyRefTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public MetadataName Culture
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      AssemblyRefTable assemblyRefTable = record.Import.AssemblyRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int culture = (int) assemblyRefTable.GetCulture(rid);
      return tables.ToMetadataName((uint) culture);
    }
  }

  public static implicit operator MetadataRecord(AssemblyRef assemblyRef) => assemblyRef.m_record;

  public static explicit operator AssemblyRef(MetadataRecord record) => record.AssemblyRef;

  public MetadataRecord Record => this.m_record;
}
