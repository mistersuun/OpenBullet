// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.BaseLoader
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io;

public abstract class BaseLoader : ILoader
{
  private readonly IBrowsingContext _context;
  private readonly Predicate<Request> _filter;
  private readonly List<IDownload> _downloads;

  public BaseLoader(IBrowsingContext context, Predicate<Request> filter)
  {
    this._context = context;
    this._filter = filter ?? (Predicate<Request>) (_ => true);
    this._downloads = new List<IDownload>();
    this.MaxRedirects = 50;
  }

  public int MaxRedirects { get; protected set; }

  protected virtual void Add(IDownload download)
  {
    lock (this)
      this._downloads.Add(download);
  }

  protected virtual void Remove(IDownload download)
  {
    lock (this)
      this._downloads.Remove(download);
  }

  protected virtual string GetCookie(Url url) => this._context.GetCookie(url);

  protected virtual void SetCookie(Url url, string value) => this._context.SetCookie(url, value);

  protected virtual IDownload DownloadAsync(Request request, INode originator)
  {
    CancellationTokenSource cts = new CancellationTokenSource();
    if (!this._filter(request))
      return (IDownload) new Download(Task.FromResult<IResponse>((IResponse) null), cts, request.Address, (object) originator);
    Task<IResponse> task = this.LoadAsync(request, cts.Token);
    Download download = new Download(task, cts, request.Address, (object) originator);
    this.Add((IDownload) download);
    task.ContinueWith((Action<Task<IResponse>>) (m => this.Remove((IDownload) download)));
    return (IDownload) download;
  }

  public IEnumerable<IDownload> GetDownloads()
  {
    lock (this)
      return (IEnumerable<IDownload>) this._downloads.ToArray();
  }

  protected async Task<IResponse> LoadAsync(Request request, CancellationToken cancel)
  {
    IEnumerable<IRequester> requesters = this._context.GetServices<IRequester>();
    IResponse response = (IResponse) null;
    int redirectCount = 0;
    this.AppendCookieTo(request);
    do
    {
      if (response != null)
      {
        ++redirectCount;
        this.ExtractCookieFrom(response);
        request = BaseLoader.CreateNewRequest(request, response);
        this.AppendCookieTo(request);
      }
      foreach (IRequester requester in requesters)
      {
        if (requester.SupportsProtocol(request.Address.Scheme))
        {
          this._context.Fire((Event) new RequestEvent(request, (IResponse) null));
          response = await requester.RequestAsync(request, cancel).ConfigureAwait(false);
          this._context.Fire((Event) new RequestEvent(request, response));
          break;
        }
      }
    }
    while (response != null && response.StatusCode.IsRedirected() && redirectCount < this.MaxRedirects);
    return response;
  }

  protected static Request CreateNewRequest(Request request, IResponse response)
  {
    HttpMethod httpMethod = request.Method;
    Stream stream = request.Content ?? Stream.Null;
    Dictionary<string, string> dictionary = new Dictionary<string, string>(request.Headers);
    string header = response.Headers[HeaderNames.Location];
    if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.SeeOther)
    {
      httpMethod = HttpMethod.Get;
      dictionary.Remove(HeaderNames.ContentType);
      stream = Stream.Null;
    }
    else if (stream.Length > 0L)
      stream.Position = 0L;
    dictionary.Remove(HeaderNames.Cookie);
    return new Request()
    {
      Address = new Url(request.Address, header),
      Method = httpMethod,
      Content = stream,
      Headers = (IDictionary<string, string>) dictionary
    };
  }

  private void AppendCookieTo(Request request)
  {
    string cookie = this.GetCookie(request.Address);
    if (cookie == null)
      return;
    request.Headers[HeaderNames.Cookie] = cookie;
  }

  private void ExtractCookieFrom(IResponse response)
  {
    string orDefault = response.Headers.GetOrDefault<string, string>(HeaderNames.SetCookie, (string) null);
    if (orDefault == null)
      return;
    this.SetCookie(response.Address, orDefault);
  }
}
