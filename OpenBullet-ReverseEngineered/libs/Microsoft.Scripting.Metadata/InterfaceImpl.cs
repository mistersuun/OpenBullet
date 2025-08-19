// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.InterfaceImpl
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct InterfaceImpl
{
  private readonly MetadataRecord m_record;

  internal InterfaceImpl(MetadataRecord record) => this.m_record = record;

  public TypeDef ImplementingType
  {
    get
    {
      MetadataRecord record = this.m_record;
      InterfaceImplTable interfaceImplTable = record.Import.InterfaceImplTable;
      record = this.m_record;
      int rid = record.Rid;
      return new MetadataRecord(new MetadataToken(MetadataTokenType.TypeDef, interfaceImplTable.GetClass(rid)), this.m_record.Tables).TypeDef;
    }
  }

  public MetadataRecord InterfaceType
  {
    get
    {
      MetadataRecord record = this.m_record;
      InterfaceImplTable interfaceImplTable = record.Import.InterfaceImplTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken token = interfaceImplTable.GetInterface(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(token, tables);
    }
  }

  public static implicit operator MetadataRecord(InterfaceImpl paramDef) => paramDef.m_record;

  public static explicit operator InterfaceImpl(MetadataRecord record) => record.InterfaceImpl;

  public MetadataRecord Record => this.m_record;
}
