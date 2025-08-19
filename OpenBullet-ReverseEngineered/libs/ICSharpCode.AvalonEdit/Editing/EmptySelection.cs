// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.EmptySelection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class EmptySelection(TextArea textArea) : Selection(textArea)
{
  public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e) => (Selection) this;

  public override TextViewPosition StartPosition => new TextViewPosition(TextLocation.Empty);

  public override TextViewPosition EndPosition => new TextViewPosition(TextLocation.Empty);

  public override ISegment SurroundingSegment => (ISegment) null;

  public override Selection SetEndpoint(TextViewPosition endPosition)
  {
    throw new NotSupportedException();
  }

  public override Selection StartSelectionOrSetEndpoint(
    TextViewPosition startPosition,
    TextViewPosition endPosition)
  {
    if (this.textArea.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    return Selection.Create(this.textArea, startPosition, endPosition);
  }

  public override IEnumerable<SelectionSegment> Segments
  {
    get => (IEnumerable<SelectionSegment>) Empty<SelectionSegment>.Array;
  }

  public override string GetText() => string.Empty;

  public override void ReplaceSelectionWithText(string newText)
  {
    if (newText == null)
      throw new ArgumentNullException(nameof (newText));
    newText = this.AddSpacesIfRequired(newText, this.textArea.Caret.Position, this.textArea.Caret.Position);
    if (newText.Length > 0 && this.textArea.ReadOnlySectionProvider.CanInsert(this.textArea.Caret.Offset))
      this.textArea.Document.Insert(this.textArea.Caret.Offset, newText);
    this.textArea.Caret.VisualColumn = -1;
  }

  public override int Length => 0;

  public override int GetHashCode() => RuntimeHelpers.GetHashCode((object) this);

  public override bool Equals(object obj) => this == obj;
}
