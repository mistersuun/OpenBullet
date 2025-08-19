// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeNesting
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct TypeNesting
{
  private readonly MetadataRecord m_record;

  internal TypeNesting(MetadataRecord record) => this.m_record = record;

  public TypeDef NestedType
  {
    get
    {
      MetadataRecord metadataRecord = this.m_record;
      NestedClassTable nestedClassTable = metadataRecord.Import.NestedClassTable;
      metadataRecord = this.m_record;
      int rid = metadataRecord.Rid;
      MetadataToken nestedType = nestedClassTable.GetNestedType(rid);
      metadataRecord = this.m_record;
      MetadataTables tables = metadataRecord.Tables;
      metadataRecord = new MetadataRecord(nestedType, tables);
      return metadataRecord.TypeDef;
    }
  }

  public TypeDef EnclosingType
  {
    get
    {
      MetadataRecord metadataRecord = this.m_record;
      NestedClassTable nestedClassTable = metadataRecord.Import.NestedClassTable;
      metadataRecord = this.m_record;
      int rid = metadataRecord.Rid;
      MetadataToken enclosingType = nestedClassTable.GetEnclosingType(rid);
      metadataRecord = this.m_record;
      MetadataTables tables = metadataRecord.Tables;
      metadataRecord = new MetadataRecord(enclosingType, tables);
      return metadataRecord.TypeDef;
    }
  }

  public static implicit operator MetadataRecord(TypeNesting nestedClassDef)
  {
    return nestedClassDef.m_record;
  }

  public static explicit operator TypeNesting(MetadataRecord record) => record.TypeNesting;

  public MetadataRecord Record => this.m_record;
}
