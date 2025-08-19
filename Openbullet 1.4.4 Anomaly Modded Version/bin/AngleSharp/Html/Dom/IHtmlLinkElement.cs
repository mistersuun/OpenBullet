// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlLinkElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLLinkElement")]
public interface IHtmlLinkElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILinkStyle,
  ILinkImport,
  ILoadableElement
{
  [DomName("disabled")]
  bool IsDisabled { get; set; }

  [DomName("href")]
  string Href { get; set; }

  [DomName("rel")]
  string Relation { get; set; }

  [DomName("rev")]
  string ReverseRelation { get; set; }

  [DomName("relList")]
  ITokenList RelationList { get; }

  [DomName("media")]
  string Media { get; set; }

  [DomName("hreflang")]
  string TargetLanguage { get; set; }

  [DomName("type")]
  string Type { get; set; }

  [DomName("sizes")]
  ISettableTokenList Sizes { get; }

  [DomName("integrity")]
  string Integrity { get; set; }

  [DomName("crossOrigin")]
  string CrossOrigin { get; set; }

  [DomName("nonce")]
  string NumberUsedOnce { get; set; }
}
