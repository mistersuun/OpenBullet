// Decompiled with JetBrains decompiler
// Type: Extreme.Net.HttpResponse
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace Extreme.Net;

public sealed class HttpResponse
{
  private static readonly byte[] _openHtmlSignature = Encoding.ASCII.GetBytes("<html");
  private static readonly byte[] _closeHtmlSignature = Encoding.ASCII.GetBytes("</html>");
  private static readonly Regex _keepAliveTimeoutRegex = new Regex("timeout(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex _keepAliveMaxRegex = new Regex("max(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static readonly Regex _contentCharsetRegex = new Regex("charset(|\\s+)=(|\\s+)(?<value>[a-z,0-9,-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private readonly HttpRequest _request;
  private HttpResponse.ReceiverHelper _receiverHelper;
  private readonly Dictionary<string, string> _headers = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private readonly CookieDictionary _rawCookies = new CookieDictionary();

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

  public int ReconnectCount { get; internal set; }

  public Uri Address { get; private set; }

  public HttpMethod Method { get; private set; }

  public Version ProtocolVersion { get; private set; }

  public HttpStatusCode StatusCode { get; private set; }

  public Uri RedirectAddress { get; private set; }

  public Encoding CharacterSet { get; private set; }

  public int ContentLength { get; private set; }

  public string ContentType { get; private set; }

  public string Location => this[nameof (Location)];

  public CookieDictionary Cookies { get; private set; }

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
    this.ContentLength = -1;
    this.ContentType = string.Empty;
  }

  public byte[] ToBytes()
  {
    if (this.HasError)
      throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
    if (this.MessageBodyLoaded)
      return new byte[0];
    MemoryStream memoryStream = new MemoryStream(this.ContentLength == -1 ? 0 : this.ContentLength);
    try
    {
      foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
        memoryStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
    }
    catch (Exception ex)
    {
      this.HasError = true;
      if (ex is IOException || ex is InvalidOperationException)
        throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
      throw;
    }
    if (this.ConnectionClosed())
      this._request.Dispose();
    this.MessageBodyLoaded = true;
    return memoryStream.ToArray();
  }

  public override string ToString()
  {
    if (this.HasError)
      throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
    if (this.MessageBodyLoaded)
      return string.Empty;
    MemoryStream memoryStream = new MemoryStream(this.ContentLength == -1 ? 0 : this.ContentLength);
    try
    {
      foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
        memoryStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
    }
    catch (Exception ex)
    {
      this.HasError = true;
      if (ex is IOException || ex is InvalidOperationException)
        throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
      throw;
    }
    if (this.ConnectionClosed())
      this._request.Dispose();
    this.MessageBodyLoaded = true;
    return this.CharacterSet.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
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
        foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
          fileStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
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
      if (ex is IOException || ex is InvalidOperationException)
        throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
      throw;
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
    MemoryStream memoryStream = new MemoryStream(this.ContentLength == -1 ? 0 : this.ContentLength);
    try
    {
      foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
        memoryStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
    }
    catch (Exception ex)
    {
      this.HasError = true;
      if (ex is IOException || ex is InvalidOperationException)
        throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
      throw;
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
        foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
          ;
      }
      catch (Exception ex)
      {
        this.HasError = true;
        if (ex is IOException || ex is InvalidOperationException)
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        throw;
      }
    }
    this.MessageBodyLoaded = true;
  }

  public bool ContainsCookie(string name) => this.Cookies != null && this.Cookies.ContainsKey(name);

  public bool ContainsRawCookie(string name) => this._rawCookies.ContainsKey(name);

  public string GetRawCookie(string name)
  {
    string empty;
    if (!this._rawCookies.TryGetValue(name, out empty))
      empty = string.Empty;
    return empty;
  }

  public Dictionary<string, string>.Enumerator EnumerateRawCookies()
  {
    return this._rawCookies.GetEnumerator();
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

  internal long LoadResponse(HttpMethod method)
  {
    this.Method = method;
    this.Address = this._request.Address;
    this.HasError = false;
    this.MessageBodyLoaded = false;
    this.KeepAliveTimeout = new int?();
    this.MaximumKeepAliveRequests = new int?();
    this._headers.Clear();
    this._rawCookies.Clear();
    this.Cookies = this._request.Cookies == null || this._request.Cookies.IsLocked ? new CookieDictionary() : this._request.Cookies;
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
    if (this.ContentLength == 0 || this.Method == HttpMethod.HEAD || this.StatusCode == HttpStatusCode.Continue || this.StatusCode == HttpStatusCode.NoContent || this.StatusCode == HttpStatusCode.NotModified)
      this.MessageBodyLoaded = true;
    long position = (long) this._receiverHelper.Position;
    if (this.ContentLength > 0)
      position += (long) this.ContentLength;
    return position;
  }

  private void ReceiveStartingLine()
  {
    string str1;
    do
    {
      str1 = this._receiverHelper.ReadLine();
      if (str1.Length == 0)
        goto label_1;
    }
    while (str1 == "\r\n");
    goto label_4;
label_1:
    HttpException httpException = this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse);
    httpException.EmptyMessageBody = true;
    throw httpException;
label_4:
    string input = str1.Substring("HTTP/", " ");
    string str2 = str1.Substring(" ", " ");
    if (str2.Length == 0)
      str2 = str1.Substring(" ", "\r\n");
    if (input.Length == 0 || str2.Length == 0)
      throw this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse);
    this.ProtocolVersion = Version.Parse(input);
    this.StatusCode = (HttpStatusCode) Enum.Parse(typeof (HttpStatusCode), str2);
  }

