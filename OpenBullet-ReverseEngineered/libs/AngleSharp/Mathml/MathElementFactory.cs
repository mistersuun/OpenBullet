// Decompiled with JetBrains decompiler
// Type: AngleSharp.Mathml.MathElementFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Mathml.Dom;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Mathml;

internal sealed class MathElementFactory : IElementFactory<Document, MathElement>
{
  private readonly Dictionary<string, MathElementFactory.Creator> creators = new Dictionary<string, MathElementFactory.Creator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      TagNames.Mn,
      (MathElementFactory.Creator) ((document, prefix) => (MathElement) new MathNumberElement(document, prefix))
    },
    {
      TagNames.Mo,
      (MathElementFactory.Creator) ((document, prefix) => (MathElement) new MathOperatorElement(document, prefix))
    },
    {
      TagNames.Mi,
      (MathElementFactory.Creator) ((document, prefix) => (MathElement) new MathIdentifierElement(document, prefix))
    },
    {
      TagNames.Ms,
      (MathElementFactory.Creator) ((document, prefix) => (MathElement) new MathStringElement(document, prefix))
    },
    {
      TagNames.Mtext,
      (MathElementFactory.Creator) ((document, prefix) => (MathElement) new MathTextElement(document, prefix))
    },
    {
      TagNames.AnnotationXml,
      (MathElementFactory.Creator) ((document, prefix) => (MathElement) new MathAnnotationXmlElement(document, prefix))
    }
  };

  public MathElement Create(Document document, string localName, string prefix = null)
  {
    MathElementFactory.Creator creator;
    return this.creators.TryGetValue(localName, out creator) ? creator(document, prefix) : new MathElement(document, localName, prefix);
  }

  private delegate MathElement Creator(Document owner, string prefix);
}
