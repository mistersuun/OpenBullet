// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetAnchorElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

public sealed class SnippetAnchorElement : SnippetElement
{
  public string Name { get; private set; }

  public SnippetAnchorElement(string name) => this.Name = name;

  public override void Insert(InsertionContext context)
  {
    TextAnchor anchor = context.Document.CreateAnchor(context.InsertionPosition);
    anchor.MovementType = AnchorMovementType.BeforeInsertion;
    anchor.SurviveDeletion = true;
    AnchorSegment segment = new AnchorSegment(anchor, anchor);
    context.RegisterActiveElement((SnippetElement) this, (IActiveElement) new AnchorElement(segment, this.Name, context));
  }
}
