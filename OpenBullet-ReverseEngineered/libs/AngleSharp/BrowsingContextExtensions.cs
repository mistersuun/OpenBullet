// Decompiled with JetBrains decompiler
// Type: AngleSharp.BrowsingContextExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Browser.Dom.Events;
using AngleSharp.Css;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Media;
using AngleSharp.Scripting;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp;

public static class BrowsingContextExtensions
{
  public static Task<IDocument> OpenNewAsync(
    this IBrowsingContext context,
    string url = null,
    CancellationToken cancellation = default (CancellationToken))
  {
    return context.OpenAsync((Action<VirtualResponse>) (m => m.Address(url ?? "http://localhost/")), cancellation);
  }

  public static Task<IDocument> OpenAsync(
    this IBrowsingContext context,
    IResponse response,
    CancellationToken cancel = default (CancellationToken))
  {
    response = response ?? throw new ArgumentNullException(nameof (response));
    context = context ?? BrowsingContext.New();
    Encoding defaultEncoding = context.GetDefaultEncoding();
    IDocumentFactory factory = context.GetFactory<IDocumentFactory>();
    CreateDocumentOptions createDocumentOptions = new CreateDocumentOptions(response, defaultEncoding);
    IBrowsingContext context1 = context;
    CreateDocumentOptions options = createDocumentOptions;
    CancellationToken cancellationToken = cancel;
    return factory.CreateAsync(context1, options, cancellationToken);
  }

  public static Task<IDocument> OpenAsync(
    this IBrowsingContext context,
    DocumentRequest request,
    CancellationToken cancel = default (CancellationToken))
  {
    request = request ?? throw new ArgumentNullException(nameof (request));
    context = context ?? BrowsingContext.New();
    return context.NavigateToAsync(request);
  }

  public static Task<IDocument> OpenAsync(
    this IBrowsingContext context,
    Url url,
    CancellationToken cancel = default (CancellationToken))
  {
    url = url ?? throw new ArgumentNullException(nameof (url));
    return context.OpenAsync(DocumentRequest.Get(url, referer: context?.Active?.DocumentUri), cancel);
  }

  public static async Task<IDocument> OpenAsync(
    this IBrowsingContext context,
    Action<VirtualResponse> request,
    CancellationToken cancel = default (CancellationToken))
  {
    request = request ?? throw new ArgumentNullException(nameof (request));
    IDocument document;
    using (IResponse response = VirtualResponse.Create(request))
      document = await context.OpenAsync(response, cancel).ConfigureAwait(false);
    return document;
  }

  public static Task<IDocument> OpenAsync(
    this IBrowsingContext context,
    string address,
    CancellationToken cancellation = default (CancellationToken))
  {
    address = address ?? throw new ArgumentNullException(nameof (address));
    return context.OpenAsync(Url.Create(address), cancellation);
  }

  internal static Task<IDocument> NavigateToAsync(
    this IBrowsingContext context,
    DocumentRequest request,
    CancellationToken cancel = default (CancellationToken))
  {
    return context.GetNavigationHandler(request.Target)?.NavigateAsync(request, cancel) ?? Task.FromResult<IDocument>((IDocument) null);
  }

  public static void NavigateTo(this IBrowsingContext context, IDocument document)
  {
    context.SessionHistory?.PushState((object) document, document.Title, document.Url);
    context.Active = document;
  }

  public static INavigationHandler GetNavigationHandler(this IBrowsingContext context, Url url)
  {
    return context.GetServices<INavigationHandler>().FirstOrDefault<INavigationHandler>((Func<INavigationHandler, bool>) (m => m.SupportsProtocol(url.Scheme)));
  }

  public static Encoding GetDefaultEncoding(this IBrowsingContext context)
  {
    return context.GetProvider<IEncodingProvider>()?.Suggest(context.GetLanguage()) ?? Encoding.UTF8;
  }

  public static CultureInfo GetCulture(this IBrowsingContext context)
  {
    return context.GetService<CultureInfo>() ?? CultureInfo.CurrentUICulture;
  }

  public static CultureInfo GetCultureFrom(this IBrowsingContext context, string language)
  {
    try
    {
      return new CultureInfo(language);
    }
    catch (CultureNotFoundException ex)
    {
      return context.GetCulture();
    }
  }

  public static string GetLanguage(this IBrowsingContext context) => context.GetCulture().Name;

  public static TFactory GetFactory<TFactory>(this IBrowsingContext context) where TFactory : class
  {
    return context.GetServices<TFactory>().Single<TFactory>();
  }

