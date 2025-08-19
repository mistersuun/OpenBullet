// Decompiled with JetBrains decompiler
// Type: LiteDB.DataBlock
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

internal class DataBlock
{
  public const int DATA_BLOCK_FIXED_SIZE = 8;

  public PageAddress Position { get; set; }

  public uint ExtendPageID { get; set; }

  public byte[] Data { get; set; }

  public DataPage Page { get; set; }

  public int Length => 8 + this.Data.Length;

  public DataBlock()
  {
    this.Position = PageAddress.Empty;
    this.ExtendPageID = uint.MaxValue;
    this.Data = new byte[0];
  }
}
