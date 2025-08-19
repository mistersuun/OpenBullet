// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.StyleOptions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Css;

public sealed class StyleOptions
{
  public StyleOptions(IDocument document) => this.Document = document;

  public IDocument Document { get; }

  public IElement Element { get; set; }

  public bool IsDisabled { get; set; }

  public bool IsAlternate { get; set; }
}
