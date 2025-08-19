// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlParserOptions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Parser;

public struct HtmlParserOptions
{
  public bool IsEmbedded { get; set; }

  public bool IsNotSupportingFrames { get; set; }

  public bool IsScripting { get; set; }

  public bool IsStrictMode { get; set; }

  public bool IsSupportingProcessingInstructions { get; set; }

  public bool IsKeepingSourceReferences { get; set; }

  public bool IsNotConsumingCharacterReferences { get; set; }

  public Action<IElement, TextPosition> OnCreated { get; set; }
}