  private void SetCookie(string value)
  {
    if (value.Length == 0)
      return;
    int num1 = value.IndexOf(';');
    int length = value.IndexOf('=');
    string key = length != -1 ? value.Substring(0, length) : throw this.NewHttpException(string.Format(Resources.HttpException_WrongCookie, (object) value, (object) this.Address.Host));
    string str;
    if (num1 == -1)
    {
      str = value.Substring(length + 1);
    }
    else
    {
      str = value.Substring(length + 1, num1 - length - 1);
      int startIndex1 = value.IndexOf("expires=");
      if (startIndex1 != -1)
      {
        int num2 = value.IndexOf(';', startIndex1);
        int startIndex2 = startIndex1 + 8;
        DateTime result;
        if (DateTime.TryParse(num2 != -1 ? value.Substring(startIndex2, num2 - startIndex2) : value.Substring(startIndex2), out result) && result < DateTime.Now)
          this.Cookies.Remove(key);
      }
    }
    if (str.Length == 0 || str.Equals("deleted", StringComparison.OrdinalIgnoreCase))
      this.Cookies.Remove(key);
    else
      this.Cookies[key] = str;
    this._rawCookies[key] = value;
  }

  private void ReceiveHeaders()
  {
    string str1;
    while (true)
    {
      str1 = this._receiverHelper.ReadLine();
      if (!(str1 == "\r\n"))
      {
        int length = str1.IndexOf(':');
        if (length != -1)
        {
          string key = str1.Substring(0, length);
          string str2 = str1.Substring(length + 1).Trim(' ', '\t', '\r', '\n');
          if (key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
            this.SetCookie(str2);
          else
            this._headers[key] = str2;
        }
        else
          goto label_3;
      }
      else
        break;
    }
    return;
label_3:
    throw this.NewHttpException(string.Format(Resources.HttpException_WrongHeader, (object) str1, (object) this.Address.Host));
  }

  private IEnumerable<HttpResponse.BytesWraper> GetMessageBodySource()
  {
    return this._headers.ContainsKey("Content-Encoding") ? this.GetMessageBodySourceZip() : this.GetMessageBodySourceStd();
  }

  private IEnumerable<HttpResponse.BytesWraper> GetMessageBodySourceStd()
  {
    if (this._headers.ContainsKey("Transfer-Encoding"))
      return this.ReceiveMessageBodyChunked();
    return this.ContentLength != -1 ? this.ReceiveMessageBody(this.ContentLength) : this.ReceiveMessageBody(this._request.ClientStream);
  }

  private IEnumerable<HttpResponse.BytesWraper> GetMessageBodySourceZip()
  {
    if (this._headers.ContainsKey("Transfer-Encoding"))
      return this.ReceiveMessageBodyChunkedZip();
    return this.ContentLength != -1 ? this.ReceiveMessageBodyZip(this.ContentLength) : this.ReceiveMessageBody(this.GetZipStream((Stream) new HttpResponse.ZipWraperStream(this._request.ClientStream, this._receiverHelper)));
  }

  private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBody(Stream stream)
  {
    HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
    int bufferSize = this._request.TcpClient.ReceiveBufferSize;
    byte[] buffer = new byte[bufferSize];
    bytesWraper.Value = buffer;
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
    bytesWraper.Length = begBytesRead;
    yield return bytesWraper;
    bool isHtml = this.FindSignature(buffer, begBytesRead, HttpResponse._openHtmlSignature);
    if (isHtml)
    {
      if (this.FindSignature(buffer, begBytesRead, HttpResponse._closeHtmlSignature))
        yield break;
    }
    int bytesRead;
    while (true)
    {
      bytesRead = stream.Read(buffer, 0, bufferSize);
      if (isHtml)
      {
        if (bytesRead == 0)
        {
          this.WaitData();
          continue;
        }
        if (this.FindSignature(buffer, bytesRead, HttpResponse._closeHtmlSignature))
          break;
      }
      else if (bytesRead == 0)
        goto label_10;
      bytesWraper.Length = bytesRead;
      yield return bytesWraper;
    }
    bytesWraper.Length = bytesRead;
    yield return bytesWraper;
    yield break;
label_10:;
  }

  private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBody(int contentLength)
  {
    Stream stream = this._request.ClientStream;
    HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
    int bufferSize = this._request.TcpClient.ReceiveBufferSize;
    byte[] buffer = new byte[bufferSize];
    bytesWraper.Value = buffer;
    int totalBytesRead = 0;
    while (totalBytesRead != contentLength)
    {
      int bytesRead = !this._receiverHelper.HasData ? stream.Read(buffer, 0, bufferSize) : this._receiverHelper.Read(buffer, 0, bufferSize);
      if (bytesRead == 0)
      {
        this.WaitData();
      }
      else
      {
        totalBytesRead += bytesRead;
        bytesWraper.Length = bytesRead;
        yield return bytesWraper;
      }
    }
  }

  private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBodyChunked()
  {
    Stream stream = this._request.ClientStream;
    HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
    int bufferSize = this._request.TcpClient.ReceiveBufferSize;
    byte[] buffer = new byte[bufferSize];
    bytesWraper.Value = buffer;
    while (true)
    {
      string line;
      do
      {
        line = this._receiverHelper.ReadLine();
      }
      while (line == "\r\n");
      line = line.Trim(' ', '\r', '\n');
      if (!(line == string.Empty))
      {
        int totalBytesRead = 0;
        int blockLength;
        try
        {
          blockLength = Convert.ToInt32(line, 16 /*0x10*/);
        }
        catch (Exception ex)
        {
          if (ex is FormatException || ex is OverflowException)
            throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, (object) line), ex);
          throw;
        }
        if (blockLength != 0)
        {
          while (totalBytesRead != blockLength)
          {
            int length = blockLength - totalBytesRead;
            if (length > bufferSize)
              length = bufferSize;
            int bytesRead = !this._receiverHelper.HasData ? stream.Read(buffer, 0, length) : this._receiverHelper.Read(buffer, 0, length);
            if (bytesRead == 0)
            {
              this.WaitData();
            }
            else
            {
              totalBytesRead += bytesRead;
              bytesWraper.Length = bytesRead;
              yield return bytesWraper;
            }
          }
          line = (string) null;
        }
        else
          goto label_2;
      }
      else
        break;
    }
    yield break;
label_2:;
  }

  private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBodyZip(int contentLength)
  {
    HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
    HttpResponse.ZipWraperStream streamWrapper = new HttpResponse.ZipWraperStream(this._request.ClientStream, this._receiverHelper);
    using (Stream stream = this.GetZipStream((Stream) streamWrapper))
    {
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWraper.Value = buffer;
      while (true)
      {
        int bytesRead = stream.Read(buffer, 0, bufferSize);
        if (bytesRead == 0)
        {
          if (streamWrapper.TotalBytesRead != contentLength)
            this.WaitData();
          else
            break;
        }
        else
        {
          bytesWraper.Length = bytesRead;
          yield return bytesWraper;
        }
      }
    }
  }

  private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBodyChunkedZip()
  {
    HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
    HttpResponse.ZipWraperStream streamWrapper = new HttpResponse.ZipWraperStream(this._request.ClientStream, this._receiverHelper);
    bool messageBodyChunkedZip;
    using (Stream stream = this.GetZipStream((Stream) streamWrapper))
    {
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWraper.Value = buffer;
      while (true)
      {
        string line;
        do
        {
          line = this._receiverHelper.ReadLine();
        }
        while (line == "\r\n");
        line = line.Trim(' ', '\r', '\n');
        if (!(line == string.Empty))
        {
          int blockLength;
          try
          {
            blockLength = Convert.ToInt32(line, 16 /*0x10*/);
          }
          catch (Exception ex)
          {
            if (ex is FormatException || ex is OverflowException)
              throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, (object) line), ex);
            throw;
          }
          if (blockLength != 0)
          {
            streamWrapper.TotalBytesRead = 0;
            streamWrapper.LimitBytesRead = blockLength;
            while (true)
            {
              int bytesRead = stream.Read(buffer, 0, bufferSize);
              if (bytesRead == 0)
              {
                if (streamWrapper.TotalBytesRead != blockLength)
                  this.WaitData();
                else
                  break;
              }
              else
              {
                bytesWraper.Length = bytesRead;
                yield return bytesWraper;
              }
            }
            line = (string) null;
          }
          else
            goto label_11;
        }
        else
          break;
      }
      messageBodyChunkedZip = false;
      goto label_19;
