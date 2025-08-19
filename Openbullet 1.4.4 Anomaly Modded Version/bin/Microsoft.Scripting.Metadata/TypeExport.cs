// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeExport
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct TypeExport
{
  private readonly MetadataRecord m_record;

  internal TypeExport(MetadataRecord record) => this.m_record = record;

  public TypeAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      ExportedTypeTable exportedTypeTable = record.Import.ExportedTypeTable;
      record = this.m_record;
      int rid = record.Rid;
      return exportedTypeTable.GetFlags(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      ExportedTypeTable exportedTypeTable = record.Import.ExportedTypeTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) exportedTypeTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public MetadataName Namespace
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      ExportedTypeTable exportedTypeTable = record.Import.ExportedTypeTable;
      record = this.m_record;
      int rid = record.Rid;
      int blob = (int) exportedTypeTable.GetNamespace(rid);
      return tables.ToMetadataName((uint) blob);
    }
  }

  public MetadataRecord Implementation
  {
    get
    {
      MetadataRecord record = this.m_record;
      ExportedTypeTable exportedTypeTable = record.Import.ExportedTypeTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken implementation = exportedTypeTable.GetImplementation(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(implementation, tables);
    }
  }

  public static implicit operator MetadataRecord(TypeExport typeExport) => typeExport.m_record;

  public static explicit operator TypeExport(MetadataRecord record) => record.TypeExport;

  public MetadataRecord Record => this.m_record;
}
