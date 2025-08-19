// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.ResponseExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;

#nullable disable
namespace AngleSharp.Io;

public static class ResponseExtensions
{
  public static MimeType GetContentType(this IResponse response)
  {
    string path = response.Address.Path;
    int startIndex = path.LastIndexOf('.');
    string defaultValue = MimeTypeNames.FromExtension(startIndex >= 0 ? path.Substring(startIndex) : ".a");
    return new MimeType(response.Headers.GetOrDefault<string, string>(HeaderNames.ContentType, defaultValue));
  }

  public static MimeType GetContentType(this IResponse response, string defaultType)
  {
    string path = response.Address.Path;
    int startIndex = path.LastIndexOf('.');
    if (startIndex >= 0)
      defaultType = MimeTypeNames.FromExtension(path.Substring(startIndex));
    return new MimeType(response.Headers.GetOrDefault<string, string>(HeaderNames.ContentType, defaultType));
  }
}
