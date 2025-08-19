// Decompiled with JetBrains decompiler
// Type: LiteDB.CollectionIndex
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

internal class CollectionIndex
{
  public static Regex IndexPattern = new Regex("^[\\w](\\.?[\\w\\$][\\w-]*){0,59}$", RegexOptions.Compiled);
  public const int INDEX_PER_COLLECTION = 16 /*0x10*/;
  public uint FreeIndexPageID;

  public int Slot { get; set; }

  public string Field { get; set; }

  public string Expression { get; set; }

  public bool Unique { get; set; }

  public PageAddress HeadNode { get; set; }

  public PageAddress TailNode { get; set; }

  public bool IsEmpty => string.IsNullOrEmpty(this.Field);

  public byte MaxLevel { get; set; }

  public CollectionPage Page { get; set; }

  public CollectionIndex() => this.Clear();

  public void Clear()
  {
    this.Field = string.Empty;
    this.Expression = string.Empty;
    this.Unique = false;
    this.HeadNode = PageAddress.Empty;
    this.FreeIndexPageID = uint.MaxValue;
    this.MaxLevel = (byte) 1;
  }
}
