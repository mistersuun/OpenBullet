// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public abstract class HttpContent : IDisposable
{
  private HttpContentHeaders _headers;
  private MemoryStream _bufferedContent;
  private bool _disposed;
  private Stream _contentReadStream;
  private bool _canCalculateLength;
  internal const long MaxBufferSize = 2147483647 /*0x7FFFFFFF*/;
  internal static readonly Encoding DefaultStringEncoding = Encoding.UTF8;
  private const int UTF8CodePage = 65001;
  private const int UTF8PreambleLength = 3;
  private const byte UTF8PreambleByte0 = 239;
  private const byte UTF8PreambleByte1 = 187;
  private const byte UTF8PreambleByte2 = 191;
  private const int UTF8PreambleFirst2Bytes = 61371;
  private const int UTF32CodePage = 12000;
  private const int UTF32PreambleLength = 4;
  private const byte UTF32PreambleByte0 = 255 /*0xFF*/;
  private const byte UTF32PreambleByte1 = 254;
  private const byte UTF32PreambleByte2 = 0;
  private const byte UTF32PreambleByte3 = 0;
  private const int UTF32OrUnicodePreambleFirst2Bytes = 65534;
  private const int UnicodeCodePage = 1200;
  private const int UnicodePreambleLength = 2;
  private const byte UnicodePreambleByte0 = 255 /*0xFF*/;
  private const byte UnicodePreambleByte1 = 254;
  private const int BigEndianUnicodeCodePage = 1201;
  private const int BigEndianUnicodePreambleLength = 2;
  private const byte BigEndianUnicodePreambleByte0 = 254;
  private const byte BigEndianUnicodePreambleByte1 = 255 /*0xFF*/;
  private const int BigEndianUnicodePreambleFirst2Bytes = 65279;

  public HttpContentHeaders Headers
  {
    get
    {
      if (this._headers == null)
        this._headers = new HttpContentHeaders(new Func<long?>(this.GetComputedOrBufferLength));
      return this._headers;
    }
  }

  private bool IsBuffered => this._bufferedContent != null;

  internal void SetBuffer(byte[] buffer, int offset, int count)
  {
    this._bufferedContent = new MemoryStream(buffer, offset, count, false, true);
  }

  internal bool TryGetBuffer(out ArraySegment<byte> buffer)
  {
    buffer = new ArraySegment<byte>();
    return this._bufferedContent != null && this._bufferedContent.TryGetBuffer(ref buffer);
  }

  protected HttpContent()
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
    this._canCalculateLength = true;
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  public Task<string> ReadAsStringAsync()
  {
    this.CheckDisposed();
    return HttpContent.WaitAndReturnAsync<HttpContent, string>(this.LoadIntoBufferAsync(), this, (Func<HttpContent, string>) (s => s.ReadBufferedContentAsString()));
  }

  internal string ReadBufferedContentAsString()
  {
    if (this._bufferedContent.Length == 0L)
      return string.Empty;
    Encoding encoding = (Encoding) null;
    int preambleLength = -1;
    ArraySegment<byte> buffer;
    if (!this.TryGetBuffer(out buffer))
      buffer = new ArraySegment<byte>(this._bufferedContent.ToArray());
    if (this.Headers.ContentType != null)
    {
      if (this.Headers.ContentType.CharSet != null)
      {
        try
        {
          encoding = Encoding.GetEncoding(this.Headers.ContentType.CharSet);
          preambleLength = HttpContent.GetPreambleLength(buffer, encoding);
        }
        catch (ArgumentException ex)
        {
          throw new InvalidOperationException(SR.net_http_content_invalid_charset, (Exception) ex);
        }
      }
    }
    if (encoding == null && !HttpContent.TryDetectEncoding(buffer, out encoding, out preambleLength))
    {
      encoding = HttpContent.DefaultStringEncoding;
      preambleLength = 0;
    }
    return encoding.GetString(buffer.Array, buffer.Offset + preambleLength, buffer.Count - preambleLength);
  }

  public Task<byte[]> ReadAsByteArrayAsync()
  {
    this.CheckDisposed();
    return HttpContent.WaitAndReturnAsync<HttpContent, byte[]>(this.LoadIntoBufferAsync(), this, (Func<HttpContent, byte[]>) (s => s.ReadBufferedContentAsByteArray()));
  }

  internal byte[] ReadBufferedContentAsByteArray() => this._bufferedContent.ToArray();

  public Task<Stream> ReadAsStreamAsync()
  {
    this.CheckDisposed();
    ArraySegment<byte> buffer;
    if (this._contentReadStream == null && this.TryGetBuffer(out buffer))
      this._contentReadStream = (Stream) new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
    return this._contentReadStream == null ? this.ReadAsStreamAsyncCore(this.CreateContentReadStreamAsync()) : Task.FromResult<Stream>(this._contentReadStream);
  }

  private async Task<Stream> ReadAsStreamAsyncCore(Task<Stream> createContentStreamTask)
  {
    HttpContent httpContent = this;
    Stream contentReadStream = httpContent._contentReadStream;
    Stream stream = await createContentStreamTask.ConfigureAwait(false);
    httpContent._contentReadStream = stream;
    httpContent = (HttpContent) null;
    return this._contentReadStream;
  }

  protected abstract Task SerializeToStreamAsync(Stream stream, TransportContext context);

  public Task CopyToAsync(Stream stream, TransportContext context)
  {
    this.CheckDisposed();
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    try
    {
      ArraySegment<byte> buffer;
      Task task;
      if (this.TryGetBuffer(out buffer))
      {
        task = stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
      }
      else
      {
        task = this.SerializeToStreamAsync(stream, context);
        this.CheckTaskNotNull(task);
      }
      return HttpContent.CopyToAsyncCore(task);
    }
    catch (Exception ex) when (HttpContent.StreamCopyExceptionNeedsWrapping(ex))
    {
      return Task.FromException(HttpContent.GetStreamCopyException(ex));
    }
  }

  private static async Task CopyToAsyncCore(Task copyTask)
  {
    try
    {
      await copyTask.ConfigureAwait(false);
    }
    catch (Exception ex) when (HttpContent.StreamCopyExceptionNeedsWrapping(ex))
    {
      throw HttpContent.GetStreamCopyException(ex);
    }
  }

  public Task CopyToAsync(Stream stream) => this.CopyToAsync(stream, (TransportContext) null);

  internal void CopyTo(Stream stream) => this.CopyToAsync(stream).Wait();

  public Task LoadIntoBufferAsync() => this.LoadIntoBufferAsync((long) int.MaxValue);

  public Task LoadIntoBufferAsync(long maxBufferSize)
  {
    this.CheckDisposed();
    if (maxBufferSize > (long) int.MaxValue)
      throw new ArgumentOutOfRangeException(nameof (maxBufferSize), (object) maxBufferSize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, (object) (long) int.MaxValue));
    if (this.IsBuffered)
      return Task.CompletedTask;
    Exception error = (Exception) null;
    MemoryStream memoryStream = this.CreateMemoryStream(maxBufferSize, out error);
    if (memoryStream == null)
      return Task.FromException(error);
    try
    {
      Task streamAsync = this.SerializeToStreamAsync((Stream) memoryStream, (TransportContext) null);
      this.CheckTaskNotNull(streamAsync);
      return this.LoadIntoBufferAsyncCore(streamAsync, memoryStream);
    }
    catch (Exception ex) when (HttpContent.StreamCopyExceptionNeedsWrapping(ex))
    {
      return Task.FromException(HttpContent.GetStreamCopyException(ex));
    }
  }

  private async Task LoadIntoBufferAsyncCore(Task serializeToStreamTask, MemoryStream tempBuffer)
  {
    try
    {
      await serializeToStreamTask.ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      tempBuffer.Dispose();
      Exception streamCopyException = HttpContent.GetStreamCopyException(ex);
      if (streamCopyException != ex)
        throw streamCopyException;
      throw;
    }
    try
    {
      tempBuffer.Seek(0L, SeekOrigin.Begin);
      this._bufferedContent = tempBuffer;
    }
    catch (Exception ex)
    {
      if (NetEventSource.Log.IsEnabled())
        NetEventSource.Exception(NetEventSource.ComponentType.Http, (object) this, "LoadIntoBufferAsync", ex);
      throw;
    }
  }

  protected virtual Task<Stream> CreateContentReadStreamAsync()
  {
    return HttpContent.WaitAndReturnAsync<HttpContent, Stream>(this.LoadIntoBufferAsync(), this, (Func<HttpContent, Stream>) (s => (Stream) s._bufferedContent));
  }

  protected internal abstract bool TryComputeLength(out long length);

  private long? GetComputedOrBufferLength()
  {
    this.CheckDisposed();
    if (this.IsBuffered)
      return new long?(this._bufferedContent.Length);
    if (this._canCalculateLength)
    {
      long length = 0;
      if (this.TryComputeLength(out length))
        return new long?(length);
      this._canCalculateLength = false;
    }
    return new long?();
  }

  private MemoryStream CreateMemoryStream(long maxBufferSize, out Exception error)
  {
    error = (Exception) null;
    long? contentLength = this.Headers.ContentLength;
    if (!contentLength.HasValue)
      return (MemoryStream) new HttpContent.LimitMemoryStream((int) maxBufferSize, 0);
    long? nullable = contentLength;
    long num = maxBufferSize;
    if ((nullable.GetValueOrDefault() > num ? (nullable.HasValue ? 1 : 0) : 0) == 0)
      return (MemoryStream) new HttpContent.LimitMemoryStream((int) maxBufferSize, (int) contentLength.Value);
    error = (Exception) new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, (object) maxBufferSize));
    return (MemoryStream) null;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing || this._disposed)
      return;
    this._disposed = true;
    if (this._contentReadStream != null)
      this._contentReadStream.Dispose();
    if (!this.IsBuffered)
      return;
    this._bufferedContent.Dispose();
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void CheckDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(this.GetType().ToString());
  }

  private void CheckTaskNotNull(Task task)
  {
    if (task == null)
    {
      if (NetEventSource.Log.IsEnabled())
        NetEventSource.PrintError(NetEventSource.ComponentType.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_log_content_no_task_returned_copytoasync, (object) this.GetType().ToString()));
      throw new InvalidOperationException(SR.net_http_content_no_task_returned);
    }
  }

  private static bool StreamCopyExceptionNeedsWrapping(Exception e)
  {
    return e is IOException || e is ObjectDisposedException;
  }

  private static Exception GetStreamCopyException(Exception originalException)
  {
    return !HttpContent.StreamCopyExceptionNeedsWrapping(originalException) ? originalException : (Exception) new HttpRequestException(SR.net_http_content_stream_copy_error, originalException);
  }

  private static int GetPreambleLength(ArraySegment<byte> buffer, Encoding encoding)
  {
    byte[] array = buffer.Array;
    int offset = buffer.Offset;
    int count = buffer.Count;
    switch (encoding.CodePage)
    {
      case 1200:
        return count < 2 || array[offset] != byte.MaxValue || array[offset + 1] != (byte) 254 ? 0 : 2;
      case 1201:
        return count < 2 || array[offset] != (byte) 254 || array[offset + 1] != byte.MaxValue ? 0 : 2;
      case 12000:
        return count < 4 || array[offset] != byte.MaxValue || array[offset + 1] != (byte) 254 || array[offset + 2] != (byte) 0 || array[offset + 3] != (byte) 0 ? 0 : 4;
      case 65001:
        return count < 3 || array[offset] != (byte) 239 || array[offset + 1] != (byte) 187 || array[offset + 2] != (byte) 191 ? 0 : 3;
      default:
        byte[] preamble = encoding.GetPreamble();
        return !HttpContent.BufferHasPrefix(buffer, preamble) ? 0 : preamble.Length;
    }
  }

  private static bool TryDetectEncoding(
    ArraySegment<byte> buffer,
    out Encoding encoding,
    out int preambleLength)
  {
    byte[] array = buffer.Array;
    int offset = buffer.Offset;
    int count = buffer.Count;
    if (count >= 2)
    {
      switch ((int) array[offset] << 8 | (int) array[offset + 1])
      {
        case 61371:
          if (count >= 3 && array[offset + 2] == (byte) 191)
          {
            encoding = Encoding.UTF8;
            preambleLength = 3;
            return true;
          }
          break;
        case 65279:
          encoding = Encoding.BigEndianUnicode;
          preambleLength = 2;
          return true;
        case 65534:
          if (count >= 4 && array[offset + 2] == (byte) 0 && array[offset + 3] == (byte) 0)
          {
            encoding = Encoding.UTF32;
            preambleLength = 4;
          }
          else
          {
            encoding = Encoding.Unicode;
            preambleLength = 2;
          }
          return true;
      }
    }
    encoding = (Encoding) null;
    preambleLength = 0;
    return false;
  }

  private static bool BufferHasPrefix(ArraySegment<byte> buffer, byte[] prefix)
  {
    byte[] array = buffer.Array;
    if (prefix == null || array == null || prefix.Length > buffer.Count || prefix.Length == 0)
      return false;
    int index = 0;
    int offset = buffer.Offset;
    while (index < prefix.Length)
    {
      if ((int) prefix[index] != (int) array[offset])
        return false;
      ++index;
      ++offset;
    }
    return true;
  }

  private static async Task<TResult> WaitAndReturnAsync<TState, TResult>(
    Task waitTask,
    TState state,
    Func<TState, TResult> returnFunc)
  {
    await waitTask.ConfigureAwait(false);
    return returnFunc(state);
  }

  internal sealed class LimitMemoryStream : MemoryStream
  {
    private readonly int _maxSize;

    public LimitMemoryStream(int maxSize, int capacity)
      : base(capacity)
    {
      this._maxSize = maxSize;
    }

    public int MaxSize => this._maxSize;

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.CheckSize(count);
      base.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
      this.CheckSize(1);
      base.WriteByte(value);
    }

    public virtual Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.CheckSize(count);
      return base.WriteAsync(buffer, offset, count, cancellationToken);
    }

    private void CheckSize(int countToAdd)
    {
      if ((long) this._maxSize - this.Length < (long) countToAdd)
        throw new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, (object) this._maxSize));
    }
  }
}
