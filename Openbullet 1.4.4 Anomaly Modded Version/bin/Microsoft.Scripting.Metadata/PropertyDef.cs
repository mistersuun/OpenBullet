// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.PropertyDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct PropertyDef
{
  private readonly MetadataRecord m_record;

  internal PropertyDef(MetadataRecord record) => this.m_record = record;

  public PropertyAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      PropertyTable propertyTable = record.Import.PropertyTable;
      record = this.m_record;
      int rid = record.Rid;
      return propertyTable.GetFlags(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      PropertyTable propertyTable = record.Import.PropertyTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) propertyTable.GetName(rid);
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
      PropertyTable propertyTable = record.Import.PropertyTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) propertyTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
    }
  }

  public PropertyAccessors GetAccessors()
  {
    MetadataImport import = this.m_record.Import;
    int methodCount;
    int methodsForProperty = import.MethodSemanticsTable.FindSemanticMethodsForProperty(this.m_record.Rid, out methodCount);
    uint rowId1 = 0;
    uint rowId2 = 0;
    for (ushort index = 0; (int) index < methodCount; ++index)
    {
      switch (import.MethodSemanticsTable.GetFlags(methodsForProperty))
      {
        case MethodSemanticsFlags.Setter:
          rowId2 = import.MethodSemanticsTable.GetMethodRid(methodsForProperty);
          break;
        case MethodSemanticsFlags.Getter:
          rowId1 = import.MethodSemanticsTable.GetMethodRid(methodsForProperty);
          break;
      }
      ++methodsForProperty;
    }
    return new PropertyAccessors(this, new MetadataToken(MetadataTokenType.MethodDef, rowId1), new MetadataToken(MetadataTokenType.MethodDef, rowId2));
  }

  public object GetDefaultValue()
  {
    MetadataRecord record = this.m_record;
    MetadataImport import = record.Import;
    record = this.m_record;
    MetadataToken token = record.Token;
    return import.GetDefaultValue(token);
  }

  public TypeDef FindDeclaringType()
  {
    MetadataRecord record = this.m_record;
    PropertyMapTable propertyMapTable = record.Import.PropertyMapTable;
    record = this.m_record;
    int rid = record.Rid;
    record = this.m_record;
    int numberOfRows = record.Import.PropertyTable.NumberOfRows;
    return new MetadataRecord(new MetadataToken(MetadataTokenType.TypeDef, propertyMapTable.FindTypeContainingProperty(rid, numberOfRows)), this.m_record.Tables).TypeDef;
  }

  public static implicit operator MetadataRecord(PropertyDef propertyDef) => propertyDef.m_record;

  public static explicit operator PropertyDef(MetadataRecord record) => record.PropertyDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
