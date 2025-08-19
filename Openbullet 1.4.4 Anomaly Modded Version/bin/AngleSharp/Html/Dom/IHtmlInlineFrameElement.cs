// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlInlineFrameElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLIFrameElement")]
public interface IHtmlInlineFrameElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILoadableElement
{
  [DomName("src")]
  string Source { get; set; }

  [DomName("srcdoc")]
  string ContentHtml { get; set; }

  [DomName("name")]
  string Name { get; set; }

  [DomName("sandbox")]
  ISettableTokenList Sandbox { get; }

  [DomName("seamless")]
  bool IsSeamless { get; set; }

  [DomName("allowFullscreen")]
  bool IsFullscreenAllowed { get; set; }

  [DomName("allowPaymentRequest")]
  bool IsPaymentRequestAllowed { get; set; }

  [DomName("referrerPolicy")]
  string ReferrerPolicy { get; set; }

  [DomName("width")]
  int DisplayWidth { get; set; }

  [DomName("height")]
  int DisplayHeight { get; set; }

  [DomName("contentDocument")]
  IDocument ContentDocument { get; }

  [DomName("contentWindow")]
  IWindow ContentWindow { get; }
}
