// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public class DocumentChangeEventArgs : TextChangeEventArgs
{
  private volatile OffsetChangeMap offsetChangeMap;

  public OffsetChangeMap OffsetChangeMap
  {
    get
    {
      OffsetChangeMap offsetChangeMap = this.offsetChangeMap;
      if (offsetChangeMap == null)
      {
        offsetChangeMap = OffsetChangeMap.FromSingleElement(this.CreateSingleChangeMapEntry());
        this.offsetChangeMap = offsetChangeMap;
      }
      return offsetChangeMap;
    }
  }

  internal OffsetChangeMapEntry CreateSingleChangeMapEntry()
  {
    return new OffsetChangeMapEntry(this.Offset, this.RemovalLength, this.InsertionLength);
  }

  internal OffsetChangeMap OffsetChangeMapOrNull => this.offsetChangeMap;

  public override int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
  {
    return this.offsetChangeMap != null ? this.offsetChangeMap.GetNewOffset(offset, movementType) : this.CreateSingleChangeMapEntry().GetNewOffset(offset, movementType);
  }

  public DocumentChangeEventArgs(int offset, string removedText, string insertedText)
    : this(offset, removedText, insertedText, (OffsetChangeMap) null)
  {
  }

  public DocumentChangeEventArgs(
    int offset,
    string removedText,
    string insertedText,
    OffsetChangeMap offsetChangeMap)
    : base(offset, removedText, insertedText)
  {
    this.SetOffsetChangeMap(offsetChangeMap);
  }

  public DocumentChangeEventArgs(
    int offset,
    ITextSource removedText,
    ITextSource insertedText,
    OffsetChangeMap offsetChangeMap)
    : base(offset, removedText, insertedText)
  {
    this.SetOffsetChangeMap(offsetChangeMap);
  }

  private void SetOffsetChangeMap(OffsetChangeMap offsetChangeMap)
  {
    if (offsetChangeMap == null)
      return;
    if (!offsetChangeMap.IsFrozen)
      throw new ArgumentException("The OffsetChangeMap must be frozen before it can be used in DocumentChangeEventArgs");
    if (!offsetChangeMap.IsValidForDocumentChange(this.Offset, this.RemovalLength, this.InsertionLength))
      throw new ArgumentException("OffsetChangeMap is not valid for this document change", nameof (offsetChangeMap));
    this.offsetChangeMap = offsetChangeMap;
  }

  public override TextChangeEventArgs Invert()
  {
    OffsetChangeMap offsetChangeMap = this.OffsetChangeMapOrNull;
    if (offsetChangeMap != null)
    {
      offsetChangeMap = offsetChangeMap.Invert();
      offsetChangeMap.Freeze();
    }
    return (TextChangeEventArgs) new DocumentChangeEventArgs(this.Offset, this.InsertedText, this.RemovedText, offsetChangeMap);
  }
}
