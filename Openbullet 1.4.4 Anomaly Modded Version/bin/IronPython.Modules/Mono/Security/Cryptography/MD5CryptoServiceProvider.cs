// Decompiled with JetBrains decompiler
// Type: Mono.Security.Cryptography.MD5CryptoServiceProvider
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.Security.Cryptography;

#nullable disable
namespace Mono.Security.Cryptography;

internal sealed class MD5CryptoServiceProvider : MD5
{
  private const int BLOCK_SIZE_BYTES = 64 /*0x40*/;
  private uint[] _H;
  private uint[] buff;
  private ulong count;
  private byte[] _ProcessingBuffer;
  private int _ProcessingBufferCount;
  private static readonly uint[] K = new uint[64 /*0x40*/]
  {
    3614090360U,
    3905402710U,
    606105819U,
    3250441966U,
    4118548399U,
    1200080426U,
    2821735955U,
    4249261313U,
    1770035416U,
    2336552879U,
    4294925233U,
    2304563134U,
    1804603682U,
    4254626195U,
    2792965006U,
    1236535329U,
    4129170786U,
    3225465664U,
    643717713U,
    3921069994U,
    3593408605U,
    38016083U,
    3634488961U,
    3889429448U,
    568446438U,
    3275163606U,
    4107603335U,
    1163531501U,
    2850285829U,
    4243563512U,
    1735328473U,
    2368359562U,
    4294588738U,
    2272392833U,
    1839030562U,
    4259657740U,
    2763975236U,
    1272893353U,
    4139469664U,
    3200236656U,
    681279174U,
    3936430074U,
    3572445317U,
    76029189U,
    3654602809U,
    3873151461U,
    530742520U,
    3299628645U,
    4096336452U,
    1126891415U,
    2878612391U,
    4237533241U,
    1700485571U,
    2399980690U,
    4293915773U,
    2240044497U,
    1873313359U,
    4264355552U,
    2734768916U,
    1309151649U,
    4149444226U,
    3174756917U,
    718787259U,
    3951481745U
  };

  public MD5CryptoServiceProvider()
  {
    this._H = new uint[4];
    this.buff = new uint[16 /*0x10*/];
    this._ProcessingBuffer = new byte[64 /*0x40*/];
    this.Initialize();
  }

  ~MD5CryptoServiceProvider() => this.Dispose(false);

  protected override void Dispose(bool disposing)
  {
    if (this._ProcessingBuffer != null)
      Array.Clear((Array) this._ProcessingBuffer, 0, this._ProcessingBuffer.Length);
    if (this._H != null)
      Array.Clear((Array) this._H, 0, this._H.Length);
    if (this.buff != null)
      Array.Clear((Array) this.buff, 0, this.buff.Length);
    base.Dispose(disposing);
  }

  protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
  {
    if (this._ProcessingBufferCount != 0)
    {
      if (cbSize < 64 /*0x40*/ - this._ProcessingBufferCount)
      {
        Buffer.BlockCopy((Array) rgb, ibStart, (Array) this._ProcessingBuffer, this._ProcessingBufferCount, cbSize);
        this._ProcessingBufferCount += cbSize;
        return;
      }
      int count = 64 /*0x40*/ - this._ProcessingBufferCount;
      Buffer.BlockCopy((Array) rgb, ibStart, (Array) this._ProcessingBuffer, this._ProcessingBufferCount, count);
      this.ProcessBlock(this._ProcessingBuffer, 0);
      this._ProcessingBufferCount = 0;
      ibStart += count;
      cbSize -= count;
    }
    for (int index = 0; index < cbSize - cbSize % 64 /*0x40*/; index += 64 /*0x40*/)
      this.ProcessBlock(rgb, ibStart + index);
    if (cbSize % 64 /*0x40*/ == 0)
      return;
    Buffer.BlockCopy((Array) rgb, cbSize - cbSize % 64 /*0x40*/ + ibStart, (Array) this._ProcessingBuffer, 0, cbSize % 64 /*0x40*/);
    this._ProcessingBufferCount = cbSize % 64 /*0x40*/;
  }

