// Decompiled with JetBrains decompiler
// Type: LiteDB.ExtendPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

internal class ExtendPage(uint pageID) : BasePage(pageID)
{
  private byte[] _data = new byte[0];

  public override PageType PageType => PageType.Extend;

  public void SetData(byte[] data, int offset, int length)
  {
    this.ItemCount = length;
    this.FreeBytes = 4071 - length;
    this._data = new byte[length];
    Buffer.BlockCopy((Array) data, offset, (Array) this._data, 0, length);
  }

  public byte[] GetData() => this._data;

  protected override void ReadContent(ByteReader reader)
  {
    this._data = reader.ReadBytes(this.ItemCount);
  }

  protected override void WriteContent(ByteWriter writer) => writer.Write(this._data);
}
