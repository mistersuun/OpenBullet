// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MemoryReader
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public class MemoryReader
{
  private readonly MemoryBlock _block;
  private int _position;

  public MemoryReader(MemoryBlock block)
  {
    this._block = block != null ? block : throw new ArgumentNullException(nameof (block));
  }

  public MemoryBlock Block => this._block;

  public int Position => this._position;

  public int RemainingBytes => this._block.Length - this._position;

  public MemoryBlock GetRemainingBlock()
  {
    return this._block.GetRange(this._position, this.RemainingBytes);
  }

  public void Seek(int position)
  {
    if (position < 0 || position > this._block.Length)
      throw new BadImageFormatException();
    this._position = position;
  }

  public void SeekRelative(int offset) => this.Seek(this._position + offset);

  internal void Align(int alignment)
  {
    int num = this._position & alignment - 1;
    if (num == 0)
      return;
    this.SeekRelative(alignment - num);
  }

  public char ReadChar()
  {
    int num = (int) this._block.ReadChar(this._position);
    this._position += 2;
    return (char) num;
  }

  [CLSCompliant(false)]
  public sbyte ReadSByte()
  {
    int num = (int) this._block.ReadSByte(this._position);
    ++this._position;
    return (sbyte) num;
  }

  public short ReadInt16()
  {
    int num = (int) this._block.ReadInt16(this._position);
    this._position += 2;
    return (short) num;
  }

  public int ReadInt32()
  {
    int num = this._block.ReadInt32(this._position);
    this._position += 4;
    return num;
  }

  public long ReadInt64()
  {
    long num = this._block.ReadInt64(this._position);
    this._position += 8;
    return num;
  }

  public byte ReadByte()
  {
    int num = (int) this._block.ReadByte(this._position);
    ++this._position;
    return (byte) num;
  }

  [CLSCompliant(false)]
  public ushort ReadUInt16()
  {
    int num = (int) this._block.ReadUInt16(this._position);
    this._position += 2;
    return (ushort) num;
  }

  [CLSCompliant(false)]
  public uint ReadUInt32()
  {
    int num = (int) this._block.ReadUInt32(this._position);
    this._position += 4;
    return (uint) num;
  }

  [CLSCompliant(false)]
  public ulong ReadUInt64()
  {
    long num = (long) this._block.ReadUInt64(this._position);
    this._position += 8;
    return (ulong) num;
  }

  public float ReadSingle()
  {
    double num = (double) this._block.ReadSingle(this._position);
    this._position += 4;
    return (float) num;
  }

  public double ReadDouble()
  {
    double num = this._block.ReadDouble(this._position);
    this._position += 8;
    return num;
  }

  public string ReadAscii(int maxByteCount)
  {
    int position = this._position;
    string str = this._block.ReadAscii(position, maxByteCount);
    this._position = position + str.Length + 1;
    return str;
  }
}
