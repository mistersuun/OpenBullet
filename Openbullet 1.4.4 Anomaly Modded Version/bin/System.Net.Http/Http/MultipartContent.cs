// Decompiled with JetBrains decompiler
// Type: System.Net.Http.MultipartContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
{
  private const string CrLf = "\r\n";
  private static readonly int s_crlfLength = MultipartContent.GetEncodedLength("\r\n");
  private static readonly int s_dashDashLength = MultipartContent.GetEncodedLength("--");
  private static readonly int s_colonSpaceLength = MultipartContent.GetEncodedLength(": ");
  private static readonly int s_commaSpaceLength = MultipartContent.GetEncodedLength(", ");
  private readonly List<HttpContent> _nestedContent;
  private readonly string _boundary;

  public MultipartContent()
    : this("mixed", MultipartContent.GetDefaultBoundary())
  {
  }

  public MultipartContent(string subtype)
    : this(subtype, MultipartContent.GetDefaultBoundary())
  {
  }

  public MultipartContent(string subtype, string boundary)
  {
    if (string.IsNullOrWhiteSpace(subtype))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (subtype));
    MultipartContent.ValidateBoundary(boundary);
    this._boundary = boundary;
    string str = boundary;
    if (!str.StartsWith("\"", StringComparison.Ordinal))
      str = $"\"{str}\"";
    this.Headers.ContentType = new MediaTypeHeaderValue("multipart/" + subtype)
    {
      Parameters = {
        new NameValueHeaderValue(nameof (boundary), str)
      }
    };
    this._nestedContent = new List<HttpContent>();
  }

  private static void ValidateBoundary(string boundary)
  {
    if (string.IsNullOrWhiteSpace(boundary))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (boundary));
    if (boundary.Length > 70)
      throw new ArgumentOutOfRangeException(nameof (boundary), (object) boundary, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_field_too_long, (object) 70));
    if (boundary.EndsWith(" ", StringComparison.Ordinal))
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) boundary), nameof (boundary));
    foreach (char ch in boundary)
    {
      if (('0' > ch || ch > '9') && ('a' > ch || ch > 'z') && ('A' > ch || ch > 'Z') && "'()+_,-./:=? ".IndexOf(ch) < 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) boundary), nameof (boundary));
    }
  }

  private static string GetDefaultBoundary() => Guid.NewGuid().ToString();

  public virtual void Add(HttpContent content)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    this._nestedContent.Add(content);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      foreach (HttpContent httpContent in this._nestedContent)
        httpContent.Dispose();
      this._nestedContent.Clear();
    }
    base.Dispose(disposing);
  }

  public IEnumerator<HttpContent> GetEnumerator()
  {
    return (IEnumerator<HttpContent>) this._nestedContent.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._nestedContent.GetEnumerator();

  protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
  {
    try
    {
      await MultipartContent.EncodeStringToStreamAsync(stream, $"--{this._boundary}\r\n").ConfigureAwait(false);
      StringBuilder output = new StringBuilder();
      for (int contentIndex = 0; contentIndex < this._nestedContent.Count; ++contentIndex)
      {
        HttpContent content = this._nestedContent[contentIndex];
        await MultipartContent.EncodeStringToStreamAsync(stream, this.SerializeHeadersToString(output, contentIndex, content)).ConfigureAwait(false);
        await content.CopyToAsync(stream).ConfigureAwait(false);
        content = (HttpContent) null;
      }
      await MultipartContent.EncodeStringToStreamAsync(stream, $"\r\n--{this._boundary}--\r\n").ConfigureAwait(false);
      output = (StringBuilder) null;
    }
    catch (Exception ex)
    {
      if (NetEventSource.Log.IsEnabled())
        NetEventSource.Exception(NetEventSource.ComponentType.Http, (object) this, nameof (SerializeToStreamAsync), ex);
      throw;
    }
  }

  protected override async Task<Stream> CreateContentReadStreamAsync()
  {
    try
    {
      Stream[] streams = new Stream[2 + this._nestedContent.Count * 2];
      StringBuilder scratch = new StringBuilder();
      int streamIndex = 0;
      streams[streamIndex++] = MultipartContent.EncodeStringToNewStream($"--{this._boundary}\r\n");
      for (int contentIndex = 0; contentIndex < this._nestedContent.Count; ++contentIndex)
      {
        HttpContent content = this._nestedContent[contentIndex];
        streams[streamIndex++] = MultipartContent.EncodeStringToNewStream(this.SerializeHeadersToString(scratch, contentIndex, content));
        ConfiguredTaskAwaitable<Stream> configuredTaskAwaitable = content.ReadAsStreamAsync().ConfigureAwait(false);
        Stream readStream = await configuredTaskAwaitable ?? (Stream) new MemoryStream();
        if (!readStream.CanSeek)
        {
          configuredTaskAwaitable = base.CreateContentReadStreamAsync().ConfigureAwait(false);
          return await configuredTaskAwaitable;
        }
        streams[streamIndex++] = readStream;
        readStream = (Stream) null;
      }
      streams[streamIndex] = MultipartContent.EncodeStringToNewStream($"\r\n--{this._boundary}--\r\n");
      return (Stream) new MultipartContent.ContentReadStream(streams);
    }
    catch (Exception ex)
    {
      if (NetEventSource.Log.IsEnabled())
        NetEventSource.Exception(NetEventSource.ComponentType.Http, (object) this, nameof (CreateContentReadStreamAsync), ex);
      throw;
    }
  }

  private string SerializeHeadersToString(
    StringBuilder scratch,
    int contentIndex,
    HttpContent content)
  {
    scratch.Clear();
    if (contentIndex != 0)
    {
      scratch.Append("\r\n--");
      scratch.Append(this._boundary);
      scratch.Append("\r\n");
    }
    foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) content.Headers)
    {
      scratch.Append(header.Key);
      scratch.Append(": ");
      string str1 = string.Empty;
      foreach (string str2 in header.Value)
      {
        scratch.Append(str1);
        scratch.Append(str2);
        str1 = ", ";
      }
      scratch.Append("\r\n");
    }
    scratch.Append("\r\n");
    return scratch.ToString();
  }

  private static Task EncodeStringToStreamAsync(Stream stream, string input)
  {
    byte[] bytes = HttpRuleParser.DefaultHttpEncoding.GetBytes(input);
    return stream.WriteAsync(bytes, 0, bytes.Length);
  }

  private static Stream EncodeStringToNewStream(string input)
  {
    return (Stream) new MemoryStream(HttpRuleParser.DefaultHttpEncoding.GetBytes(input), false);
  }

  protected internal override bool TryComputeLength(out long length)
  {
    int encodedLength = MultipartContent.GetEncodedLength(this._boundary);
    long num1 = 0;
    long num2 = (long) (MultipartContent.s_crlfLength + MultipartContent.s_dashDashLength + encodedLength + MultipartContent.s_crlfLength);
    long num3 = num1 + (long) (MultipartContent.s_dashDashLength + encodedLength + MultipartContent.s_crlfLength);
    bool flag = true;
    foreach (HttpContent httpContent in this._nestedContent)
    {
      if (flag)
        flag = false;
      else
        num3 += num2;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) httpContent.Headers)
      {
        num3 += (long) (MultipartContent.GetEncodedLength(header.Key) + MultipartContent.s_colonSpaceLength);
        int num4 = 0;
        foreach (string input in header.Value)
        {
          num3 += (long) MultipartContent.GetEncodedLength(input);
          ++num4;
        }
        if (num4 > 1)
          num3 += (long) ((num4 - 1) * MultipartContent.s_commaSpaceLength);
        num3 += (long) MultipartContent.s_crlfLength;
      }
      num3 += (long) MultipartContent.s_crlfLength;
      long length1 = 0;
      if (!httpContent.TryComputeLength(out length1))
      {
        length = 0L;
        return false;
      }
      num3 += length1;
    }
    long num5 = num3 + (long) (MultipartContent.s_crlfLength + MultipartContent.s_dashDashLength + encodedLength + MultipartContent.s_dashDashLength + MultipartContent.s_crlfLength);
    length = num5;
    return true;
  }

  private static int GetEncodedLength(string input)
  {
    return HttpRuleParser.DefaultHttpEncoding.GetByteCount(input);
  }

  private sealed class ContentReadStream : Stream
  {
    private readonly Stream[] _streams;
    private readonly long _length;
    private int _next;
    private Stream _current;
    private long _position;

    internal ContentReadStream(Stream[] streams)
    {
      this._streams = streams;
      foreach (Stream stream in streams)
        this._length += stream.Length;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      foreach (Stream stream in this._streams)
        stream.Dispose();
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override int Read(byte[] buffer, int offset, int count)
    {
      MultipartContent.ContentReadStream.ValidateReadArgs(buffer, offset, count);
      if (count == 0)
        return 0;
      int num;
      while (true)
      {
        if (this._current != null)
        {
          num = this._current.Read(buffer, offset, count);
          if (num == 0)
            this._current = (Stream) null;
          else
            break;
        }
        if (this._next < this._streams.Length)
          this._current = this._streams[this._next++];
        else
          goto label_7;
      }
      this._position += (long) num;
      return num;
label_7:
      return 0;
    }

    public virtual Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      MultipartContent.ContentReadStream.ValidateReadArgs(buffer, offset, count);
      return this.ReadAsyncPrivate(buffer, offset, count, cancellationToken);
    }

    public async Task<int> ReadAsyncPrivate(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      if (count == 0)
        return 0;
      int num;
      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();
        if (this._current != null)
        {
          num = await this._current.ReadAsync(buffer, offset, count).ConfigureAwait(false);
          if (num == 0)
            this._current = (Stream) null;
          else
            break;
        }
        if (this._next < this._streams.Length)
          this._current = this._streams[this._next++];
        else
          goto label_7;
      }
      this._position += (long) num;
      return num;
label_7:
      return 0;
    }

    public override long Position
    {
      get => this._position;
      set
      {
        if (value < 0L)
          throw new ArgumentOutOfRangeException(nameof (value));
        long num = 0;
        for (int index1 = 0; index1 < this._streams.Length; ++index1)
        {
          Stream stream = this._streams[index1];
          long length = stream.Length;
          if (value < num + length)
          {
            this._current = stream;
            int index2 = index1 + 1;
            this._next = index2;
            stream.Position = value - num;
            for (; index2 < this._streams.Length; ++index2)
              this._streams[index2].Position = 0L;
            this._position = value;
            return;
          }
          num += length;
        }
        this._current = (Stream) null;
        this._next = this._streams.Length;
        this._position = value;
      }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          this.Position = offset;
          break;
        case SeekOrigin.Current:
          this.Position += offset;
          break;
        case SeekOrigin.End:
          this.Position = this._length + offset;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (origin));
      }
      return this.Position;
    }

    public override long Length => this._length;

    private static void ValidateReadArgs(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (offset > buffer.Length - count)
        throw new ArgumentException(SR.net_http_buffer_insufficient_length, nameof (buffer));
    }

    public override void Flush()
    {
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public virtual Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }
  }
}
