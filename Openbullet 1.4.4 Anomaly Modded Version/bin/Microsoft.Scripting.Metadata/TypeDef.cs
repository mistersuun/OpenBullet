// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct TypeDef
{
  private readonly MetadataRecord m_record;

  internal TypeDef(MetadataRecord record) => this.m_record = record;

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      TypeDefTable typeDefTable = record.Import.TypeDefTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) typeDefTable.GetName(rid);
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
      TypeDefTable typeDefTable = record.Import.TypeDefTable;
      record = this.m_record;
      int rid = record.Rid;
      int blob = (int) typeDefTable.GetNamespace(rid);
      return tables.ToMetadataName((uint) blob);
    }
  }

  public TypeAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      TypeDefTable typeDefTable = record.Import.TypeDefTable;
      record = this.m_record;
      int rid = record.Rid;
      return typeDefTable.GetFlags(rid);
    }
  }

  public MetadataRecord BaseType
  {
    get
    {
      MetadataRecord record = this.m_record;
      TypeDefTable typeDefTable = record.Import.TypeDefTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken extends = typeDefTable.GetExtends(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(extends, tables);
    }
  }

  public TypeDef FindDeclaringType()
  {
    MetadataRecord record = this.m_record;
    NestedClassTable nestedClassTable = record.Import.NestedClassTable;
    record = this.m_record;
    int rid = record.Rid;
    return new MetadataRecord(new MetadataToken(MetadataTokenType.TypeDef, nestedClassTable.FindParentTypeDefRowId(rid)), this.m_record.Tables).TypeDef;
  }

  public int GetGenericParameterCount()
  {
    MetadataRecord record = this.m_record;
    GenericParamTable genericParamTable = record.Import.GenericParamTable;
    record = this.m_record;
    int rid = record.Rid;
    int genericParameterCount;
    ref int local = ref genericParameterCount;
    genericParamTable.FindGenericParametersForType(rid, out local);
    return genericParameterCount;
  }

  public static implicit operator MetadataRecord(TypeDef typeDef) => typeDef.m_record;

  public static explicit operator TypeDef(MetadataRecord record) => record.TypeDef;

  public MetadataRecord Record => this.m_record;

  public bool IsGlobal => this.m_record.m_token.Rid == 1;

  public MetadataTableView ImplementedInterfaces
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.InterfaceImpl);
  }

  public MetadataTableView GenericParameters
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.GenericPar);
  }

  public MetadataTableView Fields
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.FieldDef);
  }

  public MetadataTableView Methods
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.MethodDef);
  }

  public MetadataTableView Properties
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.Property);
  }

  public MetadataTableView Events => new MetadataTableView(this.m_record, MetadataTokenType.Event);

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
