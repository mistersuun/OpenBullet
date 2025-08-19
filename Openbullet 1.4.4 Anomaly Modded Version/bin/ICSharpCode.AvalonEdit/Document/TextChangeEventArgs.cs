// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextChangeEventArgs
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public class TextChangeEventArgs : EventArgs
{
  private readonly int offset;
  private readonly ITextSource removedText;
  private readonly ITextSource insertedText;

  public int Offset => this.offset;

  public ITextSource RemovedText => this.removedText;

  public int RemovalLength => this.removedText.TextLength;

  public ITextSource InsertedText => this.insertedText;

  public int InsertionLength => this.insertedText.TextLength;

  public TextChangeEventArgs(int offset, string removedText, string insertedText)
  {
    this.offset = offset >= 0 ? offset : throw new ArgumentOutOfRangeException(nameof (offset), (object) offset, "offset must not be negative");
    this.removedText = removedText != null ? (ITextSource) new StringTextSource(removedText) : (ITextSource) StringTextSource.Empty;
    this.insertedText = insertedText != null ? (ITextSource) new StringTextSource(insertedText) : (ITextSource) StringTextSource.Empty;
  }

  public TextChangeEventArgs(int offset, ITextSource removedText, ITextSource insertedText)
  {
    this.offset = offset >= 0 ? offset : throw new ArgumentOutOfRangeException(nameof (offset), (object) offset, "offset must not be negative");
    this.removedText = removedText ?? (ITextSource) StringTextSource.Empty;
    this.insertedText = insertedText ?? (ITextSource) StringTextSource.Empty;
  }

  public virtual int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
  {
    return offset >= this.Offset && offset <= this.Offset + this.RemovalLength ? (movementType == AnchorMovementType.BeforeInsertion ? this.Offset : this.Offset + this.InsertionLength) : (offset > this.Offset ? offset + this.InsertionLength - this.RemovalLength : offset);
  }

  public virtual TextChangeEventArgs Invert()
  {
    return new TextChangeEventArgs(this.offset, this.insertedText, this.removedText);
  }
}
