// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeRef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct TypeRef
{
  private readonly MetadataRecord m_record;

  internal TypeRef(MetadataRecord record) => this.m_record = record;

  public MetadataRecord ResolutionScope
  {
    get
    {
      MetadataRecord record = this.m_record;
      TypeRefTable typeRefTable = record.Import.TypeRefTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken resolutionScope = typeRefTable.GetResolutionScope(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(resolutionScope, tables);
    }
  }

  public MetadataName TypeName
  {
    get
    {
      MetadataTables tables = this.m_record.m_tables;
      MetadataRecord record = this.m_record;
      TypeRefTable typeRefTable = record.Import.TypeRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) typeRefTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public MetadataName TypeNamespace
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      TypeRefTable typeRefTable = record.Import.TypeRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int blob = (int) typeRefTable.GetNamespace(rid);
      return tables.ToMetadataName((uint) blob);
    }
  }

  public static implicit operator MetadataRecord(TypeRef typeRef) => typeRef.m_record;

  public static explicit operator TypeRef(MetadataRecord record) => record.TypeRef;

  public MetadataRecord Record => this.m_record;
}
