// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.ExtendPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;

#nullable disable
namespace LiteDB_V6;

internal class ExtendPage : BasePage
{
  public override PageType PageType => PageType.Extend;

  public byte[] Data { get; set; }

  public ExtendPage(uint pageID)
    : base(pageID)
  {
    this.Data = new byte[0];
  }

  protected override void ReadContent(ByteReader reader)
  {
    this.Data = reader.ReadBytes(this.ItemCount);
  }
}
