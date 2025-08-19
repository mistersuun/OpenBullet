// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.HttpResponse
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace Leaf.xNet;

public sealed class HttpResponse
{
  private static readonly byte[] OpenHtmlSignature = Encoding.ASCII.GetBytes("<html");
  private static readonly byte[] CloseHtmlSignature = Encoding.ASCII.GetBytes("</html>");
  private static readonly Regex KeepAliveTimeoutRegex = new Regex("timeout(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex KeepAliveMaxRegex = new Regex("max(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex ContentCharsetRegex = new Regex("charset(|\\s+)=(|\\s+)(?<value>[a-z,0-9,-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private readonly HttpRequest _request;
  private HttpResponse.ReceiverHelper _receiverHelper;
  private readonly Dictionary<string, string> _headers = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private Dictionary<string, string> _middleHeaders;
  private string _loadedMessageBody;

  public Dictionary<string, string> MiddleHeaders
  {
    get
    {
      return this._middleHeaders ?? (this._middleHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
    }
  }

  public bool HasError { get; private set; }

  public bool MessageBodyLoaded { get; private set; }

  public bool IsOK => this.StatusCode == HttpStatusCode.OK;

  public bool HasRedirect
  {
    get
    {
      int statusCode = (int) this.StatusCode;
      return statusCode >= 300 && statusCode < 400 || this._headers.ContainsKey("Location") || this._headers.ContainsKey("Redirect-Location");
    }
  }

  public bool HasExternalRedirect
  {
    get
    {
      return this.HasRedirect && this.RedirectAddress != (Uri) null && !this.RedirectAddress.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase) && !this.RedirectAddress.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase);
    }
  }

  public int ReconnectCount { get; internal set; }

  public Uri Address { get; private set; }

  public HttpMethod Method { get; private set; }

  public Version ProtocolVersion { get; private set; }

  public HttpStatusCode StatusCode { get; private set; }

  public Uri RedirectAddress { get; private set; }

  public Encoding CharacterSet { get; private set; }

  public long ContentLength { get; private set; }

  public string ContentType { get; private set; }

  public string Location => this[nameof (Location)];

  public CookieStorage Cookies { get; private set; }

  public int? KeepAliveTimeout { get; private set; }

  public int? MaximumKeepAliveRequests { get; private set; }

  public string this[string headerName]
  {
    get
    {
      switch (headerName)
      {
        case null:
          throw new ArgumentNullException(nameof (headerName));
        case "":
          throw ExceptionHelper.EmptyString(nameof (headerName));
        default:
          string empty;
          if (!this._headers.TryGetValue(headerName, out empty))
            empty = string.Empty;
          return empty;
      }
    }
  }

  public string this[HttpHeader header] => this[Http.Headers[header]];

  internal HttpResponse(HttpRequest request)
  {
    this._request = request;
    this.ContentLength = -1L;
    this.ContentType = string.Empty;
  }

  public byte[] ToBytes()
  {
    if (this.HasError)
      throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
    if (this.MessageBodyLoaded)
      return new byte[0];
    using (MemoryStream memoryStream = new MemoryStream())
    {
      memoryStream.SetLength(this.ContentLength == -1L ? 0L : this.ContentLength);
      try
      {
        foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
          memoryStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
      }
      catch (Exception ex)
      {
        this.HasError = true;
        switch (ex)
        {
          case IOException _:
          case InvalidOperationException _:
            throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
          default:
            throw;
        }
      }
      if (this.ConnectionClosed())
        this._request?.Dispose();
      this.MessageBodyLoaded = true;
      return memoryStream.ToArray();
    }
  }

  public override string ToString()
  {
    if (this.HasError)
      return string.Empty;
    if (this.MessageBodyLoaded)
      return this._loadedMessageBody;
    MemoryStream memoryStream = new MemoryStream();
    memoryStream.SetLength(this.ContentLength == -1L ? 0L : this.ContentLength);
    try
    {
      foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
        memoryStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
    }
    catch (Exception ex)
    {
      this.HasError = true;
      switch (ex)
      {
        case IOException _:
        case InvalidOperationException _:
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        default:
          throw;
      }
    }
    if (this.ConnectionClosed())
      this._request.Dispose();
    this.MessageBodyLoaded = true;
    this._loadedMessageBody = this.CharacterSet.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
    memoryStream.Dispose();
    return this._loadedMessageBody;
  }

  public void ToFile(string path)
  {
    if (this.HasError)
      throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
    if (path == null)
      throw new ArgumentNullException(nameof (path));
    if (this.MessageBodyLoaded)
      return;
    try
    {
      using (FileStream fileStream = new FileStream(path, FileMode.Create))
      {
        foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
          fileStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
      }
    }
    catch (ArgumentException ex)
    {
      throw ExceptionHelper.WrongPath(nameof (path), (Exception) ex);
    }
    catch (NotSupportedException ex)
    {
      throw ExceptionHelper.WrongPath(nameof (path), (Exception) ex);
    }
    catch (Exception ex)
    {
      this.HasError = true;
      switch (ex)
      {
        case IOException _:
        case InvalidOperationException _:
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        default:
          throw;
      }
    }
    if (this.ConnectionClosed())
      this._request.Dispose();
    this.MessageBodyLoaded = true;
  }

  public MemoryStream ToMemoryStream()
  {
    if (this.HasError)
      throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
    if (this.MessageBodyLoaded)
      return (MemoryStream) null;
    MemoryStream memoryStream = new MemoryStream();
    memoryStream.SetLength(this.ContentLength == -1L ? 0L : this.ContentLength);
    try
    {
      foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
        memoryStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
    }
    catch (Exception ex)
    {
      this.HasError = true;
      switch (ex)
      {
        case IOException _:
        case InvalidOperationException _:
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        default:
          throw;
      }
    }
    if (this.ConnectionClosed())
      this._request.Dispose();
    this.MessageBodyLoaded = true;
    memoryStream.Position = 0L;
    return memoryStream;
  }

  public void None()
  {
    if (this.HasError)
      throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
    if (this.MessageBodyLoaded)
      return;
    if (this.ConnectionClosed())
    {
      this._request.Dispose();
    }
    else
    {
      try
      {
        foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
          ;
      }
      catch (Exception ex)
      {
        this.HasError = true;
        switch (ex)
        {
          case IOException _:
          case InvalidOperationException _:
            throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
          default:
            throw;
        }
      }
    }
    this.MessageBodyLoaded = true;
  }

  public bool ContainsCookie(string url, string name)
  {
    return this.Cookies != null && this.Cookies.Contains(url, name);
  }

  public bool ContainsCookie(Uri uri, string name)
  {
    return this.Cookies != null && this.Cookies.Contains(uri, name);
  }

  public bool ContainsCookie(string name)
  {
    return this.Cookies != null && this.Cookies.Contains(!this.HasRedirect || this.HasExternalRedirect ? this.Address : this.RedirectAddress, name);
  }

  public bool ContainsHeader(string headerName)
  {
    switch (headerName)
    {
      case null:
        throw new ArgumentNullException(nameof (headerName));
      case "":
        throw ExceptionHelper.EmptyString(nameof (headerName));
      default:
        return this._headers.ContainsKey(headerName);
    }
  }

  public bool ContainsHeader(HttpHeader header) => this.ContainsHeader(Http.Headers[header]);

  public Dictionary<string, string>.Enumerator EnumerateHeaders() => this._headers.GetEnumerator();

  internal long LoadResponse(HttpMethod method, bool trackMiddleHeaders)
  {
    this.Method = method;
    this.Address = this._request.Address;
    this.HasError = false;
    this.MessageBodyLoaded = false;
    this.KeepAliveTimeout = new int?();
    this.MaximumKeepAliveRequests = new int?();
    if (trackMiddleHeaders && this._headers.Count > 0)
    {
      foreach (string key in this._headers.Keys)
        this.MiddleHeaders[key] = this._headers[key];
    }
    this._headers.Clear();
    if (this._request.UseCookies)
      this.Cookies = this._request.Cookies == null || this._request.Cookies.IsLocked ? new CookieStorage() : this._request.Cookies;
    if (this._receiverHelper == null)
      this._receiverHelper = new HttpResponse.ReceiverHelper(this._request.TcpClient.ReceiveBufferSize);
    this._receiverHelper.Init(this._request.ClientStream);
    try
    {
      this.ReceiveStartingLine();
      this.ReceiveHeaders();
      this.RedirectAddress = this.GetLocation();
      this.CharacterSet = this.GetCharacterSet();
      this.ContentLength = this.GetContentLength();
      this.ContentType = this.GetContentType();
      this.KeepAliveTimeout = this.GetKeepAliveTimeout();
      this.MaximumKeepAliveRequests = this.GetKeepAliveMax();
    }
    catch (Exception ex)
    {
      this.HasError = true;
      if (ex is IOException)
        throw this.NewHttpException(Resources.HttpException_FailedReceiveResponse, ex);
      throw;
    }
    if (this.ContentLength == 0L || this.Method == HttpMethod.HEAD || this.StatusCode == HttpStatusCode.Continue || this.StatusCode == HttpStatusCode.NoContent || this.StatusCode == HttpStatusCode.NotModified)
    {
      this._loadedMessageBody = string.Empty;
      this.MessageBodyLoaded = true;
    }
    long position = (long) this._receiverHelper.Position;
    if (this.ContentLength > 0L)
      position += this.ContentLength;
    return position;
  }

  private void ReceiveStartingLine()
  {
    string self;
    do
    {
      self = this._receiverHelper.ReadLine();
      if (self.Length == 0)
      {
        HttpException httpException = this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse);
        httpException.EmptyMessageBody = true;
        throw httpException;
      }
    }
    while (!(self != "\r\n"));
    string input = self.Substring("HTTP/", " ");
    string str = self.Substring(" ", " ");
    if (string.IsNullOrEmpty(str))
      str = self.Substring(" ", "\r\n");
    this.ProtocolVersion = !string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(str) ? Version.Parse(input) : throw this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse);
    this.StatusCode = (HttpStatusCode) Enum.Parse(typeof (HttpStatusCode), str);
  }

