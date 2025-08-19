// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ConsoleInputStream
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.IO;

#nullable disable
namespace Microsoft.Scripting.Utils;

public sealed class ConsoleInputStream : Stream
{
  public static readonly ConsoleInputStream Instance = new ConsoleInputStream();
  private const int MinimalBufferSize = 4096 /*0x1000*/;
  private readonly Stream _input;
  private readonly object _lock = new object();
  private readonly byte[] _buffer = new byte[4096 /*0x1000*/];
  private int _bufferPos;
  private int _bufferSize;

  private ConsoleInputStream() => this._input = Console.OpenStandardInput();

  public override bool CanRead => true;

  public override int Read(byte[] buffer, int offset, int count)
  {
    int count1;
    lock (this._lock)
    {
      if (this._bufferSize > 0)
      {
        count1 = Math.Min(count, this._bufferSize);
        Buffer.BlockCopy((Array) this._buffer, this._bufferPos, (Array) buffer, offset, count1);
        this._bufferPos += count1;
        this._bufferSize -= count1;
        offset += count1;
        count -= count1;
      }
      else
        count1 = 0;
      if (count > 0)
      {
        if (count < 4096 /*0x1000*/)
        {
          int val1 = this._input.Read(this._buffer, 0, 4096 /*0x1000*/);
          int count2 = Math.Min(val1, count);
          Buffer.BlockCopy((Array) this._buffer, 0, (Array) buffer, offset, count2);
          this._bufferSize = val1 - count2;
          this._bufferPos = count2;
          count1 += count2;
        }
        else
          count1 += this._input.Read(buffer, offset, count);
      }
    }
    return count1;
  }

  public override bool CanSeek => false;

  public override bool CanWrite => false;

  public override void Flush() => throw new NotSupportedException();

  public override long Length => throw new NotSupportedException();

  public override long Position
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    throw new NotSupportedException();
  }
}
