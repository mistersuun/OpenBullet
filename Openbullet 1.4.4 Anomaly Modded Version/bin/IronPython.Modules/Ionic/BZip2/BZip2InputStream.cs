// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.BZip2InputStream
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using Ionic.Crc;
using System;
using System.IO;

#nullable disable
namespace Ionic.BZip2;

public class BZip2InputStream : Stream
{
  private bool _disposed;
  private bool _leaveOpen;
  private long totalBytesRead;
  private int last;
  private int origPtr;
  private int blockSize100k;
  private bool blockRandomised;
  private int bsBuff;
  private int bsLive;
  private readonly CRC32 crc = new CRC32(true);
  private int nInUse;
  private Stream input;
  private int currentChar = -1;
  private BZip2InputStream.CState currentState = BZip2InputStream.CState.START_BLOCK;
  private uint storedBlockCRC;
  private uint storedCombinedCRC;
  private uint computedBlockCRC;
  private uint computedCombinedCRC;
  private int su_count;
  private int su_ch2;
  private int su_chPrev;
  private int su_i2;
  private int su_j2;
  private int su_rNToGo;
  private int su_rTPos;
  private int su_tPos;
  private char su_z;
  private BZip2InputStream.DecompressionState data;

  public BZip2InputStream(Stream input)
    : this(input, false)
  {
  }

  public BZip2InputStream(Stream input, bool leaveOpen)
  {
    this.input = input;
    this._leaveOpen = leaveOpen;
    this.init();
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    if (offset < 0)
      throw new IndexOutOfRangeException($"offset ({offset}) must be > 0");
    if (count < 0)
      throw new IndexOutOfRangeException($"count ({count}) must be > 0");
    if (offset + count > buffer.Length)
      throw new IndexOutOfRangeException($"offset({offset}) count({count}) bLength({buffer.Length})");
    if (this.input == null)
      throw new IOException("the stream is not open");
    int num1 = offset + count;
    int num2 = offset;
    int num3;
    while (num2 < num1 && (num3 = this.ReadByte()) >= 0)
      buffer[num2++] = (byte) num3;
    return num2 - offset;
  }

  private void MakeMaps()
  {
    bool[] inUse = this.data.inUse;
    byte[] seqToUnseq = this.data.seqToUnseq;
    int num = 0;
    for (int index = 0; index < 256 /*0x0100*/; ++index)
    {
      if (inUse[index])
        seqToUnseq[num++] = (byte) index;
    }
    this.nInUse = num;
  }

  public override int ReadByte()
  {
    int currentChar = this.currentChar;
    ++this.totalBytesRead;
    switch (this.currentState)
    {
      case BZip2InputStream.CState.EOF:
        return -1;
      case BZip2InputStream.CState.START_BLOCK:
        throw new IOException("bad state");
      case BZip2InputStream.CState.RAND_PART_A:
        throw new IOException("bad state");
      case BZip2InputStream.CState.RAND_PART_B:
        this.SetupRandPartB();
        break;
      case BZip2InputStream.CState.RAND_PART_C:
        this.SetupRandPartC();
        break;
      case BZip2InputStream.CState.NO_RAND_PART_A:
        throw new IOException("bad state");
      case BZip2InputStream.CState.NO_RAND_PART_B:
        this.SetupNoRandPartB();
        break;
      case BZip2InputStream.CState.NO_RAND_PART_C:
        this.SetupNoRandPartC();
        break;
      default:
        throw new IOException("bad state");
    }
    return currentChar;
  }

  public override bool CanRead
  {
    get
    {
      if (this._disposed)
        throw new ObjectDisposedException("BZip2Stream");
      return this.input.CanRead;
    }
  }

  public override bool CanSeek => false;

  public override bool CanWrite
  {
    get
    {
      if (this._disposed)
        throw new ObjectDisposedException("BZip2Stream");
      return this.input.CanWrite;
    }
  }

