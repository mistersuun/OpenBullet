// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.BoundActiveElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

internal sealed class BoundActiveElement : IActiveElement
{
  private InsertionContext context;
  private SnippetReplaceableTextElement targetSnippetElement;
  private SnippetBoundElement boundElement;
  internal IReplaceableActiveElement targetElement;
  private AnchorSegment segment;

  public BoundActiveElement(
    InsertionContext context,
    SnippetReplaceableTextElement targetSnippetElement,
    SnippetBoundElement boundElement,
    AnchorSegment segment)
  {
    this.context = context;
    this.targetSnippetElement = targetSnippetElement;
    this.boundElement = boundElement;
    this.segment = segment;
  }

  public void OnInsertionCompleted()
  {
    this.targetElement = this.context.GetActiveElement((SnippetElement) this.targetSnippetElement) as IReplaceableActiveElement;
    if (this.targetElement == null)
      return;
    this.targetElement.TextChanged += new EventHandler(this.targetElement_TextChanged);
  }

  private void targetElement_TextChanged(object sender, EventArgs e)
  {
    if (!(SimpleSegment.GetOverlap((ISegment) this.segment, this.targetElement.Segment) == SimpleSegment.Invalid))
      return;
    int offset = this.segment.Offset;
    int length = this.segment.Length;
    string text = this.boundElement.ConvertText(this.targetElement.Text);
    if (length == text.Length && !(text != this.context.Document.GetText(offset, length)))
      return;
    this.context.Document.Replace(offset, length, text);
    if (length != 0)
      return;
    this.segment = new AnchorSegment(this.context.Document, offset, text.Length);
  }

  public void Deactivate(SnippetEventArgs e)
  {
  }

  public bool IsEditable => false;

  public ISegment Segment => (ISegment) this.segment;
}
