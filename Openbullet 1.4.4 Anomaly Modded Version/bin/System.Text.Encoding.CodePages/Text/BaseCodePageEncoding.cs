// Decompiled with JetBrains decompiler
// Type: System.Text.BaseCodePageEncoding
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#nullable disable
namespace System.Text;

internal abstract class BaseCodePageEncoding : EncodingNLS, ISerializable
{
  internal const string CODE_PAGE_DATA_FILE_NAME = "codepages.nlp";
  protected int dataTableCodePage;
  protected int iExtraBytes;
  protected char[] arrayUnicodeBestFit;
  protected char[] arrayBytesBestFit;
  private const int CODEPAGE_DATA_FILE_HEADER_SIZE = 44;
  private const int CODEPAGE_HEADER_SIZE = 48 /*0x30*/;
  private static byte[] s_codePagesDataHeader = new byte[44];
  protected static Stream s_codePagesEncodingDataStream = BaseCodePageEncoding.GetEncodingDataStream("codepages.nlp");
  protected static readonly object s_streamLock = new object();
  protected byte[] m_codePageHeader = new byte[48 /*0x30*/];
  protected int m_firstDataWordOffset;
  protected int m_dataSize;
  protected SafeAllocHHandle safeNativeMemoryHandle;

  internal BaseCodePageEncoding(int codepage)
    : this(codepage, codepage)
  {
  }

  internal BaseCodePageEncoding(int codepage, int dataCodePage)
    : base(codepage, (EncoderFallback) new InternalEncoderBestFitFallback((BaseCodePageEncoding) null), (DecoderFallback) new InternalDecoderBestFitFallback((BaseCodePageEncoding) null))
  {
    this.SetFallbackEncoding();
    this.dataTableCodePage = dataCodePage;
    this.LoadCodePageTables();
  }

  internal BaseCodePageEncoding(
    int codepage,
    int dataCodePage,
    EncoderFallback enc,
    DecoderFallback dec)
    : base(codepage, enc, dec)
  {
    this.dataTableCodePage = dataCodePage;
    this.LoadCodePageTables();
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    throw new PlatformNotSupportedException();
  }

  private void SetFallbackEncoding()
  {
    (this.EncoderFallback as InternalEncoderBestFitFallback).encoding = this;
    (this.DecoderFallback as InternalDecoderBestFitFallback).encoding = this;
  }

  internal static Stream GetEncodingDataStream(string tableName)
  {
    Stream manifestResourceStream = typeof (CodePagesEncodingProvider).GetTypeInfo().Assembly.GetManifestResourceStream(tableName);
    if (manifestResourceStream == null)
      throw new InvalidOperationException();
    manifestResourceStream.Read(BaseCodePageEncoding.s_codePagesDataHeader, 0, BaseCodePageEncoding.s_codePagesDataHeader.Length);
    return manifestResourceStream;
  }

  private void LoadCodePageTables()
  {
    if (!this.FindCodePage(this.dataTableCodePage))
      throw new NotSupportedException(SR.Format(SR.NotSupported_NoCodepageData, (object) this.CodePage));
    this.LoadManagedCodePage();
  }

