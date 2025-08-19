// Decompiled with JetBrains decompiler
// Type: Mono.Security.Cryptography.SHA224Managed
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using System;

#nullable disable
namespace Mono.Security.Cryptography;

[PythonHidden(new PlatformID[] {})]
internal class SHA224Managed : SHA224
{
  private const int BLOCK_SIZE_BYTES = 64 /*0x40*/;
  private uint[] _H;
  private ulong count;
  private byte[] _ProcessingBuffer;
  private int _ProcessingBufferCount;
  private uint[] buff;

  public SHA224Managed()
  {
    this._H = new uint[8];
    this._ProcessingBuffer = new byte[64 /*0x40*/];
    this.buff = new uint[64 /*0x40*/];
    this.Initialize();
  }

  private uint Ch(uint u, uint v, uint w) => (uint) ((int) u & (int) v ^ ~(int) u & (int) w);

  private uint Maj(uint u, uint v, uint w)
  {
    return (uint) ((int) u & (int) v ^ (int) u & (int) w ^ (int) v & (int) w);
  }

  private uint Ro0(uint x)
  {
    return (uint) (((int) (x >> 7) | (int) x << 25) ^ ((int) (x >> 18) | (int) x << 14)) ^ x >> 3;
  }

  private uint Ro1(uint x)
  {
    return (uint) (((int) (x >> 17) | (int) x << 15) ^ ((int) (x >> 19) | (int) x << 13)) ^ x >> 10;
  }

  private uint Sig0(uint x)
  {
    return (uint) (((int) (x >> 2) | (int) x << 30) ^ ((int) (x >> 13) | (int) x << 19) ^ ((int) (x >> 22) | (int) x << 10));
  }

  private uint Sig1(uint x)
  {
    return (uint) (((int) (x >> 6) | (int) x << 26) ^ ((int) (x >> 11) | (int) x << 21) ^ ((int) (x >> 25) | (int) x << 7));
  }

  protected override void HashCore(byte[] rgb, int start, int size)
  {
    this.State = 1;
    if (this._ProcessingBufferCount != 0)
    {
      if (size < 64 /*0x40*/ - this._ProcessingBufferCount)
      {
        Buffer.BlockCopy((Array) rgb, start, (Array) this._ProcessingBuffer, this._ProcessingBufferCount, size);
        this._ProcessingBufferCount += size;
        return;
      }
      int count = 64 /*0x40*/ - this._ProcessingBufferCount;
      Buffer.BlockCopy((Array) rgb, start, (Array) this._ProcessingBuffer, this._ProcessingBufferCount, count);
      this.ProcessBlock(this._ProcessingBuffer, 0);
      this._ProcessingBufferCount = 0;
      start += count;
      size -= count;
    }
    for (int index = 0; index < size - size % 64 /*0x40*/; index += 64 /*0x40*/)
      this.ProcessBlock(rgb, start + index);
    if (size % 64 /*0x40*/ == 0)
      return;
    Buffer.BlockCopy((Array) rgb, size - size % 64 /*0x40*/ + start, (Array) this._ProcessingBuffer, 0, size % 64 /*0x40*/);
    this._ProcessingBufferCount = size % 64 /*0x40*/;
  }

  protected override byte[] HashFinal()
  {
    byte[] numArray = new byte[28];
    this.ProcessFinalBlock(this._ProcessingBuffer, 0, this._ProcessingBufferCount);
    for (int index1 = 0; index1 < 7; ++index1)
    {
      for (int index2 = 0; index2 < 4; ++index2)
        numArray[index1 * 4 + index2] = (byte) (this._H[index1] >> 24 - index2 * 8);
    }
    this.State = 0;
    return numArray;
  }

  public override void Initialize()
  {
    this.count = 0UL;
    this._ProcessingBufferCount = 0;
    this._H[0] = 3238371032U;
    this._H[1] = 914150663U;
    this._H[2] = 812702999U;
    this._H[3] = 4144912697U;
    this._H[4] = 4290775857U;
    this._H[5] = 1750603025U;
    this._H[6] = 1694076839U;
    this._H[7] = 3204075428U;
  }

