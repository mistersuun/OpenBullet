// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.SimpleTextSource
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class SimpleTextSource : TextSource
{
  private readonly string text;
  private readonly TextRunProperties properties;

  public SimpleTextSource(string text, TextRunProperties properties)
  {
    this.text = text;
    this.properties = properties;
  }

  public override TextRun GetTextRun(int textSourceCharacterIndex)
  {
    return textSourceCharacterIndex < this.text.Length ? (TextRun) new TextCharacters(this.text, textSourceCharacterIndex, this.text.Length - textSourceCharacterIndex, this.properties) : (TextRun) new TextEndOfParagraph(1);
  }

  public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(
    int textSourceCharacterIndex)
  {
    throw new NotImplementedException();
  }

  public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(
    int textSourceCharacterIndexLimit)
  {
    throw new NotImplementedException();
  }
}
