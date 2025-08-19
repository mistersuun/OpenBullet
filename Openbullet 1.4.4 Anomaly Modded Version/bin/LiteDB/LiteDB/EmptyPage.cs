// Decompiled with JetBrains decompiler
// Type: LiteDB.EmptyPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

internal class EmptyPage : BasePage
{
  public override PageType PageType => PageType.Empty;

  public EmptyPage(uint pageID)
    : base(pageID)
  {
    this.ItemCount = 0;
    this.FreeBytes = 4071;
  }

  public EmptyPage(BasePage page)
    : base(page.PageID)
  {
    if (page.DiskData.Length == 0)
      return;
    this.DiskData = new byte[4096 /*0x1000*/];
    Buffer.BlockCopy((Array) page.DiskData, 0, (Array) this.DiskData, 0, 4096 /*0x1000*/);
  }

  protected override void ReadContent(ByteReader reader)
  {
  }

  protected override void WriteContent(ByteWriter writer)
  {
  }
}
