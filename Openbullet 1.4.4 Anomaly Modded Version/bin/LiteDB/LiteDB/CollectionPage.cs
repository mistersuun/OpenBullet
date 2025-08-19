// Decompiled with JetBrains decompiler
// Type: LiteDB.CollectionPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

internal class CollectionPage : BasePage
{
  public const ushort MAX_COLLECTIONS_SIZE = 3000;
  public static Regex NamePattern = new Regex("^[\\w-]{1,60}$", RegexOptions.Compiled);
  public uint FreeDataPageID;

  public override PageType PageType => PageType.Collection;

  public string CollectionName { get; set; }

  public long DocumentCount { get; set; }

  public CollectionIndex[] Indexes { get; set; }

  public long Sequence { get; set; }

  public CollectionPage(uint pageID)
    : base(pageID)
  {
    this.FreeDataPageID = uint.MaxValue;
    this.DocumentCount = 0L;
    this.ItemCount = 1;
    this.FreeBytes = 0;
    this.Indexes = new CollectionIndex[16 /*0x10*/];
    this.Sequence = 0L;
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
    this.DocumentCount = reader.ReadInt64();
    this.FreeDataPageID = reader.ReadUInt32();
    foreach (CollectionIndex index in this.Indexes)
    {
      string str = reader.ReadString();
      int length = str.IndexOf('=');
      if (length > 0)
      {
        index.Field = str.Substring(0, length);
        index.Expression = str.Substring(length + 1);
      }
      else
      {
        index.Field = str;
        index.Expression = "$." + str;
      }
      index.Unique = reader.ReadBoolean();
      index.HeadNode = reader.ReadPageAddress();
      index.TailNode = reader.ReadPageAddress();
      index.FreeIndexPageID = reader.ReadUInt32();
    }
    reader.Position = 4072;
    foreach (CollectionIndex index in this.Indexes)
    {
      byte num1 = reader.ReadByte();
      int num2 = num1 == (byte) 0 ? 32 /*0x20*/ : (int) num1;
      index.MaxLevel = (byte) num2;
    }
    this.Sequence = reader.ReadInt64();
  }

  protected override void WriteContent(ByteWriter writer)
  {
    writer.Write(this.CollectionName);
    writer.Write(this.DocumentCount);
    writer.Write(this.FreeDataPageID);
    foreach (CollectionIndex index in this.Indexes)
    {
      if (index.Field.Length > 0)
        writer.Write($"{index.Field}={index.Expression}");
      else
        writer.Write("");
      writer.Write(index.Unique);
      writer.Write(index.HeadNode);
      writer.Write(index.TailNode);
      writer.Write(index.FreeIndexPageID);
    }
    writer.Position = 4072;
    foreach (CollectionIndex index in this.Indexes)
      writer.Write(index.MaxLevel);
    writer.Write(this.Sequence);
  }

  public CollectionIndex GetFreeIndex()
  {
    for (byte index = 0; (int) index < this.Indexes.Length; ++index)
    {
      if (this.Indexes[(int) index].IsEmpty)
        return this.Indexes[(int) index];
    }
    throw LiteException.IndexLimitExceeded(this.CollectionName);
  }

  public CollectionIndex GetIndex(string field)
  {
    return ((IEnumerable<CollectionIndex>) this.Indexes).FirstOrDefault<CollectionIndex>((Func<CollectionIndex, bool>) (x => x.Field == field));
  }

  public CollectionIndex PK => this.Indexes[0];

  public IEnumerable<CollectionIndex> GetIndexes(bool includePK)
  {
    return ((IEnumerable<CollectionIndex>) this.Indexes).Where<CollectionIndex>((Func<CollectionIndex, bool>) (x => !x.IsEmpty && x.Slot >= (includePK ? 0 : 1)));
  }
}
