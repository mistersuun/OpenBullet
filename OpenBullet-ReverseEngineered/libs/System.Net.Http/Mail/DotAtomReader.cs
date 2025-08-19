// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.DotAtomReader
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Net.Mime;

#nullable disable
namespace System.Net.Mail;

internal static class DotAtomReader
{
  internal static int ReadReverse(string data, int index)
  {
    int num = index;
    while (0 <= index && ((int) data[index] > MailBnfHelper.Ascii7bitMaxValue || (int) data[index] == (int) MailBnfHelper.Dot || MailBnfHelper.Atext[(int) data[index]]))
      --index;
    if (num == index)
      throw new FormatException(SR.Format(SR.MailHeaderFieldInvalidCharacter, (object) data[index]));
    if ((int) data[index + 1] == (int) MailBnfHelper.Dot)
      throw new FormatException(SR.Format(SR.MailHeaderFieldInvalidCharacter, (object) MailBnfHelper.Dot));
    return index;
  }
}
