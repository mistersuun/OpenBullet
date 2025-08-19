// Decompiled with JetBrains decompiler
// Type: LiteDB.BasePage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

internal abstract class BasePage
{
  public const int PAGE_SIZE = 4096 /*0x1000*/;
  public const int PAGE_HEADER_SIZE = 25;
  public const int PAGE_AVAILABLE_BYTES = 4071;

  public uint PageID { get; set; }

  public abstract PageType PageType { get; }

  public uint PrevPageID { get; set; }

  public uint NextPageID { get; set; }

  public int ItemCount { get; set; }

  public int FreeBytes { get; set; }

  public bool IsDirty { get; set; }

  public byte[] DiskData { get; set; }

  public BasePage(uint pageID)
  {
    this.PageID = pageID;
    this.PrevPageID = uint.MaxValue;
    this.NextPageID = uint.MaxValue;
    this.ItemCount = 0;
    this.FreeBytes = 4071;
    this.DiskData = new byte[0];
  }

  public static long GetSizeOfPages(uint pageCount)
  {
    return checked ((long) pageCount * 4096L /*0x1000*/);
  }

  public static long GetSizeOfPages(int pageCount)
  {
    return pageCount >= 0 ? BasePage.GetSizeOfPages((uint) pageCount) : throw new ArgumentOutOfRangeException(nameof (pageCount), "Could not be less than 0.");
  }

  public static T CreateInstance<T>(uint pageID) where T : BasePage
  {
    Type type = typeof (T);
    if (type == typeof (HeaderPage))
      return new HeaderPage() as T;
    if (type == typeof (CollectionPage))
      return new CollectionPage(pageID) as T;
    if (type == typeof (IndexPage))
      return new IndexPage(pageID) as T;
    if (type == typeof (DataPage))
      return new DataPage(pageID) as T;
    if (type == typeof (ExtendPage))
      return new ExtendPage(pageID) as T;
    if (type == typeof (EmptyPage))
      return new EmptyPage(pageID) as T;
    throw new Exception("Invalid base page type T");
  }

  public static BasePage CreateInstance(uint pageID, PageType pageType)
  {
    switch (pageType)
    {
      case PageType.Empty:
        return (BasePage) new EmptyPage(pageID);
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
    int pageID = (int) reader.ReadUInt32();
    PageType pageType = (PageType) reader.ReadByte();
    BasePage basePage = pageID != 0 || (byte) pageType <= (byte) 5 ? BasePage.CreateInstance((uint) pageID, pageType) : throw LiteException.InvalidDatabase();
    basePage.ReadHeader(reader);
    basePage.ReadContent(reader);
    basePage.DiskData = buffer;
    return basePage;
  }

  public byte[] WritePage()
  {
    ByteWriter writer = new ByteWriter(4096 /*0x1000*/);
    this.WriteHeader(writer);
    if (this.PageType != PageType.Empty)
      this.WriteContent(writer);
    this.DiskData = writer.Buffer;
    return writer.Buffer;
  }

  private void ReadHeader(ByteReader reader)
  {
    this.PrevPageID = reader.ReadUInt32();
    this.NextPageID = reader.ReadUInt32();
    this.ItemCount = (int) reader.ReadUInt16();
    this.FreeBytes = (int) reader.ReadUInt16();
    reader.Skip(8);
  }

  private void WriteHeader(ByteWriter writer)
  {
    writer.Write(this.PageID);
    writer.Write((byte) this.PageType);
    writer.Write(this.PrevPageID);
    writer.Write(this.NextPageID);
    writer.Write((ushort) this.ItemCount);
    writer.Write((ushort) this.FreeBytes);
    writer.Skip(8);
  }

  protected abstract void ReadContent(ByteReader reader);

  protected abstract void WriteContent(ByteWriter writer);
}