  private void ProcessBlock(byte[] inputBuffer, int inputOffset)
  {
    uint[] k1 = SHAConstants.K1;
    uint[] buff = this.buff;
    this.count += 64UL /*0x40*/;
    for (int index = 0; index < 16 /*0x10*/; ++index)
      buff[index] = (uint) ((int) inputBuffer[inputOffset + 4 * index] << 24 | (int) inputBuffer[inputOffset + 4 * index + 1] << 16 /*0x10*/ | (int) inputBuffer[inputOffset + 4 * index + 2] << 8) | (uint) inputBuffer[inputOffset + 4 * index + 3];
    for (int index = 16 /*0x10*/; index < 64 /*0x40*/; ++index)
    {
      uint num1 = buff[index - 15];
      uint num2 = (uint) (((int) (num1 >> 7) | (int) num1 << 25) ^ ((int) (num1 >> 18) | (int) num1 << 14)) ^ num1 >> 3;
      uint num3 = buff[index - 2];
      uint num4 = (uint) (((int) (num3 >> 17) | (int) num3 << 15) ^ ((int) (num3 >> 19) | (int) num3 << 13)) ^ num3 >> 10;
      buff[index] = num4 + buff[index - 7] + num2 + buff[index - 16 /*0x10*/];
    }
    uint num5 = this._H[0];
    uint num6 = this._H[1];
    uint num7 = this._H[2];
    uint num8 = this._H[3];
    uint num9 = this._H[4];
    uint num10 = this._H[5];
    uint num11 = this._H[6];
    uint num12 = this._H[7];
    for (int index = 0; index < 64 /*0x40*/; ++index)
    {
      uint num13 = (uint) ((int) num12 + (((int) (num9 >> 6) | (int) num9 << 26) ^ ((int) (num9 >> 11) | (int) num9 << 21) ^ ((int) (num9 >> 25) | (int) num9 << 7)) + ((int) num9 & (int) num10 ^ ~(int) num9 & (int) num11)) + k1[index] + buff[index];
      uint num14 = (uint) (((int) (num5 >> 2) | (int) num5 << 30) ^ ((int) (num5 >> 13) | (int) num5 << 19) ^ ((int) (num5 >> 22) | (int) num5 << 10)) + (uint) ((int) num5 & (int) num6 ^ (int) num5 & (int) num7 ^ (int) num6 & (int) num7);
      num12 = num11;
      num11 = num10;
      num10 = num9;
      num9 = num8 + num13;
      num8 = num7;
      num7 = num6;
      num6 = num5;
      num5 = num13 + num14;
    }
    this._H[0] += num5;
    this._H[1] += num6;
    this._H[2] += num7;
    this._H[3] += num8;
    this._H[4] += num9;
    this._H[5] += num10;
    this._H[6] += num11;
    this._H[7] += num12;
  }

  private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
  {
    ulong num1 = this.count + (ulong) inputCount;
    int num2 = 56 - (int) (num1 % 64UL /*0x40*/);
    if (num2 < 1)
      num2 += 64 /*0x40*/;
    byte[] numArray = new byte[inputCount + num2 + 8];
    for (int index = 0; index < inputCount; ++index)
      numArray[index] = inputBuffer[index + inputOffset];
    numArray[inputCount] = (byte) 128 /*0x80*/;
    for (int index = inputCount + 1; index < inputCount + num2; ++index)
      numArray[index] = (byte) 0;
    this.AddLength(num1 << 3, numArray, inputCount + num2);
    this.ProcessBlock(numArray, 0);
    if (inputCount + num2 + 8 != 128 /*0x80*/)
      return;
    this.ProcessBlock(numArray, 64 /*0x40*/);
  }

  internal void AddLength(ulong length, byte[] buffer, int position)
  {
    buffer[position++] = (byte) (length >> 56);
    buffer[position++] = (byte) (length >> 48 /*0x30*/);
    buffer[position++] = (byte) (length >> 40);
    buffer[position++] = (byte) (length >> 32 /*0x20*/);
    buffer[position++] = (byte) (length >> 24);
    buffer[position++] = (byte) (length >> 16 /*0x10*/);
    buffer[position++] = (byte) (length >> 8);
    buffer[position] = (byte) length;
  }
}
