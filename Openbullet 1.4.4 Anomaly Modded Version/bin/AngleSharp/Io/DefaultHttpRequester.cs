// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.DefaultHttpRequester
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io;

public sealed class DefaultHttpRequester : BaseRequester
{
  private const int BufferSize = 4096 /*0x1000*/;
  private static readonly string Version = CustomAttributeExtensions.GetCustomAttribute<AssemblyFileVersionAttribute>(((Type) IntrospectionExtensions.GetTypeInfo(typeof (DefaultHttpRequester))).Assembly).Version;
  private static readonly string AgentName = "AngleSharp/" + DefaultHttpRequester.Version;
  private static readonly Dictionary<string, PropertyInfo> PropCache = new Dictionary<string, PropertyInfo>();
  private static readonly List<string> Restricted = new List<string>();
  private TimeSpan _timeOut;
  private readonly Action<HttpWebRequest> _setup;
  private readonly Dictionary<string, string> _headers;

  public DefaultHttpRequester(string userAgent = null, Action<HttpWebRequest> setup = null)
  {
    this._timeOut = new TimeSpan(0, 0, 0, 45);
    this._setup = setup ?? (Action<HttpWebRequest>) (r => { });
    this._headers = new Dictionary<string, string>()
    {
      {
        HeaderNames.UserAgent,
        userAgent ?? DefaultHttpRequester.AgentName
      }
    };
  }

  public IDictionary<string, string> Headers => (IDictionary<string, string>) this._headers;

  public TimeSpan Timeout
  {
    get => this._timeOut;
    set => this._timeOut = value;
  }

  public override bool SupportsProtocol(string protocol)
  {
    return protocol.IsOneOf(ProtocolNames.Http, ProtocolNames.Https);
  }

