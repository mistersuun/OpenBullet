// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataNamePart
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Security;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct MetadataNamePart : IEquatable<MetadataNamePart>, IEquatable<MetadataName>
{
  private readonly MetadataName m_name;
  private readonly int m_byteCount;
  public static readonly MetadataNamePart Empty;

  internal MetadataNamePart(MetadataName name, int byteCount)
  {
    this.m_name = name;
    this.m_byteCount = byteCount;
  }

  public int Length => this.m_byteCount;

  public int IndexOf(byte b) => this.m_name.IndexOf(b, 0, this.m_byteCount);

  public int IndexOf(byte b, int start, int count)
  {
    if (start < 0)
      throw new ArgumentOutOfRangeException(nameof (start));
    if (count < 0 || count > this.Length - start)
      throw new ArgumentOutOfRangeException(nameof (count));
    return this.m_name.IndexOf(b, start, count);
  }

  public int LastIndexOf(byte b, int start, int count)
  {
    if (start < 0 || start > this.Length)
      throw new ArgumentOutOfRangeException(nameof (start));
    if (count < 0 || start < count - 1)
      throw new ArgumentOutOfRangeException(nameof (count));
    return this.m_name.LastIndexOf(b, start, count);
  }

  public MetadataNamePart GetPart(int start)
  {
    if (start < 0 || start > this.Length)
      throw new ArgumentOutOfRangeException(nameof (start));
    return new MetadataNamePart(this.m_name.GetSuffix(start), this.m_byteCount - start);
  }

  public MetadataNamePart GetPart(int start, int count)
  {
    if (start < 0)
      throw new ArgumentOutOfRangeException(nameof (start));
    if (count < 0 || count > this.Length - start)
      throw new ArgumentOutOfRangeException(nameof (count));
    return new MetadataNamePart(this.m_name.GetSuffix(start), count);
  }

  [SecuritySafeCritical]
  public override string ToString() => this.m_name.ToString(this.m_byteCount);

  [SecuritySafeCritical]
  public override int GetHashCode() => this.m_name.GetHashCode(0, this.m_byteCount);

  [SecuritySafeCritical]
  public override bool Equals(object obj)
  {
    switch (obj)
    {
      case MetadataNamePart other1 when this.Equals(other1):
        return true;
      case MetadataName other2:
        return this.Equals(other2);
      default:
        return false;
    }
  }

  [SecuritySafeCritical]
  public unsafe bool Equals(MetadataNamePart other)
  {
    int num = MetadataName.Equals(this.m_name.m_data, this.m_byteCount, other.m_name.m_data, other.m_byteCount) ? 1 : 0;
    GC.KeepAlive(this.m_name.m_keepAlive);
    GC.KeepAlive(other.m_name.m_keepAlive);
    return num != 0;
  }

  [SecuritySafeCritical]
  public unsafe bool Equals(MetadataName other)
  {
    int num = MetadataName.Equals(other.m_data, this.m_name.m_data, this.m_byteCount) ? 1 : 0;
    GC.KeepAlive(this.m_name.m_keepAlive);
    GC.KeepAlive(other.m_keepAlive);
    return num != 0;
  }

  public static bool operator ==(MetadataNamePart self, MetadataNamePart other)
  {
    return self.Equals(other);
  }

  public static bool operator ==(MetadataNamePart self, MetadataName other) => self.Equals(other);

  public static bool operator !=(MetadataNamePart self, MetadataNamePart other)
  {
    return self.Equals(other);
  }

  public static bool operator !=(MetadataNamePart self, MetadataName other) => self.Equals(other);
}
