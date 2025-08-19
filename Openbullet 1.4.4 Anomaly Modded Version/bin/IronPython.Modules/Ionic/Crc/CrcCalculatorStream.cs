// Decompiled with JetBrains decompiler
// Type: Ionic.Crc.CrcCalculatorStream
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.IO;

#nullable disable
namespace Ionic.Crc;

public class CrcCalculatorStream : Stream, IDisposable
{
  private static readonly long UnsetLengthLimit = -99;
  internal Stream _innerStream;
  private CRC32 _Crc32;
  private long _lengthLimit = -99;
  private bool _leaveOpen;

  public CrcCalculatorStream(Stream stream)
    : this(true, CrcCalculatorStream.UnsetLengthLimit, stream, (CRC32) null)
  {
  }

  public CrcCalculatorStream(Stream stream, bool leaveOpen)
    : this(leaveOpen, CrcCalculatorStream.UnsetLengthLimit, stream, (CRC32) null)
  {
  }

  public CrcCalculatorStream(Stream stream, long length)
    : this(true, length, stream, (CRC32) null)
  {
    if (length < 0L)
      throw new ArgumentException(nameof (length));
  }

  public CrcCalculatorStream(Stream stream, long length, bool leaveOpen)
    : this(leaveOpen, length, stream, (CRC32) null)
  {
    if (length < 0L)
      throw new ArgumentException(nameof (length));
  }

  public CrcCalculatorStream(Stream stream, long length, bool leaveOpen, CRC32 crc32)
    : this(leaveOpen, length, stream, crc32)
  {
    if (length < 0L)
      throw new ArgumentException(nameof (length));
  }

  private CrcCalculatorStream(bool leaveOpen, long length, Stream stream, CRC32 crc32)
  {
    this._innerStream = stream;
    this._Crc32 = crc32 ?? new CRC32();
    this._lengthLimit = length;
    this._leaveOpen = leaveOpen;
  }

  public long TotalBytesSlurped => this._Crc32.TotalBytesRead;

  public int Crc => this._Crc32.Crc32Result;

  public bool LeaveOpen
  {
    get => this._leaveOpen;
    set => this._leaveOpen = value;
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    int count1 = count;
    if (this._lengthLimit != CrcCalculatorStream.UnsetLengthLimit)
    {
      if (this._Crc32.TotalBytesRead >= this._lengthLimit)
        return 0;
      long num = this._lengthLimit - this._Crc32.TotalBytesRead;
      if (num < (long) count)
        count1 = (int) num;
    }
    int count2 = this._innerStream.Read(buffer, offset, count1);
    if (count2 > 0)
      this._Crc32.SlurpBlock(buffer, offset, count2);
    return count2;
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
    if (count > 0)
      this._Crc32.SlurpBlock(buffer, offset, count);
    this._innerStream.Write(buffer, offset, count);
  }

  public override bool CanRead => this._innerStream.CanRead;

  public override bool CanSeek => false;

  public override bool CanWrite => this._innerStream.CanWrite;

  public override void Flush() => this._innerStream.Flush();

  public override long Length
  {
    get
    {
      return this._lengthLimit == CrcCalculatorStream.UnsetLengthLimit ? this._innerStream.Length : this._lengthLimit;
    }
  }

  public override long Position
  {
    get => this._Crc32.TotalBytesRead;
    set => throw new NotSupportedException();
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      Stream innerStream = this._innerStream;
      if (innerStream != null)
      {
        try
        {
          if (!this._leaveOpen)
            innerStream.Dispose();
        }
        finally
        {
          this._innerStream = (Stream) null;
        }
      }
    }
    base.Dispose(disposing);
  }
}
