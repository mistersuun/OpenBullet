// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataTableView
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct MetadataTableView
{
  private readonly MetadataTokenType m_type;
  private readonly MetadataRecord m_parent;

  public int GetCount()
  {
    if (this.m_parent.IsNull)
      return this.m_parent.Tables.GetRowCount((int) this.m_type >> 24);
    int count;
    int enumeratorRange = (int) this.m_parent.Import.GetEnumeratorRange(this.m_type, this.m_parent.Token, out int _, out count);
    return count;
  }

  public MetadataTableEnumerator GetEnumerator()
  {
    return new MetadataTableEnumerator(this.m_parent, this.m_type);
  }

  internal MetadataTableView(MetadataRecord parent, MetadataTokenType type)
  {
    this.m_type = type;
    this.m_parent = parent;
  }
}
