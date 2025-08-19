// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlMarqueeElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomHistorical]
internal sealed class HtmlMarqueeElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Marquee, prefix, NodeFlags.Special | NodeFlags.Scoped),
  IHtmlMarqueeElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  public int MinimumDelay { get; private set; }

  public int ScrollAmount { get; set; }

  public int ScrollDelay { get; set; }

  public int Loop { get; set; }

  public void Start()
  {
    this.Owner.QueueTask((Action) (() => this.FireSimpleEvent(EventNames.Play)));
  }

  public void Stop()
  {
    this.Owner.QueueTask((Action) (() => this.FireSimpleEvent(EventNames.Pause)));
  }
}
