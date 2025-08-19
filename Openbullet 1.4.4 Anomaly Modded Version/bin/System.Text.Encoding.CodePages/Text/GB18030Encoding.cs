// Decompiled with JetBrains decompiler
// Type: System.Text.GB18030Encoding
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal sealed class GB18030Encoding : DBCSCodePageEncoding
{
  private const int GBLast4ByteCode = 39419;
  internal unsafe char* map4BytesToUnicode = (char*) null;
  internal unsafe byte* mapUnicodeTo4BytesFlags = (byte*) null;
  private const int GB18030 = 54936;
  private const int GBSurrogateOffset = 189000;
  private const int GBLastSurrogateOffset = 1237575;
  private readonly ushort[] _tableUnicodeToGBDiffs = new ushort[439]
  {
    (ushort) 32896,
    (ushort) 36,
    (ushort) 32769,
    (ushort) 2,
    (ushort) 32770,
    (ushort) 7,
    (ushort) 32770,
    (ushort) 5,
    (ushort) 32769,
    (ushort) 31 /*0x1F*/,
    (ushort) 32769,
    (ushort) 8,
    (ushort) 32770,
    (ushort) 6,
    (ushort) 32771,
    (ushort) 1,
    (ushort) 32770,
    (ushort) 4,
    (ushort) 32770,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32770,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 4,
    (ushort) 32769,
    (ushort) 17,
    (ushort) 32769,
    (ushort) 7,
    (ushort) 32769,
    (ushort) 15,
    (ushort) 32769,
    (ushort) 24,
    (ushort) 32769,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 4,
    (ushort) 32769,
    (ushort) 29,
    (ushort) 32769,
    (ushort) 98,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 28,
    (ushort) 43199,
    (ushort) 87,
    (ushort) 32769,
    (ushort) 15,
    (ushort) 32769,
    (ushort) 101,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32771,
    (ushort) 13,
    (ushort) 32769,
    (ushort) 183,
    (ushort) 32785,
    (ushort) 1,
    (ushort) 32775,
    (ushort) 7,
    (ushort) 32785,
    (ushort) 1,
    (ushort) 32775,
    (ushort) 55,
    (ushort) 32769,
    (ushort) 14,
    (ushort) 32832,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 7102,
    (ushort) 32769,
    (ushort) 2,
    (ushort) 32772,
    (ushort) 1,
    (ushort) 32770,
    (ushort) 2,
    (ushort) 32770,
    (ushort) 7,
    (ushort) 32770,
    (ushort) 9,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32770,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 5,
    (ushort) 32769,
    (ushort) 112 /*0x70*/,
    (ushort) 41699,
    (ushort) 86,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 12,
    (ushort) 32769,
    (ushort) 10,
    (ushort) 32769,
    (ushort) 62,
    (ushort) 32780,
    (ushort) 4,
    (ushort) 32778,
    (ushort) 22,
    (ushort) 32772,
    (ushort) 2,
    (ushort) 32772,
    (ushort) 110,
    (ushort) 32769,
    (ushort) 6,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 4,
    (ushort) 32769,
    (ushort) 2,
    (ushort) 32772,
    (ushort) 2,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32773,
    (ushort) 2,
    (ushort) 32769,
    (ushort) 5,
    (ushort) 32772,
    (ushort) 5,
    (ushort) 32769,
    (ushort) 10,
    (ushort) 32769,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 5,
    (ushort) 32769,
    (ushort) 13,
    (ushort) 32770,
    (ushort) 2,
    (ushort) 32772,
    (ushort) 6,
    (ushort) 32770,
    (ushort) 37,
    (ushort) 32769,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 11,
    (ushort) 32769,
    (ushort) 25,
    (ushort) 32769,
    (ushort) 82,
    (ushort) 32769,
    (ushort) 333,
    (ushort) 32778,
    (ushort) 10,
    (ushort) 32808,
    (ushort) 100,
    (ushort) 32844,
    (ushort) 4,
    (ushort) 32804,
    (ushort) 13,
    (ushort) 32783,
    (ushort) 3,
    (ushort) 32771,
    (ushort) 10,
    (ushort) 32770,
    (ushort) 16 /*0x10*/,
    (ushort) 32770,
    (ushort) 8,
    (ushort) 32770,
    (ushort) 8,
    (ushort) 32770,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 2,
    (ushort) 32770,
    (ushort) 18,
    (ushort) 32772,
    (ushort) 31 /*0x1F*/,
    (ushort) 32770,
    (ushort) 2,
    (ushort) 32769,
    (ushort) 54,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 2110,
    (ushort) 65104,
    (ushort) 2,
    (ushort) 65108,
    (ushort) 3,
    (ushort) 65111,
    (ushort) 2,
    (ushort) 65112,
    (ushort) 65117,
    (ushort) 10,
    (ushort) 65118,
    (ushort) 15,
    (ushort) 65131,
    (ushort) 2,
    (ushort) 65134,
    (ushort) 3,
    (ushort) 65137,
    (ushort) 4,
    (ushort) 65139,
    (ushort) 2,
    (ushort) 65140,
    (ushort) 65141,
    (ushort) 3,
    (ushort) 65145,
    (ushort) 14,
    (ushort) 65156,
    (ushort) 293,
    (ushort) 43402,
    (ushort) 43403,
    (ushort) 43404,
    (ushort) 43405,
    (ushort) 43406,
    (ushort) 43407,
    (ushort) 43408,
    (ushort) 43409,
    (ushort) 43410,
    (ushort) 43411,
    (ushort) 43412,
    (ushort) 43413,
    (ushort) 4,
    (ushort) 32772,
    (ushort) 1,
    (ushort) 32787,
    (ushort) 5,
    (ushort) 32770,
    (ushort) 2,
    (ushort) 32777,
    (ushort) 20,
    (ushort) 43401,
    (ushort) 2,
    (ushort) 32851,
    (ushort) 7,
    (ushort) 32772,
    (ushort) 2,
    (ushort) 32854,
    (ushort) 5,
    (ushort) 32771,
    (ushort) 6,
    (ushort) 32805,
    (ushort) 246,
    (ushort) 32778,
    (ushort) 7,
    (ushort) 32769,
    (ushort) 113,
    (ushort) 32769,
    (ushort) 234,
    (ushort) 32770,
    (ushort) 12,
    (ushort) 32771,
    (ushort) 2,
    (ushort) 32769,
    (ushort) 34,
    (ushort) 32769,
    (ushort) 9,
    (ushort) 32769,
    (ushort) 2,
    (ushort) 32770,
    (ushort) 2,
    (ushort) 32769,
    (ushort) 113,
    (ushort) 65110,
    (ushort) 43,
    (ushort) 65109,
    (ushort) 298,
    (ushort) 65114,
    (ushort) 111,
    (ushort) 65116,
    (ushort) 11,
    (ushort) 65115,
    (ushort) 765,
    (ushort) 65120,
    (ushort) 85,
    (ushort) 65119,
    (ushort) 96 /*0x60*/,
    (ushort) 65122,
    (ushort) 65125,
    (ushort) 14,
    (ushort) 65123,
    (ushort) 147,
    (ushort) 65124,
    (ushort) 218,
    (ushort) 65128,
    (ushort) 287,
    (ushort) 65129,
    (ushort) 113,
    (ushort) 65130,
    (ushort) 885,
    (ushort) 65135,
    (ushort) 264,
    (ushort) 65136,
    (ushort) 471,
    (ushort) 65138,
    (ushort) 116,
    (ushort) 65144,
    (ushort) 4,
    (ushort) 65143,
    (ushort) 43,
    (ushort) 65146,
    (ushort) 248,
    (ushort) 65147,
    (ushort) 373,
    (ushort) 65149,
    (ushort) 20,
    (ushort) 65148,
    (ushort) 193,
    (ushort) 65152,
    (ushort) 5,
    (ushort) 65153,
    (ushort) 82,
    (ushort) 65154,
    (ushort) 16 /*0x10*/,
    (ushort) 65155,
    (ushort) 441,
    (ushort) 65157,
    (ushort) 50,
    (ushort) 65158,
    (ushort) 2,
    (ushort) 65159,
    (ushort) 4,
    (ushort) 65160,
    (ushort) 65161,
    (ushort) 1,
    (ushort) 65162,
    (ushort) 65163,
    (ushort) 20,
    (ushort) 65165,
    (ushort) 3,
    (ushort) 65164,
    (ushort) 22,
    (ushort) 65167,
    (ushort) 65166,
    (ushort) 703,
    (ushort) 65174,
    (ushort) 39,
    (ushort) 65171,
    (ushort) 65172,
    (ushort) 65173,
    (ushort) 65175,
    (ushort) 65170,
    (ushort) 111,
    (ushort) 65176,
    (ushort) 65177,
    (ushort) 65178,
    (ushort) 65179,
    (ushort) 65180,
    (ushort) 65181,
    (ushort) 65182,
    (ushort) 148,
    (ushort) 65183,
    (ushort) 81,
    (ushort) 53670,
    (ushort) 14426,
    (ushort) 36716,
    (ushort) 1,
    (ushort) 32859,
    (ushort) 1,
    (ushort) 32798,
    (ushort) 13,
    (ushort) 32801,
    (ushort) 1,
    (ushort) 32771,
    (ushort) 5,
    (ushort) 32769,
    (ushort) 7,
    (ushort) 32769,
    (ushort) 4,
    (ushort) 32770,
    (ushort) 4,
    (ushort) 32770,
    (ushort) 8,
    (ushort) 32769,
    (ushort) 7,
    (ushort) 32769,
    (ushort) 16 /*0x10*/,
    (ushort) 32770,
    (ushort) 14,
    (ushort) 32769,
    (ushort) 4295,
    (ushort) 32769,
    (ushort) 76,
    (ushort) 32769,
    (ushort) 27,
    (ushort) 32769,
    (ushort) 81,
    (ushort) 32769,
    (ushort) 9,
    (ushort) 32769,
    (ushort) 26,
    (ushort) 32772,
    (ushort) 1,
    (ushort) 32769,
    (ushort) 1,
    (ushort) 32770,
    (ushort) 3,
    (ushort) 32769,
    (ushort) 6,
    (ushort) 32771,
    (ushort) 1,
    (ushort) 32770,
    (ushort) 2,
    (ushort) 32771,
    (ushort) 1030,
    (ushort) 32770,
    (ushort) 1,
    (ushort) 32786,
    (ushort) 4,
    (ushort) 32778,
    (ushort) 1,
    (ushort) 32772,
    (ushort) 1,
    (ushort) 32782,
    (ushort) 1,
    (ushort) 32772,
    (ushort) 149,
    (ushort) 32862,
    (ushort) 129,
    (ushort) 32774,
    (ushort) 26
  };

  internal GB18030Encoding()
    : base(54936, 936, EncoderFallback.ReplacementFallback, DecoderFallback.ReplacementFallback)
  {
  }

  protected override unsafe void LoadManagedCodePage()
  {
    this.iExtraBytes = 87032;
    base.LoadManagedCodePage();
    byte* handle = (byte*) (void*) this.safeNativeMemoryHandle.DangerousGetHandle();
    this.mapUnicodeTo4BytesFlags = handle + 262144 /*0x040000*/;
    this.map4BytesToUnicode = (char*) (handle + 262144 /*0x040000*/ + 8192 /*0x2000*/);
    char minValue = char.MinValue;
    ushort index1 = 0;
    for (int index2 = 0; index2 < this._tableUnicodeToGBDiffs.Length; ++index2)
    {
      ushort tableUnicodeToGbDiff = this._tableUnicodeToGBDiffs[index2];
      if (((int) tableUnicodeToGbDiff & 32768 /*0x8000*/) != 0)
      {
        if (tableUnicodeToGbDiff > (ushort) 36864 /*0x9000*/ && tableUnicodeToGbDiff != (ushort) 53670)
        {
          this.mapBytesToUnicode[tableUnicodeToGbDiff] = minValue;
          this.mapUnicodeToBytes[minValue] = tableUnicodeToGbDiff;
          ++minValue;
        }
        else
          minValue += (char) ((uint) tableUnicodeToGbDiff & (uint) short.MaxValue);
      }
      else
      {
        for (; tableUnicodeToGbDiff > (ushort) 0; --tableUnicodeToGbDiff)
        {
          this.map4BytesToUnicode[index1] = minValue;
          this.mapUnicodeToBytes[minValue] = index1;
          byte* numPtr = this.mapUnicodeTo4BytesFlags + (int) minValue / 8;
          *numPtr = (byte) ((uint) *numPtr | (uint) (byte) (1 << (int) minValue % 8));
          ++minValue;
          ++index1;
        }
      }
    }
  }

  internal unsafe bool Is4Byte(char charTest)
  {
    byte num = this.mapUnicodeTo4BytesFlags[(int) charTest / 8];
    return num != (byte) 0 && ((uint) num & (uint) (1 << (int) charTest % 8)) > 0U;
  }

  public override unsafe int GetByteCount(char* chars, int count, EncoderNLS encoder)
  {
    return this.GetBytes(chars, count, (byte*) null, 0, encoder);
  }

  public override unsafe int GetBytes(
    char* chars,
    int charCount,
    byte* bytes,
    int byteCount,
    EncoderNLS encoder)
  {
    char charFallback = char.MinValue;
    if (encoder != null)
      charFallback = encoder.charLeftOver;
    System.Text.EncodingByteBuffer encodingByteBuffer = new System.Text.EncodingByteBuffer((EncodingNLS) this, encoder, bytes, byteCount, chars, charCount);
    while (true)
    {
      while (encodingByteBuffer.MoreData)
      {
        char nextChar = encodingByteBuffer.GetNextChar();
        if (charFallback != char.MinValue)
        {
          if (!char.IsLowSurrogate(nextChar))
          {
            encodingByteBuffer.MovePrevious(false);
            if (!encodingByteBuffer.Fallback(charFallback))
            {
              charFallback = char.MinValue;
              break;
            }
            charFallback = char.MinValue;
          }
          else
          {
            int num1 = ((int) charFallback - 55296 << 10) + ((int) nextChar - 56320);
            byte b4 = (byte) (num1 % 10 + 48 /*0x30*/);
            int num2 = num1 / 10;
            byte b3 = (byte) (num2 % 126 + 129);
            int num3 = num2 / 126;
            byte b2 = (byte) (num3 % 10 + 48 /*0x30*/);
            int num4 = num3 / 10;
            charFallback = char.MinValue;
            if (!encodingByteBuffer.AddByte((byte) (num4 + 144 /*0x90*/), b2, b3, b4))
            {
              encodingByteBuffer.MovePrevious(false);
              break;
            }
            charFallback = char.MinValue;
          }
        }
        else if (nextChar <= '\u007F')
        {
          if (!encodingByteBuffer.AddByte((byte) nextChar))
            break;
        }
        else if (char.IsHighSurrogate(nextChar))
          charFallback = nextChar;
        else if (char.IsLowSurrogate(nextChar))
        {
          if (!encodingByteBuffer.Fallback(nextChar))
            break;
        }
        else
        {
          ushort num5 = this.mapUnicodeToBytes[nextChar];
          if (this.Is4Byte(nextChar))
          {
            byte b4 = (byte) ((int) num5 % 10 + 48 /*0x30*/);
            ushort num6 = (ushort) ((uint) num5 / 10U);
            byte b3 = (byte) ((int) num6 % 126 + 129);
            ushort num7 = (ushort) ((uint) num6 / 126U);
            byte b2 = (byte) ((int) num7 % 10 + 48 /*0x30*/);
            ushort num8 = (ushort) ((uint) num7 / 10U);
            if (!encodingByteBuffer.AddByte((byte) ((uint) num8 + 129U), b2, b3, b4))
              break;
          }
          else if (!encodingByteBuffer.AddByte((byte) ((uint) num5 >> 8), (byte) ((uint) num5 & (uint) byte.MaxValue)))
            break;
        }
      }
      if ((encoder == null || encoder.MustFlush) && charFallback > char.MinValue)
      {
        encodingByteBuffer.Fallback(charFallback);
        charFallback = char.MinValue;
      }
      else
        break;
    }
    if (encoder != null)
    {
      if ((IntPtr) bytes != IntPtr.Zero)
        encoder.charLeftOver = charFallback;
      encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
    }
    return encodingByteBuffer.Count;
  }

  internal bool IsGBLeadByte(short ch) => ch >= (short) 129 && ch <= (short) 254;

  internal bool IsGBTwoByteTrailing(short ch)
  {
    if (ch >= (short) 64 /*0x40*/ && ch <= (short) 126)
      return true;
    return ch >= (short) 128 /*0x80*/ && ch <= (short) 254;
  }

  internal bool IsGBFourByteTrailing(short ch) => ch >= (short) 48 /*0x30*/ && ch <= (short) 57;

  internal int GetFourBytesOffset(short offset1, short offset2, short offset3, short offset4)
  {
    return ((int) offset1 - 129) * 10 * 126 * 10 + ((int) offset2 - 48 /*0x30*/) * 126 * 10 + ((int) offset3 - 129) * 10 + (int) offset4 - 48 /*0x30*/;
  }

  public override unsafe int GetCharCount(byte* bytes, int count, DecoderNLS baseDecoder)
  {
    return this.GetChars(bytes, count, (char*) null, 0, baseDecoder);
  }

  public override unsafe int GetChars(
    byte* bytes,
    int byteCount,
    char* chars,
    int charCount,
    DecoderNLS baseDecoder)
  {
    GB18030Encoding.GB18030Decoder decoder = (GB18030Encoding.GB18030Decoder) baseDecoder;
    System.Text.EncodingCharBuffer encodingCharBuffer = new System.Text.EncodingCharBuffer((EncodingNLS) this, (DecoderNLS) decoder, chars, charCount, bytes, byteCount);
    short num1 = -1;
    short num2 = -1;
    short num3 = -1;
    short num4 = -1;
    if (decoder != null && decoder.bLeftOver1 != (short) -1)
    {
      num1 = decoder.bLeftOver1;
      num2 = decoder.bLeftOver2;
      num3 = decoder.bLeftOver3;
      num4 = decoder.bLeftOver4;
      while (num1 != (short) -1)
      {
        if (!this.IsGBLeadByte(num1))
        {
          if (num1 <= (short) sbyte.MaxValue)
          {
            if (!encodingCharBuffer.AddChar((char) num1))
              break;
          }
          else if (!encodingCharBuffer.Fallback((byte) num1))
            break;
          num1 = num2;
          num2 = num3;
          num3 = num4;
          num4 = (short) -1;
        }
        else
        {
          while (num2 == (short) -1 || this.IsGBFourByteTrailing(num2) && num4 == (short) -1)
          {
            if (!encodingCharBuffer.MoreData)
            {
              if (!decoder.MustFlush)
              {
                if ((IntPtr) chars != IntPtr.Zero)
                {
                  decoder.bLeftOver1 = num1;
                  decoder.bLeftOver2 = num2;
                  decoder.bLeftOver3 = num3;
                  decoder.bLeftOver4 = num4;
                }
                decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
                return encodingCharBuffer.Count;
              }
              break;
            }
            if (num2 == (short) -1)
              num2 = (short) encodingCharBuffer.GetNextByte();
            else if (num3 == (short) -1)
              num3 = (short) encodingCharBuffer.GetNextByte();
            else
              num4 = (short) encodingCharBuffer.GetNextByte();
          }
          if (this.IsGBTwoByteTrailing(num2))
          {
            int index = (int) num1 << 8 | (int) (byte) num2;
            if (encodingCharBuffer.AddChar(this.mapBytesToUnicode[index], 2))
            {
              num1 = (short) -1;
              num2 = (short) -1;
            }
            else
              break;
          }
          else if (this.IsGBFourByteTrailing(num2) && this.IsGBLeadByte(num3) && this.IsGBFourByteTrailing(num4))
          {
            int fourBytesOffset = this.GetFourBytesOffset(num1, num2, num3, num4);
            if (fourBytesOffset <= 39419)
            {
              if (!encodingCharBuffer.AddChar(this.map4BytesToUnicode[fourBytesOffset], 4))
                break;
            }
            else if (fourBytesOffset >= 189000 && fourBytesOffset <= 1237575)
            {
              int num5 = fourBytesOffset - 189000;
              if (!encodingCharBuffer.AddChar((char) (55296 + num5 / 1024 /*0x0400*/), (char) (56320 + num5 % 1024 /*0x0400*/), 4))
                break;
            }
            else if (!encodingCharBuffer.Fallback((byte) num1, (byte) num2, (byte) num3, (byte) num4))
              break;
            num1 = (short) -1;
            num2 = (short) -1;
            num3 = (short) -1;
            num4 = (short) -1;
          }
          else if (encodingCharBuffer.Fallback((byte) num1))
          {
            num1 = num2;
            num2 = num3;
            num3 = num4;
            num4 = (short) -1;
          }
          else
            break;
        }
      }
    }
    while (encodingCharBuffer.MoreData)
    {
      byte nextByte1 = encodingCharBuffer.GetNextByte();
      if (nextByte1 <= (byte) 127 /*0x7F*/)
      {
        if (!encodingCharBuffer.AddChar((char) nextByte1))
          break;
      }
      else if (this.IsGBLeadByte((short) nextByte1))
      {
        if (encodingCharBuffer.MoreData)
        {
          byte nextByte2 = encodingCharBuffer.GetNextByte();
          if (this.IsGBTwoByteTrailing((short) nextByte2))
          {
            int index = (int) nextByte1 << 8 | (int) nextByte2;
            if (!encodingCharBuffer.AddChar(this.mapBytesToUnicode[index], 2))
              break;
          }
          else if (this.IsGBFourByteTrailing((short) nextByte2))
          {
            if (encodingCharBuffer.EvenMoreData(2))
            {
              byte nextByte3 = encodingCharBuffer.GetNextByte();
              byte nextByte4 = encodingCharBuffer.GetNextByte();
              if (this.IsGBLeadByte((short) nextByte3) && this.IsGBFourByteTrailing((short) nextByte4))
              {
                int fourBytesOffset = this.GetFourBytesOffset((short) nextByte1, (short) nextByte2, (short) nextByte3, (short) nextByte4);
                if (fourBytesOffset <= 39419)
                {
                  if (!encodingCharBuffer.AddChar(this.map4BytesToUnicode[fourBytesOffset], 4))
                    break;
                }
                else if (fourBytesOffset >= 189000 && fourBytesOffset <= 1237575)
                {
                  int num6 = fourBytesOffset - 189000;
                  if (!encodingCharBuffer.AddChar((char) (55296 + num6 / 1024 /*0x0400*/), (char) (56320 + num6 % 1024 /*0x0400*/), 4))
                    break;
                }
                else if (!encodingCharBuffer.Fallback(nextByte1, nextByte2, nextByte3, nextByte4))
                  break;
              }
              else
              {
                encodingCharBuffer.AdjustBytes(-3);
                if (!encodingCharBuffer.Fallback(nextByte1))
                  break;
              }
            }
            else
            {
              if (decoder != null && !decoder.MustFlush)
              {
                if ((IntPtr) chars != IntPtr.Zero)
                {
                  num1 = (short) nextByte1;
                  num2 = (short) nextByte2;
                  num3 = !encodingCharBuffer.MoreData ? (short) -1 : (short) encodingCharBuffer.GetNextByte();
                  num4 = (short) -1;
                  break;
                }
                break;
              }
              if (!encodingCharBuffer.Fallback(nextByte1, nextByte2))
                break;
            }
          }
          else
          {
            encodingCharBuffer.AdjustBytes(-1);
            if (!encodingCharBuffer.Fallback(nextByte1))
              break;
          }
        }
        else
        {
          if (decoder != null && !decoder.MustFlush)
          {
            if ((IntPtr) chars != IntPtr.Zero)
            {
              num1 = (short) nextByte1;
              num2 = (short) -1;
              num3 = (short) -1;
              num4 = (short) -1;
              break;
            }
            break;
          }
          if (!encodingCharBuffer.Fallback(nextByte1))
            break;
        }
      }
      else if (!encodingCharBuffer.Fallback(nextByte1))
        break;
    }
    if (decoder != null)
    {
      if ((IntPtr) chars != IntPtr.Zero)
      {
        decoder.bLeftOver1 = num1;
        decoder.bLeftOver2 = num2;
        decoder.bLeftOver3 = num3;
        decoder.bLeftOver4 = num4;
      }
      decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
    }
    return encodingCharBuffer.Count;
  }

  public override int GetMaxByteCount(int charCount)
  {
    if (charCount < 0)
      throw new ArgumentOutOfRangeException(nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    long num1 = (long) charCount + 1L;
    if (this.EncoderFallback.MaxCharCount > 1)
      num1 *= (long) this.EncoderFallback.MaxCharCount;
    long num2 = num1 * 4L;
    return num2 <= (long) int.MaxValue ? (int) num2 : throw new ArgumentOutOfRangeException(nameof (charCount), SR.ArgumentOutOfRange_GetByteCountOverflow);
  }

  public override int GetMaxCharCount(int byteCount)
  {
    if (byteCount < 0)
      throw new ArgumentOutOfRangeException(nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    long num = (long) byteCount + 3L;
    if (this.DecoderFallback.MaxCharCount > 1)
      num *= (long) this.DecoderFallback.MaxCharCount;
    return num <= (long) int.MaxValue ? (int) num : throw new ArgumentOutOfRangeException(nameof (byteCount), SR.ArgumentOutOfRange_GetCharCountOverflow);
  }

  public override Decoder GetDecoder()
  {
    return (Decoder) new GB18030Encoding.GB18030Decoder((EncodingNLS) this);
  }

  internal sealed class GB18030Decoder : DecoderNLS
  {
    internal short bLeftOver1 = -1;
    internal short bLeftOver2 = -1;
    internal short bLeftOver3 = -1;
    internal short bLeftOver4 = -1;

    internal GB18030Decoder(EncodingNLS encoding)
      : base(encoding)
    {
    }

    public override void Reset()
    {
      this.bLeftOver1 = (short) -1;
      this.bLeftOver2 = (short) -1;
      this.bLeftOver3 = (short) -1;
      this.bLeftOver4 = (short) -1;
      if (this.m_fallbackBuffer == null)
        return;
      this.m_fallbackBuffer.Reset();
    }

    internal override bool HasState => this.bLeftOver1 >= (short) 0;
  }
}
