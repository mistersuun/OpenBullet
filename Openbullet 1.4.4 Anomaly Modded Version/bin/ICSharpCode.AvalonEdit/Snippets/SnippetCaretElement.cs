// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetCaretElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Runtime.Serialization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetCaretElement : SnippetElement
{
  [OptionalField]
  private bool setCaretOnlyIfTextIsSelected;

  public SnippetCaretElement()
  {
  }

  public SnippetCaretElement(bool setCaretOnlyIfTextIsSelected)
  {
    this.setCaretOnlyIfTextIsSelected = setCaretOnlyIfTextIsSelected;
  }

  public override void Insert(InsertionContext context)
  {
    if (this.setCaretOnlyIfTextIsSelected && string.IsNullOrEmpty(context.SelectedText))
      return;
    SnippetCaretElement.SetCaret(context);
  }

  internal static void SetCaret(InsertionContext context)
  {
    TextAnchor pos = context.Document.CreateAnchor(context.InsertionPosition);
    pos.MovementType = AnchorMovementType.BeforeInsertion;
    pos.SurviveDeletion = true;
    context.Deactivated += (EventHandler<SnippetEventArgs>) ((sender, e) =>
    {
      if (e.Reason != DeactivateReason.ReturnPressed && e.Reason != DeactivateReason.NoActiveElements)
        return;
      context.TextArea.Caret.Offset = pos.Offset;
    });
  }
}
