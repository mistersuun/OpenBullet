// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.RequesterExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Text;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io;

public static class RequesterExtensions
{
  public static bool IsRedirected(this HttpStatusCode status)
  {
    return status == HttpStatusCode.Found || status == HttpStatusCode.TemporaryRedirect || status == HttpStatusCode.SeeOther || status == HttpStatusCode.TemporaryRedirect || status == HttpStatusCode.MovedPermanently || status == HttpStatusCode.MultipleChoices;
  }

  public static IDownload FetchWithCorsAsync(this IResourceLoader loader, CorsRequest cors)
  {
    ResourceRequest request = cors.Request;
    CorsSetting setting = cors.Setting;
    Url target = request.Target;
    if (request.Origin == target.Origin || target.Scheme == ProtocolNames.Data || target.Href == "about:blank")
      return loader.FetchFromSameOriginAsync(target, cors);
    if (setting == CorsSetting.Anonymous || setting == CorsSetting.UseCredentials)
      return loader.FetchFromDifferentOriginAsync(cors);
    if (setting == CorsSetting.None)
      return loader.FetchWithoutCorsAsync(request, cors.Behavior);
    throw new DomException(DomError.Network);
  }

  private static IDownload FetchFromSameOriginAsync(
    this IResourceLoader loader,
    Url url,
    CorsRequest cors)
  {
    ResourceRequest request = cors.Request;
    IDownload download = loader.FetchAsync(new ResourceRequest(request.Source, url)
    {
      Origin = request.Origin,
      IsManualRedirectDesired = true
    });
    return download.Wrap((Func<IResponse, IDownload>) (response =>
    {
      if (!response.IsRedirected())
        return cors.CheckIntegrity(download);
      url.Href = response.Headers.GetOrDefault<string, string>(HeaderNames.Location, url.Href);
      return !request.Origin.Is(url.Origin) ? loader.FetchFromSameOriginAsync(url, cors) : loader.FetchWithCorsAsync(cors.RedirectTo(url));
    }));
  }

  private static IDownload FetchFromDifferentOriginAsync(
    this IResourceLoader loader,
    CorsRequest cors)
  {
    ResourceRequest request = cors.Request;
    request.IsCredentialOmitted = cors.IsAnonymous();
    IDownload download = loader.FetchAsync(request);
    return download.Wrap((Func<IResponse, IDownload>) (response =>
    {
      if ((response != null ? (response.StatusCode != HttpStatusCode.OK ? 1 : 0) : 1) != 0)
      {
        response?.Dispose();
        throw new DomException(DomError.Network);
      }
      return cors.CheckIntegrity(download);
    }));
  }

  private static IDownload FetchWithoutCorsAsync(
    this IResourceLoader loader,
    ResourceRequest request,
    OriginBehavior behavior)
  {
    if (behavior == OriginBehavior.Fail)
      throw new DomException(DomError.Network);
    return loader.FetchAsync(request);
  }

  private static bool IsAnonymous(this CorsRequest cors) => cors.Setting == CorsSetting.Anonymous;

  private static IDownload Wrap(this IDownload download, Func<IResponse, IDownload> callback)
  {
    CancellationTokenSource cts = new CancellationTokenSource();
    return (IDownload) new Download(download.Task.Wrap(callback), cts, download.Target, download.Source);
  }

  private static IDownload Wrap(this IDownload download, IResponse response)
  {
    CancellationTokenSource cts = new CancellationTokenSource();
    return (IDownload) new Download(Task.FromResult<IResponse>(response), cts, download.Target, download.Source);
  }

  private static async Task<IResponse> Wrap(
    this Task<IResponse> task,
    Func<IResponse, IDownload> callback)
  {
    return await callback(await task.ConfigureAwait(false)).Task.ConfigureAwait(false);
  }

  private static bool IsRedirected(this IResponse response)
  {
    return (response != null ? response.StatusCode : HttpStatusCode.NotFound).IsRedirected();
  }

  private static CorsRequest RedirectTo(this CorsRequest cors, Url url)
  {
    ResourceRequest request = cors.Request;
    return new CorsRequest(new ResourceRequest(request.Source, url)
    {
      IsCookieBlocked = request.IsCookieBlocked,
      IsSameOriginForced = request.IsSameOriginForced,
      Origin = request.Origin
    })
    {
      Setting = cors.Setting,
      Behavior = cors.Behavior,
      Integrity = cors.Integrity
    };
  }

  private static IDownload CheckIntegrity(this CorsRequest cors, IDownload download)
  {
    IResponse result = download.Task.Result;
    string attribute = cors.Request.Source?.GetAttribute(AttributeNames.Integrity);
    IIntegrityProvider integrity = cors.Integrity;
    if (string.IsNullOrEmpty(attribute) || integrity == null || result == null)
      return download;
    MemoryStream destination = new MemoryStream();
    result.Content.CopyTo((Stream) destination);
    destination.Position = 0L;
    if (!integrity.IsSatisfied(destination.ToArray(), attribute))
    {
      result.Dispose();
      throw new DomException(DomError.Security);
    }
    return download.Wrap((IResponse) new DefaultResponse()
    {
      Address = result.Address,
      Content = (Stream) destination,
      Headers = result.Headers,
      StatusCode = result.StatusCode
    });
  }
}
