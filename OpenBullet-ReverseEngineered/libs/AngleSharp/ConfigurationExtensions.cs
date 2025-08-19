// Decompiled with JetBrains decompiler
// Type: AngleSharp.ConfigurationExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Common;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace AngleSharp;

public static class ConfigurationExtensions
{
  public static IConfiguration With(this IConfiguration configuration, object service)
  {
    configuration = configuration ?? throw new ArgumentNullException(nameof (configuration));
    service = service ?? throw new ArgumentNullException(nameof (service));
    return (IConfiguration) new Configuration(configuration.Services.Concat<object>(service));
  }

  public static IConfiguration WithOnly<TService>(
    this IConfiguration configuration,
    TService service)
  {
    return (object) service != null ? configuration.Without<TService>().With((object) service) : throw new ArgumentNullException(nameof (service));
  }

  public static IConfiguration WithOnly<TService>(
    this IConfiguration configuration,
    Func<IBrowsingContext, TService> creator)
  {
    creator = creator ?? throw new ArgumentNullException(nameof (creator));
    return configuration.Without<TService>().With<TService>(creator);
  }

  public static IConfiguration Without(this IConfiguration configuration, object service)
  {
    configuration = configuration ?? throw new ArgumentNullException(nameof (configuration));
    service = service ?? throw new ArgumentNullException(nameof (service));
    return (IConfiguration) new Configuration(configuration.Services.Except<object>(service));
  }

  public static IConfiguration With(this IConfiguration configuration, IEnumerable<object> services)
  {
    configuration = configuration ?? throw new ArgumentNullException(nameof (configuration));
    services = services ?? throw new ArgumentNullException(nameof (services));
    return (IConfiguration) new Configuration(services.Concat<object>(configuration.Services));
  }

  public static IConfiguration Without(
    this IConfiguration configuration,
    IEnumerable<object> services)
  {
    configuration = configuration ?? throw new ArgumentNullException(nameof (configuration));
    services = services ?? throw new ArgumentNullException(nameof (services));
    return (IConfiguration) new Configuration(configuration.Services.Except<object>(services));
  }

  public static IConfiguration With<TService>(
    this IConfiguration configuration,
    Func<IBrowsingContext, TService> creator)
  {
    creator = creator ?? throw new ArgumentNullException(nameof (creator));
    return configuration.With((object) creator);
  }

  public static IConfiguration Without<TService>(this IConfiguration configuration)
  {
    configuration = configuration ?? throw new ArgumentNullException(nameof (configuration));
    IEnumerable<object> services1 = configuration.Services.OfType<TService>().Cast<object>();
    IEnumerable<Func<IBrowsingContext, TService>> services2 = configuration.Services.OfType<Func<IBrowsingContext, TService>>();
    return ConfigurationExtensions.Without(ConfigurationExtensions.Without(configuration, services1), (IEnumerable<object>) services2);
  }

  public static bool Has<TService>(this IConfiguration configuration)
  {
    configuration = configuration ?? throw new ArgumentNullException(nameof (configuration));
    return configuration.Services.OfType<TService>().Any<TService>() || configuration.Services.OfType<Func<IBrowsingContext, TService>>().Any<Func<IBrowsingContext, TService>>();
  }

  public static IConfiguration WithDefaultLoader(
    this IConfiguration configuration,
    LoaderOptions setup = null)
  {
    LoaderOptions config = setup ?? new LoaderOptions();
    if (!configuration.Has<IRequester>())
      configuration = configuration.With((object) new DefaultHttpRequester());
    if (!config.IsNavigationDisabled)
      configuration = configuration.With<IDocumentLoader>((Func<IBrowsingContext, IDocumentLoader>) (ctx => (IDocumentLoader) new DefaultDocumentLoader(ctx, config.Filter)));
    if (config.IsResourceLoadingEnabled)
      configuration = configuration.With<IResourceLoader>((Func<IBrowsingContext, IResourceLoader>) (ctx => (IResourceLoader) new DefaultResourceLoader(ctx, config.Filter)));
    return configuration;
  }

  public static IConfiguration WithCulture(this IConfiguration configuration, string name)
  {
    CultureInfo culture = new CultureInfo(name);
    return configuration.WithCulture(culture);
  }

  public static IConfiguration WithCulture(this IConfiguration configuration, CultureInfo culture)
  {
    return configuration.With((object) culture);
  }

  public static IConfiguration WithMetaRefresh(
    this IConfiguration configuration,
    Predicate<Url> shouldRefresh = null)
  {
    RefreshMetaHandler service = new RefreshMetaHandler(shouldRefresh);
    return configuration.With((object) service);
  }

  public static IConfiguration WithLocaleBasedEncoding(this IConfiguration configuration)
  {
    LocaleEncodingProvider service = new LocaleEncodingProvider();
    return configuration.With((object) service);
  }

  public static IConfiguration WithDefaultCookies(this IConfiguration configuration)
  {
    MemoryCookieProvider service = new MemoryCookieProvider();
    return configuration.With((object) service);
  }
}
