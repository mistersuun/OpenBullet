// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FileDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct FileDef
{
  private readonly MetadataRecord m_record;

  internal FileDef(MetadataRecord record) => this.m_record = record;

  public AssemblyFileAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      FileTable fileTable = record.Import.FileTable;
      record = this.m_record;
      int rid = record.Rid;
      return fileTable.GetFlags(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      FileTable fileTable = record.Import.FileTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) fileTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public byte[] GetHashValue()
  {
    MetadataRecord record = this.m_record;
    MetadataImport import = record.Import;
    FileTable fileTable = import.FileTable;
    record = this.m_record;
    int rid = record.Rid;
    return import.GetBlob(fileTable.GetHashValue(rid));
  }

  public static implicit operator MetadataRecord(FileDef fileDef) => fileDef.m_record;

  public static explicit operator FileDef(MetadataRecord record) => record.FileDef;

  public MetadataRecord Record => this.m_record;
}
