// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetTextElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetTextElement : SnippetElement
{
  private string text;

  public string Text
  {
    get => this.text;
    set => this.text = value;
  }

  public override void Insert(InsertionContext context)
  {
    if (this.text == null)
      return;
    context.InsertText(this.text);
  }

  public override Inline ToTextRun() => (Inline) new Run(this.text ?? string.Empty);
}
