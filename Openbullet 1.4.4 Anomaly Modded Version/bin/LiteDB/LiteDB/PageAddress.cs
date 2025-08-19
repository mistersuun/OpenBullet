// Decompiled with JetBrains decompiler
// Type: LiteDB.PageAddress
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

internal struct PageAddress(uint pageID, ushort index)
{
  public const int SIZE = 6;
  public static PageAddress Empty = new PageAddress(uint.MaxValue, ushort.MaxValue);
  public uint PageID = pageID;
  public ushort Index = index;

  public bool IsEmpty => this.PageID == uint.MaxValue;

  public override bool Equals(object obj)
  {
    PageAddress pageAddress = (PageAddress) obj;
    return (int) this.PageID == (int) pageAddress.PageID && (int) this.Index == (int) pageAddress.Index;
  }

  public override int GetHashCode() => (17 * 23 + (int) this.PageID) * 23 + (int) this.Index;

  public override string ToString()
  {
    return !this.IsEmpty ? $"{this.PageID.ToString()}:{this.Index.ToString()}" : "----";
  }
}
