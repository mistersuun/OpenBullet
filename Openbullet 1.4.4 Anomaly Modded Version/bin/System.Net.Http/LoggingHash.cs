// Decompiled with JetBrains decompiler
// Type: System.Net.LoggingHash
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;

#nullable disable
namespace System.Net;

internal static class LoggingHash
{
  internal static string GetObjectName(object obj)
  {
    if (obj == null)
      return "null";
    if ((object) (obj as Uri) == null)
    {
      switch (obj)
      {
        case IPAddress _:
        case IPEndPoint _:
        case string _:
          break;
        default:
          return obj.GetType().ToString();
      }
    }
    return obj.ToString();
  }

  internal static int HashInt(object objectValue)
  {
    return objectValue == null ? 0 : objectValue.GetHashCode();
  }

  internal static string ObjectToString(object objectValue)
  {
    switch (objectValue)
    {
      case null:
        return "(null)";
      case string _ when ((string) objectValue).Length == 0:
        return "(string.empty)";
      case Exception _:
        return LoggingHash.ExceptionMessage(objectValue as Exception);
      case IntPtr num:
        return "0x" + num.ToString("x");
      default:
        return objectValue.ToString();
    }
  }

  private static string ExceptionMessage(Exception exception)
  {
    if (exception == null)
      return string.Empty;
    return exception.InnerException == null ? exception.Message : $"{exception.Message} ({LoggingHash.ExceptionMessage(exception.InnerException)})";
  }

  internal static string HashString(object objectValue)
  {
    if (objectValue == null)
      return "(null)";
    return objectValue is string && ((string) objectValue).Length == 0 ? "(string.empty)" : objectValue.GetHashCode().ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
  }

  internal static object[] GetObjectLogHash(object obj)
  {
    return new object[2]
    {
      (object) LoggingHash.GetObjectName(obj),
      (object) LoggingHash.HashInt(obj)
    };
  }
}
