// Decompiled with JetBrains decompiler
// Type: System.Text.CodePagesEncodingProvider
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace System.Text;

public sealed class CodePagesEncodingProvider : EncodingProvider
{
  private static readonly EncodingProvider s_singleton = (EncodingProvider) new CodePagesEncodingProvider();
  private Dictionary<int, Encoding> _encodings = new Dictionary<int, Encoding>();
  private ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
  private const int ISCIIAssemese = 57006;
  private const int ISCIIBengali = 57003;
  private const int ISCIIDevanagari = 57002;
  private const int ISCIIGujarathi = 57010;
  private const int ISCIIKannada = 57008;
  private const int ISCIIMalayalam = 57009;
  private const int ISCIIOriya = 57007;
  private const int ISCIIPanjabi = 57011;
  private const int ISCIITamil = 57004;
  private const int ISCIITelugu = 57005;
  private const int ISOKorean = 50225;
  private const int ChineseHZ = 52936;
  private const int ISO2022JP = 50220;
  private const int ISO2022JPESC = 50221;
  private const int ISO2022JPSISO = 50222;
  private const int ISOSimplifiedCN = 50227;
  private const int EUCJP = 51932;
  private const int CodePageMacGB2312 = 10008;
  private const int CodePageMacKorean = 10003;
  private const int CodePageGB2312 = 20936;
  private const int CodePageDLLKorean = 20949;
  private const int GB18030 = 54936;
  private const int DuplicateEUCCN = 51936;
  private const int EUCKR = 51949;
  private const int EUCCN = 936;
  private const int ISO_8859_8I = 38598;
  private const int ISO_8859_8_Visual = 28598;

  internal CodePagesEncodingProvider()
  {
  }

  public static EncodingProvider Instance => CodePagesEncodingProvider.s_singleton;

  public override Encoding GetEncoding(int codepage)
  {
    if (codepage < 0 || codepage > (int) ushort.MaxValue)
      return (Encoding) null;
    if (codepage == 0)
    {
      int systemDefaultCodePage = CodePagesEncodingProvider.SystemDefaultCodePage;
      return systemDefaultCodePage == 0 ? (Encoding) null : this.GetEncoding(systemDefaultCodePage);
    }
    Encoding encoding1 = (Encoding) null;
    this._cacheLock.EnterUpgradeableReadLock();
    try
    {
      if (this._encodings.TryGetValue(codepage, out encoding1))
        return encoding1;
      switch (BaseCodePageEncoding.GetCodePageByteSize(codepage))
      {
        case 1:
          encoding1 = (Encoding) new SBCSCodePageEncoding(codepage);
          break;
        case 2:
          encoding1 = (Encoding) new DBCSCodePageEncoding(codepage);
          break;
        default:
          encoding1 = CodePagesEncodingProvider.GetEncodingRare(codepage);
          if (encoding1 == null)
            return (Encoding) null;
          break;
      }
      this._cacheLock.EnterWriteLock();
      try
      {
        Encoding encoding2;
        if (this._encodings.TryGetValue(codepage, out encoding2))
          return encoding2;
        this._encodings.Add(codepage, encoding1);
      }
      finally
      {
        this._cacheLock.ExitWriteLock();
      }
    }
    finally
    {
      this._cacheLock.ExitUpgradeableReadLock();
    }
    return encoding1;
  }

  public override Encoding GetEncoding(string name)
  {
    int codePageFromName = EncodingTable.GetCodePageFromName(name);
    return codePageFromName == 0 ? (Encoding) null : this.GetEncoding(codePageFromName);
  }

  private static Encoding GetEncodingRare(int codepage)
  {
    Encoding encodingRare = (Encoding) null;
    switch (codepage)
    {
      case 10003:
        encodingRare = (Encoding) new DBCSCodePageEncoding(10003, 20949);
        break;
      case 10008:
        encodingRare = (Encoding) new DBCSCodePageEncoding(10008, 20936);
        break;
      case 38598:
        encodingRare = (Encoding) new SBCSCodePageEncoding(codepage, 28598);
        break;
      case 50220:
      case 50221:
      case 50222:
      case 50225:
      case 52936:
        encodingRare = (Encoding) new ISO2022Encoding(codepage);
        break;
      case 50227:
      case 51936:
        encodingRare = (Encoding) new DBCSCodePageEncoding(codepage, 936);
        break;
      case 51932:
        encodingRare = (Encoding) new EUCJPEncoding();
        break;
      case 51949:
        encodingRare = (Encoding) new DBCSCodePageEncoding(codepage, 20949);
        break;
      case 54936:
        encodingRare = (Encoding) new GB18030Encoding();
        break;
      case 57002:
      case 57003:
      case 57004:
      case 57005:
      case 57006:
      case 57007:
      case 57008:
      case 57009:
      case 57010:
      case 57011:
        encodingRare = (Encoding) new ISCIIEncoding(codepage);
        break;
    }
    return encodingRare;
  }

  private static int SystemDefaultCodePage
  {
    get
    {
      int codePage;
      return !Interop.Kernel32.TryGetACPCodePage(out codePage) ? 0 : codePage;
    }
  }
}
