// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.SignatureDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct SignatureDef
{
  private readonly MetadataRecord m_record;

  internal SignatureDef(MetadataRecord record) => this.m_record = record;

  public MemoryBlock Signature
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataImport import = record.Import;
      record = this.m_record;
      StandAloneSigTable standAloneSigTable = record.Import.StandAloneSigTable;
      record = this.m_record;
      int rid = record.Rid;
      int signature = (int) standAloneSigTable.GetSignature(rid);
      return import.GetBlobBlock((uint) signature);
    }
  }

  public static implicit operator MetadataRecord(SignatureDef signatureDef)
  {
    return signatureDef.m_record;
  }

  public static explicit operator SignatureDef(MetadataRecord record) => record.SignatureDef;

  public MetadataRecord Record => this.m_record;
}
