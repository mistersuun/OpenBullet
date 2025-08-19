// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.CollectionService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB_V6;

internal class CollectionService
{
  private PageService _pager;

  public CollectionService(PageService pager) => this._pager = pager;

  public CollectionPage Get(string name)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentNullException(nameof (name));
    uint pageID;
    if (this._pager.GetPage<HeaderPage>(0U).CollectionPages.TryGetValue(name, out pageID))
      return this._pager.GetPage<CollectionPage>(pageID);
    throw new Exception("Collection not found: " + name);
  }
}
