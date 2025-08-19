// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ClrStubs
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Text;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class ClrStubs
{
  internal static unsafe int GetCharCount(
    this Encoding encoding,
    byte* bytes,
    int byteCount,
    object nls)
  {
    return encoding.GetCharCount(bytes, byteCount);
  }

  internal static unsafe void GetChars(
    this Encoding encoding,
    byte* bytes,
    int byteCount,
    char* chars,
    int charCount,
    object nls)
  {
    encoding.GetChars(bytes, byteCount, chars, charCount);
  }
}
