// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ManifestResourceDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct ManifestResourceDef
{
  private readonly MetadataRecord m_record;

  internal ManifestResourceDef(MetadataRecord record) => this.m_record = record;

  [CLSCompliant(false)]
  public uint Offset
  {
    get
    {
      MetadataRecord record = this.m_record;
      ManifestResourceTable manifestResourceTable = record.Import.ManifestResourceTable;
      record = this.m_record;
      int rid = record.Rid;
      return manifestResourceTable.GetOffset(rid);
    }
  }

  public ManifestResourceAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      ManifestResourceTable manifestResourceTable = record.Import.ManifestResourceTable;
      record = this.m_record;
      int rid = record.Rid;
      return manifestResourceTable.GetFlags(rid);
    }
  }

  public MetadataRecord Implementation
  {
    get
    {
      MetadataRecord record = this.m_record;
      ManifestResourceTable manifestResourceTable = record.Import.ManifestResourceTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken implementation = manifestResourceTable.GetImplementation(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(implementation, tables);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      ManifestResourceTable manifestResourceTable = record.Import.ManifestResourceTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) manifestResourceTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public static implicit operator MetadataRecord(ManifestResourceDef resourceDef)
  {
    return resourceDef.m_record;
  }

  public static explicit operator ManifestResourceDef(MetadataRecord record) => record.ResourceDef;

  public MetadataRecord Record => this.m_record;
}
