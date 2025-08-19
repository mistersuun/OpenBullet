// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetContainerElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetContainerElement : SnippetElement
{
  private NullSafeCollection<SnippetElement> elements = new NullSafeCollection<SnippetElement>();

  public IList<SnippetElement> Elements => (IList<SnippetElement>) this.elements;

  public override void Insert(InsertionContext context)
  {
    foreach (SnippetElement element in (IEnumerable<SnippetElement>) this.Elements)
      element.Insert(context);
  }

  public override Inline ToTextRun()
  {
    Span textRun1 = new Span();
    foreach (SnippetElement element in (IEnumerable<SnippetElement>) this.Elements)
    {
      Inline textRun2 = element.ToTextRun();
      if (textRun2 != null)
        textRun1.Inlines.Add(textRun2);
    }
    return (Inline) textRun1;
  }
}
