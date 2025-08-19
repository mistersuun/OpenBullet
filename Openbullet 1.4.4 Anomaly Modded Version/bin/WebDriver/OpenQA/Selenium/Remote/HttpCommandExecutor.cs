// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.HttpCommandExecutor
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class HttpCommandExecutor : ICommandExecutor, IDisposable
{
  private const string JsonMimeType = "application/json";
  private const string PngMimeType = "image/png";
  private const string CharsetType = "charset=utf-8";
  private const string ContentTypeHeader = "application/json;charset=utf-8";
  private const string RequestAcceptHeader = "application/json, image/png";
  private const string UserAgentHeaderTemplate = "selenium/{0} (.net {1})";
  private Uri remoteServerUri;
  private TimeSpan serverResponseTimeout;
  private bool enableKeepAlive;
  private bool isDisposed;
  private IWebProxy proxy;
  private CommandInfoRepository commandInfoRepository = (CommandInfoRepository) new WebDriverWireProtocolCommandInfoRepository();

  public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout)
    : this(addressOfRemoteServer, timeout, true)
  {
  }

  public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout, bool enableKeepAlive)
  {
    if (addressOfRemoteServer == (Uri) null)
      throw new ArgumentNullException(nameof (addressOfRemoteServer), "You must specify a remote address to connect to");
    if (!addressOfRemoteServer.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
      addressOfRemoteServer = new Uri(addressOfRemoteServer.ToString() + "/");
    this.remoteServerUri = addressOfRemoteServer;
    this.serverResponseTimeout = timeout;
    this.enableKeepAlive = enableKeepAlive;
    ServicePointManager.Expect100Continue = false;
    ServicePointManager.DefaultConnectionLimit = 2000;
    if (!(Type.GetType("Mono.Runtime", false, true) == (Type) null))
      return;
    HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
  }

  public event EventHandler<SendingRemoteHttpRequestEventArgs> SendingRemoteHttpRequest;

  public CommandInfoRepository CommandInfoRepository => this.commandInfoRepository;

  public IWebProxy Proxy
  {
    get => this.proxy;
    set => this.proxy = value;
  }

  public bool IsKeepAliveEnabled
  {
    get => this.enableKeepAlive;
    set => this.enableKeepAlive = value;
  }

  public virtual Response Execute(Command commandToExecute)
  {
    if (commandToExecute == null)
      throw new ArgumentNullException(nameof (commandToExecute), "commandToExecute cannot be null");
    CommandInfo commandInfo = this.commandInfoRepository.GetCommandInfo(commandToExecute.Name);
    Response response = this.CreateResponse(this.MakeHttpRequest(new HttpCommandExecutor.HttpRequestInfo(this.remoteServerUri, commandToExecute, commandInfo)));
    if (commandToExecute.Name == DriverCommand.NewSession && response.IsSpecificationCompliant)
      this.commandInfoRepository = (CommandInfoRepository) new W3CWireProtocolCommandInfoRepository();
    return response;
  }

  protected virtual void OnSendingRemoteHttpRequest(SendingRemoteHttpRequestEventArgs eventArgs)
  {
    if (eventArgs == null)
      throw new ArgumentNullException(nameof (eventArgs), "eventArgs must not be null");
    if (this.SendingRemoteHttpRequest == null)
      return;
    this.SendingRemoteHttpRequest((object) this, eventArgs);
  }

  private static string GetTextOfWebResponse(HttpWebResponse webResponse)
  {
    StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
    string textOfWebResponse = streamReader.ReadToEnd();
    streamReader.Close();
    if (textOfWebResponse.IndexOf(char.MinValue) >= 0)
      textOfWebResponse = textOfWebResponse.Substring(0, textOfWebResponse.IndexOf(char.MinValue));
    return textOfWebResponse;
  }

  private HttpCommandExecutor.HttpResponseInfo MakeHttpRequest(
    HttpCommandExecutor.HttpRequestInfo requestInfo)
  {
    HttpWebRequest request = WebRequest.Create(requestInfo.FullUri) as HttpWebRequest;
    if (!string.IsNullOrEmpty(requestInfo.FullUri.UserInfo) && requestInfo.FullUri.UserInfo.Contains(":"))
    {
      string[] strArray = this.remoteServerUri.UserInfo.Split(new char[1]
      {
        ':'
      }, 2);
      request.Credentials = (ICredentials) new NetworkCredential(strArray[0], strArray[1]);
      request.PreAuthenticate = true;
    }
    string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "selenium/{0} (.net {1})", (object) ResourceUtilities.AssemblyVersion, (object) ResourceUtilities.PlatformFamily);
    request.UserAgent = str;
    request.Method = requestInfo.HttpMethod;
    request.Timeout = (int) this.serverResponseTimeout.TotalMilliseconds;
    request.Accept = "application/json, image/png";
    request.KeepAlive = this.enableKeepAlive;
    request.Proxy = this.proxy;
    request.ServicePoint.ConnectionLimit = 2000;
    if (request.Method == "GET")
      request.Headers.Add("Cache-Control", "no-cache");
    SendingRemoteHttpRequestEventArgs eventArgs = new SendingRemoteHttpRequestEventArgs(request, requestInfo.RequestBody);
    this.OnSendingRemoteHttpRequest(eventArgs);
    if (request.Method == "POST")
    {
      byte[] bytes = Encoding.UTF8.GetBytes(eventArgs.RequestBody);
      request.ContentType = "application/json;charset=utf-8";
      Stream requestStream = request.GetRequestStream();
      requestStream.Write(bytes, 0, bytes.Length);
      requestStream.Close();
    }
    HttpCommandExecutor.HttpResponseInfo httpResponseInfo = new HttpCommandExecutor.HttpResponseInfo();
    HttpWebResponse response;
    try
    {
      response = request.GetResponse() as HttpWebResponse;
    }
    catch (WebException ex)
    {
      response = ex.Response as HttpWebResponse;
      if (ex.Status == WebExceptionStatus.Timeout)
        throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The HTTP request to the remote WebDriver server for URL {0} timed out after {1} seconds.", (object) request.RequestUri.AbsoluteUri, (object) this.serverResponseTimeout.TotalSeconds), (Exception) ex);
      if (ex.Response == null)
        throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A exception with a null response was thrown sending an HTTP request to the remote WebDriver server for URL {0}. The status of the exception was {1}, and the message was: {2}", (object) request.RequestUri.AbsoluteUri, (object) ex.Status, (object) ex.Message), (Exception) ex);
    }
    httpResponseInfo.Body = response != null ? HttpCommandExecutor.GetTextOfWebResponse(response) : throw new WebDriverException("No response from server for url " + request.RequestUri.AbsoluteUri);
    httpResponseInfo.ContentType = response.ContentType;
    httpResponseInfo.StatusCode = response.StatusCode;
    return httpResponseInfo;
  }

  private Response CreateResponse(HttpCommandExecutor.HttpResponseInfo stuff)
  {
    Response response = new Response();
    string body = stuff.Body;
    if (stuff.ContentType != null && stuff.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
      response = Response.FromJson(body);
    else
      response.Value = (object) body;
    if (this.commandInfoRepository.SpecificationLevel < 1 && (stuff.StatusCode < HttpStatusCode.OK || stuff.StatusCode >= HttpStatusCode.BadRequest))
    {
      if (stuff.StatusCode >= HttpStatusCode.BadRequest && stuff.StatusCode < HttpStatusCode.InternalServerError)
        response.Status = WebDriverResult.UnhandledError;
      else if (stuff.StatusCode >= HttpStatusCode.InternalServerError)
      {
        if (stuff.StatusCode == HttpStatusCode.NotImplemented)
          response.Status = WebDriverResult.UnknownCommand;
        else if (response.Status == WebDriverResult.Success)
          response.Status = WebDriverResult.UnhandledError;
      }
      else
        response.Status = WebDriverResult.UnhandledError;
    }
    if (response.Value is string)
      response.Value = (object) ((string) response.Value).Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
    return response;
  }

  public void Dispose() => this.Dispose(true);

  protected virtual void Dispose(bool disposing)
  {
    if (this.isDisposed)
      return;
    this.isDisposed = true;
  }

  private class HttpRequestInfo
  {
    public HttpRequestInfo(Uri serverUri, Command commandToExecute, CommandInfo commandInfo)
    {
      this.FullUri = commandInfo.CreateCommandUri(serverUri, commandToExecute);
      this.HttpMethod = commandInfo.Method;
      this.RequestBody = commandToExecute.ParametersAsJsonString;
    }

    public Uri FullUri { get; set; }

    public string HttpMethod { get; set; }

    public string RequestBody { get; set; }
  }

  private class HttpResponseInfo
  {
    public HttpStatusCode StatusCode { get; set; }

    public string Body { get; set; }

    public string ContentType { get; set; }
  }
}
