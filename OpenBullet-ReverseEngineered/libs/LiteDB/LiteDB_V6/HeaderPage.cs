// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.HeaderPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB_V6;

internal class HeaderPage : BasePage
{
  private const string HEADER_INFO = "** This is a LiteDB file **";
  private const byte FILE_VERSION = 6;
  public uint FreeEmptyPageID;
  public ushort DbVersion;
  public byte[] Password = new byte[20];

  public override PageType PageType => PageType.Header;

  public ushort ChangeID { get; set; }

  public uint LastPageID { get; set; }

  public Dictionary<string, uint> CollectionPages { get; set; }

  public HeaderPage()
    : base(0U)
  {
    this.FreeEmptyPageID = uint.MaxValue;
    this.ChangeID = (ushort) 0;
    this.LastPageID = 0U;
    this.ItemCount = 1;
    this.DbVersion = (ushort) 0;
    this.Password = new byte[20];
    this.CollectionPages = new Dictionary<string, uint>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }

  protected override void ReadContent(ByteReader reader)
  {
    reader.ReadString("** This is a LiteDB file **".Length);
    int num1 = (int) reader.ReadByte();
    this.ChangeID = reader.ReadUInt16();
    this.FreeEmptyPageID = reader.ReadUInt32();
    this.LastPageID = reader.ReadUInt32();
    this.DbVersion = reader.ReadUInt16();
    this.Password = reader.ReadBytes(this.Password.Length);
    byte num2 = reader.ReadByte();
    for (int index = 0; index < (int) num2; ++index)
      this.CollectionPages.Add(reader.ReadString(), reader.ReadUInt32());
  }
}