  private unsafe bool FindCodePage(int codePage)
  {
    byte[] buffer = new byte[sizeof (BaseCodePageEncoding.CodePageIndex)];
    lock (BaseCodePageEncoding.s_streamLock)
    {
      BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek(44L, SeekOrigin.Begin);
      int codePageCount;
      fixed (byte* numPtr = &BaseCodePageEncoding.s_codePagesDataHeader[0])
        codePageCount = (int) ((BaseCodePageEncoding.CodePageDataFileHeader*) numPtr)->CodePageCount;
      fixed (byte* numPtr = &buffer[0])
      {
        for (int index = 0; index < codePageCount; ++index)
        {
          BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, buffer.Length);
          if ((int) ((BaseCodePageEncoding.CodePageIndex*) numPtr)->CodePage == codePage)
          {
            long position = BaseCodePageEncoding.s_codePagesEncodingDataStream.Position;
            BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek((long) ((BaseCodePageEncoding.CodePageIndex*) numPtr)->Offset, SeekOrigin.Begin);
            BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(this.m_codePageHeader, 0, this.m_codePageHeader.Length);
            this.m_firstDataWordOffset = (int) BaseCodePageEncoding.s_codePagesEncodingDataStream.Position;
            if (index == codePageCount - 1)
            {
              this.m_dataSize = (int) (BaseCodePageEncoding.s_codePagesEncodingDataStream.Length - (long) ((BaseCodePageEncoding.CodePageIndex*) numPtr)->Offset - (long) this.m_codePageHeader.Length);
            }
            else
            {
              BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek(position, SeekOrigin.Begin);
              int offset = ((BaseCodePageEncoding.CodePageIndex*) numPtr)->Offset;
              BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, buffer.Length);
              this.m_dataSize = ((BaseCodePageEncoding.CodePageIndex*) numPtr)->Offset - offset - this.m_codePageHeader.Length;
            }
            return true;
          }
        }
      }
    }
    return false;
  }

  internal static unsafe int GetCodePageByteSize(int codePage)
  {
    byte[] buffer = new byte[sizeof (BaseCodePageEncoding.CodePageIndex)];
    lock (BaseCodePageEncoding.s_streamLock)
    {
      BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek(44L, SeekOrigin.Begin);
      int codePageCount;
      fixed (byte* numPtr = &BaseCodePageEncoding.s_codePagesDataHeader[0])
        codePageCount = (int) ((BaseCodePageEncoding.CodePageDataFileHeader*) numPtr)->CodePageCount;
      fixed (byte* numPtr = &buffer[0])
      {
        for (int index = 0; index < codePageCount; ++index)
        {
          BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, buffer.Length);
          if ((int) ((BaseCodePageEncoding.CodePageIndex*) numPtr)->CodePage == codePage)
            return (int) ((BaseCodePageEncoding.CodePageIndex*) numPtr)->ByteCount;
        }
      }
    }
    return 0;
  }

  protected abstract void LoadManagedCodePage();

  protected unsafe byte* GetNativeMemory(int iSize)
  {
    if (this.safeNativeMemoryHandle == null)
      this.safeNativeMemoryHandle = new SafeAllocHHandle((IntPtr) (void*) Marshal.AllocHGlobal(iSize));
    return (byte*) (void*) this.safeNativeMemoryHandle.DangerousGetHandle();
  }

  protected abstract void ReadBestFitTable();

  internal new char[] GetBestFitUnicodeToBytesData()
  {
    if (this.arrayUnicodeBestFit == null)
      this.ReadBestFitTable();
    return this.arrayUnicodeBestFit;
  }

  internal new char[] GetBestFitBytesToUnicodeData()
  {
    if (this.arrayBytesBestFit == null)
      this.ReadBestFitTable();
    return this.arrayBytesBestFit;
  }

  internal void CheckMemorySection()
  {
    if (this.safeNativeMemoryHandle == null || !(this.safeNativeMemoryHandle.DangerousGetHandle() == IntPtr.Zero))
      return;
    this.LoadManagedCodePage();
  }

  [StructLayout(LayoutKind.Explicit)]
  internal struct CodePageDataFileHeader
  {
    [FieldOffset(0)]
    internal char TableName;
    [FieldOffset(32 /*0x20*/)]
    internal ushort Version;
    [FieldOffset(40)]
    internal short CodePageCount;
    [FieldOffset(42)]
    internal short unused1;
  }

  [StructLayout(LayoutKind.Explicit, Pack = 2)]
  internal struct CodePageIndex
  {
    [FieldOffset(0)]
    internal char CodePageName;
    [FieldOffset(32 /*0x20*/)]
    internal short CodePage;
    [FieldOffset(34)]
    internal short ByteCount;
    [FieldOffset(36)]
    internal int Offset;
  }

  [StructLayout(LayoutKind.Explicit)]
  internal struct CodePageHeader
  {
    [FieldOffset(0)]
    internal char CodePageName;
    [FieldOffset(32 /*0x20*/)]
    internal ushort VersionMajor;
    [FieldOffset(34)]
    internal ushort VersionMinor;
    [FieldOffset(36)]
    internal ushort VersionRevision;
    [FieldOffset(38)]
    internal ushort VersionBuild;
    [FieldOffset(40)]
    internal short CodePage;
    [FieldOffset(42)]
    internal short ByteCount;
    [FieldOffset(44)]
    internal char UnicodeReplace;
    [FieldOffset(46)]
    internal ushort ByteReplace;
  }
}
