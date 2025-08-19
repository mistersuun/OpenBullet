// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonReader
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Text;

#nullable disable
namespace LiteDB;

internal class BsonReader
{
  private bool _utcDate;

  public BsonReader(bool utcDate) => this._utcDate = utcDate;

  public BsonDocument Deserialize(byte[] bson) => this.ReadDocument(new ByteReader(bson));

  public BsonDocument ReadDocument(ByteReader reader)
  {
    int num1 = reader.ReadInt32();
    int num2 = reader.Position + num1 - 5;
    BsonDocument bsonDocument = new BsonDocument();
    while (reader.Position < num2)
    {
      string name;
      BsonValue bsonValue = this.ReadElement(reader, out name);
      bsonDocument.RawValue[name] = bsonValue;
    }
    int num3 = (int) reader.ReadByte();
    return bsonDocument;
  }

  public BsonArray ReadArray(ByteReader reader)
  {
    int num1 = reader.ReadInt32();
    int num2 = reader.Position + num1 - 5;
    BsonArray bsonArray = new BsonArray();
    while (reader.Position < num2)
    {
      BsonValue bsonValue = this.ReadElement(reader, out string _);
      bsonArray.Add(bsonValue);
    }
    int num3 = (int) reader.ReadByte();
    return bsonArray;
  }

  private BsonValue ReadElement(ByteReader reader, out string name)
  {
    byte num1 = reader.ReadByte();
    name = this.ReadCString(reader);
    switch (num1)
    {
      case 1:
        return (BsonValue) reader.ReadDouble();
      case 2:
        return (BsonValue) this.ReadString(reader);
      case 3:
        return (BsonValue) this.ReadDocument(reader);
      case 4:
        return (BsonValue) this.ReadArray(reader);
      case 5:
        int count = reader.ReadInt32();
        byte num2 = reader.ReadByte();
        byte[] b = reader.ReadBytes(count);
        if (num2 == (byte) 0)
          return (BsonValue) b;
        if (num2 == (byte) 4)
          return (BsonValue) new Guid(b);
        break;
      case 7:
        return (BsonValue) new ObjectId(reader.ReadBytes(12));
      case 8:
        return (BsonValue) reader.ReadBoolean();
      case 9:
        long num3 = reader.ReadInt64();
        switch (num3)
        {
          case -62135596800000:
            return (BsonValue) DateTime.MinValue;
          case 253402300800000:
            return (BsonValue) DateTime.MaxValue;
          default:
            DateTime dateTime = BsonValue.UnixEpoch.AddMilliseconds((double) num3);
            return (BsonValue) (this._utcDate ? dateTime : dateTime.ToLocalTime());
        }
      case 10:
        return BsonValue.Null;
      case 16 /*0x10*/:
        return (BsonValue) reader.ReadInt32();
      case 18:
        return (BsonValue) reader.ReadInt64();
      case 19:
        return (BsonValue) reader.ReadDecimal();
      case 127 /*0x7F*/:
        return BsonValue.MaxValue;
      case byte.MaxValue:
        return BsonValue.MinValue;
    }
    throw new NotSupportedException("BSON type not supported");
  }

  private string ReadString(ByteReader reader)
  {
    int num1 = reader.ReadInt32();
    byte[] bytes = reader.ReadBytes(num1 - 1);
    int num2 = (int) reader.ReadByte();
    return Encoding.UTF8.GetString(bytes, 0, num1 - 1);
  }

  private string ReadCString(ByteReader reader)
  {
    int count = 0;
    byte[] bytes = new byte[200];
    while (true)
    {
      byte num = reader.ReadByte();
      if (num != (byte) 0 && count != 200)
        bytes[count++] = num;
      else
        break;
    }
    return Encoding.UTF8.GetString(bytes, 0, count);
  }
}
