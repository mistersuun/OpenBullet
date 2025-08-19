// Decompiled with JetBrains decompiler
// Type: System.Text.EUCJPEncoding
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal class EUCJPEncoding : DBCSCodePageEncoding
{
  public EUCJPEncoding()
    : base(51932, 932)
  {
  }

  protected override bool CleanUpBytes(ref int bytes)
  {
    if (bytes >= 256 /*0x0100*/)
    {
      if (bytes >= 64064 && bytes <= 64587)
      {
        if (bytes >= 64064 && bytes <= 64091)
        {
          if (bytes <= 64073)
            bytes -= 2897;
          else if (bytes >= 64074 && bytes <= 64083)
            bytes -= 29430;
          else if (bytes >= 64084 && bytes <= 64087)
            bytes -= 2907;
          else if (bytes == 64088)
            bytes = 34698;
          else if (bytes == 64089)
            bytes = 34690;
          else if (bytes == 64090)
            bytes = 34692;
          else if (bytes == 64091)
            bytes = 34714;
        }
        else if (bytes >= 64092 && bytes <= 64587)
        {
          byte num = (byte) bytes;
          if (num < (byte) 92)
            bytes -= 3423;
          else if (num >= (byte) 128 /*0x80*/ && num <= (byte) 155)
            bytes -= 3357;
          else
            bytes -= 3356;
        }
      }
      byte num1 = (byte) (bytes >> 8);
      byte num2 = (byte) bytes;
      byte num3 = (byte) (((int) (byte) ((int) num1 - (num1 > (byte) 159 ? 177 : 113)) << 1) + 1);
      byte num4;
      if (num2 > (byte) 158)
      {
        num4 = (byte) ((uint) num2 - 126U);
        ++num3;
      }
      else
      {
        if (num2 > (byte) 126)
          --num2;
        num4 = (byte) ((uint) num2 - 31U /*0x1F*/);
      }
      bytes = (int) num3 << 8 | (int) num4 | 32896;
      if ((bytes & 65280) < 41216 || (bytes & 65280) > 65024 || (bytes & (int) byte.MaxValue) < 161 || (bytes & (int) byte.MaxValue) > 254)
        return false;
    }
    else
    {
      if (bytes >= 161 && bytes <= 223)
      {
        bytes |= 36352;
        return true;
      }
      if (bytes >= 129 && bytes != 160 /*0xA0*/ && bytes != (int) byte.MaxValue)
        return false;
    }
    return true;
  }

  protected override unsafe void CleanUpEndBytes(char* chars)
  {
    for (int index = 161; index <= 254; ++index)
      chars[index] = '\uFFFE';
    chars[142] = '\uFFFE';
  }
}
