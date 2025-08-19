// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataName
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Security;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct MetadataName : IEquatable<MetadataName>, IEquatable<MetadataNamePart>
{
  internal readonly unsafe byte* m_data;
  internal readonly object m_keepAlive;
  public static readonly MetadataName Empty;

  internal unsafe MetadataName(byte* data, object keepAlive)
  {
    this.m_data = data;
    this.m_keepAlive = keepAlive;
  }

  public unsafe bool IsEmpty
  {
    get
    {
      int num = (IntPtr) this.m_data == IntPtr.Zero ? 1 : (*this.m_data == (byte) 0 ? 1 : 0);
      GC.KeepAlive(this.m_keepAlive);
      return num != 0;
    }
  }

  [SecuritySafeCritical]
  public override bool Equals(object obj)
  {
    switch (obj)
    {
      case MetadataName other1 when this.Equals(other1):
        return true;
      case MetadataNamePart other2:
        return this.Equals(other2);
      default:
        return false;
    }
  }

  [SecuritySafeCritical]
  public unsafe bool Equals(MetadataName other)
  {
    int num = MetadataName.Equals(this.m_data, other.m_data) ? 1 : 0;
    GC.KeepAlive(this.m_keepAlive);
    GC.KeepAlive(other.m_keepAlive);
    return num != 0;
  }

  [SecuritySafeCritical]
  public bool Equals(MetadataNamePart other) => other.Equals(this);

  public static bool operator ==(MetadataName self, MetadataNamePart other) => self.Equals(other);

  public static bool operator ==(MetadataName self, MetadataName other) => self.Equals(other);

  public static bool operator !=(MetadataName self, MetadataNamePart other) => self.Equals(other);

  public static bool operator !=(MetadataName self, MetadataName other) => self.Equals(other);

  public unsafe bool Equals(byte[] bytes, int start, int count)
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    if (start < 0)
      throw new ArgumentOutOfRangeException(nameof (start));
    if (count < 0 || count > bytes.Length - start)
      throw new ArgumentOutOfRangeException(nameof (count));
    int num;
    fixed (byte* q = &bytes[start])
      num = MetadataName.Equals(this.m_data, q, count) ? 1 : 0;
    GC.KeepAlive(this.m_keepAlive);
    return num != 0;
  }

  [SecuritySafeCritical]
  public override unsafe string ToString()
  {
    if ((IntPtr) this.m_data == IntPtr.Zero)
      return string.Empty;
    int length = this.GetLength();
    int charCount = Encoding.UTF8.GetCharCount(this.m_data, length, (object) null);
    string str;
    fixed (char* chars = str = new string(char.MinValue, charCount))
      Encoding.UTF8.GetChars(this.m_data, length, chars, charCount, (object) null);
    GC.KeepAlive(this.m_keepAlive);
    return str;
  }

  internal unsafe string ToString(int byteCount)
  {
    if ((IntPtr) this.m_data == IntPtr.Zero)
      return string.Empty;
    string str = new string((sbyte*) this.m_data, 0, byteCount, Encoding.UTF8);
    GC.KeepAlive(this.m_keepAlive);
    return str;
  }

  [SecuritySafeCritical]
  public override unsafe int GetHashCode()
  {
    int byteHashCode = MetadataName.GetByteHashCode(this.m_data);
    GC.KeepAlive(this.m_keepAlive);
    return byteHashCode;
  }

  internal unsafe int GetHashCode(int start, int count)
  {
    int byteHashCode = MetadataName.GetByteHashCode((IntPtr) this.m_data != IntPtr.Zero ? this.m_data + start : (byte*) null, count);
    GC.KeepAlive(this.m_keepAlive);
    return byteHashCode;
  }

  public unsafe int GetLength()
  {
    int length = MetadataName.GetLength(this.m_data);
    GC.KeepAlive(this.m_keepAlive);
    return length;
  }

  public MetadataNamePart GetExtent() => new MetadataNamePart(this, this.GetLength());

  internal unsafe MetadataName GetSuffix(int start)
  {
    return (IntPtr) this.m_data == IntPtr.Zero ? MetadataName.Empty : new MetadataName(this.m_data + start, this.m_keepAlive);
  }

  internal unsafe int IndexOf(byte b)
  {
    int num = MetadataName.IndexOf(this.m_data, b);
    GC.KeepAlive(this.m_keepAlive);
    return num;
  }

  internal unsafe int IndexOf(byte b, int start, int count)
  {
    int num = MetadataName.IndexOf(this.m_data, b, start, count);
    GC.KeepAlive(this.m_keepAlive);
    return num;
  }

  internal unsafe int LastIndexOf(byte b, int start, int count)
  {
    if ((IntPtr) this.m_data == IntPtr.Zero)
      return -1;
    byte* previous = MetadataName.FindPrevious(this.m_data + start, this.m_data + start - count + 1, b);
    GC.KeepAlive(this.m_keepAlive);
    return (IntPtr) previous == IntPtr.Zero ? -1 : (int) (previous - this.m_data);
  }

  internal static unsafe int GetLength(byte* bytes)
  {
    if ((IntPtr) bytes == IntPtr.Zero)
      return 0;
    byte* numPtr = bytes;
    while (*numPtr != (byte) 0)
      ++numPtr;
    return (int) (numPtr - bytes);
  }

  internal static unsafe int IndexOf(byte* bytes, byte b)
  {
    if ((IntPtr) bytes == IntPtr.Zero)
      return -1;
    byte* numPtr = bytes;
    while (*numPtr != (byte) 0 && (int) *numPtr != (int) b)
      ++numPtr;
    return *numPtr == (byte) 0 ? -1 : (int) (numPtr - bytes);
  }

  internal static unsafe int IndexOf(byte* bytes, byte b, int start, int count)
  {
    if ((IntPtr) bytes == IntPtr.Zero)
      return -1;
    byte* numPtr1 = bytes + start;
    for (byte* numPtr2 = bytes + start + count; numPtr1 < numPtr2; ++numPtr1)
    {
      if ((int) *numPtr1 == (int) b)
        return (int) (numPtr1 - bytes);
    }
    return -1;
  }

  internal static unsafe byte* FindPrevious(byte* start, byte* last, byte b)
  {
    for (byte* previous = start - 1; previous >= last; --previous)
    {
      if ((int) *previous == (int) b)
        return previous;
    }
    return (byte*) null;
  }

  internal static unsafe int GetByteHashCode(byte* bytes)
  {
    int num1 = 5381;
    int num2 = num1;
    if ((IntPtr) bytes != IntPtr.Zero)
    {
      int num3;
      for (byte* numPtr = bytes; (num3 = (int) *numPtr) != 0; numPtr += 2)
      {
        num1 = (num1 << 5) + num1 ^ num3;
        int num4 = (int) numPtr[1];
        if (num4 != 0)
          num2 = (num2 << 5) + num2 ^ num4;
        else
          break;
      }
    }
    return num1 + num2 * 1566083941;
  }

  internal static unsafe int GetByteHashCode(byte* bytes, int count)
  {
    int num1 = 5381;
    int num2 = num1;
    if ((IntPtr) bytes != IntPtr.Zero)
    {
      byte* numPtr1 = bytes + count - 1;
      byte* numPtr2;
      for (numPtr2 = bytes; numPtr2 < numPtr1; numPtr2 += 2)
      {
        num1 = (num1 << 5) + num1 ^ (int) *numPtr2;
        num2 = (num2 << 5) + num2 ^ (int) numPtr2[1];
      }
      if (numPtr2 < bytes + count)
        num1 = (num1 << 5) + num1 ^ (int) *numPtr2;
    }
    return num1 + num2 * 1566083941;
  }

  internal static unsafe bool Equals(byte* p, byte* q)
  {
    if (p == q)
      return true;
    if ((IntPtr) p == IntPtr.Zero)
      return *q == (byte) 0;
    if ((IntPtr) q == IntPtr.Zero)
      return *p == (byte) 0;
    for (; (int) *p == (int) *q; ++q)
    {
      if (*p == (byte) 0)
        return true;
      ++p;
    }
    return false;
  }

  internal static unsafe bool Equals(byte* p, byte* q, int qCount)
  {
    if ((IntPtr) p == IntPtr.Zero)
      return qCount == 0;
    byte* numPtr = q + qCount;
    while (*p != (byte) 0)
    {
      if (q == numPtr || (int) *p != (int) *q)
        return false;
      ++p;
      ++q;
    }
    return q == numPtr;
  }

  internal static unsafe bool Equals(byte* p, int pCount, byte* q, int qCount)
  {
    if (pCount != qCount)
      return false;
    if (p == q || pCount == 0)
      return true;
    byte* numPtr = p + pCount;
    while (p < numPtr)
    {
      if ((int) *p != (int) *q)
        return false;
      ++p;
      ++q;
    }
    return true;
  }
}
