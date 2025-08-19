// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextAnchor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class TextAnchor : ITextAnchor
{
  private readonly TextDocument document;
  internal TextAnchorNode node;

  internal TextAnchor(TextDocument document) => this.document = document;

  public TextDocument Document => this.document;

  public AnchorMovementType MovementType { get; set; }

  public bool SurviveDeletion { get; set; }

  public bool IsDeleted => this.node == null;

  public event EventHandler Deleted;

  internal void OnDeleted(DelayedEvents delayedEvents)
  {
    this.node = (TextAnchorNode) null;
    delayedEvents.DelayedRaise(this.Deleted, (object) this, EventArgs.Empty);
  }

  public int Offset
  {
    get
    {
      TextAnchorNode textAnchorNode = this.node;
      int offset = textAnchorNode != null ? textAnchorNode.length : throw new InvalidOperationException();
      if (textAnchorNode.left != null)
        offset += textAnchorNode.left.totalLength;
      for (; textAnchorNode.parent != null; textAnchorNode = textAnchorNode.parent)
      {
        if (textAnchorNode == textAnchorNode.parent.right)
        {
          if (textAnchorNode.parent.left != null)
            offset += textAnchorNode.parent.left.totalLength;
          offset += textAnchorNode.parent.length;
        }
      }
      return offset;
    }
  }

  public int Line => this.document.GetLineByOffset(this.Offset).LineNumber;

  public int Column
  {
    get
    {
      int offset = this.Offset;
      return offset - this.document.GetLineByOffset(offset).Offset + 1;
    }
  }

  public TextLocation Location => this.document.GetLocation(this.Offset);

  public override string ToString() => $"[TextAnchor Offset={(object) this.Offset}]";
}
