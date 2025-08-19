// Decompiled with JetBrains decompiler
// Type: Extreme.Net.ExceptionHelper
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;

#nullable disable
namespace Extreme.Net;

internal static class ExceptionHelper
{
  internal static ArgumentException EmptyString(string paramName)
  {
    return new ArgumentException(Resources.ArgumentException_EmptyString, paramName);
  }

  internal static ArgumentOutOfRangeException CanNotBeLess<T>(string paramName, T value) where T : struct
  {
    return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLess, (object) value));
  }

  internal static ArgumentOutOfRangeException CanNotBeGreater<T>(string paramName, T value) where T : struct
  {
    return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeGreater, (object) value));
  }

  internal static ArgumentException WrongPath(string paramName, Exception innerException = null)
  {
    return new ArgumentException(Resources.ArgumentException_WrongPath, paramName, innerException);
  }

  internal static ArgumentOutOfRangeException WrongTcpPort(string paramName)
  {
    return new ArgumentOutOfRangeException("port", string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLessOrGreater, (object) 1, (object) (int) ushort.MaxValue));
  }

  internal static bool ValidateTcpPort(int port) => port >= 1 && port <= (int) ushort.MaxValue;
}
