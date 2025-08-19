// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ModuleRef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct ModuleRef
{
  private readonly MetadataRecord m_record;

  internal ModuleRef(MetadataRecord record) => this.m_record = record;

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      ModuleRefTable moduleRefTable = record.Import.ModuleRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) moduleRefTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public static implicit operator MetadataRecord(ModuleRef moduleDef) => moduleDef.m_record;

  public static explicit operator ModuleRef(MetadataRecord record) => record.ModuleRef;

  public MetadataRecord Record => this.m_record;
}
