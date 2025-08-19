// Decompiled with JetBrains decompiler
// Type: System.Net.Http.StreamContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public class StreamContent : HttpContent
{
  private const int DefaultBufferSize = 4096 /*0x1000*/;
  private Stream _content;
  private int _bufferSize;
  private CancellationToken _cancellationToken;
  private bool _contentConsumed;
  private long _start;

  public StreamContent(Stream content)
    : this(content, 4096 /*0x1000*/)
  {
  }

  public StreamContent(Stream content, int bufferSize)
    : this(content, bufferSize, CancellationToken.None)
  {
  }

  internal StreamContent(Stream content, CancellationToken cancellationToken)
    : this(content, 4096 /*0x1000*/, cancellationToken)
  {
  }

  private StreamContent(Stream content, int bufferSize, CancellationToken cancellationToken)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (bufferSize <= 0)
      throw new ArgumentOutOfRangeException(nameof (bufferSize));
    this._content = content;
    this._bufferSize = bufferSize;
    this._cancellationToken = cancellationToken;
    if (content.CanSeek)
      this._start = content.Position;
    if (!HttpEventSource.Log.IsEnabled())
      return;
    HttpEventSource.Associate((object) this, (object) content);
  }

  protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
  {
    this.PrepareContent();
    return StreamToStreamCopy.CopyAsync(this._content, stream, this._bufferSize, !this._content.CanSeek, this._cancellationToken);
  }

  protected internal override bool TryComputeLength(out long length)
  {
    if (this._content.CanSeek)
    {
      length = this._content.Length - this._start;
      return true;
    }
    length = 0L;
    return false;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
      this._content.Dispose();
    base.Dispose(disposing);
  }

  protected override Task<Stream> CreateContentReadStreamAsync()
  {
    return Task.FromResult<Stream>((Stream) new StreamContent.ReadOnlyStream(this._content));
  }

  private void PrepareContent()
  {
    if (this._contentConsumed)
    {
      if (!this._content.CanSeek)
        throw new InvalidOperationException(SR.net_http_content_stream_already_read);
      this._content.Position = this._start;
    }
    this._contentConsumed = true;
  }

  private class ReadOnlyStream(Stream innerStream) : DelegatingStream(innerStream)
  {
    public override bool CanWrite => false;

    public override int WriteTimeout
    {
      get => throw new NotSupportedException(SR.net_http_content_readonly_stream);
      set => throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }

    public override void Flush()
    {
      throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
      throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }

    public override void WriteByte(byte value)
    {
      throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException(SR.net_http_content_readonly_stream);
    }
  }
}
