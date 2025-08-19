// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FieldDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct FieldDef
{
  private readonly MetadataRecord m_record;

  internal FieldDef(MetadataRecord record) => this.m_record = record;

  public FieldAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      FieldTable fieldTable = record.Import.FieldTable;
      record = this.m_record;
      int rid = record.Rid;
      return fieldTable.GetFlags(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      FieldTable fieldTable = record.Import.FieldTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) fieldTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public MemoryBlock Signature
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataImport import = record.Import;
      record = this.m_record;
      FieldTable fieldTable = record.Import.FieldTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) fieldTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
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

  public MemoryBlock GetData(int size)
  {
    if (size < 0)
      throw new ArgumentOutOfRangeException(nameof (size));
    FieldRVATable fieldRvaTable = this.m_record.Import.FieldRVATable;
    MetadataRecord record = this.m_record;
    int rid = record.Rid;
    uint fieldRva = fieldRvaTable.GetFieldRVA(rid);
    if (fieldRva == 0U)
      return (MemoryBlock) null;
    record = this.m_record;
    return record.Import.RvaToMemoryBlock(fieldRva, (uint) size);
  }

  public TypeDef FindDeclaringType()
  {
    MetadataRecord record = this.m_record;
    TypeDefTable typeDefTable = record.Import.TypeDefTable;
    record = this.m_record;
    int rid = record.Rid;
    record = this.m_record;
    int numberOfRows = record.Import.FieldTable.NumberOfRows;
    return new MetadataRecord(new MetadataToken(MetadataTokenType.TypeDef, typeDefTable.FindTypeContainingField(rid, numberOfRows)), this.m_record.Tables).TypeDef;
  }

  public static implicit operator MetadataRecord(FieldDef fieldDef) => fieldDef.m_record;

  public static explicit operator FieldDef(MetadataRecord record) => record.FieldDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
