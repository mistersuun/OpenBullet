// Decompiled with JetBrains decompiler
// Type: AngleSharp.Configuration
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Css;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Mathml;
using AngleSharp.Mathml.Dom;
using AngleSharp.Svg;
using AngleSharp.Svg.Dom;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp;

public class Configuration : IConfiguration
{
  private readonly IEnumerable<object> _services;

  private static T Instance<T>(T instance) => instance;

  private static Func<IBrowsingContext, T> Creator<T>(Func<IBrowsingContext, T> creator) => creator;

  public Configuration(IEnumerable<object> services = null)
  {
    object obj = (object) services;
    if (obj == null)
      obj = (object) new object[15]
      {
        (object) Configuration.Instance<IElementFactory<Document, HtmlElement>>((IElementFactory<Document, HtmlElement>) new HtmlElementFactory()),
        (object) Configuration.Instance<IElementFactory<Document, MathElement>>((IElementFactory<Document, MathElement>) new MathElementFactory()),
        (object) Configuration.Instance<IElementFactory<Document, SvgElement>>((IElementFactory<Document, SvgElement>) new SvgElementFactory()),
        (object) Configuration.Instance<IEventFactory>((IEventFactory) new DefaultEventFactory()),
        (object) Configuration.Instance<IInputTypeFactory>((IInputTypeFactory) new DefaultInputTypeFactory()),
        (object) Configuration.Instance<IAttributeSelectorFactory>((IAttributeSelectorFactory) new DefaultAttributeSelectorFactory()),
        (object) Configuration.Instance<IPseudoElementSelectorFactory>((IPseudoElementSelectorFactory) new DefaultPseudoElementSelectorFactory()),
        (object) Configuration.Instance<IPseudoClassSelectorFactory>((IPseudoClassSelectorFactory) new DefaultPseudoClassSelectorFactory()),
        (object) Configuration.Instance<ILinkRelationFactory>((ILinkRelationFactory) new DefaultLinkRelationFactory()),
        (object) Configuration.Instance<IDocumentFactory>((IDocumentFactory) new DefaultDocumentFactory()),
        (object) Configuration.Instance<IAttributeObserver>((IAttributeObserver) new DefaultAttributeObserver()),
        (object) Configuration.Instance<IMetaHandler>((IMetaHandler) new EncodingMetaHandler()),
        (object) Configuration.Creator<ICssSelectorParser>((Func<IBrowsingContext, ICssSelectorParser>) (ctx => (ICssSelectorParser) new CssSelectorParser(ctx))),
        (object) Configuration.Creator<IHtmlParser>((Func<IBrowsingContext, IHtmlParser>) (ctx => (IHtmlParser) new HtmlParser(ctx))),
        (object) Configuration.Creator<INavigationHandler>((Func<IBrowsingContext, INavigationHandler>) (ctx => (INavigationHandler) new DefaultNavigationHandler(ctx)))
      };
    this._services = (IEnumerable<object>) obj;
  }

  public static IConfiguration Default => (IConfiguration) new Configuration();

  public IEnumerable<object> Services => this._services;
}
