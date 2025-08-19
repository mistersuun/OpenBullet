// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.CustomAttributeDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct CustomAttributeDef
{
  private readonly MetadataRecord m_record;

  internal CustomAttributeDef(MetadataRecord record) => this.m_record = record;

  public MetadataRecord Parent
  {
    get
    {
      MetadataRecord record = this.m_record;
      CustomAttributeTable customAttributeTable = record.Import.CustomAttributeTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken parent = customAttributeTable.GetParent(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(parent, tables);
    }
  }

  public MetadataRecord Constructor
  {
    get
    {
      MetadataRecord record = this.m_record;
      CustomAttributeTable customAttributeTable = record.Import.CustomAttributeTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken constructor = customAttributeTable.GetConstructor(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(constructor, tables);
    }
  }

  public MemoryBlock Value
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataImport import = record.Import;
      record = this.m_record;
      CustomAttributeTable customAttributeTable = record.Import.CustomAttributeTable;
      record = this.m_record;
      int rid = record.Rid;
      int blob = (int) customAttributeTable.GetValue(rid);
      return import.GetBlobBlock((uint) blob);
    }
  }

  public static implicit operator MetadataRecord(CustomAttributeDef customAttributeDef)
  {
    return customAttributeDef.m_record;
  }

  public static explicit operator CustomAttributeDef(MetadataRecord record)
  {
    return record.CustomAttributeDef;
  }

  public MetadataRecord Record => this.m_record;
}