  public override void Flush()
  {
    if (this._disposed)
      throw new ObjectDisposedException("BZip2Stream");
    this.input.Flush();
  }

  public override long Length => throw new NotImplementedException();

  public override long Position
  {
    get => this.totalBytesRead;
    set => throw new NotImplementedException();
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

  public override void SetLength(long value) => throw new NotImplementedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    throw new NotImplementedException();
  }

  protected override void Dispose(bool disposing)
  {
    if (this._disposed)
      return;
    if (disposing)
    {
      Stream input = this.input;
      if (input != null)
      {
        try
        {
          if (!this._leaveOpen)
            input.Dispose();
        }
        finally
        {
          this.data = (BZip2InputStream.DecompressionState) null;
          this.input = (Stream) null;
        }
      }
    }
    this._disposed = true;
    base.Dispose(disposing);
  }

  private void init()
  {
    if (this.input == null)
      throw new IOException("No input Stream");
    if (!this.input.CanRead)
      throw new IOException("Unreadable input Stream");
    this.CheckMagicChar('B', 0);
    this.CheckMagicChar('Z', 1);
    this.CheckMagicChar('h', 2);
    int num = this.input.ReadByte();
    if (num < 49 || num > 57)
      throw new IOException("Stream is not BZip2 formatted: illegal blocksize " + ((char) num).ToString());
    this.blockSize100k = num - 48 /*0x30*/;
    this.InitBlock();
    this.SetupBlock();
  }

  private void CheckMagicChar(char expected, int position)
  {
    int num = this.input.ReadByte();
    if (num != (int) expected)
      throw new IOException($"Not a valid BZip2 stream. byte {position}, expected '{(int) expected}', got '{num}'");
  }

  private void InitBlock()
  {
    char ubyte1 = this.bsGetUByte();
    char ubyte2 = this.bsGetUByte();
    char ubyte3 = this.bsGetUByte();
    char ubyte4 = this.bsGetUByte();
    char ubyte5 = this.bsGetUByte();
    char ubyte6 = this.bsGetUByte();
    if (ubyte1 == '\u0017' && ubyte2 == 'r' && ubyte3 == 'E' && ubyte4 == '8' && ubyte5 == 'P' && ubyte6 == '\u0090')
    {
      this.complete();
    }
    else
    {
      if (ubyte1 != '1' || ubyte2 != 'A' || ubyte3 != 'Y' || ubyte4 != '&' || ubyte5 != 'S' || ubyte6 != 'Y')
      {
        this.currentState = BZip2InputStream.CState.EOF;
        throw new IOException($"bad block header at offset 0x{this.input.Position:X}");
      }
      this.storedBlockCRC = this.bsGetInt();
      this.blockRandomised = this.GetBits(1) == 1;
      if (this.data == null)
        this.data = new BZip2InputStream.DecompressionState(this.blockSize100k);
      this.getAndMoveToFrontDecode();
      this.crc.Reset();
      this.currentState = BZip2InputStream.CState.START_BLOCK;
    }
  }

  private void EndBlock()
  {
    this.computedBlockCRC = (uint) this.crc.Crc32Result;
    if ((int) this.storedBlockCRC != (int) this.computedBlockCRC)
      throw new IOException($"BZip2 CRC error (expected {this.storedBlockCRC:X8}, computed {this.computedBlockCRC:X8})");
    this.computedCombinedCRC = this.computedCombinedCRC << 1 | this.computedCombinedCRC >> 31 /*0x1F*/;
    this.computedCombinedCRC ^= this.computedBlockCRC;
  }

  private void complete()
  {
    this.storedCombinedCRC = this.bsGetInt();
    this.currentState = BZip2InputStream.CState.EOF;
    this.data = (BZip2InputStream.DecompressionState) null;
    if ((int) this.storedCombinedCRC != (int) this.computedCombinedCRC)
      throw new IOException($"BZip2 CRC error (expected {this.storedCombinedCRC:X8}, computed {this.computedCombinedCRC:X8})");
  }

