// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.IndexPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB_V6;

internal class IndexPage : BasePage
{
  public override PageType PageType => PageType.Index;

  public Dictionary<ushort, IndexNode> Nodes { get; set; }

  public IndexPage(uint pageID)
    : base(pageID)
  {
    this.Nodes = new Dictionary<ushort, IndexNode>();
  }

  protected override void ReadContent(ByteReader reader)
  {
    this.Nodes = new Dictionary<ushort, IndexNode>(this.ItemCount);
    for (int index1 = 0; index1 < this.ItemCount; ++index1)
    {
      ushort index2 = reader.ReadUInt16();
      IndexNode indexNode = new IndexNode(reader.ReadByte())
      {
        Page = this,
        Position = new PageAddress(this.PageID, index2),
        KeyLength = reader.ReadUInt16()
      };
      indexNode.Key = this.ReadBsonValue(reader, indexNode.KeyLength);
      indexNode.DataBlock = reader.ReadPageAddress();
      for (int index3 = 0; index3 < indexNode.Prev.Length; ++index3)
      {
        indexNode.Prev[index3] = reader.ReadPageAddress();
        indexNode.Next[index3] = reader.ReadPageAddress();
      }
      this.Nodes.Add(indexNode.Position.Index, indexNode);
    }
  }

  private BsonValue ReadBsonValue(ByteReader reader, ushort length)
  {
    switch (reader.ReadByte())
    {
      case 0:
        return BsonValue.MinValue;
      case 1:
        return BsonValue.Null;
      case 2:
        return (BsonValue) reader.ReadInt32();
      case 3:
        return (BsonValue) reader.ReadInt64();
      case 4:
        return (BsonValue) reader.ReadDouble();
      case 5:
        return (BsonValue) reader.ReadString((int) length);
      case 6:
        return (BsonValue) new BsonReader(false).ReadDocument(reader);
      case 7:
        return (BsonValue) new BsonReader(false).ReadArray(reader);
      case 8:
        return (BsonValue) reader.ReadBytes((int) length);
      case 9:
        return (BsonValue) reader.ReadObjectId();
      case 10:
        return (BsonValue) reader.ReadGuid();
      case 11:
        return (BsonValue) reader.ReadBoolean();
      case 12:
        return (BsonValue) reader.ReadDateTime();
      case 13:
        return BsonValue.MaxValue;
      default:
        throw new NotImplementedException();
    }
  }
}
