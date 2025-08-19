// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetSelectionElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetSelectionElement : SnippetElement
{
  public int Indentation { get; set; }

  public override void Insert(InsertionContext context)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < this.Indentation; ++index)
      stringBuilder.Append(context.Tab);
    string str = stringBuilder.ToString();
    string text = context.SelectedText.TrimStart(' ', '\t').Replace(context.LineTerminator, context.LineTerminator + str);
    context.Document.Insert(context.InsertionPosition, text);
    context.InsertionPosition += text.Length;
    if (!string.IsNullOrEmpty(context.SelectedText))
      return;
    SnippetCaretElement.SetCaret(context);
  }
}
