// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlStyleElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLStyleElement")]
public interface IHtmlStyleElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILinkStyle
{
  [DomName("disabled")]
  bool IsDisabled { get; set; }

  [DomName("media")]
  string Media { get; set; }

  [DomName("type")]
  string Type { get; set; }

  [DomName("scoped")]
  bool IsScoped { get; set; }
}
