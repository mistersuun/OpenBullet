// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Configuration.Assemblies;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct AssemblyDef
{
  private readonly MetadataRecord m_record;

  internal AssemblyDef(MetadataRecord record) => this.m_record = record;

  public AssemblyHashAlgorithm HashAlgorithm
  {
    get
    {
      MetadataRecord record = this.m_record;
      AssemblyTable assemblyTable = record.Import.AssemblyTable;
      record = this.m_record;
      int rid = record.Rid;
      return assemblyTable.GetHashAlgorithm(rid);
    }
  }

  public Version Version
  {
    get
    {
      MetadataRecord record = this.m_record;
      AssemblyTable assemblyTable = record.Import.AssemblyTable;
      record = this.m_record;
      int rid = record.Rid;
      return assemblyTable.GetVersion(rid);
    }
  }

  public AssemblyNameFlags NameFlags
  {
    get
    {
      MetadataRecord record = this.m_record;
      AssemblyTable assemblyTable = record.Import.AssemblyTable;
      record = this.m_record;
      int rid = record.Rid;
      return assemblyTable.GetFlags(rid);
    }
  }

  public byte[] GetPublicKey()
  {
    MetadataRecord record = this.m_record;
    MetadataImport import = record.Import;
    AssemblyTable assemblyTable = import.AssemblyTable;
    record = this.m_record;
    int rid = record.Rid;
    return import.GetBlob(assemblyTable.GetPublicKey(rid));
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      AssemblyTable assemblyTable = record.Import.AssemblyTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) assemblyTable.GetName(rid);
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
      AssemblyTable assemblyTable = record.Import.AssemblyTable;
      record = this.m_record;
      int rid = record.Rid;
      int culture = (int) assemblyTable.GetCulture(rid);
      return tables.ToMetadataName((uint) culture);
    }
  }

  public static implicit operator MetadataRecord(AssemblyDef assemblyDef) => assemblyDef.m_record;

  public static explicit operator AssemblyDef(MetadataRecord record) => record.AssemblyDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
