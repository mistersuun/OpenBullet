// Decompiled with JetBrains decompiler
// Type: LiteDB.IndexNodeComparer
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class IndexNodeComparer : IEqualityComparer<IndexNode>
{
  public bool Equals(IndexNode x, IndexNode y)
  {
    if (x == y)
      return true;
    return x != null && y != null && x.DataBlock.Equals((object) y.DataBlock);
  }

  public int GetHashCode(IndexNode obj) => obj.DataBlock.GetHashCode();
}