  public static TProvider GetProvider<TProvider>(this IBrowsingContext context) where TProvider : class
  {
    return context.GetServices<TProvider>().SingleOrDefault<TProvider>();
  }

  public static IResourceService<TResource> GetResourceService<TResource>(
    this IBrowsingContext context,
    string type)
    where TResource : IResourceInfo
  {
    foreach (IResourceService<TResource> service in context.GetServices<IResourceService<TResource>>())
    {
      if (service.SupportsType(type))
        return service;
    }
    return (IResourceService<TResource>) null;
  }

  public static string GetCookie(this IBrowsingContext context, Url url)
  {
    return context.GetProvider<ICookieProvider>()?.GetCookie(url) ?? string.Empty;
  }

  public static void SetCookie(this IBrowsingContext context, Url url, string value)
  {
    context.GetProvider<ICookieProvider>()?.SetCookie(url, value);
  }

  public static ISpellCheckService GetSpellCheck(this IBrowsingContext context, string language)
  {
    ISpellCheckService spellCheck1 = (ISpellCheckService) null;
    IEnumerable<ISpellCheckService> services = context.GetServices<ISpellCheckService>();
    CultureInfo cultureFrom = context.GetCultureFrom(language);
    string letterIsoLanguageName1 = cultureFrom.TwoLetterISOLanguageName;
    foreach (ISpellCheckService spellCheck2 in services)
    {
      CultureInfo culture = spellCheck2.Culture;
      if (culture != null)
      {
        string letterIsoLanguageName2 = culture.TwoLetterISOLanguageName;
        if (culture.Equals((object) cultureFrom))
          return spellCheck2;
        if (spellCheck1 == null && letterIsoLanguageName2.Is(letterIsoLanguageName1))
          spellCheck1 = spellCheck2;
      }
    }
    return spellCheck1;
  }

  public static IStylingService GetCssStyling(this IBrowsingContext context)
  {
    return context.GetStyling(MimeTypeNames.Css);
  }

  public static IStylingService GetStyling(this IBrowsingContext context, string type)
  {
    foreach (IStylingService service in context.GetServices<IStylingService>())
    {
      if (service.SupportsType(type))
        return service;
    }
    return (IStylingService) null;
  }

  public static bool IsScripting(this IBrowsingContext context)
  {
    return context.GetServices<IScriptingService>().Any<IScriptingService>();
  }

  public static IScriptingService GetJsScripting(this IBrowsingContext context)
  {
    return context.GetScripting(MimeTypeNames.DefaultJavaScript);
  }

  public static IScriptingService GetScripting(this IBrowsingContext context, string type)
  {
    foreach (IScriptingService service in context.GetServices<IScriptingService>())
    {
      if (service.SupportsType(type))
        return service;
    }
    return (IScriptingService) null;
  }

  public static ICommand GetCommand(this IBrowsingContext context, string commandId)
  {
    return context.GetProvider<ICommandProvider>()?.GetCommand(commandId);
  }

  public static Task InteractAsync<T>(this IBrowsingContext context, string eventName, T data)
  {
    InteractivityEvent<T> eventData = new InteractivityEvent<T>(eventName, data);
    context.Fire((Event) eventData);
    return eventData.Result ?? (Task) Task.FromResult<bool>(false);
  }

  public static IBrowsingContext ResolveTargetContext(this IBrowsingContext context, string target)
  {
    bool flag = false;
    IBrowsingContext browsingContext = context;
    if (!string.IsNullOrEmpty(target))
    {
      browsingContext = context.FindChildFor(target);
      flag = browsingContext == null;
    }
    if (flag)
      browsingContext = context.CreateChildFor(target);
    return browsingContext;
  }

  public static IBrowsingContext CreateChildFor(this IBrowsingContext context, string target)
  {
    Sandboxes security = Sandboxes.None;
    if (target.Is("_blank"))
      target = (string) null;
    return context.CreateChild(target, security);
  }

  public static IBrowsingContext FindChildFor(this IBrowsingContext context, string target)
  {
    if (string.IsNullOrEmpty(target) || target.Is("_self"))
      return context;
    if (target.Is("_parent"))
      return context.Parent ?? context;
    return target.Is("_top") ? context : context.FindChild(target);
  }

  public static IEnumerable<Task> GetDownloads<T>(this IBrowsingContext context) where T : INode
  {
    IResourceLoader service = context.GetService<IResourceLoader>();
    return service == null ? Enumerable.Empty<Task>() : (IEnumerable<Task>) service.GetDownloads().Where<IDownload>((Func<IDownload, bool>) (m => m.Source is T)).Select<IDownload, Task<IResponse>>((Func<IDownload, Task<IResponse>>) (m => m.Task));
  }
}
