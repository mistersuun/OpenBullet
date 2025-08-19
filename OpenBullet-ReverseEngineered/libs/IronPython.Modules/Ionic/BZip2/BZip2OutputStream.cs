// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.BZip2OutputStream
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

#nullable disable
namespace Ionic.BZip2;

public class BZip2OutputStream : Stream
{
  private int totalBytesWrittenIn;
  private bool leaveOpen;
  private BZip2Compressor compressor;
  private uint combinedCRC;
  private Stream output;
  private BitWriter bw;
  private int blockSize100k;
  private BZip2OutputStream.TraceBits desiredTrace = BZip2OutputStream.TraceBits.Crc | BZip2OutputStream.TraceBits.Write;

  public BZip2OutputStream(Stream output)
    : this(output, Ionic.BZip2.BZip2.MaxBlockSize, false)
  {
  }

  public BZip2OutputStream(Stream output, int blockSize)
    : this(output, blockSize, false)
  {
  }

  public BZip2OutputStream(Stream output, bool leaveOpen)
    : this(output, Ionic.BZip2.BZip2.MaxBlockSize, leaveOpen)
  {
  }

  public BZip2OutputStream(Stream output, int blockSize, bool leaveOpen)
  {
    if (blockSize < Ionic.BZip2.BZip2.MinBlockSize || blockSize > Ionic.BZip2.BZip2.MaxBlockSize)
      throw new ArgumentException($"blockSize={blockSize} is out of range; must be between {Ionic.BZip2.BZip2.MinBlockSize} and {Ionic.BZip2.BZip2.MaxBlockSize}", nameof (blockSize));
    this.output = output;
    this.bw = this.output.CanWrite ? new BitWriter(this.output) : throw new ArgumentException("The stream is not writable.", nameof (output));
    this.blockSize100k = blockSize;
    this.compressor = new BZip2Compressor(this.bw, blockSize);
    this.leaveOpen = leaveOpen;
    this.combinedCRC = 0U;
    this.EmitHeader();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      Stream output = this.output;
      if (output != null)
      {
        this.Finish();
        if (!this.leaveOpen)
          output.Dispose();
      }
    }
    base.Dispose(disposing);
  }

  public override void Flush()
  {
    if (this.output == null)
      return;
    this.bw.Flush();
    this.output.Flush();
  }

  private void EmitHeader()
  {
    byte[] buffer = new byte[4]
    {
      (byte) 66,
      (byte) 90,
      (byte) 104,
      (byte) (48 /*0x30*/ + this.blockSize100k)
    };
    this.output.Write(buffer, 0, buffer.Length);
  }

  private void EmitTrailer()
  {
    this.bw.WriteByte((byte) 23);
    this.bw.WriteByte((byte) 114);
    this.bw.WriteByte((byte) 69);
    this.bw.WriteByte((byte) 56);
    this.bw.WriteByte((byte) 80 /*0x50*/);
    this.bw.WriteByte((byte) 144 /*0x90*/);
    this.bw.WriteInt(this.combinedCRC);
    this.bw.FinishAndPad();
  }

  private void Finish()
  {
    try
    {
      int totalBytesWrittenOut = this.bw.TotalBytesWrittenOut;
      this.compressor.CompressAndWrite();
      this.combinedCRC = this.combinedCRC << 1 | this.combinedCRC >> 31 /*0x1F*/;
      this.combinedCRC ^= this.compressor.Crc32;
      this.EmitTrailer();
    }
    finally
    {
      this.output = (Stream) null;
      this.compressor = (BZip2Compressor) null;
      this.bw = (BitWriter) null;
    }
  }

  public int BlockSize => this.blockSize100k;

  public override void Write(byte[] buffer, int offset, int count)
  {
    if (offset < 0)
      throw new IndexOutOfRangeException($"offset ({offset}) must be > 0");
    if (count < 0)
      throw new IndexOutOfRangeException($"count ({count}) must be > 0");
    if (offset + count > buffer.Length)
      throw new IndexOutOfRangeException($"offset({offset}) count({count}) bLength({buffer.Length})");
    if (this.output == null)
      throw new IOException("the stream is not open");
    if (count == 0)
      return;
    int num1 = 0;
    int count1 = count;
    do
    {
      int num2 = this.compressor.Fill(buffer, offset, count1);
      if (num2 != count1)
      {
        int totalBytesWrittenOut = this.bw.TotalBytesWrittenOut;
        this.compressor.CompressAndWrite();
        this.combinedCRC = this.combinedCRC << 1 | this.combinedCRC >> 31 /*0x1F*/;
        this.combinedCRC ^= this.compressor.Crc32;
        offset += num2;
      }
      count1 -= num2;
      num1 += num2;
    }
    while (count1 > 0);
    this.totalBytesWrittenIn += num1;
  }

  public override bool CanRead => false;

  public override bool CanSeek => false;

  public override bool CanWrite
  {
    get
    {
      return this.output != null ? this.output.CanWrite : throw new ObjectDisposedException("BZip2Stream");
    }
  }

  public override long Length => throw new NotImplementedException();

  public override long Position
  {
    get => (long) this.totalBytesWrittenIn;
    set => throw new NotImplementedException();
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

  public override void SetLength(long value) => throw new NotImplementedException();

  public override int Read(byte[] buffer, int offset, int count)
  {
    throw new NotImplementedException();
  }

  [Conditional("Trace")]
  private void TraceOutput(
    BZip2OutputStream.TraceBits bits,
    string format,
    params object[] varParams)
  {
    if ((bits & this.desiredTrace) == BZip2OutputStream.TraceBits.None)
      return;
    int hashCode = Thread.CurrentThread.GetHashCode();
    Console.ForegroundColor = (ConsoleColor) (hashCode % 8 + 10);
    Console.Write("{0:000} PBOS ", (object) hashCode);
    Console.WriteLine(format, varParams);
    Console.ResetColor();
  }

  [Flags]
  private enum TraceBits : uint
  {
    None = 0,
    Crc = 1,
    Write = 2,
    All = 4294967295, // 0xFFFFFFFF
  }
}
