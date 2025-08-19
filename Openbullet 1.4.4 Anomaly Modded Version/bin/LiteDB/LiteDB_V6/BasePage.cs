// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.BasePage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;

#nullable disable
namespace LiteDB_V6;

internal abstract class BasePage
{
  public const int PAGE_SIZE = 4096 /*0x1000*/;

  public uint PageID { get; set; }

  public abstract PageType PageType { get; }

  public uint PrevPageID { get; set; }

  public uint NextPageID { get; set; }

  public int ItemCount { get; set; }

  public BasePage(uint pageID)
  {
    this.PageID = pageID;
    this.PrevPageID = uint.MaxValue;
    this.NextPageID = uint.MaxValue;
    this.ItemCount = 0;
  }

  public static BasePage CreateInstance(uint pageID, PageType pageType)
  {
    switch (pageType)
    {
      case PageType.Collection:
        return (BasePage) new CollectionPage(pageID);
      case PageType.Index:
        return (BasePage) new IndexPage(pageID);
      case PageType.Data:
        return (BasePage) new DataPage(pageID);
      case PageType.Extend:
        return (BasePage) new ExtendPage(pageID);
      default:
        return (BasePage) new HeaderPage();
    }
  }

  public static BasePage ReadPage(byte[] buffer)
  {
    ByteReader reader = new ByteReader(buffer);
    BasePage instance = BasePage.CreateInstance(reader.ReadUInt32(), (PageType) reader.ReadByte());
    instance.ReadHeader(reader);
    instance.ReadContent(reader);
    return instance;
  }

  public static long GetSizeOfPages(uint pageCount)
  {
    return checked ((long) pageCount * 4096L /*0x1000*/);
  }

  private void ReadHeader(ByteReader reader)
  {
    this.PrevPageID = reader.ReadUInt32();
    this.NextPageID = reader.ReadUInt32();
    this.ItemCount = (int) reader.ReadUInt16();
    int num = (int) reader.ReadUInt16();
    reader.Skip(8);
  }

  protected abstract void ReadContent(ByteReader reader);
}
