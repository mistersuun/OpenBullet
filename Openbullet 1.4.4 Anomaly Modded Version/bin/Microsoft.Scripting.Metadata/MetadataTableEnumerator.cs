// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataTableEnumerator
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public sealed class MetadataTableEnumerator
{
  private readonly int m_startRid;
  private readonly int m_endRid;
  private readonly MetadataTokenType m_type;
  private readonly EnumerationIndirection m_indirection;
  private MetadataTables m_tables;
  private int m_currentRid;
  private MetadataToken m_currentToken;

  internal MetadataTableEnumerator(MetadataRecord parent, MetadataTokenType type)
  {
    this.m_type = type;
    this.m_tables = parent.m_tables;
    int startRid;
    int count;
    this.m_indirection = parent.m_tables.m_import.GetEnumeratorRange(type, parent.Token, out startRid, out count);
    this.m_startRid = startRid;
    this.m_endRid = startRid + count;
    this.m_currentRid = startRid - 1;
  }

  public int Count => this.m_endRid - this.m_startRid;

  public void Reset()
  {
    this.m_currentRid = this.m_startRid;
    this.m_currentToken = new MetadataToken();
  }

  public bool MoveNext()
  {
    int num = this.m_currentRid + 1;
    if (num >= this.m_endRid)
    {
      if (this.m_tables == null)
        throw new ObjectDisposedException(nameof (MetadataTableEnumerator));
      return false;
    }
    this.m_currentRid = num;
    switch (this.m_indirection)
    {
      case EnumerationIndirection.Method:
        this.m_currentToken = this.m_tables.m_import.MethodPtrTable.GetMethodFor(this.m_currentRid);
        break;
      case EnumerationIndirection.Field:
        this.m_currentToken = this.m_tables.m_import.FieldPtrTable.GetFieldFor(this.m_currentRid);
        break;
      case EnumerationIndirection.Property:
        this.m_currentToken = this.m_tables.m_import.PropertyPtrTable.GetPropertyFor(this.m_currentRid);
        break;
      case EnumerationIndirection.Event:
        this.m_currentToken = this.m_tables.m_import.EventPtrTable.GetEventFor(this.m_currentRid);
        break;
      case EnumerationIndirection.Param:
        this.m_currentToken = this.m_tables.m_import.ParamPtrTable.GetParamFor(this.m_currentRid);
        break;
      default:
        this.m_currentToken = new MetadataToken(this.m_type, (uint) this.m_currentRid);
        break;
    }
    return true;
  }

  public MetadataRecord Current => new MetadataRecord(this.m_currentToken, this.m_tables);
}