  private void ReceiveHeaders()
  {
    string str;
    while (true)
    {
      str = this._receiverHelper.ReadLine();
      if (!(str == "\r\n"))
      {
        int length = str.IndexOf(':');
        if (length != -1)
        {
          string key = str.Substring(0, length);
          string headerValue = str.Substring(length + 1).Trim(' ', '\t', '\r', '\n');
          if (key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
            this.ParseCookieFromHeader(headerValue);
          else
            this._headers[key] = headerValue;
        }
        else
          goto label_2;
      }
      else
        break;
    }
    return;
label_2:
    throw this.NewHttpException(string.Format(Resources.HttpException_WrongHeader, (object) str, (object) this.Address.Host));
  }

  private void ParseCookieFromHeader(string headerValue)
  {
    if (!this._request.UseCookies)
      return;
    this.Cookies.Set(this._request.Address, headerValue);
  }

  private IEnumerable<HttpResponse.BytesWrapper> GetMessageBodySource()
  {
    return this._headers.ContainsKey("Content-Encoding") && !string.Equals(this._headers["Content-Encoding"], "utf-8", StringComparison.OrdinalIgnoreCase) ? this.GetMessageBodySourceZip() : this.GetMessageBodySourceStd();
  }

  private IEnumerable<HttpResponse.BytesWrapper> GetMessageBodySourceStd()
  {
    if (this._headers.ContainsKey("Transfer-Encoding"))
      return this.ReceiveMessageBodyChunked();
    return this.ContentLength == -1L ? this.ReceiveMessageBody(this._request.ClientStream) : this.ReceiveMessageBody(this.ContentLength);
  }

  private IEnumerable<HttpResponse.BytesWrapper> GetMessageBodySourceZip()
  {
    if (this._headers.ContainsKey("Transfer-Encoding"))
      return this.ReceiveMessageBodyChunkedZip();
    return this.ContentLength != -1L ? this.ReceiveMessageBodyZip(this.ContentLength) : this.ReceiveMessageBody(this.GetZipStream((Stream) new HttpResponse.ZipWrapperStream(this._request.ClientStream, this._receiverHelper)));
  }

  private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBody(Stream stream)
  {
    HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
    int bufferSize = this._request.TcpClient.ReceiveBufferSize;
    byte[] buffer = new byte[bufferSize];
    bytesWrapper.Value = buffer;
    int begBytesRead = 0;
    if (stream is GZipStream || stream is DeflateStream)
    {
      begBytesRead = stream.Read(buffer, 0, bufferSize);
    }
    else
    {
      if (this._receiverHelper.HasData)
        begBytesRead = this._receiverHelper.Read(buffer, 0, bufferSize);
      if (begBytesRead < bufferSize)
        begBytesRead += stream.Read(buffer, begBytesRead, bufferSize - begBytesRead);
    }
    bytesWrapper.Length = begBytesRead;
    yield return bytesWrapper;
    bool isHtml = HttpResponse.FindSignature(buffer, begBytesRead, HttpResponse.OpenHtmlSignature);
    if (!isHtml || !HttpResponse.FindSignature(buffer, begBytesRead, HttpResponse.CloseHtmlSignature))
    {
      int sourceLength;
      while (true)
      {
        sourceLength = stream.Read(buffer, 0, bufferSize);
        if (isHtml)
        {
          if (sourceLength == 0)
          {
            this.WaitData();
            continue;
          }
          if (HttpResponse.FindSignature(buffer, sourceLength, HttpResponse.CloseHtmlSignature))
            break;
        }
        else if (sourceLength == 0)
          goto label_8;
        bytesWrapper.Length = sourceLength;
        yield return bytesWrapper;
      }
      bytesWrapper.Length = sourceLength;
      yield return bytesWrapper;
      yield break;
label_8:;
    }
  }

  private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBody(long contentLength)
  {
    Stream stream = this._request.ClientStream;
    HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
    int bufferSize = this._request.TcpClient.ReceiveBufferSize;
    byte[] buffer = new byte[bufferSize];
    bytesWrapper.Value = buffer;
    int totalBytesRead = 0;
    while ((long) totalBytesRead != contentLength)
    {
      int num = this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, 0, bufferSize) : stream.Read(buffer, 0, bufferSize);
      if (num == 0)
      {
        this.WaitData();
      }
      else
      {
        totalBytesRead += num;
        bytesWrapper.Length = num;
        yield return bytesWrapper;
      }
    }
  }

