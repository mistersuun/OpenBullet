// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodSpec
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct MethodSpec
{
  private readonly MetadataRecord m_record;

  internal MethodSpec(MetadataRecord record) => this.m_record = record;

  public MetadataRecord GenericMethod
  {
    get
    {
      MetadataRecord record = this.m_record;
      MethodSpecTable methodSpecTable = record.Import.MethodSpecTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken genericMethod = methodSpecTable.GetGenericMethod(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(genericMethod, tables);
    }
  }

  public MemoryBlock Signature
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataImport import = record.Import;
      record = this.m_record;
      MethodSpecTable methodSpecTable = record.Import.MethodSpecTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) methodSpecTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
    }
  }

  public static implicit operator MetadataRecord(MethodSpec methodSpec) => methodSpec.m_record;

  public static explicit operator MethodSpec(MetadataRecord record) => record.MethodSpec;

  public MetadataRecord Record => this.m_record;
}
