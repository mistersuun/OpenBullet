// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.CollectionPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System;

#nullable disable
namespace LiteDB_V6;

internal class CollectionPage : BasePage
{
  public uint FreeDataPageID;

  public override PageType PageType => PageType.Collection;

  public string CollectionName { get; set; }

  public long DocumentCount { get; set; }

  public CollectionIndex[] Indexes { get; set; }

  public CollectionPage(uint pageID)
    : base(pageID)
  {
    this.FreeDataPageID = uint.MaxValue;
    this.DocumentCount = 0L;
    this.ItemCount = 1;
    this.Indexes = new CollectionIndex[16 /*0x10*/];
    for (int index = 0; index < this.Indexes.Length; ++index)
      this.Indexes[index] = new CollectionIndex()
      {
        Page = this,
        Slot = index
      };
  }

  protected override void ReadContent(ByteReader reader)
  {
    this.CollectionName = reader.ReadString();
    this.FreeDataPageID = reader.ReadUInt32();
    uint val1 = reader.ReadUInt32();
    foreach (CollectionIndex index in this.Indexes)
    {
      index.Field = reader.ReadString();
      index.HeadNode = reader.ReadPageAddress();
      index.TailNode = reader.ReadPageAddress();
      index.FreeIndexPageID = reader.ReadUInt32();
      index.Unique = reader.ReadBoolean();
      reader.ReadBoolean();
      reader.ReadBoolean();
      reader.ReadBoolean();
      reader.ReadBoolean();
    }
    long val2 = reader.ReadInt64();
    this.DocumentCount = Math.Max((long) val1, val2);
  }

  public CollectionIndex PK => this.Indexes[0];
}
