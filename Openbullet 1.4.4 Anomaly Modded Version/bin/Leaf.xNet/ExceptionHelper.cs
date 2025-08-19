// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.ExceptionHelper
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

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
    return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLessOrGreater, (object) 1, (object) (int) ushort.MaxValue));
  }

  internal static bool ValidateTcpPort(int port) => port >= 1 && port <= (int) ushort.MaxValue;
}
