// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpRequestException
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http;

public class HttpRequestException : Exception
{
  public HttpRequestException()
    : this((string) null, (Exception) null)
  {
  }

  public HttpRequestException(string message)
    : this(message, (Exception) null)
  {
  }

  public HttpRequestException(string message, Exception inner)
    : base(message, inner)
  {
    if (inner == null)
      return;
    this.HResult = inner.HResult;
  }
}
