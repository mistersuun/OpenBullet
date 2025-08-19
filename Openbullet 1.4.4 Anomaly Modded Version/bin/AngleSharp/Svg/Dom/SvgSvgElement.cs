// Decompiled with JetBrains decompiler
// Type: AngleSharp.Svg.Dom.SvgSvgElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Svg.Dom;

internal sealed class SvgSvgElement(Document owner, string prefix = null) : 
  SvgElement(owner, TagNames.Svg, prefix),
  ISvgSvgElement,
  ISvgElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode
{
}
