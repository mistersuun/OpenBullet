// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetBoundElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetBoundElement : SnippetElement
{
  private SnippetReplaceableTextElement targetElement;

  public SnippetReplaceableTextElement TargetElement
  {
    get => this.targetElement;
    set => this.targetElement = value;
  }

  public virtual string ConvertText(string input) => input;

  public override void Insert(InsertionContext context)
  {
    if (this.targetElement == null)
      return;
    TextAnchor anchor1 = context.Document.CreateAnchor(context.InsertionPosition);
    anchor1.MovementType = AnchorMovementType.BeforeInsertion;
    anchor1.SurviveDeletion = true;
    string text = this.targetElement.Text;
    if (text != null)
      context.InsertText(this.ConvertText(text));
    TextAnchor anchor2 = context.Document.CreateAnchor(context.InsertionPosition);
    anchor2.MovementType = AnchorMovementType.BeforeInsertion;
    anchor2.SurviveDeletion = true;
    AnchorSegment segment = new AnchorSegment(anchor1, anchor2);
    context.RegisterActiveElement((SnippetElement) this, (IActiveElement) new BoundActiveElement(context, this.targetElement, this, segment));
  }

  public override Inline ToTextRun()
  {
    if (this.targetElement != null)
    {
      string text = this.targetElement.Text;
      if (text != null)
        return (Inline) new Italic((Inline) new Run(this.ConvertText(text)));
    }
    return base.ToTextRun();
  }
}
