// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.DataBlock
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;

#nullable disable
namespace LiteDB_V6;

internal class DataBlock
{
  public PageAddress Position { get; set; }

  public PageAddress[] IndexRef { get; set; }

  public uint ExtendPageID { get; set; }

  public byte[] Data { get; set; }

  public DataPage Page { get; set; }

  public DataBlock()
  {
    this.Position = PageAddress.Empty;
    this.ExtendPageID = uint.MaxValue;
    this.Data = new byte[0];
    this.IndexRef = new PageAddress[16 /*0x10*/];
    for (int index = 0; index < 16 /*0x10*/; ++index)
      this.IndexRef[index] = PageAddress.Empty;
  }
}