  private int GetBits(int n)
  {
    int bsLive = this.bsLive;
    int num1 = this.bsBuff;
    if (bsLive < n)
    {
      do
      {
        int num2 = this.input.ReadByte();
        if (num2 < 0)
          throw new IOException("unexpected end of stream");
        num1 = num1 << 8 | num2;
        bsLive += 8;
      }
      while (bsLive < n);
      this.bsBuff = num1;
    }
    this.bsLive = bsLive - n;
    return num1 >> bsLive - n & (1 << n) - 1;
  }

  private bool bsGetBit() => this.GetBits(1) != 0;

  private char bsGetUByte() => (char) this.GetBits(8);

  private uint bsGetInt()
  {
    return (uint) (((this.GetBits(8) << 8 | this.GetBits(8)) << 8 | this.GetBits(8)) << 8 | this.GetBits(8));
  }

  private static void hbCreateDecodeTables(
    int[] limit,
    int[] bbase,
    int[] perm,
    char[] length,
    int minLen,
    int maxLen,
    int alphaSize)
  {
    int num1 = minLen;
    int num2 = 0;
    for (; num1 <= maxLen; ++num1)
    {
      for (int index = 0; index < alphaSize; ++index)
      {
        if ((int) length[index] == num1)
          perm[num2++] = index;
      }
    }
    int maxCodeLength = Ionic.BZip2.BZip2.MaxCodeLength;
    while (--maxCodeLength > 0)
    {
      bbase[maxCodeLength] = 0;
      limit[maxCodeLength] = 0;
    }
    for (int index = 0; index < alphaSize; ++index)
      ++bbase[(int) length[index] + 1];
    int index1 = 1;
    int num3 = bbase[0];
    for (; index1 < Ionic.BZip2.BZip2.MaxCodeLength; ++index1)
    {
      num3 += bbase[index1];
      bbase[index1] = num3;
    }
    int index2 = minLen;
    int num4 = 0;
    int num5 = bbase[index2];
    for (; index2 <= maxLen; ++index2)
    {
      int num6 = bbase[index2 + 1];
      int num7 = num4 + (num6 - num5);
      num5 = num6;
      limit[index2] = num7 - 1;
      num4 = num7 << 1;
    }
    for (int index3 = minLen + 1; index3 <= maxLen; ++index3)
      bbase[index3] = (limit[index3 - 1] + 1 << 1) - bbase[index3];
  }