  protected override byte[] HashFinal()
  {
    byte[] numArray = new byte[16 /*0x10*/];
    this.ProcessFinalBlock(this._ProcessingBuffer, 0, this._ProcessingBufferCount);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      for (int index2 = 0; index2 < 4; ++index2)
        numArray[index1 * 4 + index2] = (byte) (this._H[index1] >> index2 * 8);
    }
    return numArray;
  }

  public override void Initialize()
  {
    this.count = 0UL;
    this._ProcessingBufferCount = 0;
    this._H[0] = 1732584193U;
    this._H[1] = 4023233417U;
    this._H[2] = 2562383102U;
    this._H[3] = 271733878U;
  }

  private void ProcessBlock(byte[] inputBuffer, int inputOffset)
  {
    this.count += 64UL /*0x40*/;
    for (int index = 0; index < 16 /*0x10*/; ++index)
      this.buff[index] = (uint) ((int) inputBuffer[inputOffset + 4 * index] | (int) inputBuffer[inputOffset + 4 * index + 1] << 8 | (int) inputBuffer[inputOffset + 4 * index + 2] << 16 /*0x10*/ | (int) inputBuffer[inputOffset + 4 * index + 3] << 24);
    uint num1 = this._H[0];
    uint num2 = this._H[1];
    uint num3 = this._H[2];
    uint num4 = this._H[3];
    uint num5 = num1 + (((num3 ^ num4) & num2 ^ num4) + MD5CryptoServiceProvider.K[0] + this.buff[0]);
    uint num6 = (num5 << 7 | num5 >> 25) + num2;
    uint num7 = num4 + (((num2 ^ num3) & num6 ^ num3) + MD5CryptoServiceProvider.K[1] + this.buff[1]);
    uint num8 = (num7 << 12 | num7 >> 20) + num6;
    uint num9 = num3 + (((num6 ^ num2) & num8 ^ num2) + MD5CryptoServiceProvider.K[2] + this.buff[2]);
    uint num10 = (num9 << 17 | num9 >> 15) + num8;
    uint num11 = num2 + (((num8 ^ num6) & num10 ^ num6) + MD5CryptoServiceProvider.K[3] + this.buff[3]);
    uint num12 = (num11 << 22 | num11 >> 10) + num10;
    uint num13 = num6 + (((num10 ^ num8) & num12 ^ num8) + MD5CryptoServiceProvider.K[4] + this.buff[4]);
    uint num14 = (num13 << 7 | num13 >> 25) + num12;
    uint num15 = num8 + (((num12 ^ num10) & num14 ^ num10) + MD5CryptoServiceProvider.K[5] + this.buff[5]);
    uint num16 = (num15 << 12 | num15 >> 20) + num14;
    uint num17 = num10 + (((num14 ^ num12) & num16 ^ num12) + MD5CryptoServiceProvider.K[6] + this.buff[6]);
    uint num18 = (num17 << 17 | num17 >> 15) + num16;
    uint num19 = num12 + (((num16 ^ num14) & num18 ^ num14) + MD5CryptoServiceProvider.K[7] + this.buff[7]);
    uint num20 = (num19 << 22 | num19 >> 10) + num18;
    uint num21 = num14 + (((num18 ^ num16) & num20 ^ num16) + MD5CryptoServiceProvider.K[8] + this.buff[8]);
    uint num22 = (num21 << 7 | num21 >> 25) + num20;
    uint num23 = num16 + (((num20 ^ num18) & num22 ^ num18) + MD5CryptoServiceProvider.K[9] + this.buff[9]);
    uint num24 = (num23 << 12 | num23 >> 20) + num22;
    uint num25 = num18 + (((num22 ^ num20) & num24 ^ num20) + MD5CryptoServiceProvider.K[10] + this.buff[10]);
    uint num26 = (num25 << 17 | num25 >> 15) + num24;
    uint num27 = num20 + (((num24 ^ num22) & num26 ^ num22) + MD5CryptoServiceProvider.K[11] + this.buff[11]);
    uint num28 = (num27 << 22 | num27 >> 10) + num26;
    uint num29 = num22 + (((num26 ^ num24) & num28 ^ num24) + MD5CryptoServiceProvider.K[12] + this.buff[12]);
    uint num30 = (num29 << 7 | num29 >> 25) + num28;
    uint num31 = num24 + (((num28 ^ num26) & num30 ^ num26) + MD5CryptoServiceProvider.K[13] + this.buff[13]);
    uint num32 = (num31 << 12 | num31 >> 20) + num30;
    uint num33 = num26 + (((num30 ^ num28) & num32 ^ num28) + MD5CryptoServiceProvider.K[14] + this.buff[14]);
    uint num34 = (num33 << 17 | num33 >> 15) + num32;
    uint num35 = num28 + (((num32 ^ num30) & num34 ^ num30) + MD5CryptoServiceProvider.K[15] + this.buff[15]);
    uint num36 = (num35 << 22 | num35 >> 10) + num34;
    uint num37 = num30 + (((num36 ^ num34) & num32 ^ num34) + MD5CryptoServiceProvider.K[16 /*0x10*/] + this.buff[1]);
    uint num38 = (num37 << 5 | num37 >> 27) + num36;
    uint num39 = num32 + (((num38 ^ num36) & num34 ^ num36) + MD5CryptoServiceProvider.K[17] + this.buff[6]);
    uint num40 = (num39 << 9 | num39 >> 23) + num38;
    uint num41 = num34 + (((num40 ^ num38) & num36 ^ num38) + MD5CryptoServiceProvider.K[18] + this.buff[11]);
    uint num42 = (num41 << 14 | num41 >> 18) + num40;
    uint num43 = num36 + (((num42 ^ num40) & num38 ^ num40) + MD5CryptoServiceProvider.K[19] + this.buff[0]);
    uint num44 = (num43 << 20 | num43 >> 12) + num42;
    uint num45 = num38 + (((num44 ^ num42) & num40 ^ num42) + MD5CryptoServiceProvider.K[20] + this.buff[5]);
    uint num46 = (num45 << 5 | num45 >> 27) + num44;
    uint num47 = num40 + (((num46 ^ num44) & num42 ^ num44) + MD5CryptoServiceProvider.K[21] + this.buff[10]);
    uint num48 = (num47 << 9 | num47 >> 23) + num46;
    uint num49 = num42 + (((num48 ^ num46) & num44 ^ num46) + MD5CryptoServiceProvider.K[22] + this.buff[15]);
    uint num50 = (num49 << 14 | num49 >> 18) + num48;
    uint num51 = num44 + (((num50 ^ num48) & num46 ^ num48) + MD5CryptoServiceProvider.K[23] + this.buff[4]);
    uint num52 = (num51 << 20 | num51 >> 12) + num50;
    uint num53 = num46 + (((num52 ^ num50) & num48 ^ num50) + MD5CryptoServiceProvider.K[24] + this.buff[9]);
    uint num54 = (num53 << 5 | num53 >> 27) + num52;
    uint num55 = num48 + (((num54 ^ num52) & num50 ^ num52) + MD5CryptoServiceProvider.K[25] + this.buff[14]);
    uint num56 = (num55 << 9 | num55 >> 23) + num54;
    uint num57 = num50 + (((num56 ^ num54) & num52 ^ num54) + MD5CryptoServiceProvider.K[26] + this.buff[3]);
    uint num58 = (num57 << 14 | num57 >> 18) + num56;
    uint num59 = num52 + (((num58 ^ num56) & num54 ^ num56) + MD5CryptoServiceProvider.K[27] + this.buff[8]);
    uint num60 = (num59 << 20 | num59 >> 12) + num58;
    uint num61 = num54 + (((num60 ^ num58) & num56 ^ num58) + MD5CryptoServiceProvider.K[28] + this.buff[13]);
    uint num62 = (num61 << 5 | num61 >> 27) + num60;
    uint num63 = num56 + (((num62 ^ num60) & num58 ^ num60) + MD5CryptoServiceProvider.K[29] + this.buff[2]);
    uint num64 = (num63 << 9 | num63 >> 23) + num62;
    uint num65 = num58 + (((num64 ^ num62) & num60 ^ num62) + MD5CryptoServiceProvider.K[30] + this.buff[7]);
    uint num66 = (num65 << 14 | num65 >> 18) + num64;
    uint num67 = num60 + (((num66 ^ num64) & num62 ^ num64) + MD5CryptoServiceProvider.K[31 /*0x1F*/] + this.buff[12]);
    uint num68 = (num67 << 20 | num67 >> 12) + num66;
    uint num69 = num62 + ((num68 ^ num66 ^ num64) + MD5CryptoServiceProvider.K[32 /*0x20*/] + this.buff[5]);
    uint num70 = (num69 << 4 | num69 >> 28) + num68;
    uint num71 = num64 + ((num70 ^ num68 ^ num66) + MD5CryptoServiceProvider.K[33] + this.buff[8]);
    uint num72 = (num71 << 11 | num71 >> 21) + num70;
    uint num73 = num66 + ((num72 ^ num70 ^ num68) + MD5CryptoServiceProvider.K[34] + this.buff[11]);
    uint num74 = (num73 << 16 /*0x10*/ | num73 >> 16 /*0x10*/) + num72;
    uint num75 = num68 + ((num74 ^ num72 ^ num70) + MD5CryptoServiceProvider.K[35] + this.buff[14]);
    uint num76 = (num75 << 23 | num75 >> 9) + num74;
    uint num77 = num70 + ((num76 ^ num74 ^ num72) + MD5CryptoServiceProvider.K[36] + this.buff[1]);
    uint num78 = (num77 << 4 | num77 >> 28) + num76;
    uint num79 = num72 + ((num78 ^ num76 ^ num74) + MD5CryptoServiceProvider.K[37] + this.buff[4]);
    uint num80 = (num79 << 11 | num79 >> 21) + num78;
    uint num81 = num74 + ((num80 ^ num78 ^ num76) + MD5CryptoServiceProvider.K[38] + this.buff[7]);
    uint num82 = (num81 << 16 /*0x10*/ | num81 >> 16 /*0x10*/) + num80;
    uint num83 = num76 + ((num82 ^ num80 ^ num78) + MD5CryptoServiceProvider.K[39] + this.buff[10]);
    uint num84 = (num83 << 23 | num83 >> 9) + num82;
    uint num85 = num78 + ((num84 ^ num82 ^ num80) + MD5CryptoServiceProvider.K[40] + this.buff[13]);
    uint num86 = (num85 << 4 | num85 >> 28) + num84;
    uint num87 = num80 + ((num86 ^ num84 ^ num82) + MD5CryptoServiceProvider.K[41] + this.buff[0]);
    uint num88 = (num87 << 11 | num87 >> 21) + num86;
    uint num89 = num82 + ((num88 ^ num86 ^ num84) + MD5CryptoServiceProvider.K[42] + this.buff[3]);
    uint num90 = (num89 << 16 /*0x10*/ | num89 >> 16 /*0x10*/) + num88;
    uint num91 = num84 + ((num90 ^ num88 ^ num86) + MD5CryptoServiceProvider.K[43] + this.buff[6]);
    uint num92 = (num91 << 23 | num91 >> 9) + num90;
    uint num93 = num86 + ((num92 ^ num90 ^ num88) + MD5CryptoServiceProvider.K[44] + this.buff[9]);
    uint num94 = (num93 << 4 | num93 >> 28) + num92;
    uint num95 = num88 + ((num94 ^ num92 ^ num90) + MD5CryptoServiceProvider.K[45] + this.buff[12]);
    uint num96 = (num95 << 11 | num95 >> 21) + num94;
    uint num97 = num90 + ((num96 ^ num94 ^ num92) + MD5CryptoServiceProvider.K[46] + this.buff[15]);
    uint num98 = (num97 << 16 /*0x10*/ | num97 >> 16 /*0x10*/) + num96;
    uint num99 = num92 + ((num98 ^ num96 ^ num94) + MD5CryptoServiceProvider.K[47] + this.buff[2]);
    uint num100 = (num99 << 23 | num99 >> 9) + num98;
    uint num101 = num94 + (((~num96 | num100) ^ num98) + MD5CryptoServiceProvider.K[48 /*0x30*/] + this.buff[0]);
    uint num102 = (num101 << 6 | num101 >> 26) + num100;
    uint num103 = num96 + (((~num98 | num102) ^ num100) + MD5CryptoServiceProvider.K[49] + this.buff[7]);
    uint num104 = (num103 << 10 | num103 >> 22) + num102;
    uint num105 = num98 + (((~num100 | num104) ^ num102) + MD5CryptoServiceProvider.K[50] + this.buff[14]);
    uint num106 = (num105 << 15 | num105 >> 17) + num104;
    uint num107 = num100 + (((~num102 | num106) ^ num104) + MD5CryptoServiceProvider.K[51] + this.buff[5]);
    uint num108 = (num107 << 21 | num107 >> 11) + num106;
    uint num109 = num102 + (((~num104 | num108) ^ num106) + MD5CryptoServiceProvider.K[52] + this.buff[12]);
    uint num110 = (num109 << 6 | num109 >> 26) + num108;
    uint num111 = num104 + (((~num106 | num110) ^ num108) + MD5CryptoServiceProvider.K[53] + this.buff[3]);
    uint num112 = (num111 << 10 | num111 >> 22) + num110;
    uint num113 = num106 + (((~num108 | num112) ^ num110) + MD5CryptoServiceProvider.K[54] + this.buff[10]);
    uint num114 = (num113 << 15 | num113 >> 17) + num112;
    uint num115 = num108 + (((~num110 | num114) ^ num112) + MD5CryptoServiceProvider.K[55] + this.buff[1]);
    uint num116 = (num115 << 21 | num115 >> 11) + num114;
    uint num117 = num110 + (((~num112 | num116) ^ num114) + MD5CryptoServiceProvider.K[56] + this.buff[8]);
    uint num118 = (num117 << 6 | num117 >> 26) + num116;
    uint num119 = num112 + (((~num114 | num118) ^ num116) + MD5CryptoServiceProvider.K[57] + this.buff[15]);
    uint num120 = (num119 << 10 | num119 >> 22) + num118;
    uint num121 = num114 + (((~num116 | num120) ^ num118) + MD5CryptoServiceProvider.K[58] + this.buff[6]);
    uint num122 = (num121 << 15 | num121 >> 17) + num120;
    uint num123 = num116 + (((~num118 | num122) ^ num120) + MD5CryptoServiceProvider.K[59] + this.buff[13]);
    uint num124 = (num123 << 21 | num123 >> 11) + num122;
    uint num125 = num118 + (((~num120 | num124) ^ num122) + MD5CryptoServiceProvider.K[60] + this.buff[4]);
    uint num126 = (num125 << 6 | num125 >> 26) + num124;
    uint num127 = num120 + (((~num122 | num126) ^ num124) + MD5CryptoServiceProvider.K[61] + this.buff[11]);
    uint num128 = (num127 << 10 | num127 >> 22) + num126;
    uint num129 = num122 + (((~num124 | num128) ^ num126) + MD5CryptoServiceProvider.K[62] + this.buff[2]);
    uint num130 = (num129 << 15 | num129 >> 17) + num128;
    uint num131 = num124 + (((~num126 | num130) ^ num128) + MD5CryptoServiceProvider.K[63 /*0x3F*/] + this.buff[9]);
    uint num132 = (num131 << 21 | num131 >> 11) + num130;
    this._H[0] += num126;
    this._H[1] += num132;
    this._H[2] += num130;
    this._H[3] += num128;
  }

  private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
  {
    ulong num1 = this.count + (ulong) inputCount;
    int num2 = (int) (56L - (long) (num1 % 64UL /*0x40*/));
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
    buffer[position++] = (byte) length;
    buffer[position++] = (byte) (length >> 8);
    buffer[position++] = (byte) (length >> 16 /*0x10*/);
    buffer[position++] = (byte) (length >> 24);
    buffer[position++] = (byte) (length >> 32 /*0x20*/);
    buffer[position++] = (byte) (length >> 40);
    buffer[position++] = (byte) (length >> 48 /*0x30*/);
    buffer[position] = (byte) (length >> 56);
  }
}