label_11:
      messageBodyChunkedZip = false;
label_19:;
    }
    return messageBodyChunkedZip;
  }

  private bool ConnectionClosed()
  {
    return this._headers.ContainsKey("Connection") && this._headers["Connection"].Equals("close", StringComparison.OrdinalIgnoreCase) || this._headers.ContainsKey("Proxy-Connection") && this._headers["Proxy-Connection"].Equals("close", StringComparison.OrdinalIgnoreCase);
  }

  private int? GetKeepAliveTimeout()
  {
    if (!this._headers.ContainsKey("Keep-Alive"))
      return new int?();
    string header = this._headers["Keep-Alive"];
    System.Text.RegularExpressions.Match match = HttpResponse._keepAliveTimeoutRegex.Match(header);
    return match.Success ? new int?(int.Parse(match.Groups["value"].Value) * 1000) : new int?();
  }

  private int? GetKeepAliveMax()
  {
    if (!this._headers.ContainsKey("Keep-Alive"))
      return new int?();
    string header = this._headers["Keep-Alive"];
    System.Text.RegularExpressions.Match match = HttpResponse._keepAliveMaxRegex.Match(header);
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
    System.Text.RegularExpressions.Match match = HttpResponse._contentCharsetRegex.Match(header);
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

  private int GetContentLength()
  {
    if (!this._headers.ContainsKey("Content-Length"))
      return -1;
    int result;
    int.TryParse(this._headers["Content-Length"], out result);
    return result;
  }

  private string GetContentType()
  {
    if (!this._headers.ContainsKey("Content-Type"))
      return string.Empty;
    string contentType = this._headers["Content-Type"];
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
    string lower = this._headers["Content-Encoding"].ToLower();
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

  private bool FindSignature(byte[] source, int sourceLength, byte[] signature)
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

  private sealed class BytesWraper
  {
    public int Length { get; set; }

    public byte[] Value { get; set; }
  }

  private sealed class ReceiverHelper
  {
    private const int InitialLineSize = 1000;
    private Stream _stream;
    private byte[] _buffer;
    private int _bufferSize;
    private int _linePosition;
    private byte[] _lineBuffer = new byte[1000];

    public bool HasData => this.Length - this.Position != 0;

    public int Length { get; private set; }

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
        if (this.Position == this.Length)
        {
          this.Position = 0;
          this.Length = this._stream.Read(this._buffer, 0, this._bufferSize);
          if (this.Length == 0)
            break;
        }
        byte num = this._buffer[this.Position++];
        this._lineBuffer[this._linePosition++] = num;
        if (num != (byte) 10)
        {
          if (this._linePosition == this._lineBuffer.Length)
          {
            byte[] numArray = new byte[this._lineBuffer.Length * 2];
            this._lineBuffer.CopyTo((Array) numArray, 0);
            this._lineBuffer = numArray;
          }
        }
        else
          break;
      }
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

  private sealed class ZipWraperStream : Stream
  {
    private Stream _baseStream;
    private HttpResponse.ReceiverHelper _receiverHelper;

    public int BytesRead { get; private set; }

    public int TotalBytesRead { get; set; }

    public int LimitBytesRead { get; set; }

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

    public ZipWraperStream(Stream baseStream, HttpResponse.ReceiverHelper receiverHelper)
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
        this.BytesRead = !this._receiverHelper.HasData ? this._baseStream.Read(buffer, offset, num) : this._receiverHelper.Read(buffer, offset, num);
      }
      else
        this.BytesRead = !this._receiverHelper.HasData ? this._baseStream.Read(buffer, offset, count) : this._receiverHelper.Read(buffer, offset, count);
      this.TotalBytesRead += this.BytesRead;
      return this.BytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._baseStream.Write(buffer, offset, count);
    }
  }
}
