// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlDialogElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLDialogElement")]
public interface IHtmlDialogElement : 
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
  [DomName("open")]
  bool Open { get; set; }

  [DomName("returnValue")]
  string ReturnValue { get; set; }

  [DomName("show")]
  void Show(IElement anchor = null);

  [DomName("showModal")]
  void ShowModal(IElement anchor = null);

  [DomName("close")]
  void Close(string returnValue = null);
}
