// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.OffsetChangeMap
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public sealed class OffsetChangeMap : Collection<OffsetChangeMapEntry>
{
  public static readonly OffsetChangeMap Empty = new OffsetChangeMap((IList<OffsetChangeMapEntry>) ICSharpCode.AvalonEdit.Utils.Empty<OffsetChangeMapEntry>.Array, true);
  private bool isFrozen;

  public static OffsetChangeMap FromSingleElement(OffsetChangeMapEntry entry)
  {
    return new OffsetChangeMap((IList<OffsetChangeMapEntry>) new OffsetChangeMapEntry[1]
    {
      entry
    }, true);
  }

  public OffsetChangeMap()
  {
  }

  internal OffsetChangeMap(int capacity)
    : base((IList<OffsetChangeMapEntry>) new List<OffsetChangeMapEntry>(capacity))
  {
  }

  private OffsetChangeMap(IList<OffsetChangeMapEntry> entries, bool isFrozen)
    : base(entries)
  {
    this.isFrozen = isFrozen;
  }

  public int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
  {
    IList<OffsetChangeMapEntry> items = this.Items;
    int count = items.Count;
    for (int index = 0; index < count; ++index)
      offset = items[index].GetNewOffset(offset, movementType);
    return offset;
  }

  public bool IsValidForDocumentChange(int offset, int removalLength, int insertionLength)
  {
    int num = offset + removalLength;
    foreach (OffsetChangeMapEntry offsetChangeMapEntry in (Collection<OffsetChangeMapEntry>) this)
    {
      if (offsetChangeMapEntry.Offset < offset || offsetChangeMapEntry.Offset + offsetChangeMapEntry.RemovalLength > num)
        return false;
      num += offsetChangeMapEntry.InsertionLength - offsetChangeMapEntry.RemovalLength;
    }
    return num == offset + insertionLength;
  }

  public OffsetChangeMap Invert()
  {
    if (this == OffsetChangeMap.Empty)
      return this;
    OffsetChangeMap offsetChangeMap = new OffsetChangeMap(this.Count);
    for (int index = this.Count - 1; index >= 0; --index)
    {
      OffsetChangeMapEntry offsetChangeMapEntry = this[index];
      offsetChangeMap.Add(new OffsetChangeMapEntry(offsetChangeMapEntry.Offset, offsetChangeMapEntry.InsertionLength, offsetChangeMapEntry.RemovalLength));
    }
    return offsetChangeMap;
  }

  protected override void ClearItems()
  {
    this.CheckFrozen();
    base.ClearItems();
  }

  protected override void InsertItem(int index, OffsetChangeMapEntry item)
  {
    this.CheckFrozen();
    base.InsertItem(index, item);
  }

  protected override void RemoveItem(int index)
  {
    this.CheckFrozen();
    base.RemoveItem(index);
  }

  protected override void SetItem(int index, OffsetChangeMapEntry item)
  {
    this.CheckFrozen();
    base.SetItem(index, item);
  }

  private void CheckFrozen()
  {
    if (this.isFrozen)
      throw new InvalidOperationException("This instance is frozen and cannot be modified.");
  }

  public bool IsFrozen => this.isFrozen;

  public void Freeze() => this.isFrozen = true;
}
