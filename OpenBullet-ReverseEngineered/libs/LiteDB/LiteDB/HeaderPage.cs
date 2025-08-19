// Decompiled with JetBrains decompiler
// Type: LiteDB.HeaderPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class HeaderPage : BasePage
{
  private const string HEADER_INFO = "** This is a LiteDB file **";
  private const byte FILE_VERSION = 7;
  public uint FreeEmptyPageID;

  public override PageType PageType => PageType.Header;

  public ushort ChangeID { get; set; }

  public uint LastPageID { get; set; }

  public ushort UserVersion { get; set; }

  public byte[] Password { get; set; }

  public byte[] Salt { get; set; }

  public bool Recovery { get; set; }

  public Dictionary<string, uint> CollectionPages { get; set; }

  public HeaderPage()
    : base(0U)
  {
    this.ChangeID = (ushort) 0;
    this.FreeEmptyPageID = uint.MaxValue;
    this.LastPageID = 0U;
    this.ItemCount = 1;
    this.FreeBytes = 0;
    this.UserVersion = (ushort) 0;
    this.Password = new byte[20];
    this.Salt = new byte[16 /*0x10*/];
    this.CollectionPages = new Dictionary<string, uint>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }

  protected override void ReadContent(ByteReader reader)
  {
    string str = reader.ReadString("** This is a LiteDB file **".Length);
    byte version = reader.ReadByte();
    if (str != "** This is a LiteDB file **")
      throw LiteException.InvalidDatabase();
    if (version != (byte) 7)
      throw LiteException.InvalidDatabaseVersion((int) version);
    this.ChangeID = reader.ReadUInt16();
    this.FreeEmptyPageID = reader.ReadUInt32();
    this.LastPageID = reader.ReadUInt32();
    this.UserVersion = reader.ReadUInt16();
    this.Password = reader.ReadBytes(this.Password.Length);
    this.Salt = reader.ReadBytes(this.Salt.Length);
    byte num = reader.ReadByte();
    for (int index = 0; index < (int) num; ++index)
      this.CollectionPages.Add(reader.ReadString(), reader.ReadUInt32());
    reader.Position = 4095 /*0x0FFF*/;
    this.Recovery = reader.ReadBoolean();
  }

  protected override void WriteContent(ByteWriter writer)
  {
    writer.Write("** This is a LiteDB file **", "** This is a LiteDB file **".Length);
    writer.Write((byte) 7);
    writer.Write(this.ChangeID);
    writer.Write(this.FreeEmptyPageID);
    writer.Write(this.LastPageID);
    writer.Write(this.UserVersion);
    writer.Write(this.Password);
    writer.Write(this.Salt);
    writer.Write((byte) this.CollectionPages.Count);
    foreach (string key in this.CollectionPages.Keys)
    {
      writer.Write(key);
      writer.Write(this.CollectionPages[key]);
    }
    writer.Position = 4095 /*0x0FFF*/;
    writer.Write(this.Recovery);
  }
}