  private void recvDecodingTables()
  {
    BZip2InputStream.DecompressionState data = this.data;
    bool[] inUse = data.inUse;
    byte[] decodingTablesPos = data.recvDecodingTables_pos;
    int num1 = 0;
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      if (this.bsGetBit())
        num1 |= 1 << index;
    }
    int index1 = 256 /*0x0100*/;
    while (--index1 >= 0)
      inUse[index1] = false;
    for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
    {
      if ((num1 & 1 << index2) != 0)
      {
        int num2 = index2 << 4;
        for (int index3 = 0; index3 < 16 /*0x10*/; ++index3)
        {
          if (this.bsGetBit())
            inUse[num2 + index3] = true;
        }
      }
    }
    this.MakeMaps();
    int alphaSize = this.nInUse + 2;
    int bits1 = this.GetBits(3);
    int bits2 = this.GetBits(15);
    for (int index4 = 0; index4 < bits2; ++index4)
    {
      int num3 = 0;
      while (this.bsGetBit())
        ++num3;
      data.selectorMtf[index4] = (byte) num3;
    }
    int index5 = bits1;
    while (--index5 >= 0)
      decodingTablesPos[index5] = (byte) index5;
    for (int index6 = 0; index6 < bits2; ++index6)
    {
      int index7 = (int) data.selectorMtf[index6];
      byte num4 = decodingTablesPos[index7];
      for (; index7 > 0; --index7)
        decodingTablesPos[index7] = decodingTablesPos[index7 - 1];
      decodingTablesPos[0] = num4;
      data.selector[index6] = num4;
    }
    char[][] tempCharArray2d = data.temp_charArray2d;
    for (int index8 = 0; index8 < bits1; ++index8)
    {
      int bits3 = this.GetBits(5);
      char[] chArray = tempCharArray2d[index8];
      for (int index9 = 0; index9 < alphaSize; ++index9)
      {
        while (this.bsGetBit())
          bits3 += this.bsGetBit() ? -1 : 1;
        chArray[index9] = (char) bits3;
      }
    }
    this.createHuffmanDecodingTables(alphaSize, bits1);
  }

  private void createHuffmanDecodingTables(int alphaSize, int nGroups)
  {
    BZip2InputStream.DecompressionState data = this.data;
    char[][] tempCharArray2d = data.temp_charArray2d;
    for (int index1 = 0; index1 < nGroups; ++index1)
    {
      int minLen = 32 /*0x20*/;
      int maxLen = 0;
      char[] chArray = tempCharArray2d[index1];
      int index2 = alphaSize;
      while (--index2 >= 0)
      {
        char ch = chArray[index2];
        if ((int) ch > maxLen)
          maxLen = (int) ch;
        if ((int) ch < minLen)
          minLen = (int) ch;
      }
      BZip2InputStream.hbCreateDecodeTables(data.gLimit[index1], data.gBase[index1], data.gPerm[index1], tempCharArray2d[index1], minLen, maxLen, alphaSize);
      data.gMinlen[index1] = minLen;
    }
  }

  private void getAndMoveToFrontDecode()
  {
    BZip2InputStream.DecompressionState data = this.data;
    this.origPtr = this.GetBits(24);
    if (this.origPtr < 0)
      throw new IOException("BZ_DATA_ERROR");
    if (this.origPtr > 10 + Ionic.BZip2.BZip2.BlockSizeMultiple * this.blockSize100k)
      throw new IOException("BZ_DATA_ERROR");
    this.recvDecodingTables();
    byte[] moveToFrontDecodeYy = data.getAndMoveToFrontDecode_yy;
    int num1 = this.blockSize100k * Ionic.BZip2.BZip2.BlockSizeMultiple;
    int index1 = 256 /*0x0100*/;
    while (--index1 >= 0)
    {
      moveToFrontDecodeYy[index1] = (byte) index1;
      data.unzftab[index1] = 0;
    }
    int index2 = 0;
    int num2 = Ionic.BZip2.BZip2.G_SIZE - 1;
    int num3 = this.nInUse + 1;
    int moveToFrontDecode0 = this.getAndMoveToFrontDecode0(0);
    int num4 = this.bsBuff;
    int bsLive = this.bsLive;
    int index3 = -1;
    int index4 = (int) data.selector[index2] & (int) byte.MaxValue;
    int[] numArray1 = data.gBase[index4];
    int[] numArray2 = data.gLimit[index4];
    int[] numArray3 = data.gPerm[index4];
    int num5 = data.gMinlen[index4];
    while (moveToFrontDecode0 != num3)
    {
      if (moveToFrontDecode0 == (int) Ionic.BZip2.BZip2.RUNA || moveToFrontDecode0 == (int) Ionic.BZip2.BZip2.RUNB)
      {
        int num6 = -1;
        int num7 = 1;
        while (true)
        {
          if (moveToFrontDecode0 == (int) Ionic.BZip2.BZip2.RUNA)
            num6 += num7;
          else if (moveToFrontDecode0 == (int) Ionic.BZip2.BZip2.RUNB)
            num6 += num7 << 1;
          else
            break;
          if (num2 == 0)
          {
            num2 = Ionic.BZip2.BZip2.G_SIZE - 1;
            int index5 = (int) data.selector[++index2] & (int) byte.MaxValue;
            numArray1 = data.gBase[index5];
            numArray2 = data.gLimit[index5];
            numArray3 = data.gPerm[index5];
            num5 = data.gMinlen[index5];
          }
          else
            --num2;
          int index6;
          for (index6 = num5; bsLive < index6; bsLive += 8)
          {
            int num8 = this.input.ReadByte();
            if (num8 < 0)
              throw new IOException("unexpected end of stream");
            num4 = num4 << 8 | num8;
          }
          int num9 = num4 >> bsLive - index6 & (1 << index6) - 1;
          bsLive -= index6;
          for (; num9 > numArray2[index6]; num9 = num9 << 1 | num4 >> bsLive & 1)
          {
            ++index6;
            for (; bsLive < 1; bsLive += 8)
            {
              int num10 = this.input.ReadByte();
              if (num10 < 0)
                throw new IOException("unexpected end of stream");
              num4 = num4 << 8 | num10;
            }
            --bsLive;
          }
          moveToFrontDecode0 = numArray3[num9 - numArray1[index6]];
          num7 <<= 1;
        }
        byte num11 = data.seqToUnseq[(int) moveToFrontDecodeYy[0]];
        data.unzftab[(int) num11 & (int) byte.MaxValue] += num6 + 1;
        while (num6-- >= 0)
          data.ll8[++index3] = num11;
        if (index3 >= num1)
          throw new IOException("block overrun");
      }
      else
      {
        if (++index3 >= num1)
          throw new IOException("block overrun");
        byte index7 = moveToFrontDecodeYy[moveToFrontDecode0 - 1];
        ++data.unzftab[(int) data.seqToUnseq[(int) index7] & (int) byte.MaxValue];
        data.ll8[index3] = data.seqToUnseq[(int) index7];
        if (moveToFrontDecode0 <= 16 /*0x10*/)
        {
          int index8 = moveToFrontDecode0 - 1;
          while (index8 > 0)
            moveToFrontDecodeYy[index8] = moveToFrontDecodeYy[--index8];
        }
        else
          Buffer.BlockCopy((Array) moveToFrontDecodeYy, 0, (Array) moveToFrontDecodeYy, 1, moveToFrontDecode0 - 1);
        moveToFrontDecodeYy[0] = index7;
        if (num2 == 0)
        {
          num2 = Ionic.BZip2.BZip2.G_SIZE - 1;
          int index9 = (int) data.selector[++index2] & (int) byte.MaxValue;
          numArray1 = data.gBase[index9];
          numArray2 = data.gLimit[index9];
          numArray3 = data.gPerm[index9];
          num5 = data.gMinlen[index9];
        }
        else
          --num2;
        int index10;
        for (index10 = num5; bsLive < index10; bsLive += 8)
        {
          int num12 = this.input.ReadByte();
          if (num12 < 0)
            throw new IOException("unexpected end of stream");
          num4 = num4 << 8 | num12;
        }
        int num13 = num4 >> bsLive - index10 & (1 << index10) - 1;
        bsLive -= index10;
        for (; num13 > numArray2[index10]; num13 = num13 << 1 | num4 >> bsLive & 1)
        {
          ++index10;
          for (; bsLive < 1; bsLive += 8)
          {
            int num14 = this.input.ReadByte();
            if (num14 < 0)
              throw new IOException("unexpected end of stream");
            num4 = num4 << 8 | num14;
          }
          --bsLive;
        }
        moveToFrontDecode0 = numArray3[num13 - numArray1[index10]];
      }
    }
    this.last = index3;
    this.bsLive = bsLive;
    this.bsBuff = num4;
  }

  private int getAndMoveToFrontDecode0(int groupNo)
  {
    BZip2InputStream.DecompressionState data = this.data;
    int index = (int) data.selector[groupNo] & (int) byte.MaxValue;
    int[] numArray = data.gLimit[index];
    int n = data.gMinlen[index];
    int num1 = this.GetBits(n);
    int bsLive = this.bsLive;
    int num2 = this.bsBuff;
    for (; num1 > numArray[n]; num1 = num1 << 1 | num2 >> bsLive & 1)
    {
      ++n;
      for (; bsLive < 1; bsLive += 8)
      {
        int num3 = this.input.ReadByte();
        if (num3 < 0)
          throw new IOException("unexpected end of stream");
        num2 = num2 << 8 | num3;
      }
      --bsLive;
    }
    this.bsLive = bsLive;
    this.bsBuff = num2;
    return data.gPerm[index][num1 - data.gBase[index][n]];
  }

  private void SetupBlock()
  {
    if (this.data == null)
      return;
    BZip2InputStream.DecompressionState data = this.data;
    int[] numArray = data.initTT(this.last + 1);
    for (int index = 0; index <= (int) byte.MaxValue; ++index)
    {
      if (data.unzftab[index] < 0 || data.unzftab[index] > this.last)
        throw new Exception("BZ_DATA_ERROR");
    }
    data.cftab[0] = 0;
    for (int index = 1; index <= 256 /*0x0100*/; ++index)
      data.cftab[index] = data.unzftab[index - 1];
    for (int index = 1; index <= 256 /*0x0100*/; ++index)
      data.cftab[index] += data.cftab[index - 1];
    for (int index = 0; index <= 256 /*0x0100*/; ++index)
    {
      if (data.cftab[index] < 0 || data.cftab[index] > this.last + 1)
        throw new Exception($"BZ_DATA_ERROR: cftab[{index}]={data.cftab[index]} last={this.last}");
    }
    for (int index = 1; index <= 256 /*0x0100*/; ++index)
    {
      if (data.cftab[index - 1] > data.cftab[index])
        throw new Exception("BZ_DATA_ERROR");
    }
    int index1 = 0;
    for (int last = this.last; index1 <= last; ++index1)
      numArray[data.cftab[(int) data.ll8[index1] & (int) byte.MaxValue]++] = index1;
    if (this.origPtr < 0 || this.origPtr >= numArray.Length)
      throw new IOException("stream corrupted");
    this.su_tPos = numArray[this.origPtr];
    this.su_count = 0;
    this.su_i2 = 0;
    this.su_ch2 = 256 /*0x0100*/;
    if (this.blockRandomised)
    {
      this.su_rNToGo = 0;
      this.su_rTPos = 0;
      this.SetupRandPartA();
    }
    else
      this.SetupNoRandPartA();
  }

  private void SetupRandPartA()
  {
    if (this.su_i2 <= this.last)
    {
      this.su_chPrev = this.su_ch2;
      int num1 = (int) this.data.ll8[this.su_tPos] & (int) byte.MaxValue;
      this.su_tPos = this.data.tt[this.su_tPos];
      if (this.su_rNToGo == 0)
      {
        this.su_rNToGo = Rand.Rnums(this.su_rTPos) - 1;
        if (++this.su_rTPos == 512 /*0x0200*/)
          this.su_rTPos = 0;
      }
      else
        --this.su_rNToGo;
      int num2 = num1;
      int num3 = this.su_rNToGo == 1 ? 1 : 0;
      int b;
      this.su_ch2 = b = num2 ^ num3;
      ++this.su_i2;
      this.currentChar = b;
      this.currentState = BZip2InputStream.CState.RAND_PART_B;
      this.crc.UpdateCRC((byte) b);
    }
    else
    {
      this.EndBlock();
      this.InitBlock();
      this.SetupBlock();
    }
  }

  private void SetupNoRandPartA()
  {
    if (this.su_i2 <= this.last)
    {
      this.su_chPrev = this.su_ch2;
      int b = (int) this.data.ll8[this.su_tPos] & (int) byte.MaxValue;
      this.su_ch2 = b;
      this.su_tPos = this.data.tt[this.su_tPos];
      ++this.su_i2;
      this.currentChar = b;
      this.currentState = BZip2InputStream.CState.NO_RAND_PART_B;
      this.crc.UpdateCRC((byte) b);
    }
    else
    {
      this.currentState = BZip2InputStream.CState.NO_RAND_PART_A;
      this.EndBlock();
      this.InitBlock();
      this.SetupBlock();
    }
  }

  private void SetupRandPartB()
  {
    if (this.su_ch2 != this.su_chPrev)
    {
      this.currentState = BZip2InputStream.CState.RAND_PART_A;
      this.su_count = 1;
      this.SetupRandPartA();
    }
    else if (++this.su_count >= 4)
    {
      this.su_z = (char) ((uint) this.data.ll8[this.su_tPos] & (uint) byte.MaxValue);
      this.su_tPos = this.data.tt[this.su_tPos];
      if (this.su_rNToGo == 0)
      {
        this.su_rNToGo = Rand.Rnums(this.su_rTPos) - 1;
        if (++this.su_rTPos == 512 /*0x0200*/)
          this.su_rTPos = 0;
      }
      else
        --this.su_rNToGo;
      this.su_j2 = 0;
      this.currentState = BZip2InputStream.CState.RAND_PART_C;
      if (this.su_rNToGo == 1)
        this.su_z ^= '\u0001';
      this.SetupRandPartC();
    }
    else
    {
      this.currentState = BZip2InputStream.CState.RAND_PART_A;
      this.SetupRandPartA();
    }
  }

  private void SetupRandPartC()
  {
    if (this.su_j2 < (int) this.su_z)
    {
      this.currentChar = this.su_ch2;
      this.crc.UpdateCRC((byte) this.su_ch2);
      ++this.su_j2;
    }
    else
    {
      this.currentState = BZip2InputStream.CState.RAND_PART_A;
      ++this.su_i2;
      this.su_count = 0;
      this.SetupRandPartA();
    }
  }

  private void SetupNoRandPartB()
  {
    if (this.su_ch2 != this.su_chPrev)
    {
      this.su_count = 1;
      this.SetupNoRandPartA();
    }
    else if (++this.su_count >= 4)
    {
      this.su_z = (char) ((uint) this.data.ll8[this.su_tPos] & (uint) byte.MaxValue);
      this.su_tPos = this.data.tt[this.su_tPos];
      this.su_j2 = 0;
      this.SetupNoRandPartC();
    }
    else
      this.SetupNoRandPartA();
  }

  private void SetupNoRandPartC()
  {
    if (this.su_j2 < (int) this.su_z)
    {
      int suCh2 = this.su_ch2;
      this.currentChar = suCh2;
      this.crc.UpdateCRC((byte) suCh2);
      ++this.su_j2;
      this.currentState = BZip2InputStream.CState.NO_RAND_PART_C;
    }
    else
    {
      ++this.su_i2;
      this.su_count = 0;
      this.SetupNoRandPartA();
    }
  }

  internal object DumpState()
  {
    return (object) new BZip2InputStream.StateDump()
    {
      _disposed = this._disposed,
      totalBytesRead = this.totalBytesRead,
      last = this.last,
      origPtr = this.origPtr,
      blockRandomised = this.blockRandomised,
      bsBuff = this.bsBuff,
      bsLive = this.bsLive,
      nInUse = this.nInUse,
      currentChar = this.currentChar,
      currentState = this.currentState,
      storedBlockCRC = this.storedBlockCRC,
      storedCombinedCRC = this.storedCombinedCRC,
      computedBlockCRC = this.computedBlockCRC,
      computedCombinedCRC = this.computedCombinedCRC,
      su_count = this.su_count,
      su_ch2 = this.su_ch2,
      su_chPrev = this.su_chPrev,
      su_i2 = this.su_i2,
      su_j2 = this.su_j2,
      su_rNToGo = this.su_rNToGo,
      su_rTPos = this.su_rTPos,
      su_tPos = this.su_tPos,
      su_z = this.su_z
    };
  }

  internal void RestoreState(object o)
  {
    BZip2InputStream.StateDump stateDump = (BZip2InputStream.StateDump) o;
    this._disposed = stateDump._disposed;
    this.totalBytesRead = stateDump.totalBytesRead;
    this.last = stateDump.last;
    this.origPtr = stateDump.origPtr;
    this.bsBuff = stateDump.bsBuff;
    this.bsLive = stateDump.bsLive;
    this.nInUse = stateDump.nInUse;
    this.currentChar = stateDump.currentChar;
    this.currentState = stateDump.currentState;
    this.storedBlockCRC = stateDump.storedBlockCRC;
    this.storedCombinedCRC = stateDump.storedCombinedCRC;
    this.computedBlockCRC = stateDump.computedBlockCRC;
    this.computedCombinedCRC = stateDump.computedCombinedCRC;
    this.su_count = stateDump.su_count;
    this.su_ch2 = stateDump.su_ch2;
    this.su_chPrev = stateDump.su_chPrev;
    this.su_i2 = stateDump.su_i2;
    this.su_j2 = stateDump.su_j2;
    this.su_rNToGo = stateDump.su_rNToGo;
    this.su_rTPos = stateDump.su_rTPos;
    this.su_tPos = stateDump.su_tPos;
    this.su_z = stateDump.su_z;
  }

  private enum CState
  {
    EOF,
    START_BLOCK,
    RAND_PART_A,
    RAND_PART_B,
    RAND_PART_C,
    NO_RAND_PART_A,
    NO_RAND_PART_B,
    NO_RAND_PART_C,
  }

  private sealed class DecompressionState
  {
    public readonly bool[] inUse = new bool[256 /*0x0100*/];
    public readonly byte[] seqToUnseq = new byte[256 /*0x0100*/];
    public readonly byte[] selector = new byte[Ionic.BZip2.BZip2.MaxSelectors];
    public readonly byte[] selectorMtf = new byte[Ionic.BZip2.BZip2.MaxSelectors];
    public readonly int[] unzftab;
    public readonly int[][] gLimit;
    public readonly int[][] gBase;
    public readonly int[][] gPerm;
    public readonly int[] gMinlen;
    public readonly int[] cftab;
    public readonly byte[] getAndMoveToFrontDecode_yy;
    public readonly char[][] temp_charArray2d;
    public readonly byte[] recvDecodingTables_pos;
    public int[] tt;
    public byte[] ll8;

    public DecompressionState(int blockSize100k)
    {
      this.unzftab = new int[256 /*0x0100*/];
      this.gLimit = Ionic.BZip2.BZip2.InitRectangularArray<int>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
      this.gBase = Ionic.BZip2.BZip2.InitRectangularArray<int>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
      this.gPerm = Ionic.BZip2.BZip2.InitRectangularArray<int>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
      this.gMinlen = new int[Ionic.BZip2.BZip2.NGroups];
      this.cftab = new int[257];
      this.getAndMoveToFrontDecode_yy = new byte[256 /*0x0100*/];
      this.temp_charArray2d = Ionic.BZip2.BZip2.InitRectangularArray<char>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
      this.recvDecodingTables_pos = new byte[Ionic.BZip2.BZip2.NGroups];
      this.ll8 = new byte[blockSize100k * Ionic.BZip2.BZip2.BlockSizeMultiple];
    }

    public int[] initTT(int length)
    {
      int[] numArray = this.tt;
      if (numArray == null || numArray.Length < length)
        this.tt = numArray = new int[length];
      return numArray;
    }
  }

  private struct StateDump
  {
    public bool _disposed;
    public long totalBytesRead;
    public int last;
    public int origPtr;
    public bool blockRandomised;
    public int bsBuff;
    public int bsLive;
    public int nInUse;
    public int currentChar;
    public BZip2InputStream.CState currentState;
    public uint storedBlockCRC;
    public uint storedCombinedCRC;
    public uint computedBlockCRC;
    public uint computedCombinedCRC;
    public int su_count;
    public int su_ch2;
    public int su_chPrev;
    public int su_i2;
    public int su_j2;
    public int su_rNToGo;
    public int su_rTPos;
    public int su_tPos;
    public char su_z;
  }
}
