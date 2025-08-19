// Decompiled with JetBrains decompiler
// Type: AngleSharp.Svg.SvgElementFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Svg.Dom;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Svg;

internal sealed class SvgElementFactory : IElementFactory<Document, SvgElement>
{
  private readonly Dictionary<string, SvgElementFactory.Creator> creators = new Dictionary<string, SvgElementFactory.Creator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      TagNames.Svg,
      (SvgElementFactory.Creator) ((document, prefix) => (SvgElement) new SvgSvgElement(document, prefix))
    },
    {
      TagNames.Circle,
      (SvgElementFactory.Creator) ((document, prefix) => (SvgElement) new SvgCircleElement(document, prefix))
    },
    {
      TagNames.Desc,
      (SvgElementFactory.Creator) ((document, prefix) => (SvgElement) new SvgDescElement(document, prefix))
    },
    {
      TagNames.ForeignObject,
      (SvgElementFactory.Creator) ((document, prefix) => (SvgElement) new SvgForeignObjectElement(document, prefix))
    },
    {
      TagNames.Title,
      (SvgElementFactory.Creator) ((document, prefix) => (SvgElement) new SvgTitleElement(document, prefix))
    }
  };

  public SvgElement Create(Document document, string localName, string prefix = null)
  {
    SvgElementFactory.Creator creator;
    return this.creators.TryGetValue(localName, out creator) ? creator(document, prefix) : new SvgElement(document, localName);
  }

  private delegate SvgElement Creator(Document owner, string prefix);
}
