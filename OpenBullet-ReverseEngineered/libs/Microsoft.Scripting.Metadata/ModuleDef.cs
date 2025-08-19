// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ModuleDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct ModuleDef
{
  private readonly MetadataRecord m_record;

  internal ModuleDef(MetadataRecord record) => this.m_record = record;

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      ModuleTable moduleTable = record.Import.ModuleTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) moduleTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public Guid Mvid
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataImport import = record.Import;
      record = this.m_record;
      ModuleTable moduleTable = record.Import.ModuleTable;
      record = this.m_record;
      int rid = record.Rid;
      int mvId = (int) moduleTable.GetMVId(rid);
      return import.GetGuid((uint) mvId);
    }
  }

  public static implicit operator MetadataRecord(ModuleDef moduleDef) => moduleDef.m_record;

  public static explicit operator ModuleDef(MetadataRecord record) => record.ModuleDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
