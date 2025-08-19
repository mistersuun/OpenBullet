// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlCanvasElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Media.Dom;
using System;
using System.IO;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLCanvasElement")]
public interface IHtmlCanvasElement : 
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
  [DomName("width")]
  int Width { get; set; }

  [DomName("height")]
  int Height { get; set; }

  [DomName("toDataURL")]
  string ToDataUrl(string type = null);

  [DomName("toBlob")]
  void ToBlob(Action<Stream> callback, string type = null);

  [DomName("getContext")]
  IRenderingContext GetContext(string contextId);

  [DomName("setContext")]
  void SetContext(IRenderingContext context);

  [DomName("probablySupportsContext")]
  bool IsSupportingContext(string contextId);
}
