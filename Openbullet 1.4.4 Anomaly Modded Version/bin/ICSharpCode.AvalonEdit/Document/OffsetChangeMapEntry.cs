// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.OffsetChangeMapEntry
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public struct OffsetChangeMapEntry : IEquatable<OffsetChangeMapEntry>
{
  private readonly int offset;
  private readonly uint insertionLengthWithMovementFlag;
  private readonly uint removalLengthWithDeletionFlag;

  public int Offset => this.offset;

  public int InsertionLength => (int) this.insertionLengthWithMovementFlag & int.MaxValue;

  public int RemovalLength => (int) this.removalLengthWithDeletionFlag & int.MaxValue;

  public bool RemovalNeverCausesAnchorDeletion
  {
    get => ((int) this.removalLengthWithDeletionFlag & int.MinValue) != 0;
  }

  public bool DefaultAnchorMovementIsBeforeInsertion
  {
    get => ((int) this.insertionLengthWithMovementFlag & int.MinValue) != 0;
  }

  public int GetNewOffset(int oldOffset, AnchorMovementType movementType = AnchorMovementType.Default)
  {
    int insertionLength = this.InsertionLength;
    int removalLength = this.RemovalLength;
    if (removalLength != 0 || oldOffset != this.offset)
    {
      if (oldOffset <= this.offset)
        return oldOffset;
      if (oldOffset >= this.offset + removalLength)
        return oldOffset + insertionLength - removalLength;
    }
    return movementType == AnchorMovementType.AfterInsertion || movementType != AnchorMovementType.BeforeInsertion && !this.DefaultAnchorMovementIsBeforeInsertion ? this.offset + insertionLength : this.offset;
  }

  public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength)
  {
    ThrowUtil.CheckNotNegative(offset, nameof (offset));
    ThrowUtil.CheckNotNegative(removalLength, nameof (removalLength));
    ThrowUtil.CheckNotNegative(insertionLength, nameof (insertionLength));
    this.offset = offset;
    this.removalLengthWithDeletionFlag = (uint) removalLength;
    this.insertionLengthWithMovementFlag = (uint) insertionLength;
  }

  public OffsetChangeMapEntry(
    int offset,
    int removalLength,
    int insertionLength,
    bool removalNeverCausesAnchorDeletion,
    bool defaultAnchorMovementIsBeforeInsertion)
    : this(offset, removalLength, insertionLength)
  {
    if (removalNeverCausesAnchorDeletion)
      this.removalLengthWithDeletionFlag |= 2147483648U /*0x80000000*/;
    if (!defaultAnchorMovementIsBeforeInsertion)
      return;
    this.insertionLengthWithMovementFlag |= 2147483648U /*0x80000000*/;
  }

  public override int GetHashCode()
  {
    return this.offset + 3559 * (int) this.insertionLengthWithMovementFlag + 3571 * (int) this.removalLengthWithDeletionFlag;
  }

  public override bool Equals(object obj)
  {
    return obj is OffsetChangeMapEntry other && this.Equals(other);
  }

  public bool Equals(OffsetChangeMapEntry other)
  {
    return this.offset == other.offset && (int) this.insertionLengthWithMovementFlag == (int) other.insertionLengthWithMovementFlag && (int) this.removalLengthWithDeletionFlag == (int) other.removalLengthWithDeletionFlag;
  }

  public static bool operator ==(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
  {
    return !left.Equals(right);
  }
}
