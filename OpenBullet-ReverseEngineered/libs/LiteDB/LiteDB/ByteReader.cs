// Decompiled with JetBrains decompiler
// Type: LiteDB.ByteReader
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Text;

#nullable disable
namespace LiteDB;

internal class ByteReader
{
  private byte[] _buffer;
  private int _pos;

  public int Position
  {
    get => this._pos;
    set => this._pos = value;
  }

  public ByteReader(byte[] buffer)
  {
    this._buffer = buffer;
    this._pos = 0;
  }

  public void Skip(int length) => this._pos += length;

  public byte ReadByte()
  {
    int num = (int) this._buffer[this._pos];
    ++this._pos;
    return (byte) num;
  }

  public bool ReadBoolean()
  {
    int num = (int) this._buffer[this._pos];
    ++this._pos;
    return num != 0;
  }

  public ushort ReadUInt16()
  {
    this._pos += 2;
    return BitConverter.ToUInt16(this._buffer, this._pos - 2);
  }

  public uint ReadUInt32()
  {
    this._pos += 4;
    return BitConverter.ToUInt32(this._buffer, this._pos - 4);
  }

  public ulong ReadUInt64()
  {
    this._pos += 8;
    return BitConverter.ToUInt64(this._buffer, this._pos - 8);
  }

  public short ReadInt16()
  {
    this._pos += 2;
    return BitConverter.ToInt16(this._buffer, this._pos - 2);
  }

  public int ReadInt32()
  {
    this._pos += 4;
    return BitConverter.ToInt32(this._buffer, this._pos - 4);
  }

  public long ReadInt64()
  {
    this._pos += 8;
    return BitConverter.ToInt64(this._buffer, this._pos - 8);
  }

  public float ReadSingle()
  {
    this._pos += 4;
    return BitConverter.ToSingle(this._buffer, this._pos - 4);
  }

  public double ReadDouble()
  {
    this._pos += 8;
    return BitConverter.ToDouble(this._buffer, this._pos - 8);
  }

  public Decimal ReadDecimal()
  {
    this._pos += 16 /*0x10*/;
    return new Decimal(new int[4]
    {
      BitConverter.ToInt32(this._buffer, this._pos - 16 /*0x10*/),
      BitConverter.ToInt32(this._buffer, this._pos - 12),
      BitConverter.ToInt32(this._buffer, this._pos - 8),
      BitConverter.ToInt32(this._buffer, this._pos - 4)
    });
  }

  public byte[] ReadBytes(int count)
  {
    byte[] dst = new byte[count];
    Buffer.BlockCopy((Array) this._buffer, this._pos, (Array) dst, 0, count);
    this._pos += count;
    return dst;
  }

  public string ReadString()
  {
    int count = this.ReadInt32();
    return Encoding.UTF8.GetString(this.ReadBytes(count), 0, count);
  }

  public string ReadString(int length)
  {
    return Encoding.UTF8.GetString(this.ReadBytes(length), 0, length);
  }

  public DateTime ReadDateTime() => new DateTime(this.ReadInt64(), DateTimeKind.Utc).ToLocalTime();

  public Guid ReadGuid() => new Guid(this.ReadBytes(16 /*0x10*/));

  public ObjectId ReadObjectId() => new ObjectId(this.ReadBytes(12));

  public PageAddress ReadPageAddress() => new PageAddress(this.ReadUInt32(), this.ReadUInt16());

  public BsonValue ReadBsonValue(ushort length)
  {
    switch (this.ReadByte())
    {
      case 0:
        return BsonValue.MinValue;
      case 1:
        return BsonValue.Null;
      case 2:
        return (BsonValue) this.ReadInt32();
      case 3:
        return (BsonValue) this.ReadInt64();
      case 4:
        return (BsonValue) this.ReadDouble();
      case 5:
        return (BsonValue) this.ReadDecimal();
      case 6:
        return (BsonValue) this.ReadString((int) length);
      case 7:
        return (BsonValue) new BsonReader(false).ReadDocument(this);
      case 8:
        return (BsonValue) new BsonReader(false).ReadArray(this);
      case 9:
        return (BsonValue) this.ReadBytes((int) length);
      case 10:
        return (BsonValue) this.ReadObjectId();
      case 11:
        return (BsonValue) this.ReadGuid();
      case 12:
        return (BsonValue) this.ReadBoolean();
      case 13:
        return (BsonValue) this.ReadDateTime();
      case 14:
        return BsonValue.MaxValue;
      default:
        throw new NotImplementedException();
    }
  }
}