  private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBodyChunked()
  {
    Stream stream = this._request.ClientStream;
    HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
    int bufferSize = this._request.TcpClient.ReceiveBufferSize;
    byte[] buffer = new byte[bufferSize];
    bytesWrapper.Value = buffer;
label_1:
    string str1;
    do
    {
      str1 = this._receiverHelper.ReadLine();
    }
    while (str1 == "\r\n");
    string str2 = str1.Trim(' ', '\r', '\n');
    if (!(str2 == string.Empty))
    {
      int totalBytesRead = 0;
      int blockLength;
      try
      {
        blockLength = Convert.ToInt32(str2, 16 /*0x10*/);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case FormatException _:
          case OverflowException _:
            throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, (object) str2), ex);
          default:
            throw;
        }
      }
      if (blockLength != 0)
      {
        while (totalBytesRead != blockLength)
        {
          int num1 = blockLength - totalBytesRead;
          if (num1 > bufferSize)
            num1 = bufferSize;
          int num2 = this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, 0, num1) : stream.Read(buffer, 0, num1);
          if (num2 == 0)
          {
            this.WaitData();
          }
          else
          {
            totalBytesRead += num2;
            bytesWrapper.Length = num2;
            yield return bytesWrapper;
          }
        }
        goto label_1;
      }
    }
  }

  private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBodyZip(long contentLength)
  {
    HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
    HttpResponse.ZipWrapperStream streamWrapper = new HttpResponse.ZipWrapperStream(this._request.ClientStream, this._receiverHelper);
    using (Stream stream = this.GetZipStream((Stream) streamWrapper))
    {
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWrapper.Value = buffer;
      while (true)
      {
        int num = stream.Read(buffer, 0, bufferSize);
        if (num == 0)
        {
          if ((long) streamWrapper.TotalBytesRead != contentLength)
            this.WaitData();
          else
            break;
        }
        else
        {
          bytesWrapper.Length = num;
          yield return bytesWrapper;
        }
      }
    }
  }

  private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBodyChunkedZip()
  {
    HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
    HttpResponse.ZipWrapperStream streamWrapper = new HttpResponse.ZipWrapperStream(this._request.ClientStream, this._receiverHelper);
    bool messageBodyChunkedZip;
    using (Stream stream = this.GetZipStream((Stream) streamWrapper))
    {
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWrapper.Value = buffer;
label_1:
      string str1;
      do
      {
        str1 = this._receiverHelper.ReadLine();
      }
      while (str1 == "\r\n");
      string str2 = str1.Trim(' ', '\r', '\n');
      if (str2 == string.Empty)
      {
        messageBodyChunkedZip = false;
      }
      else
      {
        int blockLength;
        try
        {
          blockLength = Convert.ToInt32(str2, 16 /*0x10*/);
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case FormatException _:
            case OverflowException _:
              throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, (object) str2), ex);
            default:
              throw;
          }
        }
        if (blockLength == 0)
        {
          messageBodyChunkedZip = false;
        }
        else
        {
          streamWrapper.TotalBytesRead = 0;
          streamWrapper.LimitBytesRead = blockLength;
          while (true)
          {
            int num = stream.Read(buffer, 0, bufferSize);
            if (num == 0)
            {
              if (streamWrapper.TotalBytesRead != blockLength)
                this.WaitData();
              else
                goto label_1;
            }
            else
            {
              bytesWrapper.Length = num;
              yield return bytesWrapper;
            }
          }
        }
      }
    }
    return messageBodyChunkedZip;
  }

  private bool ConnectionClosed()
  {
    if (this._headers.ContainsKey("Connection") && this._headers["Connection"].Equals("close", StringComparison.OrdinalIgnoreCase))
      return true;
    return this._headers.ContainsKey("Proxy-Connection") && this._headers["Proxy-Connection"].Equals("close", StringComparison.OrdinalIgnoreCase);
  }

  private int? GetKeepAliveTimeout()
  {
    if (!this._headers.ContainsKey("Keep-Alive"))
      return new int?();
    string header = this._headers["Keep-Alive"];
    System.Text.RegularExpressions.Match match = HttpResponse.KeepAliveTimeoutRegex.Match(header);
    return match.Success ? new int?(int.Parse(match.Groups["value"].Value) * 1000) : new int?();
  }

  private int? GetKeepAliveMax()
  {
    if (!this._headers.ContainsKey("Keep-Alive"))
      return new int?();
    string header = this._headers["Keep-Alive"];
    System.Text.RegularExpressions.Match match = HttpResponse.KeepAliveMaxRegex.Match(header);
    return match.Success ? new int?(int.Parse(match.Groups["value"].Value)) : new int?();
  }

  private Uri GetLocation()
  {
    string relativeUri;
    if (!this._headers.TryGetValue("Location", out relativeUri))
      this._headers.TryGetValue("Redirect-Location", out relativeUri);
    if (string.IsNullOrEmpty(relativeUri))
      return (Uri) null;
    Uri result;
    Uri.TryCreate(this._request.Address, relativeUri, out result);
    return result;
  }

  private Encoding GetCharacterSet()
  {
    if (!this._headers.ContainsKey("Content-Type"))
      return this._request.CharacterSet ?? Encoding.Default;
    string header = this._headers["Content-Type"];
    System.Text.RegularExpressions.Match match = HttpResponse.ContentCharsetRegex.Match(header);
    if (!match.Success)
      return this._request.CharacterSet ?? Encoding.Default;
    Group group = match.Groups["value"];
    try
    {
      return Encoding.GetEncoding(group.Value);
    }
    catch (ArgumentException ex)
    {
      return this._request.CharacterSet ?? Encoding.Default;
    }
  }

  private long GetContentLength()
  {
    string header = Http.Headers[HttpHeader.ContentLength];
    if (!this._headers.ContainsKey(header))
      return -1;
    long result;
    if (!long.TryParse(this._headers[header], out result))
      throw new FormatException($"Invalid response header \"{header}\" value");
    return result;
  }

  private string GetContentType()
  {
    string header = Http.Headers[HttpHeader.ContentType];
    if (!this._headers.ContainsKey(header))
      return string.Empty;
    string contentType = this._headers[header];
    int length = contentType.IndexOf(';');
    if (length != -1)
      contentType = contentType.Substring(0, length);
    return contentType;
  }

  private void WaitData()
  {
    int num1 = 0;
    int num2 = this._request.TcpClient.ReceiveTimeout < 10 ? 10 : this._request.TcpClient.ReceiveTimeout;
    while (!this._request.ClientNetworkStream.DataAvailable)
    {
      if (num1 >= num2)
        throw this.NewHttpException(Resources.HttpException_WaitDataTimeout);
      num1 += 10;
      Thread.Sleep(10);
    }
  }

  private Stream GetZipStream(Stream stream)
  {
    string lower = this._headers[Http.Headers[HttpHeader.ContentEncoding]].ToLower();
    switch (lower)
    {
      case "gzip":
        return (Stream) new GZipStream(stream, CompressionMode.Decompress, true);
      case "deflate":
        return (Stream) new DeflateStream(stream, CompressionMode.Decompress, true);
      default:
        throw new InvalidOperationException(string.Format(Resources.InvalidOperationException_NotSupportedEncodingFormat, (object) lower));
    }
  }

  private static bool FindSignature(byte[] source, int sourceLength, byte[] signature)
  {
    int num = sourceLength - signature.Length + 1;
    for (int index1 = 0; index1 < num; ++index1)
    {
      for (int index2 = 0; index2 < signature.Length; ++index2)
      {
        char lower = (char) source[index2 + index1];
        if (char.IsLetter(lower))
          lower = char.ToLower(lower);
        if ((int) (byte) lower == (int) signature[index2])
        {
          if (index2 == signature.Length - 1)
            return true;
        }
        else
          break;
      }
    }
    return false;
  }

  private HttpException NewHttpException(string message, Exception innerException = null)
  {
    return new HttpException(string.Format(message, (object) this.Address.Host), HttpExceptionStatus.ReceiveFailure, innerException: innerException);
  }

  private sealed class BytesWrapper
  {
    public int Length { get; set; }

    public byte[] Value { get; set; }
  }

  private sealed class ReceiverHelper
  {
    private const int InitialLineSize = 1000;
    private Stream _stream;
    private readonly byte[] _buffer;
    private readonly int _bufferSize;
    private int _linePosition;
    private byte[] _lineBuffer = new byte[1000];

    public bool HasData => this.Length - this.Position != 0;

    private int Length { get; set; }

    public int Position { get; private set; }

    public ReceiverHelper(int bufferSize)
    {
      this._bufferSize = bufferSize;
      this._buffer = new byte[this._bufferSize];
    }

    public void Init(Stream stream)
    {
      this._stream = stream;
      this._linePosition = 0;
      this.Length = 0;
      this.Position = 0;
    }

    public string ReadLine()
    {
      this._linePosition = 0;
      while (true)
      {
        do
        {
          if (this.Position == this.Length)
          {
            this.Position = 0;
            this.Length = this._stream.Read(this._buffer, 0, this._bufferSize);
            if (this.Length == 0)
              goto label_6;
          }
          byte num = this._buffer[this.Position++];
          this._lineBuffer[this._linePosition++] = num;
          if (num == (byte) 10)
            goto label_6;
        }
        while (this._linePosition != this._lineBuffer.Length);
        byte[] numArray = new byte[this._lineBuffer.Length * 2];
        this._lineBuffer.CopyTo((Array) numArray, 0);
        this._lineBuffer = numArray;
      }
label_6:
      return Encoding.ASCII.GetString(this._lineBuffer, 0, this._linePosition);
    }

    public int Read(byte[] buffer, int index, int length)
    {
      int length1 = this.Length - this.Position;
      if (length1 > length)
        length1 = length;
      Array.Copy((Array) this._buffer, this.Position, (Array) buffer, index, length1);
      this.Position += length1;
      return length1;
    }
  }

  private sealed class ZipWrapperStream : Stream
  {
    private readonly Stream _baseStream;
    private readonly HttpResponse.ReceiverHelper _receiverHelper;

    private int BytesRead { get; set; }

    public int TotalBytesRead { get; set; }

    public int LimitBytesRead { private get; set; }

    public override bool CanRead => this._baseStream.CanRead;

    public override bool CanSeek => this._baseStream.CanSeek;

    public override bool CanTimeout => this._baseStream.CanTimeout;

    public override bool CanWrite => this._baseStream.CanWrite;

    public override long Length => this._baseStream.Length;

    public override long Position
    {
      get => this._baseStream.Position;
      set => this._baseStream.Position = value;
    }

    public ZipWrapperStream(Stream baseStream, HttpResponse.ReceiverHelper receiverHelper)
    {
      this._baseStream = baseStream;
      this._receiverHelper = receiverHelper;
    }

    public override void Flush() => this._baseStream.Flush();

    public override void SetLength(long value) => this._baseStream.SetLength(value);

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this._baseStream.Seek(offset, origin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.LimitBytesRead != 0)
      {
        int num = this.LimitBytesRead - this.TotalBytesRead;
        if (num == 0)
          return 0;
        if (num > buffer.Length)
          num = buffer.Length;
        this.BytesRead = this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, offset, num) : this._baseStream.Read(buffer, offset, num);
      }
      else
        this.BytesRead = this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, offset, count) : this._baseStream.Read(buffer, offset, count);
      this.TotalBytesRead += this.BytesRead;
      return this.BytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._baseStream.Write(buffer, offset, count);
    }
  }
}
