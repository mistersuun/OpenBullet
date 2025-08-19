// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MemoryBlock
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public sealed class MemoryBlock
{
  private readonly unsafe byte* _pointer;
  private readonly int _length;
  private readonly object _owner;

  internal unsafe MemoryBlock(object owner, byte* pointer, int length)
  {
    this._pointer = pointer;
    this._length = length;
    this._owner = owner;
  }

  public unsafe MemoryBlock GetRange(int start, int length)
  {
    if (start < 0)
      throw new ArgumentOutOfRangeException(nameof (start));
    if (length < 0 || length > this._length - start)
      throw new ArgumentOutOfRangeException(nameof (length));
    return new MemoryBlock(this._owner, this._pointer + start, length);
  }

  [CLSCompliant(false)]
  public unsafe byte* Pointer => this._pointer;

  public int Length => this._length;

  public unsafe byte ReadByte(int offset)
  {
    if (offset < 0 || offset > this._length - 1)
      throw new ArgumentOutOfRangeException(nameof (offset));
    int num = (int) this._pointer[offset];
    GC.KeepAlive(this._owner);
    return (byte) num;
  }

  public unsafe short ReadInt16(int offset)
  {
    if (offset < 0 || offset > this._length - 2)
      throw new ArgumentOutOfRangeException(nameof (offset));
    int num = (int) *(short*) (this._pointer + offset);
    GC.KeepAlive(this._owner);
    return (short) num;
  }

  public unsafe int ReadInt32(int offset)
  {
    if (offset < 0 || offset > this._length - 4)
      throw new ArgumentOutOfRangeException(nameof (offset));
    int num = *(int*) (this._pointer + offset);
    GC.KeepAlive(this._owner);
    return num;
  }

  public unsafe long ReadInt64(int offset)
  {
    if (offset < 0 || offset > this._length - 8)
      throw new ArgumentOutOfRangeException(nameof (offset));
    long num = *(long*) (this._pointer + offset);
    GC.KeepAlive(this._owner);
    return num;
  }

  public unsafe Guid ReadGuid(int offset)
  {
    if (offset < 0 || offset > this._length - sizeof (Guid))
      throw new ArgumentOutOfRangeException(nameof (offset));
    Guid guid = *(Guid*) (this._pointer + offset);
    GC.KeepAlive(this._owner);
    return guid;
  }

  public unsafe void Read(int offset, byte[] result)
  {
    if (result == null)
      throw new ArgumentNullException(nameof (result));
    if (offset < 0 || offset > this._length - result.Length)
      throw new ArgumentOutOfRangeException(nameof (offset));
    byte* numPtr1 = this._pointer + offset;
    byte* numPtr2 = numPtr1 + result.Length;
    fixed (byte* numPtr3 = result)
    {
      byte* numPtr4 = numPtr3;
      while (numPtr1 < numPtr2)
      {
        *numPtr4 = *numPtr1;
        ++numPtr1;
        ++numPtr4;
      }
    }
    GC.KeepAlive(this._owner);
  }

  public unsafe string ReadUtf16(int offset, int byteCount)
  {
    if (offset < 0 || offset > this._length - byteCount)
      throw new ArgumentOutOfRangeException(nameof (offset));
    if (byteCount <= 1)
      return string.Empty;
    string stringUni = Marshal.PtrToStringUni((IntPtr) (void*) (this._pointer + offset), byteCount / 2);
    GC.KeepAlive(this._owner);
    return stringUni;
  }

  public string ReadAscii(int offset) => this.ReadAscii(offset, this._length - offset);

  public unsafe string ReadAscii(int offset, int maxByteCount)
  {
    if (maxByteCount < 0)
      throw new ArgumentOutOfRangeException(nameof (maxByteCount));
    if (offset < 0 || offset > this._length - maxByteCount)
      throw new ArgumentOutOfRangeException(nameof (offset));
    sbyte* numPtr1 = (sbyte*) (this._pointer + offset);
    sbyte* numPtr2 = numPtr1 + maxByteCount;
    sbyte* numPtr3 = numPtr1;
    while (*numPtr3 != (sbyte) 0 && numPtr3 < numPtr2)
      ++numPtr3;
    return new string(numPtr1, 0, (int) (numPtr3 - numPtr1), Encoding.ASCII);
  }

  [CLSCompliant(false)]
  public sbyte ReadSByte(int offset) => (sbyte) this.ReadByte(offset);

  [CLSCompliant(false)]
  public ushort ReadUInt16(int offset) => (ushort) this.ReadInt16(offset);

  public char ReadChar(int offset) => (char) this.ReadInt16(offset);

  [CLSCompliant(false)]
  public uint ReadUInt32(int offset) => (uint) this.ReadInt32(offset);

  [CLSCompliant(false)]
  public ulong ReadUInt64(int offset) => (ulong) this.ReadInt64(offset);

  public float ReadSingle(int offset) => (float) this.ReadInt32(offset);

  public double ReadDouble(int offset) => (double) this.ReadInt64(offset);

  internal uint ReadReference(int offset, bool smallRefSize)
  {
    return smallRefSize ? (uint) this.ReadUInt16(offset) : this.ReadUInt32(offset);
  }

  internal int ReadCompressedInt32(int offset, out int numberOfBytesRead)
  {
    byte num1 = this.ReadByte(offset);
    int num2;
    if (((int) num1 & 128 /*0x80*/) == 0)
    {
      num2 = (int) num1;
      numberOfBytesRead = 1;
    }
    else if (((int) num1 & 64 /*0x40*/) == 0)
    {
      num2 = ((int) num1 & 63 /*0x3F*/) << 8 | (int) this.ReadByte(offset + 1);
      numberOfBytesRead = 2;
    }
    else
    {
      if (num1 == byte.MaxValue)
        throw new BadImageFormatException();
      num2 = ((int) num1 & 63 /*0x3F*/) << 24 | (int) this.ReadByte(offset + 1) << 16 /*0x10*/ | (int) this.ReadByte(offset + 2) << 8 | (int) this.ReadByte(offset + 3);
      numberOfBytesRead = 4;
    }
    return num2;
  }

  internal unsafe MetadataName ReadName(uint offset)
  {
    if ((long) offset >= (long) this._length)
      throw new BadImageFormatException();
    return new MetadataName(this._pointer + offset, this._owner);
  }

  internal int BinarySearchForSlot(
    int numberOfRows,
    int numberOfChildren,
    int rowSize,
    int referenceOffset,
    uint childRid,
    bool isReferenceSmall)
  {
    int num1 = 0;
    int num2 = numberOfRows;
    uint num3 = this.ReadReference(num1 * rowSize + referenceOffset, isReferenceSmall);
    uint num4 = (uint) (numberOfChildren + 1);
    if (childRid < num3 || childRid >= num4)
      return -1;
    while (num2 - num1 > 1)
    {
      int num5 = (num1 + num2) / 2;
      uint num6 = this.ReadReference(num5 * rowSize + referenceOffset, isReferenceSmall);
      if (childRid > num6)
      {
        num1 = num5;
        num3 = num6;
      }
      else if (childRid < num6)
      {
        num2 = num5;
      }
      else
      {
        num1 = num5;
        num3 = num6;
        break;
      }
    }
    while (num1 < numberOfRows - 1 && (int) num3 == (int) this.ReadReference((num1 + 1) * rowSize + referenceOffset, isReferenceSmall))
      ++num1;
    return num1;
  }

  internal int BinarySearchReference(
    int numberOfRows,
    int rowSize,
    int referenceOffset,
    uint referenceValue,
    bool isReferenceSmall)
  {
    int num1 = 0;
    int num2 = numberOfRows - 1;
    while (num1 <= num2)
    {
      int num3 = (num1 + num2) / 2;
      uint num4 = this.ReadReference(num3 * rowSize + referenceOffset, isReferenceSmall);
      if (referenceValue > num4)
      {
        num1 = num3 + 1;
      }
      else
      {
        if (referenceValue >= num4)
          return num3;
        num2 = num3 - 1;
      }
    }
    return -1;
  }

  internal int LinearSearchReference(
    int rowSize,
    int referenceOffset,
    uint referenceValue,
    bool isReferenceSmall)
  {
    int offset = referenceOffset;
    for (int length = this._length; offset < length; offset += rowSize)
    {
      if ((int) this.ReadReference(offset, isReferenceSmall) == (int) referenceValue)
        return offset / rowSize;
    }
    return -1;
  }
}
