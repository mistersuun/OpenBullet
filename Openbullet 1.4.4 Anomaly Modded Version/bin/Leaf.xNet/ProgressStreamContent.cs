// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.ProgressStreamContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Leaf.xNet;

internal class ProgressStreamContent : System.Net.Http.StreamContent
{
  private long _totalBytes;
  private long _totalBytesExpected = -1;
  private ProgressDelegate _progress;

  public ProgressStreamContent(Stream stream, CancellationToken token)
    : this(new ProgressStreamContent.ProgressStream(stream, token))
  {
  }

  public ProgressStreamContent(Stream stream, int bufferSize)
    : this(new ProgressStreamContent.ProgressStream(stream, CancellationToken.None), bufferSize)
  {
  }

  private ProgressStreamContent(ProgressStreamContent.ProgressStream stream)
    : base((Stream) stream)
  {
    this.Init(stream);
  }

  private ProgressStreamContent(ProgressStreamContent.ProgressStream stream, int bufferSize)
    : base((Stream) stream, bufferSize)
  {
    this.Init(stream);
  }

  private void Init(ProgressStreamContent.ProgressStream stream)
  {
    stream.ReadCallback = new Action<long>(this.ReadBytes);
    this.Progress = (ProgressDelegate) ((_param1, _param2, _param3) => { });
  }

  private void Reset() => this._totalBytes = 0L;

  private void ReadBytes(long bytes)
  {
    if (this._totalBytesExpected == -1L)
      this._totalBytesExpected = this.Headers.ContentLength ?? -1L;
    long length;
    if (this._totalBytesExpected == -1L && this.TryComputeLength(out length))
      this._totalBytesExpected = length == 0L ? -1L : length;
    this._totalBytesExpected = Math.Max(-1L, this._totalBytesExpected);
    this._totalBytes += bytes;
    this.Progress(bytes, this._totalBytes, this._totalBytesExpected);
  }

  public ProgressDelegate Progress
  {
    get => this._progress;
    set => this._progress = value ?? (ProgressDelegate) ((_param1, _param2, _param3) => { });
  }

  protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
  {
    this.Reset();
    return base.SerializeToStreamAsync(stream, context);
  }

  protected override bool TryComputeLength(out long length)
  {
    int num = base.TryComputeLength(out length) ? 1 : 0;
    this._totalBytesExpected = length;
    return num != 0;
  }

  private class ProgressStream : Stream
  {
    private readonly CancellationToken _token;
    private bool _disposed;

    public ProgressStream(Stream stream, CancellationToken token)
    {
      this.ParentStream = stream;
      this._token = token;
      this.ReadCallback = (Action<long>) (_param1 => { });
      this.WriteCallback = (Action<long>) (_param1 => { });
    }

    public Action<long> ReadCallback { private get; set; }

    private Action<long> WriteCallback { get; }

    private Stream ParentStream { get; }

    public override bool CanRead => this.ParentStream.CanRead;

    public override bool CanSeek => this.ParentStream.CanSeek;

    public override bool CanWrite => this.ParentStream.CanWrite;

    public override bool CanTimeout => this.ParentStream.CanTimeout;

    public override long Length => this.ParentStream.Length;

    public override void Flush() => this.ParentStream.Flush();

    public virtual Task FlushAsync(CancellationToken cancellationToken)
    {
      return this.ParentStream.FlushAsync(cancellationToken);
    }

    public override long Position
    {
      get => this.ParentStream.Position;
      set => this.ParentStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this._token.ThrowIfCancellationRequested();
      int num = this.ParentStream.Read(buffer, offset, count);
      this.ReadCallback((long) num);
      return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this._token.ThrowIfCancellationRequested();
      return this.ParentStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this._token.ThrowIfCancellationRequested();
      this.ParentStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._token.ThrowIfCancellationRequested();
      this.ParentStream.Write(buffer, offset, count);
      this.WriteCallback((long) count);
    }

    public virtual async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this._token.ThrowIfCancellationRequested();
      CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._token, cancellationToken);
      int num = await this.ParentStream.ReadAsync(buffer, offset, count, linkedTokenSource.Token);
      this.ReadCallback((long) num);
      return num;
    }

    public virtual Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this._token.ThrowIfCancellationRequested();
      CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._token, cancellationToken);
      Task task = this.ParentStream.WriteAsync(buffer, offset, count, linkedTokenSource.Token);
      this.WriteCallback((long) count);
      return task;
    }

    protected override void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
        this.ParentStream?.Dispose();
      this._disposed = true;
    }
  }
}
