// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct MethodDef
{
  private readonly MetadataRecord m_record;

  internal MethodDef(MetadataRecord record) => this.m_record = record;

  public MethodImplAttributes ImplAttributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      MethodTable methodTable = record.Import.MethodTable;
      record = this.m_record;
      int rid = record.Rid;
      return methodTable.GetImplFlags(rid);
    }
  }

  public MethodAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      MethodTable methodTable = record.Import.MethodTable;
      record = this.m_record;
      int rid = record.Rid;
      return methodTable.GetFlags(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      MethodTable methodTable = record.Import.MethodTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) methodTable.GetName(rid);
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
      MethodTable methodTable = record.Import.MethodTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) methodTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
    }
  }

  public MemoryBlock GetBody()
  {
    MetadataRecord record = this.m_record;
    MethodTable methodTable = record.Import.MethodTable;
    record = this.m_record;
    int rid = record.Rid;
    uint rva = methodTable.GetRVA(rid);
    return rva == 0U ? (MemoryBlock) null : this.m_record.Import.RvaToMemoryBlock(rva, 0U);
  }

  public TypeDef FindDeclaringType()
  {
    MetadataRecord record = this.m_record;
    TypeDefTable typeDefTable = record.Import.TypeDefTable;
    record = this.m_record;
    int rid = record.Rid;
    record = this.m_record;
    int numberOfRows = record.Import.MethodTable.NumberOfRows;
    return new MetadataRecord(new MetadataToken(MetadataTokenType.TypeDef, typeDefTable.FindTypeContainingMethod(rid, numberOfRows)), this.m_record.Tables).TypeDef;
  }

  public int GetGenericParameterCount()
  {
    MetadataRecord record = this.m_record;
    GenericParamTable genericParamTable = record.Import.GenericParamTable;
    record = this.m_record;
    int rid = record.Rid;
    int genericParameterCount;
    ref int local = ref genericParameterCount;
    genericParamTable.FindGenericParametersForMethod(rid, out local);
    return genericParameterCount;
  }

  public static implicit operator MetadataRecord(MethodDef methodDef) => methodDef.m_record;

  public static explicit operator MethodDef(MetadataRecord record) => record.MethodDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView Parameters
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.ParamDef);
  }

  public MetadataTableView GenericParameters
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.GenericPar);
  }

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
