// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlMeterElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLMeterElement")]
public interface IHtmlMeterElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILabelabelElement
{
  [DomName("value")]
  double Value { get; set; }

  [DomName("min")]
  double Minimum { get; set; }

  [DomName("max")]
  double Maximum { get; set; }

  [DomName("low")]
  double Low { get; set; }

  [DomName("high")]
  double High { get; set; }

  [DomName("optimum")]
  double Optimum { get; set; }
}
