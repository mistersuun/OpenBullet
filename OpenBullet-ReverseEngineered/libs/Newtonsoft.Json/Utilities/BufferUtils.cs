// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.BufferUtils
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal static class BufferUtils
{
  public static char[] RentBuffer(IArrayPool<char> bufferPool, int minSize)
  {
    return bufferPool == null ? new char[minSize] : bufferPool.Rent(minSize);
  }

  public static void ReturnBuffer(IArrayPool<char> bufferPool, char[] buffer)
  {
    bufferPool?.Return(buffer);
  }

  public static char[] EnsureBufferSize(IArrayPool<char> bufferPool, int size, char[] buffer)
  {
    if (bufferPool == null)
      return new char[size];
    if (buffer != null)
      bufferPool.Return(buffer);
    return bufferPool.Rent(size);
  }
}
