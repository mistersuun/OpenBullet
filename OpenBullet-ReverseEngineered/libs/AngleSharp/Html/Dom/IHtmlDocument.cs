// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlDocument
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLDocument")]
public interface IHtmlDocument : 
  IDocument,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IGlobalEventHandlers,
  IDocumentStyle,
  INonElementParentNode,
  IDisposable
{
}
