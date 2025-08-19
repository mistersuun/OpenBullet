// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Cloudflare.CloudflareBypass
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using Leaf.xNet.Services.Captcha;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

#nullable disable
namespace Leaf.xNet.Services.Cloudflare;

public static class CloudflareBypass
{
  public const string CfClearanceCookie = "cf_clearance";
  private const string LogPrefix = "[Cloudflare] ";

  public static string DefaultAcceptLanguage { get; set; } = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";

  public static int MaxRetries { get; set; } = 4;

  public static int DelayMilliseconds { get; set; } = 5000;

  public static bool IsCloudflared(this HttpResponse response)
  {
    return ((response.StatusCode == Leaf.xNet.HttpStatusCode.ServiceUnavailable ? 1 : (response.StatusCode == Leaf.xNet.HttpStatusCode.Forbidden ? 1 : 0)) & (response[HttpHeader.Server].IndexOf("cloudflare", StringComparison.OrdinalIgnoreCase) != -1 ? 1 : 0)) != 0;
  }

  public static HttpResponse GetThroughCloudflare(
    this HttpRequest request,
    Uri uri,
    CloudflareBypass.DLog log = null,
    CancellationToken cancellationToken = default (CancellationToken),
    ICaptchaSolver captchaSolver = null)
  {
    if (!request.UseCookies)
      throw new CloudflareException("[Cloudflare] Cookies must be enabled. Please set $UseCookies to true.");
    if (string.IsNullOrEmpty(request.UserAgent))
      request.UserAgent = Leaf.xNet.Http.ChromeUserAgent();
    if (log != null)
      log($"[Cloudflare] Checking availability at: {uri.AbsoluteUri} ...");
    int num = 0;
    if (num >= CloudflareBypass.MaxRetries)
      throw new CloudflareException(CloudflareBypass.MaxRetries, "[Cloudflare] ERROR. Rate limit reached.");
    string retry = $". Retry {num + 1} / {CloudflareBypass.MaxRetries}.";
    if (log != null)
      log("[Cloudflare] Trying to bypass" + retry);
    HttpResponse response = request.ManualGet(uri);
    if (!response.IsCloudflared())
    {
      if (log != null)
        log("[Cloudflare]  OK. Not found at: " + uri.AbsoluteUri);
      return response;
    }
    foreach (Cookie cookie in request.Cookies.GetCookies(uri))
    {
      if (!(cookie.Name != "cf_clearance"))
      {
        cookie.Expired = true;
        break;
      }
    }
    if (cancellationToken != new CancellationToken())
      cancellationToken.ThrowIfCancellationRequested();
    if (CloudflareBypass.HasJsChallenge(response))
      CloudflareBypass.SolveJsChallenge(ref response, request, uri, retry, log, cancellationToken);
    if (CloudflareBypass.HasRecaptchaChallenge(response))
      CloudflareBypass.SolveRecaptchaChallenge(ref response, request, uri, retry, log, cancellationToken);
    return !response.IsCloudflared() ? response : throw new CloudflareException(CloudflareBypass.HasAccessDeniedError(response) ? "Access denied. Try to use another IP address." : "Unknown challenge type");
  }

  public static HttpResponse GetThroughCloudflare(
    this HttpRequest request,
    string url,
    CloudflareBypass.DLog log = null,
    CancellationToken cancellationToken = default (CancellationToken),
    ICaptchaSolver captchaSolver = null)
  {
    Uri uri = !(request.BaseAddress != (Uri) null) || !url.StartsWith("/") ? new Uri(url) : new Uri(request.BaseAddress, url);
    return request.GetThroughCloudflare(uri, log, cancellationToken, captchaSolver);
  }

  private static bool IsChallengePassed(
    string tag,
    ref HttpResponse response,
    HttpRequest request,
    Uri uri,
    string retry,
    CloudflareBypass.DLog log)
  {
    switch (response.StatusCode)
    {
      case Leaf.xNet.HttpStatusCode.Found:
        if (response.HasRedirect)
        {
          if (!response.ContainsCookie(uri, "cf_clearance"))
            return false;
          if (log != null)
            log($"[Cloudflare] Passed [{tag}]. Trying to get the original response at: {uri.AbsoluteUri} ...");
          bool ignoreProtocolErrors = request.IgnoreProtocolErrors;
          request.IgnoreProtocolErrors = true;
          request.AddCloudflareHeaders(uri);
          response = request.Get(response.RedirectAddress.AbsoluteUri);
          request.IgnoreProtocolErrors = ignoreProtocolErrors;
          if (response.IsCloudflared())
          {
            if (log != null)
              log($"[Cloudflare] ERROR [{tag}]. Unable to get he original response at: {uri.AbsoluteUri}");
            return false;
          }
        }
        if (log != null)
          log($"[Cloudflare] OK [{tag}]. Done: {uri.AbsoluteUri}");
        return true;
      case Leaf.xNet.HttpStatusCode.Forbidden:
      case Leaf.xNet.HttpStatusCode.ServiceUnavailable:
        return false;
      default:
        if (log != null)
          log($"{"[Cloudflare] "}ERROR [{tag}]. Status code : {response.StatusCode}{retry}.");
        return false;
    }
  }

  private static bool HasJsChallenge(HttpResponse response)
  {
    return response.ToString().ContainsInsensitive("jschl-answer");
  }

