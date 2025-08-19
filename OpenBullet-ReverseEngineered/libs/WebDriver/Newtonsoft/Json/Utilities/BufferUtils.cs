// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.BufferUtils
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
