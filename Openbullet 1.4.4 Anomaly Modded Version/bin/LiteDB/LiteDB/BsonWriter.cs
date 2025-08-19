// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonWriter
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace LiteDB;

internal class BsonWriter
{
  public byte[] Serialize(BsonDocument doc)
  {
    ByteWriter writer = new ByteWriter(doc.GetBytesCount(true));
    this.WriteDocument(writer, doc);
    return writer.Buffer;
  }

  public void WriteDocument(ByteWriter writer, BsonDocument doc)
  {
    writer.Write(doc.GetBytesCount(false));
    foreach (string key1 in (IEnumerable<string>) doc.Keys)
    {
      ByteWriter writer1 = writer;
      string key2 = key1;
      BsonValue bsonValue = doc[key1];
      if ((object) bsonValue == null)
        bsonValue = BsonValue.Null;
      this.WriteElement(writer1, key2, bsonValue);
    }
    writer.Write((byte) 0);
  }

  public void WriteArray(ByteWriter writer, BsonArray array)
  {
    writer.Write(array.GetBytesCount(false));
    for (int index = 0; index < array.Count; ++index)
    {
      ByteWriter writer1 = writer;
      string key = index.ToString();
      BsonValue bsonValue = array[index];
      if ((object) bsonValue == null)
        bsonValue = BsonValue.Null;
      this.WriteElement(writer1, key, bsonValue);
    }
    writer.Write((byte) 0);
  }

  private void WriteElement(ByteWriter writer, string key, BsonValue value)
  {
    switch (value.Type)
    {
      case BsonType.MinValue:
        writer.Write(byte.MaxValue);
        this.WriteCString(writer, key);
        break;
      case BsonType.Null:
        writer.Write((byte) 10);
        this.WriteCString(writer, key);
        break;
      case BsonType.Int32:
        writer.Write((byte) 16 /*0x10*/);
        this.WriteCString(writer, key);
        writer.Write((int) value.RawValue);
        break;
      case BsonType.Int64:
        writer.Write((byte) 18);
        this.WriteCString(writer, key);
        writer.Write((long) value.RawValue);
        break;
      case BsonType.Double:
        writer.Write((byte) 1);
        this.WriteCString(writer, key);
        writer.Write((double) value.RawValue);
        break;
      case BsonType.Decimal:
        writer.Write((byte) 19);
        this.WriteCString(writer, key);
        writer.Write((Decimal) value.RawValue);
        break;
      case BsonType.String:
        writer.Write((byte) 2);
        this.WriteCString(writer, key);
        this.WriteString(writer, (string) value.RawValue);
        break;
      case BsonType.Document:
        writer.Write((byte) 3);
        this.WriteCString(writer, key);
        this.WriteDocument(writer, new BsonDocument((Dictionary<string, BsonValue>) value.RawValue));
        break;
      case BsonType.Array:
        writer.Write((byte) 4);
        this.WriteCString(writer, key);
        this.WriteArray(writer, new BsonArray((List<BsonValue>) value.RawValue));
        break;
      case BsonType.Binary:
        writer.Write((byte) 5);
        this.WriteCString(writer, key);
        byte[] rawValue1 = (byte[]) value.RawValue;
        writer.Write(rawValue1.Length);
        writer.Write((byte) 0);
        writer.Write(rawValue1);
        break;
      case BsonType.ObjectId:
        writer.Write((byte) 7);
        this.WriteCString(writer, key);
        writer.Write(((ObjectId) value.RawValue).ToByteArray());
        break;
      case BsonType.Guid:
        writer.Write((byte) 5);
        this.WriteCString(writer, key);
        byte[] byteArray = ((Guid) value.RawValue).ToByteArray();
        writer.Write(byteArray.Length);
        writer.Write((byte) 4);
        writer.Write(byteArray);
        break;
      case BsonType.Boolean:
        writer.Write((byte) 8);
        this.WriteCString(writer, key);
        writer.Write((bool) value.RawValue ? (byte) 1 : (byte) 0);
        break;
      case BsonType.DateTime:
        writer.Write((byte) 9);
        this.WriteCString(writer, key);
        DateTime rawValue2 = (DateTime) value.RawValue;
        TimeSpan timeSpan = (rawValue2 == DateTime.MinValue || rawValue2 == DateTime.MaxValue ? rawValue2 : rawValue2.ToUniversalTime()) - BsonValue.UnixEpoch;
        writer.Write(Convert.ToInt64(timeSpan.TotalMilliseconds));
        break;
      case BsonType.MaxValue:
        writer.Write((byte) 127 /*0x7F*/);
        this.WriteCString(writer, key);
        break;
    }
  }

  private void WriteString(ByteWriter writer, string s)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(s);
    writer.Write(bytes.Length + 1);
    writer.Write(bytes);
    writer.Write((byte) 0);
  }

  private void WriteCString(ByteWriter writer, string s)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(s);
    writer.Write(bytes);
    writer.Write((byte) 0);
  }
}
