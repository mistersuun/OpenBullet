// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.GenericParamConstraint
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct GenericParamConstraint
{
  private readonly MetadataRecord m_record;

  internal GenericParamConstraint(MetadataRecord record) => this.m_record = record;

  public GenericParamDef Owner
  {
    get
    {
      MetadataRecord metadataRecord = this.m_record;
      GenericParamConstraintTable paramConstraintTable = metadataRecord.Import.GenericParamConstraintTable;
      metadataRecord = this.m_record;
      int rid = metadataRecord.Rid;
      MetadataToken owner = paramConstraintTable.GetOwner(rid);
      metadataRecord = this.m_record;
      MetadataTables tables = metadataRecord.Tables;
      metadataRecord = new MetadataRecord(owner, tables);
      return metadataRecord.GenericParamDef;
    }
  }

  public MetadataRecord Constraint
  {
    get
    {
      MetadataRecord record = this.m_record;
      GenericParamConstraintTable paramConstraintTable = record.Import.GenericParamConstraintTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken constraint = paramConstraintTable.GetConstraint(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(constraint, tables);
    }
  }

  public static implicit operator MetadataRecord(GenericParamConstraint constraint)
  {
    return constraint.m_record;
  }

  public static explicit operator GenericParamConstraint(MetadataRecord record)
  {
    return record.GenericParamConstraint;
  }

  public MetadataRecord Record => this.m_record;
}
