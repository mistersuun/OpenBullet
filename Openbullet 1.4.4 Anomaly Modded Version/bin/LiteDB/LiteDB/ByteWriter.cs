// Decompiled with JetBrains decompiler
// Type: LiteDB.ByteWriter
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Text;

#nullable disable
namespace LiteDB;

internal class ByteWriter
{
  private byte[] _buffer;
  private int _pos;

  public byte[] Buffer => this._buffer;

  public int Position
  {
    get => this._pos;
    set => this._pos = value;
  }

  public ByteWriter(int length)
  {
    this._buffer = new byte[length];
    this._pos = 0;
  }

  public ByteWriter(byte[] buffer)
  {
    this._buffer = buffer;
    this._pos = 0;
  }

  public void Skip(int length) => this._pos += length;

  public void Write(byte value)
  {
    this._buffer[this._pos] = value;
    ++this._pos;
  }

  public void Write(bool value)
  {
    this._buffer[this._pos] = value ? (byte) 1 : (byte) 0;
    ++this._pos;
  }

  public void Write(ushort value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._pos += 2;
  }

  public void Write(uint value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._buffer[this._pos + 2] = bytes[2];
    this._buffer[this._pos + 3] = bytes[3];
    this._pos += 4;
  }

  public void Write(ulong value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._buffer[this._pos + 2] = bytes[2];
    this._buffer[this._pos + 3] = bytes[3];
    this._buffer[this._pos + 4] = bytes[4];
    this._buffer[this._pos + 5] = bytes[5];
    this._buffer[this._pos + 6] = bytes[6];
    this._buffer[this._pos + 7] = bytes[7];
    this._pos += 8;
  }

  public void Write(short value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._pos += 2;
  }

  public void Write(int value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._buffer[this._pos + 2] = bytes[2];
    this._buffer[this._pos + 3] = bytes[3];
    this._pos += 4;
  }

  public void Write(long value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._buffer[this._pos + 2] = bytes[2];
    this._buffer[this._pos + 3] = bytes[3];
    this._buffer[this._pos + 4] = bytes[4];
    this._buffer[this._pos + 5] = bytes[5];
    this._buffer[this._pos + 6] = bytes[6];
    this._buffer[this._pos + 7] = bytes[7];
    this._pos += 8;
  }

  public void Write(float value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._buffer[this._pos + 2] = bytes[2];
    this._buffer[this._pos + 3] = bytes[3];
    this._pos += 4;
  }

  public void Write(double value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    this._buffer[this._pos] = bytes[0];
    this._buffer[this._pos + 1] = bytes[1];
    this._buffer[this._pos + 2] = bytes[2];
    this._buffer[this._pos + 3] = bytes[3];
    this._buffer[this._pos + 4] = bytes[4];
    this._buffer[this._pos + 5] = bytes[5];
    this._buffer[this._pos + 6] = bytes[6];
    this._buffer[this._pos + 7] = bytes[7];
    this._pos += 8;
  }

  public void Write(Decimal value)
  {
    int[] bits = Decimal.GetBits(value);
    this.Write(bits[0]);
    this.Write(bits[1]);
    this.Write(bits[2]);
    this.Write(bits[3]);
  }

  public void Write(byte[] value)
  {
    System.Buffer.BlockCopy((Array) value, 0, (Array) this._buffer, this._pos, value.Length);
    this._pos += value.Length;
  }

  public void Write(string value)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(value);
    this.Write(bytes.Length);
    this.Write(bytes);
  }

  public void Write(string value, int length)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(value);
    if (bytes.Length != length)
      throw new ArgumentException("Invalid string length");
    this.Write(bytes);
  }

  public void Write(DateTime value) => this.Write(value.ToUniversalTime().Ticks);

  public void Write(Guid value) => this.Write(value.ToByteArray());

  public void Write(ObjectId value) => this.Write(value.ToByteArray());

  public void Write(PageAddress value)
  {
    this.Write(value.PageID);
    this.Write(value.Index);
  }

  public void WriteBsonValue(BsonValue value, ushort length)
  {
    this.Write((byte) value.Type);
    switch (value.Type)
    {
      case BsonType.MinValue:
        break;
      case BsonType.Null:
        break;
      case BsonType.Int32:
        this.Write((int) value.RawValue);
        break;
      case BsonType.Int64:
        this.Write((long) value.RawValue);
        break;
      case BsonType.Double:
        this.Write((double) value.RawValue);
        break;
      case BsonType.Decimal:
        this.Write((Decimal) value.RawValue);
        break;
      case BsonType.String:
        this.Write((string) value.RawValue, (int) length);
        break;
      case BsonType.Document:
        new BsonWriter().WriteDocument(this, value.AsDocument);
        break;
      case BsonType.Array:
        new BsonWriter().WriteArray(this, value.AsArray);
        break;
      case BsonType.Binary:
        this.Write((byte[]) value.RawValue);
        break;
      case BsonType.ObjectId:
        this.Write((ObjectId) value.RawValue);
        break;
      case BsonType.Guid:
        this.Write((Guid) value.RawValue);
        break;
      case BsonType.Boolean:
        this.Write((bool) value.RawValue);
        break;
      case BsonType.DateTime:
        this.Write((DateTime) value.RawValue);
        break;
      case BsonType.MaxValue:
        break;
      default:
        throw new NotImplementedException();
    }
  }
}
