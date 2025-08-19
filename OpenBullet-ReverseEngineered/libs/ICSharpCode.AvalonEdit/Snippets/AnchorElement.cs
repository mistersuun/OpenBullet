// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.AnchorElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

public sealed class AnchorElement : IActiveElement
{
  private AnchorSegment segment;
  private InsertionContext context;

  public bool IsEditable => false;

  public ISegment Segment => (ISegment) this.segment;

  public AnchorElement(AnchorSegment segment, string name, InsertionContext context)
  {
    this.segment = segment;
    this.context = context;
    this.Name = name;
  }

  public string Text
  {
    get => this.context.Document.GetText((ISegment) this.segment);
    set
    {
      int offset = this.segment.Offset;
      int length = this.segment.Length;
      this.context.Document.Replace(offset, length, value);
      if (length != 0)
        return;
      this.segment = new AnchorSegment(this.context.Document, offset, value.Length);
    }
  }

  public string Name { get; private set; }

  public void OnInsertionCompleted()
  {
  }

  public void Deactivate(SnippetEventArgs e)
  {
  }
}
