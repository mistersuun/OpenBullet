// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MemberRef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct MemberRef
{
  private readonly MetadataRecord m_record;

  internal MemberRef(MetadataRecord record) => this.m_record = record;

  public MetadataRecord Class
  {
    get
    {
      MetadataRecord record = this.m_record;
      MemberRefTable memberRefTable = record.Import.MemberRefTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken token = memberRefTable.GetClass(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(token, tables);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      MemberRefTable memberRefTable = record.Import.MemberRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) memberRefTable.GetName(rid);
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
      MemberRefTable memberRefTable = record.Import.MemberRefTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) memberRefTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
    }
  }

  public static implicit operator MetadataRecord(MemberRef memberRef) => memberRef.m_record;

  public static explicit operator MemberRef(MetadataRecord record) => record.MemberRef;

  public MetadataRecord Record => this.m_record;
}
