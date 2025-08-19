// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.GenericParamDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct GenericParamDef
{
  private readonly MetadataRecord m_record;

  internal GenericParamDef(MetadataRecord record) => this.m_record = record;

  public GenericParameterAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      GenericParamTable genericParamTable = record.Import.GenericParamTable;
      record = this.m_record;
      int rid = record.Rid;
      return genericParamTable.GetFlags(rid);
    }
  }

  public int Index
  {
    get
    {
      MetadataRecord record = this.m_record;
      GenericParamTable genericParamTable = record.Import.GenericParamTable;
      record = this.m_record;
      int rid = record.Rid;
      return genericParamTable.GetIndex(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      GenericParamTable genericParamTable = record.Import.GenericParamTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) genericParamTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public MetadataRecord Owner
  {
    get
    {
      MetadataRecord record = this.m_record;
      GenericParamTable genericParamTable = record.Import.GenericParamTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken owner = genericParamTable.GetOwner(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(owner, tables);
    }
  }

  public static implicit operator MetadataRecord(GenericParamDef genericParamDef)
  {
    return genericParamDef.m_record;
  }

  public static explicit operator GenericParamDef(MetadataRecord record) => record.GenericParamDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView Constraints
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.GenericParamConstraint);
  }

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
