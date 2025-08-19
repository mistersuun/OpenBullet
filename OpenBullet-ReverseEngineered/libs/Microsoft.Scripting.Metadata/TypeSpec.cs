// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeSpec
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct TypeSpec
{
  private readonly MetadataRecord m_record;

  internal TypeSpec(MetadataRecord record) => this.m_record = record;

  public MemoryBlock Signature
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataImport import = record.Import;
      record = this.m_record;
      TypeSpecTable typeSpecTable = record.Import.TypeSpecTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) typeSpecTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
    }
  }

  public static implicit operator MetadataRecord(TypeSpec typeSpec) => typeSpec.m_record;

  public static explicit operator TypeSpec(MetadataRecord record) => record.TypeSpec;

  public MetadataRecord Record => this.m_record;
}
