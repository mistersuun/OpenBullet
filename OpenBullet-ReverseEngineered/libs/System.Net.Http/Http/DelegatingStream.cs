// Decompiled with JetBrains decompiler
// Type: System.Net.Http.DelegatingStream
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

internal abstract class DelegatingStream : Stream
{
  private Stream _innerStream;

  public override bool CanRead => this._innerStream.CanRead;

  public override bool CanSeek => this._innerStream.CanSeek;

  public override bool CanWrite => this._innerStream.CanWrite;

  public override long Length => this._innerStream.Length;

  public override long Position
  {
    get => this._innerStream.Position;
    set => this._innerStream.Position = value;
  }

  public override int ReadTimeout
  {
    get => this._innerStream.ReadTimeout;
    set => this._innerStream.ReadTimeout = value;
  }

  public override bool CanTimeout => this._innerStream.CanTimeout;

  public override int WriteTimeout
  {
    get => this._innerStream.WriteTimeout;
    set => this._innerStream.WriteTimeout = value;
  }

  protected DelegatingStream(Stream innerStream) => this._innerStream = innerStream;

  protected override void Dispose(bool disposing)
  {
    if (disposing)
      this._innerStream.Dispose();
    base.Dispose(disposing);
  }

  public override long Seek(long offset, SeekOrigin origin)
  {
    return this._innerStream.Seek(offset, origin);
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    return this._innerStream.Read(buffer, offset, count);
  }

  public override int ReadByte() => this._innerStream.ReadByte();

  public virtual Task<int> ReadAsync(
    byte[] buffer,
    int offset,
    int count,
    CancellationToken cancellationToken)
  {
    return this._innerStream.ReadAsync(buffer, offset, count, cancellationToken);
  }

  public override void Flush() => this._innerStream.Flush();

  public virtual Task FlushAsync(CancellationToken cancellationToken)
  {
    return this._innerStream.FlushAsync(cancellationToken);
  }

  public override void SetLength(long value) => this._innerStream.SetLength(value);

  public override void Write(byte[] buffer, int offset, int count)
  {
    this._innerStream.Write(buffer, offset, count);
  }

  public override void WriteByte(byte value) => this._innerStream.WriteByte(value);

  public virtual Task WriteAsync(
    byte[] buffer,
    int offset,
    int count,
    CancellationToken cancellationToken)
  {
    return this._innerStream.WriteAsync(buffer, offset, count, cancellationToken);
  }

  public virtual Task CopyToAsync(
    Stream destination,
    int bufferSize,
    CancellationToken cancellationToken)
  {
    return this._innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
  }
}