  private static bool SolveJsChallenge(
    ref HttpResponse response,
    HttpRequest request,
    Uri uri,
    string retry,
    CloudflareBypass.DLog log,
    CancellationToken cancellationToken)
  {
    if (log != null)
      log($"[Cloudflare] Solving JS Challenge for URL: {uri.AbsoluteUri} ...");
    response = CloudflareBypass.PassClearance(request, response, uri, log, cancellationToken);
    return CloudflareBypass.IsChallengePassed("JS", ref response, request, uri, retry, log);
  }

  private static Uri GetSolutionUri(HttpResponse response)
  {
    string challengePageContent = response.ToString();
    string scheme = response.Address.Scheme;
    string host = response.Address.Host;
    int port = response.Address.Port;
    string targetHost = host;
    int targetPort = port;
    ChallengeSolution challengeSolution = ChallengeSolver.Solve(challengePageContent, targetHost, targetPort);
    return new Uri($"{scheme}://{host}:{port}{challengeSolution.ClearanceQuery}");
  }

  private static HttpResponse PassClearance(
    HttpRequest request,
    HttpResponse response,
    Uri refererUri,
    CloudflareBypass.DLog log,
    CancellationToken cancellationToken)
  {
    Uri solutionUri = CloudflareBypass.GetSolutionUri(response);
    CloudflareBypass.Delay(CloudflareBypass.DelayMilliseconds, log, cancellationToken);
    return request.ManualGet(solutionUri, refererUri);
  }

  private static bool HasRecaptchaChallenge(HttpResponse response)
  {
    return response.ToString().IndexOf("<div class=\"g-recaptcha\">", StringComparison.OrdinalIgnoreCase) != -1;
  }

  private static bool SolveRecaptchaChallenge(
    ref HttpResponse response,
    HttpRequest request,
    Uri uri,
    string retry,
    CloudflareBypass.DLog log,
    CancellationToken cancelToken)
  {
    if (log != null)
      log($"[Cloudflare] Solving Recaptcha Challenge for URL: {uri.AbsoluteUri} ...");
    if (request.CaptchaSolver == null)
      throw new CloudflareException("CaptchaSolver required");
    string self = response.ToString();
    string siteKey = self.Substring("data-sitekey=\"", "\"") ?? throw new CloudflareException("Value of \"data-sitekey\" not found");
    string str1 = self.Substring("name=\"s\" value=\"", "\"") ?? throw new CloudflareException("Value of \"s\" not found");
    string str2 = self.Substring("data-ray=\"", "\"") ?? throw new CloudflareException("Ray Id not found");
    string str3 = request.CaptchaSolver.SolveRecaptcha(uri.AbsoluteUri, siteKey, cancelToken);
    cancelToken.ThrowIfCancellationRequested();
    RequestParams requestParams = new RequestParams()
    {
      ["s"] = (object) str1,
      ["id"] = (object) str2,
      ["g-recaptcha-response"] = (object) str3
    };
    string str4 = self.Substring("'bf_challenge_id', '", "'");
    if (str4 != null)
    {
      requestParams.Add(new KeyValuePair<string, string>("bf_challenge_id", str4));
      requestParams.Add(new KeyValuePair<string, string>("bf_execution_time", "4"));
      requestParams.Add(new KeyValuePair<string, string>("bf_result_hash", string.Empty));
    }
    response = request.ManualGet(new Uri(uri, "/cdn-cgi/l/chk_captcha"), uri, requestParams);
    return CloudflareBypass.IsChallengePassed("ReCaptcha", ref response, request, uri, retry, log);
  }

  private static bool HasAccessDeniedError(HttpResponse response)
  {
    string self1 = response.ToString();
    string self2 = self1.Substring("class=\"cf-subheadline\">", "<") ?? self1.Substring("<title>", "</title>");
    return !string.IsNullOrEmpty(self2) && self2.ContainsInsensitive("Access denied");
  }

  private static HttpResponse ManualGet(
    this HttpRequest request,
    Uri uri,
    Uri refererUri = null,
    RequestParams requestParams = null)
  {
    request.ManualMode = true;
    HttpRequest request1 = request;
    Uri refererUri1 = refererUri;
    if ((object) refererUri1 == null)
      refererUri1 = uri;
    request1.AddCloudflareHeaders(refererUri1);
    HttpResponse httpResponse = requestParams == null ? request.Get(uri) : request.Get(uri, requestParams);
    request.ManualMode = false;
    return httpResponse;
  }

  private static void AddCloudflareHeaders(this HttpRequest request, Uri refererUri)
  {
    request.AddHeader(HttpHeader.Referer, refererUri.AbsoluteUri);
    request.AddHeader(HttpHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
    request.AddHeader("Upgrade-Insecure-Requests", "1");
    if (request.ContainsHeader(HttpHeader.AcceptLanguage))
      return;
    request.AddHeader(HttpHeader.AcceptLanguage, CloudflareBypass.DefaultAcceptLanguage);
  }

  private static void Delay(
    int milliseconds,
    CloudflareBypass.DLog log,
    CancellationToken cancellationToken)
  {
    if (log != null)
      log($"{"[Cloudflare] "}: delay {milliseconds} ms...");
    if (cancellationToken == new CancellationToken())
    {
      Thread.Sleep(milliseconds);
    }
    else
    {
      cancellationToken.WaitHandle.WaitOne(milliseconds);
      cancellationToken.ThrowIfCancellationRequested();
    }
  }

  public delegate void DLog(string message);
}