  protected override async Task<IResponse> PerformRequestAsync(
    Request request,
    CancellationToken cancellationToken)
  {
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(this._timeOut);
    DefaultHttpRequester.RequestState requestState = new DefaultHttpRequester.RequestState(request, (IDictionary<string, string>) this._headers, this._setup);
    IResponse response;
    using (cancellationToken.Register(new Action(cancellationTokenSource.Cancel)))
      response = await requestState.RequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);
    return response;
  }

  private static void RaiseConnectionLimit(HttpWebRequest http)
  {
    object obj = typeof (HttpWebRequest).GetField("_ServicePoint")?.GetValue((object) http);
    obj?.GetType().GetProperty("ConnectionLimit")?.SetValue(obj, (object) 1024 /*0x0400*/, (object[]) null);
  }

  private sealed class RequestState
  {
    private static MethodInfo _serverString;
    private readonly CookieContainer _cookies;
    private readonly IDictionary<string, string> _headers;
    private readonly HttpWebRequest _http;
    private readonly Request _request;
    private readonly byte[] _buffer;

    public RequestState(
      Request request,
      IDictionary<string, string> headers,
      Action<HttpWebRequest> setup)
    {
      this._cookies = new CookieContainer();
      this._headers = headers;
      this._request = request;
      this._http = WebRequest.Create((Uri) request.Address) as HttpWebRequest;
      this._http.CookieContainer = this._cookies;
      this._http.Method = request.Method.ToString().ToUpperInvariant();
      this._buffer = new byte[4096 /*0x1000*/];
      this.SetHeaders();
      this.SetCookies();
      this.AllowCompression();
      this.DisableAutoRedirect();
      setup(this._http);
    }

    public async Task<IResponse> RequestAsync(CancellationToken cancellationToken)
    {
      cancellationToken.Register(new Action(((WebRequest) this._http).Abort));
      if (this._request.Method == HttpMethod.Post || this._request.Method == HttpMethod.Put)
        this.SendRequest(await Task.Factory.FromAsync<Stream>(new Func<AsyncCallback, object, IAsyncResult>(((WebRequest) this._http).BeginGetRequestStream), new Func<IAsyncResult, Stream>(((WebRequest) this._http).EndGetRequestStream), (object) null).ConfigureAwait(false));
      WebResponse response;
      try
      {
        response = await Task.Factory.FromAsync<WebResponse>(new Func<AsyncCallback, object, IAsyncResult>(((WebRequest) this._http).BeginGetResponse), new Func<IAsyncResult, WebResponse>(((WebRequest) this._http).EndGetResponse), (object) null).ConfigureAwait(false);
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      DefaultHttpRequester.RaiseConnectionLimit(this._http);
      return (IResponse) this.GetResponse(response as HttpWebResponse);
    }

    private void SendRequest(Stream target)
    {
      Stream content = this._request.Content;
      while (content != null)
      {
        int count = content.Read(this._buffer, 0, 4096 /*0x1000*/);
        if (count == 0)
          break;
        target.Write(this._buffer, 0, count);
      }
    }

    private DefaultResponse GetResponse(HttpWebResponse response)
    {
      if (response == null)
        return (DefaultResponse) null;
      CookieCollection cookies = this._cookies.GetCookies((Uri) this._request.Address);
      Cookie[] array = this._cookies.GetCookies(response.ResponseUri).OfType<Cookie>().Except<Cookie>(cookies.OfType<Cookie>()).ToArray<Cookie>();
      IEnumerable<\u003C\u003Ef__AnonymousType2<string, string>> datas = ((IEnumerable<string>) response.Headers.AllKeys).Select(m => new
      {
        Key = m,
        Value = response.Headers[m]
      });
      DefaultResponse response1 = new DefaultResponse()
      {
        Content = response.GetResponseStream(),
        StatusCode = response.StatusCode,
        Address = Url.Convert(response.ResponseUri)
      };
      foreach (var data in datas)
        response1.Headers.Add(data.Key, data.Value);
      if (array.Length != 0)
      {
        IEnumerable<string> values = ((IEnumerable<Cookie>) array).Select<Cookie, string>(new Func<Cookie, string>(DefaultHttpRequester.RequestState.Stringify));
        response1.Headers[HeaderNames.SetCookie] = string.Join(", ", values);
      }
      return response1;
    }

    private static string Stringify(Cookie cookie)
    {
      if (DefaultHttpRequester.RequestState._serverString == (MethodInfo) null)
      {
        MethodInfo[] methods = typeof (Cookie).GetMethods();
        MethodInfo methodInfo = ((IEnumerable<MethodInfo>) methods).FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name.Equals("ToServerString")));
        if ((object) methodInfo == null)
          methodInfo = ((IEnumerable<MethodInfo>) methods).FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name.Equals("ToString")));
        DefaultHttpRequester.RequestState._serverString = methodInfo;
      }
      return DefaultHttpRequester.RequestState._serverString.Invoke((object) cookie, (object[]) null).ToString();
    }

    private void AddHeader(string key, string value)
    {
      if (key.Is(HeaderNames.Accept))
        this._http.Accept = value;
      else if (key.Is(HeaderNames.ContentType))
        this._http.ContentType = value;
      else if (key.Is(HeaderNames.Expect))
        this.SetProperty(HeaderNames.Expect, (object) value);
      else if (key.Is(HeaderNames.Date))
        this.SetProperty(HeaderNames.Date, (object) DateTime.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture));
      else if (key.Is(HeaderNames.Host))
        this.SetProperty(HeaderNames.Host, (object) value);
      else if (key.Is(HeaderNames.IfModifiedSince))
        this.SetProperty("IfModifiedSince", (object) DateTime.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture));
      else if (key.Is(HeaderNames.Referer))
        this.SetProperty(HeaderNames.Referer, (object) value);
      else if (key.Is(HeaderNames.UserAgent))
      {
        this.SetProperty("UserAgent", (object) value);
      }
      else
      {
        if (key.Is(HeaderNames.Connection) || key.Is(HeaderNames.Range) || key.Is(HeaderNames.ContentLength) || key.Is(HeaderNames.TransferEncoding))
          return;
        this._http.Headers[key] = value;
      }
    }

    private void SetCookies()
    {
      this._cookies.SetCookies(this._http.RequestUri, this._request.Headers.GetOrDefault<string, string>(HeaderNames.Cookie, string.Empty).Replace(';', ',').Replace("$", ""));
    }

    private void SetHeaders()
    {
      foreach (KeyValuePair<string, string> header in (IEnumerable<KeyValuePair<string, string>>) this._headers)
        this.AddHeader(header.Key, header.Value);
      foreach (KeyValuePair<string, string> header in (IEnumerable<KeyValuePair<string, string>>) this._request.Headers)
      {
        if (!header.Key.Is(HeaderNames.Cookie))
          this.AddHeader(header.Key, header.Value);
      }
    }

    private void AllowCompression() => this.SetProperty("AutomaticDecompression", (object) 3);

    private void DisableAutoRedirect() => this.SetProperty("AllowAutoRedirect", (object) false);

    private void SetProperty(string name, object value)
    {
      PropertyInfo property;
      if (!DefaultHttpRequester.PropCache.TryGetValue(name, out property))
      {
        lock (DefaultHttpRequester.PropCache)
        {
          if (!DefaultHttpRequester.PropCache.TryGetValue(name, out property))
          {
            property = this._http.GetType().GetProperty(name);
            DefaultHttpRequester.PropCache.Add(name, property);
          }
        }
      }
      if (DefaultHttpRequester.Restricted.Contains(name) || !(property != (PropertyInfo) null))
        return;
      if (!property.CanWrite)
        return;
      try
      {
        property.SetValue((object) this._http, value, (object[]) null);
      }
      catch
      {
        lock (DefaultHttpRequester.Restricted)
        {
          if (DefaultHttpRequester.Restricted.Contains(name))
            return;
          DefaultHttpRequester.Restricted.Add(name);
        }
      }
    }
  }
}
