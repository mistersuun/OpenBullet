// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ParamDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct ParamDef
{
  private readonly MetadataRecord m_record;

  internal ParamDef(MetadataRecord record) => this.m_record = record;

  public ParameterAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      ParamTable paramTable = record.Import.ParamTable;
      record = this.m_record;
      int rid = record.Rid;
      return paramTable.GetFlags(rid);
    }
  }

  public int Index
  {
    get
    {
      MetadataRecord record = this.m_record;
      ParamTable paramTable = record.Import.ParamTable;
      record = this.m_record;
      int rid = record.Rid;
      return (int) paramTable.GetSequence(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      ParamTable paramTable = record.Import.ParamTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) paramTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public object GetDefaultValue()
  {
    MetadataRecord record = this.m_record;
    MetadataImport import = record.Import;
    record = this.m_record;
    MetadataToken token = record.Token;
    return import.GetDefaultValue(token);
  }

  public MethodDef FindDeclaringMethod()
  {
    MetadataRecord record = this.m_record;
    MethodTable methodTable = record.Import.MethodTable;
    record = this.m_record;
    int rid = record.Rid;
    record = this.m_record;
    int numberOfRows = record.Import.ParamTable.NumberOfRows;
    return new MetadataRecord(new MetadataToken(MetadataTokenType.MethodDef, methodTable.FindMethodContainingParam(rid, numberOfRows)), this.m_record.Tables).MethodDef;
  }

  public static implicit operator MetadataRecord(ParamDef paramDef) => paramDef.m_record;

  public static explicit operator ParamDef(MetadataRecord record) => record.ParamDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
